using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibCpp2IL.Logging;
using WasmDisassembler;

namespace LibCpp2IL.Wasm;

public sealed class WasmFile : Il2CppBinary
{
	public static Dictionary<string, string>? RemappedDynCallFunctions;

	public readonly List<WasmFunctionDefinition> FunctionTable = new List<WasmFunctionDefinition>();

	internal readonly List<WasmSection> Sections = new List<WasmSection>();

	private byte[] _raw;

	private WasmMemoryBlock _memoryBlock;

	private readonly Dictionary<string, WasmDynCallCoefficients> DynCallCoefficients = new Dictionary<string, WasmDynCallCoefficients>();

	public override long RawLength => _raw.Length;

	internal WasmGlobalType[] GlobalTypes => (from e in ImportSection.Entries
		where e.Kind == WasmExternalKind.EXT_GLOBAL
		select e.GlobalEntry).Concat(GlobalSection.Globals.Select((WasmGlobalEntry g) => g.Type)).ToArray();

	internal WasmGlobalSection GlobalSection => (WasmGlobalSection)Sections.First((WasmSection s) => s.Type == WasmSectionId.SEC_GLOBAL);

	internal WasmTypeSection TypeSection => (WasmTypeSection)Sections.First((WasmSection s) => s.Type == WasmSectionId.SEC_TYPE);

	internal WasmFunctionSection FunctionSection => (WasmFunctionSection)Sections.First((WasmSection s) => s.Type == WasmSectionId.SEC_FUNCTION);

	internal WasmDataSection DataSection => (WasmDataSection)Sections.First((WasmSection s) => s.Type == WasmSectionId.SEC_DATA);

	internal WasmCodeSection CodeSection => (WasmCodeSection)Sections.First((WasmSection s) => s.Type == WasmSectionId.SEC_CODE);

	internal WasmImportSection ImportSection => (WasmImportSection)Sections.First((WasmSection s) => s.Type == WasmSectionId.SEC_IMPORT);

	internal WasmElementSection ElementSection => (WasmElementSection)Sections.First((WasmSection s) => s.Type == WasmSectionId.SEC_ELEMENT);

	internal WasmExportSection ExportSection => (WasmExportSection)Sections.First((WasmSection s) => s.Type == WasmSectionId.SEC_EXPORT);

	public override Stream BaseStream => _memoryBlock?.BaseStream ?? base.BaseStream;

	public WasmFile(MemoryStream input, long maxMetadataUsages)
		: base(input, maxMetadataUsages)
	{
		is32Bit = true;
		InstructionSet = InstructionSet.WASM;
		_raw = input.GetBuffer();
		uint num = ReadUInt32();
		int num2 = ReadInt32();
		if (num != 1836278016)
		{
			throw new Exception($"WASM magic mismatch; got 0x{num:X}");
		}
		if (num2 != 1)
		{
			throw new Exception($"Unknown version, expecting 1, got {num2}");
		}
		LibLogger.VerboseNewline("\tWASM Magic and version match expectations. Reading sections...");
		int num3 = 0;
		while (base.Position < RawLength && num3 < 1000)
		{
			WasmSection wasmSection = WasmSection.MakeSection(this);
			base.Position = wasmSection.Pointer + (long)wasmSection.Size;
			Sections.Add(wasmSection);
			num3++;
		}
		LibLogger.VerboseNewline($"\tRead {Sections.Count} WASM sections. Allocating memory block...");
		_memoryBlock = new WasmMemoryBlock(this);
		LibLogger.VerboseNewline($"\tAllocated memory block of {_memoryBlock.Bytes.Length} (0x{_memoryBlock.Bytes.Length:X}) bytes ({_memoryBlock.Bytes.Length / 1024 / 1024:F2}MB). Constructing function table...");
		foreach (WasmImportEntry entry in ImportSection.Entries)
		{
			if (entry.Kind == WasmExternalKind.EXT_FUNCTION)
			{
				FunctionTable.Add(new WasmFunctionDefinition(entry));
			}
		}
		for (int i = 0; i < CodeSection.Functions.Count; i++)
		{
			WasmFunctionBody body = CodeSection.Functions[i];
			FunctionTable.Add(new WasmFunctionDefinition(this, body, i));
		}
		LibLogger.VerboseNewline($"\tBuilt function table of {FunctionTable.Count} entries. Calculating dynCall coefficients...");
		CalculateDynCallOffsets();
		LibLogger.VerboseNewline($"\tGot dynCall coefficients for {DynCallCoefficients.Count} signatures");
	}

	public override byte GetByteAtRawAddress(ulong addr)
	{
		return _raw[addr];
	}

	public override byte[] GetRawBinaryContent()
	{
		return _raw;
	}

	public WasmFunctionDefinition GetFunctionFromIndexAndSignature(ulong index, string signature)
	{
		if (!DynCallCoefficients.TryGetValue(signature, out var value))
		{
			throw new Exception("Can't get function with signature " + signature + ", as it's not defined in the binary");
		}
		index = (index & value.andWith) + value.addConstant;
		ulong num = ElementSection.Elements[0].FunctionIndices[(int)index];
		return FunctionTable[(int)num];
	}

	private void CalculateDynCallOffsets()
	{
		if (RemappedDynCallFunctions != null)
		{
			foreach (WasmExportEntry item in ExportSection.Exports.Where((WasmExportEntry e) => e.Kind == WasmExternalKind.EXT_FUNCTION))
			{
				if (RemappedDynCallFunctions.TryGetValue(item.Name, out string value))
				{
					LibLogger.VerboseNewline($"\t\tRemapped exported function {item.Name} to {value}");
					item.Name.Value = value;
				}
			}
		}
		foreach (WasmExportEntry item2 in ExportSection.Exports.Where((WasmExportEntry e) => e.Kind == WasmExternalKind.EXT_FUNCTION && e.Name.Value.StartsWith("dynCall_")))
		{
			string value2 = item2.Name.Value;
			int length = "dynCall_".Length;
			string text = value2.Substring(length, value2.Length - length);
			WasmFunctionBody associatedFunctionBody = FunctionTable[(int)item2.Index].AssociatedFunctionBody;
			List<WasmInstruction> list = Disassembler.Disassemble(associatedFunctionBody.Instructions, (uint)associatedFunctionBody.InstructionsOffset);
			WasmInstruction[] array = list.Where(delegate(WasmInstruction i)
			{
				WasmMnemonic mnemonic = i.Mnemonic;
				return mnemonic == WasmMnemonic.I32Const || mnemonic == WasmMnemonic.I32And || mnemonic == WasmMnemonic.I32Add;
			}).ToArray();
			ulong andWith;
			ulong addConstant;
			if (array.Length == 2)
			{
				if (array[^1].Mnemonic == WasmMnemonic.I32And)
				{
					andWith = (ulong)array[0].Operands[0];
					addConstant = 0uL;
				}
				else
				{
					if (array[^1].Mnemonic != WasmMnemonic.I32Add)
					{
						LibLogger.WarnNewline($"\t\tCouldn't calculate coefficients for {text}, got only 2 instructions but the last was {array[^1].Mnemonic}, not I32And or I32Add");
						continue;
					}
					addConstant = (ulong)array[0].Operands[0];
					andWith = 2147483647uL;
				}
			}
			else if (array.Length == 4)
			{
				if (!array.Select((WasmInstruction i) => i.Mnemonic).SequenceEqual(new WasmMnemonic[4]
				{
					WasmMnemonic.I32Const,
					WasmMnemonic.I32And,
					WasmMnemonic.I32Const,
					WasmMnemonic.I32Add
				}))
				{
					LibLogger.WarnNewline("\t\tCouldn't calculate coefficients for " + text + ", got mnemonics " + string.Join(", ", array.Select((WasmInstruction i) => i.Mnemonic)) + ", expecting I32Const, I32And, I32Const, I32Add");
					continue;
				}
				andWith = (ulong)array[0].Operands[0];
				addConstant = (ulong)array[2].Operands[0];
			}
			else
			{
				if (!list.All(delegate(WasmInstruction d)
				{
					WasmMnemonic mnemonic2 = d.Mnemonic;
					return mnemonic2 == WasmMnemonic.LocalGet || mnemonic2 == WasmMnemonic.CallIndirect || mnemonic2 == WasmMnemonic.End;
				}))
				{
					if (list[list.Count - 1].Mnemonic == WasmMnemonic.End)
					{
						if (list[list.Count - 2].Mnemonic == WasmMnemonic.CallIndirect)
						{
							if (list[list.Count - 3].Mnemonic == WasmMnemonic.LocalGet)
							{
								if ((byte)list[list.Count - 3].Operands[0] == 0)
								{
									LibLogger.WarnNewline("\t\tAssuming index is not touched, but couldn't get a proper calculation for dynCall_" + text + ". Might cause issues later down the line.");
									andWith = 2147483647uL;
									addConstant = 0uL;
									goto IL_03c0;
								}
							}
						}
					}
					LibLogger.WarnNewline($"\t\tCouldn't calculate coefficients for {text}, got {array.Length} instructions; expecting 4");
					continue;
				}
				andWith = 2147483647uL;
				addConstant = 0uL;
			}
			goto IL_03c0;
			IL_03c0:
			DynCallCoefficients[text] = new WasmDynCallCoefficients
			{
				andWith = andWith,
				addConstant = addConstant
			};
		}
	}

	public override long MapVirtualAddressToRaw(ulong uiAddr)
	{
		if (uiAddr > (ulong)(_memoryBlock.Bytes.Length + _raw.Length))
		{
			throw new Exception("Way out of bounds");
		}
		return (long)uiAddr;
	}

	public override ulong MapRawAddressToVirtual(uint offset)
	{
		WasmDataSection dataSection = DataSection;
		if (offset > dataSection.Pointer && offset < dataSection.Pointer + (long)dataSection.Size)
		{
			WasmDataSegment wasmDataSegment = dataSection.DataEntries.FirstOrDefault((WasmDataSegment entry) => offset >= entry.FileOffset && offset < entry.FileOffset + entry.Data.Length);
			if (wasmDataSegment != null && wasmDataSegment.VirtualOffset < ulong.MaxValue)
			{
				return wasmDataSegment.VirtualOffset + (ulong)(offset - wasmDataSegment.FileOffset);
			}
		}
		return offset;
	}

	internal override object? ReadPrimitive(Type type, bool overrideArchCheck = false)
	{
		return _memoryBlock.ReadPrimitive(type, overrideArchCheck);
	}

	public override string ReadStringToNull(long offset)
	{
		return _memoryBlock.ReadStringToNull(offset);
	}

	public override ulong GetRVA(ulong pointer)
	{
		return pointer;
	}

	public override ulong[] GetAllExportedIl2CppFunctionPointers()
	{
		return Array.Empty<ulong>();
	}

	public override ulong GetVirtualAddressOfExportedFunctionByName(string toFind)
	{
		return 0uL;
	}

	public override byte[] GetEntirePrimaryExecutableSection()
	{
		return ((WasmCodeSection)Sections.First((WasmSection s) => s.Type == WasmSectionId.SEC_CODE)).RawSectionContent;
	}

	public override ulong GetVirtualAddressOfPrimaryExecutableSection()
	{
		return (ulong)Sections.First((WasmSection s) => s.Type == WasmSectionId.SEC_CODE).Pointer;
	}
}
