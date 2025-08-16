using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibCpp2IL.Logging;
using LibCpp2IL.Metadata;
using LibCpp2IL.PE;

namespace LibCpp2IL.Elf;

public sealed class ElfFile : Il2CppBinary
{
	private byte[] _raw;

	private List<IElfProgramHeaderEntry> _elfProgramHeaderEntries;

	private readonly List<ElfSectionHeaderEntry> _elfSectionHeaderEntries;

	private ElfFileIdent? _elfFileIdent;

	private ElfFileHeader? _elfHeader;

	private readonly List<ElfDynamicEntry> _dynamicSection = new List<ElfDynamicEntry>();

	private readonly List<ElfSymbolTableEntry> _symbolTable = new List<ElfSymbolTableEntry>();

	private readonly Dictionary<string, ElfSymbolTableEntry> _exportTable = new Dictionary<string, ElfSymbolTableEntry>();

	private List<long>? _initializerPointers;

	private readonly List<(ulong start, ulong end)> relocationBlocks = new List<(ulong, ulong)>();

	private long _globalOffset;

	public override long RawLength => _raw.Length;

	public ElfFile(MemoryStream input, long maxMetadataUsages)
		: base(input, maxMetadataUsages)
	{
		_raw = input.GetBuffer();
		LibLogger.Verbose("\tReading Elf File Ident...");
		DateTime now = DateTime.Now;
		ReadAndValidateIdent();
		bool flag = _elfFileIdent.Endianness == 2;
		LibLogger.VerboseNewline($"OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		LibLogger.VerboseNewline("\tBinary is " + (is32Bit ? "32-bit" : "64-bit") + " and " + (flag ? "big-endian" : "little-endian") + ".");
		if (flag)
		{
			SetBigEndian();
		}
		LibLogger.Verbose("\tReading and validating full ELF header...");
		now = DateTime.Now;
		ReadHeader();
		LibLogger.VerboseNewline($"OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		LibLogger.VerboseNewline($"\tElf File contains instructions of type {InstructionSet}");
		LibLogger.Verbose("\tReading ELF program header table...");
		now = DateTime.Now;
		ReadProgramHeaderTable();
		LibLogger.VerboseNewline($"Read {_elfProgramHeaderEntries.Count} OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		LibLogger.VerboseNewline("\tReading ELF section header table and names...");
		now = DateTime.Now;
		_elfSectionHeaderEntries = ReadClassArrayAtRawAddr<ElfSectionHeaderEntry>(_elfHeader.pSectionHeader, _elfHeader.SectionHeaderEntryCount).ToList();
		if (_elfHeader.SectionNameSectionOffset >= 0 && _elfHeader.SectionNameSectionOffset < _elfSectionHeaderEntries.Count)
		{
			ulong rawAddress = _elfSectionHeaderEntries[_elfHeader.SectionNameSectionOffset].RawAddress;
			foreach (ElfSectionHeaderEntry elfSectionHeaderEntry2 in _elfSectionHeaderEntries)
			{
				elfSectionHeaderEntry2.Name = ReadStringToNull(rawAddress + elfSectionHeaderEntry2.NameOffset);
				LibLogger.VerboseNewline($"\t\t-Name for section at 0x{elfSectionHeaderEntry2.RawAddress:X} is {elfSectionHeaderEntry2.Name}");
			}
		}
		LibLogger.VerboseNewline($"\tRead {_elfSectionHeaderEntries.Count} OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		ElfSectionHeaderEntry elfSectionHeaderEntry = _elfSectionHeaderEntries.FirstOrDefault((ElfSectionHeaderEntry s) => s.Name == ".text");
		if (elfSectionHeaderEntry != null)
		{
			_globalOffset = (long)(elfSectionHeaderEntry.VirtualAddress - elfSectionHeaderEntry.RawAddress);
		}
		else
		{
			IElfProgramHeaderEntry elfProgramHeaderEntry = _elfProgramHeaderEntries.First((IElfProgramHeaderEntry p) => (p.Flags & ElfProgramHeaderFlags.PF_X) != 0);
			_globalOffset = (long)(elfProgramHeaderEntry.VirtualAddress - elfProgramHeaderEntry.RawAddress);
		}
		LibLogger.VerboseNewline($"\tELF global offset is 0x{_globalOffset:X}");
		IElfProgramHeaderEntry programHeaderOfType = GetProgramHeaderOfType(ElfProgramEntryType.PT_DYNAMIC);
		if (programHeaderOfType != null)
		{
			_dynamicSection = ReadClassArrayAtRawAddr<ElfDynamicEntry>(programHeaderOfType.RawAddress, programHeaderOfType.RawSize / (ulong)(is32Bit ? 8 : 16)).ToList();
		}
		LibLogger.VerboseNewline("\tFinding Relocations...");
		now = DateTime.Now;
		ProcessRelocations();
		LibLogger.VerboseNewline($"OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		LibLogger.VerboseNewline("\tProcessing Symbols...");
		now = DateTime.Now;
		ProcessSymbols();
		LibLogger.VerboseNewline($"\tOK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		LibLogger.Verbose("\tProcessing Initializers...");
		now = DateTime.Now;
		ProcessInitializers();
		LibLogger.VerboseNewline($"Got {_initializerPointers.Count} OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
	}

	private void ReadAndValidateIdent()
	{
		_elfFileIdent = ReadClassAtRawAddr<ElfFileIdent>(0L);
		if (_elfFileIdent.Magic != 1179403647)
		{
			throw new FormatException("ERROR: Magic number mismatch.");
		}
		if (_elfFileIdent.Architecture == 1)
		{
			is32Bit = true;
		}
		else if (_elfFileIdent.Architecture != 2)
		{
			throw new FormatException($"Invalid arch number (expecting 1 or 2): {_elfFileIdent.Architecture}");
		}
		if (_elfFileIdent.Version != 1)
		{
			throw new FormatException($"ELF Version is not 1? File header has version {_elfFileIdent.Version}");
		}
	}

	private void ReadHeader()
	{
		_elfHeader = ReadClassAtRawAddr<ElfFileHeader>(16L);
		InstructionSet = _elfHeader.Machine switch
		{
			3 => InstructionSet.X86_32, 
			62 => InstructionSet.X86_64, 
			40 => InstructionSet.ARM32, 
			183 => InstructionSet.ARM64, 
			_ => throw new NotImplementedException($"ELF Machine {_elfHeader.Machine} not implemented"), 
		};
		if (_elfHeader.Version != 1)
		{
			throw new FormatException($"Full ELF header specifies version {_elfHeader.Version}, only supported version is 1.");
		}
	}

	private void ReadProgramHeaderTable()
	{
		_elfProgramHeaderEntries = (is32Bit ? ReadClassArrayAtRawAddr<ElfProgramHeaderEntry32>(_elfHeader.pProgramHeader, _elfHeader.ProgramHeaderEntryCount).Cast<IElfProgramHeaderEntry>().ToList() : ReadClassArrayAtRawAddr<ElfProgramHeaderEntry64>(_elfHeader.pProgramHeader, _elfHeader.ProgramHeaderEntryCount).Cast<IElfProgramHeaderEntry>().ToList());
	}

	private IElfProgramHeaderEntry? GetProgramHeaderOfType(ElfProgramEntryType type)
	{
		return _elfProgramHeaderEntries.FirstOrDefault((IElfProgramHeaderEntry p) => p.Type == type);
	}

	private IEnumerable<ElfSectionHeaderEntry> GetSections(ElfSectionEntryType type)
	{
		return _elfSectionHeaderEntries.Where((ElfSectionHeaderEntry s) => s.Type == type);
	}

	private ElfSectionHeaderEntry? GetSingleSection(ElfSectionEntryType type)
	{
		return GetSections(type).FirstOrDefault();
	}

	private ElfDynamicEntry? GetDynamicEntryOfType(ElfDynamicType type)
	{
		return _dynamicSection.FirstOrDefault((ElfDynamicEntry d) => d.Tag == type);
	}

	private void ProcessRelocations()
	{
		try
		{
			HashSet<ElfRelocation> hashSet = new HashSet<ElfRelocation>();
			List<ulong> list = new List<ulong>();
			foreach (ElfSectionHeaderEntry section in GetSections(ElfSectionEntryType.SHT_REL))
			{
				ulong relatedTablePointer = _elfSectionHeaderEntries[section.LinkedSectionIndex].RawAddress;
				ElfRelEntry[] array = ReadClassArrayAtRawAddr<ElfRelEntry>(section.RawAddress, section.Size / (ulong)section.EntrySize);
				LibLogger.VerboseNewline($"\t\t-Got {array.Length} from REL section {section.Name}");
				relocationBlocks.Add((section.RawAddress, section.RawAddress + section.Size));
				list.Add(section.RawAddress);
				hashSet.UnionWith(array.Select((ElfRelEntry r) => new ElfRelocation(this, r, relatedTablePointer)));
			}
			foreach (ElfSectionHeaderEntry section2 in GetSections(ElfSectionEntryType.SHT_RELA))
			{
				ulong relatedTablePointer2 = _elfSectionHeaderEntries[section2.LinkedSectionIndex].RawAddress;
				ElfRelaEntry[] array2 = ReadClassArrayAtRawAddr<ElfRelaEntry>(section2.RawAddress, section2.Size / (ulong)section2.EntrySize);
				LibLogger.VerboseNewline($"\t\t-Got {array2.Length} from RELA section {section2.Name}");
				relocationBlocks.Add((section2.RawAddress, section2.RawAddress + section2.Size));
				hashSet.UnionWith(array2.Select((ElfRelaEntry r) => new ElfRelocation(this, r, relatedTablePointer2)));
			}
			ElfDynamicEntry dynamicEntryOfType = GetDynamicEntryOfType(ElfDynamicType.DT_REL);
			if (dynamicEntryOfType != null)
			{
				uint num = (uint)MapVirtualAddressToRaw(dynamicEntryOfType.Value);
				if (!list.Contains(num))
				{
					ulong value = GetDynamicEntryOfType(ElfDynamicType.DT_RELSZ).Value;
					int num2 = (int)(value / GetDynamicEntryOfType(ElfDynamicType.DT_RELENT).Value);
					ElfRelEntry[] array3 = ReadClassArrayAtRawAddr<ElfRelEntry>(num, num2);
					LibLogger.VerboseNewline($"\t\t-Got {array3.Length} from dynamic REL section.");
					ulong pSymTab = GetDynamicEntryOfType(ElfDynamicType.DT_SYMTAB).Value;
					relocationBlocks.Add((num, num + value));
					hashSet.UnionWith(array3.Select((ElfRelEntry r) => new ElfRelocation(this, r, pSymTab)));
				}
			}
			ElfDynamicEntry dynamicEntryOfType2 = GetDynamicEntryOfType(ElfDynamicType.DT_RELA);
			if (dynamicEntryOfType2 != null)
			{
				ulong value2 = GetDynamicEntryOfType(ElfDynamicType.DT_RELASZ).Value;
				int num3 = (int)(value2 / GetDynamicEntryOfType(ElfDynamicType.DT_RELAENT).Value);
				uint num4 = (uint)MapVirtualAddressToRaw(dynamicEntryOfType2.Value);
				ElfRelaEntry[] array4 = ReadClassArrayAtRawAddr<ElfRelaEntry>(num4, num3);
				LibLogger.VerboseNewline($"\t\t-Got {array4.Length} from dynamic RELA section.");
				ulong pSymTab2 = GetDynamicEntryOfType(ElfDynamicType.DT_SYMTAB).Value;
				relocationBlocks.Add((num4, num4 + value2));
				hashSet.UnionWith(array4.Select((ElfRelaEntry r) => new ElfRelocation(this, r, pSymTab2)));
			}
			ulong num5 = (ulong)(is32Bit ? LibCpp2ILUtils.VersionAwareSizeOf(typeof(ElfDynamicSymbol32), dontCheckVersionAttributes: true, downsize: false) : LibCpp2ILUtils.VersionAwareSizeOf(typeof(ElfDynamicSymbol64), dontCheckVersionAttributes: true, downsize: false));
			LibLogger.Verbose($"\t-Now Processing {hashSet.Count} relocations...");
			foreach (ElfRelocation item in hashSet)
			{
				ulong num6 = item.pRelatedSymbolTable + item.IndexInSymbolTable * num5;
				ulong value3;
				try
				{
					IElfDynamicSymbol elfDynamicSymbol2;
					if (!is32Bit)
					{
						IElfDynamicSymbol elfDynamicSymbol = ReadClassAtRawAddr<ElfDynamicSymbol64>((long)num6);
						elfDynamicSymbol2 = elfDynamicSymbol;
					}
					else
					{
						IElfDynamicSymbol elfDynamicSymbol = ReadClassAtRawAddr<ElfDynamicSymbol32>((long)num6);
						elfDynamicSymbol2 = elfDynamicSymbol;
					}
					value3 = elfDynamicSymbol2.Value;
				}
				catch
				{
					LibLogger.ErrorNewline($"Exception reading dynamic symbol for rel of type {item.Type} at pointer 0x{num6:X} (length of file is 0x{RawLength:X}, pointer - length is 0x{num6 - (ulong)RawLength:X})");
					throw;
				}
				long num7;
				try
				{
					num7 = MapVirtualAddressToRaw(item.Offset);
				}
				catch (InvalidOperationException)
				{
					continue;
				}
				ulong num8 = item.Addend ?? ReadClassAtRawAddr<ulong>(num7);
				ElfRelocationType type = item.Type;
				InstructionSet instructionSet = InstructionSet;
				(ulong, bool) tuple;
				switch (type)
				{
				case ElfRelocationType.R_ARM_ABS32:
					if (instructionSet != 0)
					{
						if (instructionSet != InstructionSet.ARM32)
						{
							goto default;
						}
						tuple = (value3 + num8, true);
						break;
					}
					tuple = (value3 + num8 - item.Offset, true);
					break;
				case ElfRelocationType.R_ARM_REL32:
					if (instructionSet != InstructionSet.ARM32)
					{
						goto default;
					}
					tuple = (value3 + item.Offset - num8, true);
					break;
				case ElfRelocationType.R_ARM_COPY:
					if (instructionSet != InstructionSet.ARM32)
					{
						goto default;
					}
					tuple = (value3, true);
					break;
				case ElfRelocationType.R_AARCH64_ABS64:
					if (instructionSet != InstructionSet.ARM64)
					{
						goto default;
					}
					tuple = (value3 + num8, true);
					break;
				case ElfRelocationType.R_AARCH64_PREL64:
					if (instructionSet != InstructionSet.ARM64)
					{
						goto default;
					}
					tuple = (value3 + num8 - item.Offset, true);
					break;
				case ElfRelocationType.R_AARCH64_GLOB_DAT:
					if (instructionSet != InstructionSet.ARM64)
					{
						goto default;
					}
					tuple = (value3 + num8, true);
					break;
				case ElfRelocationType.R_AARCH64_JUMP_SLOT:
					if (instructionSet != InstructionSet.ARM64)
					{
						goto default;
					}
					tuple = (value3 + num8, true);
					break;
				case ElfRelocationType.R_AARCH64_RELATIVE:
					if (instructionSet != InstructionSet.ARM64)
					{
						goto default;
					}
					tuple = (value3 + num8, true);
					break;
				case ElfRelocationType.R_386_32:
					if (instructionSet != 0)
					{
						if (instructionSet != InstructionSet.X86_64)
						{
							goto default;
						}
						tuple = (value3 + num8, true);
						break;
					}
					tuple = (value3 + num8, true);
					break;
				case ElfRelocationType.R_386_GLOB_DAT:
					if (instructionSet != 0)
					{
						goto default;
					}
					tuple = (value3, true);
					break;
				case ElfRelocationType.R_386_JMP_SLOT:
					if (instructionSet != 0)
					{
						goto default;
					}
					tuple = (value3, true);
					break;
				case ElfRelocationType.R_AMD64_RELATIVE:
					if (instructionSet != InstructionSet.X86_64)
					{
						goto default;
					}
					tuple = (num8, true);
					break;
				default:
					tuple = (0uL, false);
					break;
				}
				(ulong, bool) tuple2 = tuple;
				if (tuple2.Item2)
				{
					WriteWord((int)num7, tuple2.Item1);
				}
			}
		}
		catch
		{
			LibLogger.Info("Exception during relocation mapping!");
			throw;
		}
	}

	private void ProcessSymbols()
	{
		List<(ulong, ulong, ulong)> list = new List<(ulong, ulong, ulong)>();
		ElfSectionHeaderEntry singleSection = GetSingleSection(ElfSectionEntryType.SHT_STRTAB);
		if (singleSection != null)
		{
			ElfSectionHeaderEntry singleSection2 = GetSingleSection(ElfSectionEntryType.SHT_SYMTAB);
			if (singleSection2 != null)
			{
				LibLogger.VerboseNewline($"\t\t-Found .symtab at 0x{singleSection2.RawAddress:X}");
				list.Add((singleSection2.RawAddress, singleSection2.Size / (ulong)singleSection2.EntrySize, singleSection.RawAddress));
			}
			ElfSectionHeaderEntry singleSection3 = GetSingleSection(ElfSectionEntryType.SHT_DYNSYM);
			if (singleSection3 != null)
			{
				LibLogger.VerboseNewline($"\t\t-Found .dynsym at 0x{singleSection3.RawAddress:X}");
				list.Add((singleSection3.RawAddress, singleSection3.Size / (ulong)singleSection3.EntrySize, singleSection.RawAddress));
			}
		}
		ElfDynamicEntry dynamicEntryOfType = GetDynamicEntryOfType(ElfDynamicType.DT_STRTAB);
		if (dynamicEntryOfType != null)
		{
			ElfDynamicEntry dynamicSymTab = GetDynamicEntryOfType(ElfDynamicType.DT_SYMTAB);
			if (dynamicSymTab != null)
			{
				ulong value = (from x in _dynamicSection
					where x.Value > dynamicSymTab.Value
					orderby x.Value
					select x).First().Value;
				ulong num = (ulong)LibCpp2ILUtils.VersionAwareSizeOf(is32Bit ? typeof(ElfDynamicSymbol32) : typeof(ElfDynamicSymbol64), dontCheckVersionAttributes: true, downsize: false);
				ulong num2 = (ulong)MapVirtualAddressToRaw(dynamicSymTab.Value);
				LibLogger.VerboseNewline($"\t\t-Found DT_SYMTAB at 0x{num2:X}");
				list.Add((num2, (value - dynamicSymTab.Value) / num, dynamicEntryOfType.Value));
			}
		}
		_symbolTable.Clear();
		_exportTable.Clear();
		foreach (var item4 in list)
		{
			ulong item = item4.Item1;
			ulong item2 = item4.Item2;
			ulong item3 = item4.Item3;
			List<IElfDynamicSymbol> list2 = (is32Bit ? ReadClassArrayAtRawAddr<ElfDynamicSymbol32>(item, item2).Cast<IElfDynamicSymbol>().ToList() : ReadClassArrayAtRawAddr<ElfDynamicSymbol64>(item, item2).Cast<IElfDynamicSymbol>().ToList());
			LibLogger.VerboseNewline($"\t\t-Found {list2.Count} symbols in table at 0x{item:X}");
			foreach (IElfDynamicSymbol item5 in list2)
			{
				string text;
				try
				{
					text = ReadStringToNull(item3 + item5.NameOffset);
				}
				catch (ArgumentOutOfRangeException)
				{
					continue;
				}
				ElfSymbolTableEntry.ElfSymbolEntryType type = ((item5.Shndx == 0) ? ElfSymbolTableEntry.ElfSymbolEntryType.IMPORT : ((item5.Type != ElfDynamicSymbolType.STT_FUNC) ? ((item5.Type == ElfDynamicSymbolType.STT_OBJECT || item5.Type == ElfDynamicSymbolType.STT_COMMON) ? ElfSymbolTableEntry.ElfSymbolEntryType.NAME : ElfSymbolTableEntry.ElfSymbolEntryType.UNKNOWN) : ElfSymbolTableEntry.ElfSymbolEntryType.FUNCTION));
				ulong value2 = item5.Value;
				ElfSymbolTableEntry elfSymbolTableEntry = new ElfSymbolTableEntry
				{
					Name = text,
					Type = type,
					VirtualAddress = value2
				};
				_symbolTable.Add(elfSymbolTableEntry);
				if (item5.Shndx != 0)
				{
					Extensions.TryAdd(_exportTable, text, elfSymbolTableEntry);
				}
			}
		}
	}

	private void ProcessInitializers()
	{
		ElfDynamicEntry dynamicEntryOfType = GetDynamicEntryOfType(ElfDynamicType.DT_INIT_ARRAY);
		if (dynamicEntryOfType == null)
		{
			return;
		}
		ElfDynamicEntry dynamicEntryOfType2 = GetDynamicEntryOfType(ElfDynamicType.DT_INIT_ARRAYSZ);
		if (dynamicEntryOfType2 != null)
		{
			ulong offset = (ulong)MapVirtualAddressToRaw(dynamicEntryOfType.Value);
			ulong count = dynamicEntryOfType2.Value / (ulong)(is32Bit ? 4 : 8);
			ulong[] source = ReadClassArrayAtRawAddr<ulong>(offset, count);
			ElfDynamicEntry dynamicEntryOfType3 = GetDynamicEntryOfType(ElfDynamicType.DT_INIT);
			if (dynamicEntryOfType3 != null)
			{
				source = source.Append(dynamicEntryOfType3.Value).ToArray();
			}
			_initializerPointers = source.Select(MapVirtualAddressToRaw).ToList();
		}
	}

	public (ulong codeReg, ulong metaReg) FindCodeAndMetadataReg()
	{
		LibLogger.Verbose("\tChecking ELF Symbol Table for code and/or meta reg...");
		ulong num = 0uL;
		ulong num2 = 0uL;
		ElfSymbolTableEntry elfSymbolTableEntry = _symbolTable.FirstOrDefault((ElfSymbolTableEntry s) => s.Name.Contains("g_CodeRegistration"));
		if (elfSymbolTableEntry != null)
		{
			num = elfSymbolTableEntry.VirtualAddress;
		}
		ElfSymbolTableEntry elfSymbolTableEntry2 = _symbolTable.FirstOrDefault((ElfSymbolTableEntry s) => s.Name.Contains("g_MetadataRegistration"));
		if (elfSymbolTableEntry2 != null)
		{
			num2 = elfSymbolTableEntry2.VirtualAddress;
		}
		if (num != 0L && num2 != 0L)
		{
			LibLogger.VerboseNewline("Found them.");
			return (codeReg: num, metaReg: num2);
		}
		LibLogger.VerboseNewline("Didn't find them, scanning binary...");
		switch (InstructionSet)
		{
		case InstructionSet.ARM32:
			if (LibCpp2IlMain.MetadataVersion < 24.2f)
			{
				return FindCodeAndMetadataRegArm32();
			}
			break;
		case InstructionSet.ARM64:
			if (LibCpp2IlMain.MetadataVersion < 24.2f)
			{
				return FindCodeAndMetadataRegArm64();
			}
			break;
		}
		return FindCodeAndMetadataRegDefaultBehavior();
	}

	private (ulong codeReg, ulong metaReg) FindCodeAndMetadataRegArm32()
	{
		byte[] first = new byte[8] { 0, 0, 143, 224, 1, 16, 143, 224 };
		byte[] first2 = new byte[3] { 16, 159, 229 };
		LibLogger.VerboseNewline($"\tARM-32 MODE: Checking {_initializerPointers.Count} initializer pointers...");
		foreach (long initializerPointer in _initializerPointers)
		{
			byte[] array = ReadByteArrayAtRawAddress(initializerPointer, 24);
			if (!first.SequenceEqual(array.Skip(16)) || !first2.SequenceEqual(array.Skip(9).Take(3)))
			{
				continue;
			}
			long offset = array[8] + initializerPointer + 16;
			uint num = ReadClassAtRawAddr<uint>(offset);
			num += (uint)((int)initializerPointer + 28);
			uint[] array2 = ReadClassArrayAtRawAddr<uint>(num, 10L);
			if (array2[6].Bits(24, 8) != 234)
			{
				continue;
			}
			uint[] array3 = new uint[3];
			bool flag = false;
			for (uint num2 = 0u; num2 <= 2; num2++)
			{
				if (flag)
				{
					break;
				}
				var (num3, num4) = ArmUtils.GetOperandsForLiteralLdr(array2[num2]);
				if (num3 > 2 || num4 == 0)
				{
					flag = true;
				}
				else
				{
					array3[num3] = num4 + num2 * 4 + 8;
				}
			}
			if (flag)
			{
				continue;
			}
			uint[] array4 = new uint[3];
			for (uint num5 = 3u; num5 <= 5; num5++)
			{
				if (flag)
				{
					break;
				}
				var (num6, num7, num8) = ArmUtils.GetOperandsForRegisterAdd(array2[num5]);
				var (num9, num10, num11) = ArmUtils.GetOperandsForRegisterLdr(array2[num5]);
				if (num7 == 15 && num6 == num8 && num6 <= 2)
				{
					array4[num6] = num + num5 * 4 + array2[array3[num6] / 4] + 8;
				}
				else if (num10 == 15 && num9 == num11 && num9 <= 2)
				{
					uint num12 = num + num5 * 4 + array2[array3[num9] / 4] + 8;
					array4[num9] = ReadClassAtVirtualAddress<uint>(num12);
				}
				else
				{
					flag = true;
				}
			}
			if (flag)
			{
				LibLogger.VerboseNewline($"\t\tInitializer function at 0x{initializerPointer:X} is probably NOT the il2cpp initializer.");
				continue;
			}
			LibLogger.VerboseNewline($"\t\tFound valid sequence of bytes for il2cpp initializer function at 0x{initializerPointer:X}.");
			return (codeReg: array4[0], metaReg: array4[1]);
		}
		return (codeReg: 0uL, metaReg: 0uL);
	}

	private (ulong codeReg, ulong metaReg) FindCodeAndMetadataRegArm64()
	{
		LibLogger.VerboseNewline($"\tARM-64 MODE: Checking {_initializerPointers.Count} initializer pointers...");
		foreach (long initializerPointer in _initializerPointers)
		{
			List<uint> list = MiniArm64Decompiler.ReadFunctionAtRawAddress(this, (uint)initializerPointer, 7u);
			if (!MiniArm64Decompiler.IsB(list[list.Count - 1]))
			{
				continue;
			}
			Dictionary<uint, ulong> addressesLoadedIntoRegisters = MiniArm64Decompiler.GetAddressesLoadedIntoRegisters(list, (ulong)(_globalOffset + initializerPointer), this);
			if (addressesLoadedIntoRegisters.Count == 2 && addressesLoadedIntoRegisters.ContainsKey(0u) && addressesLoadedIntoRegisters.TryGetValue(1u, out var value))
			{
				List<uint> list2 = MiniArm64Decompiler.ReadFunctionAtRawAddress(this, (uint)MapVirtualAddressToRaw(value), 7u);
				if (!MiniArm64Decompiler.IsB(list2[list2.Count - 1]))
				{
					continue;
				}
				addressesLoadedIntoRegisters = MiniArm64Decompiler.GetAddressesLoadedIntoRegisters(list2, value, this);
			}
			if (addressesLoadedIntoRegisters.Count == 3 && addressesLoadedIntoRegisters.TryGetValue(0u, out var value2) && addressesLoadedIntoRegisters.TryGetValue(1u, out value) && addressesLoadedIntoRegisters.ContainsKey(2u))
			{
				LibLogger.VerboseNewline($"\t\tFound valid sequence of bytes for il2cpp initializer function at 0x{initializerPointer:X}.");
				return (codeReg: value2, metaReg: value);
			}
			LibLogger.VerboseNewline(string.Format("\t\tInitializer function at 0x{0:X} is probably NOT the il2cpp initializer - got {1} register values with keys {2}.", initializerPointer, addressesLoadedIntoRegisters.Count, string.Join(", ", addressesLoadedIntoRegisters.Keys)));
		}
		return (codeReg: 0uL, metaReg: 0uL);
	}

	private (ulong codeReg, ulong metaReg) FindCodeAndMetadataRegDefaultBehavior()
	{
		LibLogger.VerboseNewline("Searching for il2cpp structures in an ELF binary using non-arch-specific method...");
		var (item, item2) = PlusSearch(LibCpp2IlMain.TheMetadata.methodDefs.Count((Il2CppMethodDefinition x) => x.methodIndex >= 0), LibCpp2IlMain.TheMetadata.typeDefs.Length);
		return (codeReg: item, metaReg: item2);
	}

	public override long MapVirtualAddressToRaw(ulong addr)
	{
		IElfProgramHeaderEntry elfProgramHeaderEntry = _elfProgramHeaderEntries.FirstOrDefault((IElfProgramHeaderEntry x) => addr >= x.VirtualAddress && addr <= x.VirtualAddress + x.VirtualSize);
		if (elfProgramHeaderEntry == null)
		{
			throw new InvalidOperationException($"No entry in the Elf PHT contains virtual address 0x{addr:X}");
		}
		return (long)(addr - (elfProgramHeaderEntry.VirtualAddress - elfProgramHeaderEntry.RawAddress));
	}

	public override ulong MapRawAddressToVirtual(uint offset)
	{
		if (relocationBlocks.Any(((ulong start, ulong end) b) => b.start <= offset && b.end >= offset))
		{
			throw new InvalidOperationException("Attempt to map a relocation block to a virtual address");
		}
		IElfProgramHeaderEntry elfProgramHeaderEntry = _elfProgramHeaderEntries.First((IElfProgramHeaderEntry x) => offset >= x.RawAddress && offset < x.RawAddress + x.RawSize);
		return elfProgramHeaderEntry.VirtualAddress + offset - elfProgramHeaderEntry.RawAddress;
	}

	public override byte GetByteAtRawAddress(ulong addr)
	{
		return _raw[addr];
	}

	public override ulong GetRVA(ulong pointer)
	{
		return pointer - (ulong)_globalOffset;
	}

	public override byte[] GetRawBinaryContent()
	{
		return _raw;
	}

	public override ulong[] GetAllExportedIl2CppFunctionPointers()
	{
		return (from p in _exportTable
			where p.Key.StartsWith("il2cpp_")
			select p.Value.VirtualAddress into va
			where va != 0
			select va).ToArray();
	}

	public override ulong GetVirtualAddressOfExportedFunctionByName(string toFind)
	{
		if (!_exportTable.TryGetValue(toFind, out ElfSymbolTableEntry value))
		{
			return 0uL;
		}
		return value.VirtualAddress;
	}

	public override ulong GetVirtualAddressOfPrimaryExecutableSection()
	{
		return _elfSectionHeaderEntries.FirstOrDefault((ElfSectionHeaderEntry s) => s.Name == ".text")?.VirtualAddress ?? 0;
	}

	public override byte[] GetEntirePrimaryExecutableSection()
	{
		ElfSectionHeaderEntry elfSectionHeaderEntry = _elfSectionHeaderEntries.FirstOrDefault((ElfSectionHeaderEntry s) => s.Name == ".text");
		if (elfSectionHeaderEntry == null)
		{
			return Array.Empty<byte>();
		}
		return GetRawBinaryContent().SubArray((int)elfSectionHeaderEntry.RawAddress, (int)elfSectionHeaderEntry.Size);
	}
}
