using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibCpp2IL.Elf;
using LibCpp2IL.Logging;

namespace LibCpp2IL.NintendoSwitch;

public sealed class NsoFile : Il2CppBinary
{
	private const ulong NSO_GLOBAL_OFFSET = 0uL;

	private byte[] _raw;

	private NsoHeader header;

	private NsoModHeader _modHeader;

	private bool isTextCompressed;

	private bool isRoDataCompressed;

	private bool isDataCompressed;

	private List<NsoSegmentHeader> segments = new List<NsoSegmentHeader>();

	private List<ElfDynamicEntry> dynamicEntries = new List<ElfDynamicEntry>();

	public ElfDynamicSymbol64[] SymbolTable;

	private bool isCompressed
	{
		get
		{
			if (!isTextCompressed && !isRoDataCompressed)
			{
				return isDataCompressed;
			}
			return true;
		}
	}

	public override long RawLength => _raw.Length;

	public NsoFile(MemoryStream input, long maxMetadataUsages)
		: base(input, maxMetadataUsages)
	{
		_raw = input.GetBuffer();
		is32Bit = false;
		InstructionSet = InstructionSet.ARM64;
		LibLogger.VerboseNewline("\tReading NSO Early Header...");
		header = new NsoHeader
		{
			Magic = ReadUInt32(),
			Version = ReadUInt32(),
			Reserved = ReadUInt32(),
			Flags = ReadUInt32()
		};
		if (header.Magic != 810505038)
		{
			throw new Exception($"NSO file should have a magic number of 0x304F534E, got 0x{header.Magic:X}");
		}
		LibLogger.VerboseNewline($"\tOK. Magic number is 0x{header.Magic:X}, version is {header.Version}.");
		isTextCompressed = (header.Flags & 1) != 0;
		isRoDataCompressed = (header.Flags & 2) != 0;
		isDataCompressed = (header.Flags & 4) != 0;
		LibLogger.VerboseNewline($"\tCompression flags: text: {isTextCompressed}, rodata: {isRoDataCompressed}, data: {isDataCompressed}.");
		header.TextSegment = new NsoSegmentHeader
		{
			FileOffset = ReadUInt32(),
			MemoryOffset = ReadUInt32(),
			DecompressedSize = ReadUInt32()
		};
		segments.Add(header.TextSegment);
		LibLogger.VerboseNewline("\tRead text segment header ok. Reading rodata segment header...");
		header.ModuleOffset = ReadUInt32();
		header.RoDataSegment = new NsoSegmentHeader
		{
			FileOffset = ReadUInt32(),
			MemoryOffset = ReadUInt32(),
			DecompressedSize = ReadUInt32()
		};
		segments.Add(header.RoDataSegment);
		LibLogger.VerboseNewline("\tRead rodata segment header OK. Reading data segment header...");
		header.ModuleFileSize = ReadUInt32();
		header.DataSegment = new NsoSegmentHeader
		{
			FileOffset = ReadUInt32(),
			MemoryOffset = ReadUInt32(),
			DecompressedSize = ReadUInt32()
		};
		segments.Add(header.DataSegment);
		LibLogger.VerboseNewline("\tRead data segment OK. Reading post-segment fields...");
		header.BssSize = ReadUInt32();
		header.DigestBuildID = ReadBytes(32);
		header.TextCompressedSize = ReadUInt32();
		header.RoDataCompressedSize = ReadUInt32();
		header.DataCompressedSize = ReadUInt32();
		header.NsoHeaderReserved = ReadBytes(28);
		LibLogger.VerboseNewline("\tRead post-segment fields OK. Reading Dynamic section and Api Info offsets...");
		header.APIInfo = new NsoRelativeExtent
		{
			RegionRoDataOffset = ReadUInt32(),
			RegionSize = ReadUInt32()
		};
		header.DynStr = new NsoRelativeExtent
		{
			RegionRoDataOffset = ReadUInt32(),
			RegionSize = ReadUInt32()
		};
		header.DynSym = new NsoRelativeExtent
		{
			RegionRoDataOffset = ReadUInt32(),
			RegionSize = ReadUInt32()
		};
		LibLogger.VerboseNewline("\tRead offsets OK. Reading hashes...");
		header.TextHash = ReadBytes(32);
		header.RoDataHash = ReadBytes(32);
		header.DataHash = ReadBytes(32);
		LibLogger.VerboseNewline("\tRead hashes ok.");
		if (!isCompressed)
		{
			ReadModHeader();
			ReadDynamicSection();
			ReadSymbolTable();
			ApplyRelocations();
		}
		LibLogger.VerboseNewline("\tNSO Read completed OK.");
	}

	private void ReadModHeader()
	{
		LibLogger.VerboseNewline("\tNSO is decompressed. Reading MOD segment header...");
		_modHeader = new NsoModHeader();
		base.Position = header.TextSegment.FileOffset + 4;
		_modHeader.ModOffset = ReadUInt32();
		base.Position = header.TextSegment.FileOffset + _modHeader.ModOffset + 4;
		_modHeader.DynamicOffset = ReadUInt32() + _modHeader.ModOffset;
		_modHeader.BssStart = ReadUInt32();
		_modHeader.BssEnd = ReadUInt32();
		_modHeader.BssSegment = new NsoSegmentHeader
		{
			FileOffset = _modHeader.BssStart,
			MemoryOffset = _modHeader.BssStart,
			DecompressedSize = _modHeader.BssEnd - _modHeader.BssStart
		};
		_modHeader.EhFrameHdrStart = ReadUInt32();
		_modHeader.EhFrameHdrEnd = ReadUInt32();
	}

	private void ReadDynamicSection()
	{
		LibLogger.VerboseNewline("\tReading NSO Dynamic section...");
		base.Position = MapVirtualAddressToRaw(_modHeader.DynamicOffset);
		uint num = (header.DataSegment.MemoryOffset + header.DataSegment.DecompressedSize - _modHeader.DynamicOffset) / 16;
		for (int i = 0; i < num; i++)
		{
			ElfDynamicEntry elfDynamicEntry = ReadClassAtRawAddr<ElfDynamicEntry>(-1L);
			if (elfDynamicEntry.Tag != ElfDynamicType.DT_NULL)
			{
				dynamicEntries.Add(elfDynamicEntry);
				continue;
			}
			break;
		}
	}

	private void ReadSymbolTable()
	{
		LibLogger.Verbose("\tReading NSO symbol table...");
		ElfDynamicEntry dynamicEntry = GetDynamicEntry(ElfDynamicType.DT_HASH);
		base.Position = MapVirtualAddressToRaw(dynamicEntry.Value);
		ReadUInt32();
		uint num = ReadUInt32();
		ElfDynamicEntry dynamicEntry2 = GetDynamicEntry(ElfDynamicType.DT_SYMTAB);
		SymbolTable = ReadClassArrayAtVirtualAddress<ElfDynamicSymbol64>((ulong)MapVirtualAddressToRaw(dynamicEntry2.Value), num);
		LibLogger.VerboseNewline($"\tGot {SymbolTable.Length} symbols");
	}

	private void ApplyRelocations()
	{
		ElfRelaEntry[] array;
		try
		{
			ElfDynamicEntry dynamicEntry = GetDynamicEntry(ElfDynamicType.DT_RELA);
			ElfDynamicEntry dynamicEntry2 = GetDynamicEntry(ElfDynamicType.DT_RELASZ);
			array = ReadClassArrayAtVirtualAddress<ElfRelaEntry>(dynamicEntry.Value, (long)(dynamicEntry2.Value / 24));
		}
		catch
		{
			return;
		}
		LibLogger.VerboseNewline($"\tApplying {array.Length} relocations from DT_RELA...");
		ElfRelaEntry[] array2 = array;
		foreach (ElfRelaEntry elfRelaEntry in array2)
		{
			switch (elfRelaEntry.Type)
			{
			case ElfRelocationType.R_AARCH64_ABS64:
			{
				ElfDynamicSymbol64 elfDynamicSymbol = SymbolTable[elfRelaEntry.Symbol];
				WriteWord((int)MapVirtualAddressToRaw(elfRelaEntry.Offset), elfDynamicSymbol.Value + elfRelaEntry.Addend);
				break;
			}
			case ElfRelocationType.R_AARCH64_RELATIVE:
				WriteWord((int)MapVirtualAddressToRaw(elfRelaEntry.Offset), elfRelaEntry.Addend);
				break;
			}
		}
	}

	public ElfDynamicEntry GetDynamicEntry(ElfDynamicType tag)
	{
		return dynamicEntries.Find((ElfDynamicEntry x) => x.Tag == tag);
	}

	public NsoFile Decompress()
	{
		if (!isCompressed)
		{
			return this;
		}
		LibLogger.InfoNewline("\tDecompressing NSO file...");
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(header.Magic);
		binaryWriter.Write(header.Version);
		binaryWriter.Write(header.Reserved);
		binaryWriter.Write(0);
		binaryWriter.Write(header.TextSegment.FileOffset);
		binaryWriter.Write(header.TextSegment.MemoryOffset);
		binaryWriter.Write(header.TextSegment.DecompressedSize);
		binaryWriter.Write(header.ModuleOffset);
		uint num = header.TextSegment.FileOffset + header.TextSegment.DecompressedSize;
		binaryWriter.Write(num);
		binaryWriter.Write(header.RoDataSegment.MemoryOffset);
		binaryWriter.Write(header.RoDataSegment.DecompressedSize);
		binaryWriter.Write(header.ModuleFileSize);
		binaryWriter.Write(num + header.RoDataSegment.DecompressedSize);
		binaryWriter.Write(header.DataSegment.MemoryOffset);
		binaryWriter.Write(header.DataSegment.DecompressedSize);
		binaryWriter.Write(header.BssSize);
		binaryWriter.Write(header.DigestBuildID);
		binaryWriter.Write(header.TextCompressedSize);
		binaryWriter.Write(header.RoDataCompressedSize);
		binaryWriter.Write(header.DataCompressedSize);
		binaryWriter.Write(header.NsoHeaderReserved);
		binaryWriter.Write(header.APIInfo.RegionRoDataOffset);
		binaryWriter.Write(header.APIInfo.RegionSize);
		binaryWriter.Write(header.DynStr.RegionRoDataOffset);
		binaryWriter.Write(header.DynStr.RegionSize);
		binaryWriter.Write(header.DynSym.RegionRoDataOffset);
		binaryWriter.Write(header.DynSym.RegionSize);
		binaryWriter.Write(header.TextHash);
		binaryWriter.Write(header.RoDataHash);
		binaryWriter.Write(header.DataHash);
		binaryWriter.BaseStream.Position = header.TextSegment.FileOffset;
		base.Position = header.TextSegment.FileOffset;
		byte[] buffer = ReadBytes((int)header.TextCompressedSize);
		if (isTextCompressed)
		{
			byte[] array = new byte[header.TextSegment.DecompressedSize];
			using (Lz4DecodeStream lz4DecodeStream = new Lz4DecodeStream(new MemoryStream(buffer)))
			{
				lz4DecodeStream.Read(array, 0, array.Length);
			}
			binaryWriter.Write(array);
		}
		else
		{
			binaryWriter.Write(buffer);
		}
		byte[] buffer2 = ReadBytes((int)header.RoDataCompressedSize);
		if (isRoDataCompressed)
		{
			byte[] array2 = new byte[header.RoDataSegment.DecompressedSize];
			using (Lz4DecodeStream lz4DecodeStream2 = new Lz4DecodeStream(new MemoryStream(buffer2)))
			{
				lz4DecodeStream2.Read(array2, 0, array2.Length);
			}
			binaryWriter.Write(array2);
		}
		else
		{
			binaryWriter.Write(buffer2);
		}
		byte[] buffer3 = ReadBytes((int)header.DataCompressedSize);
		if (isDataCompressed)
		{
			byte[] array3 = new byte[header.DataSegment.DecompressedSize];
			using (Lz4DecodeStream lz4DecodeStream3 = new Lz4DecodeStream(new MemoryStream(buffer3)))
			{
				lz4DecodeStream3.Read(array3, 0, array3.Length);
			}
			binaryWriter.Write(array3);
		}
		else
		{
			binaryWriter.Write(buffer3);
		}
		binaryWriter.Flush();
		memoryStream.Position = 0L;
		return new NsoFile(memoryStream, maxMetadataUsages);
	}

	public override byte GetByteAtRawAddress(ulong addr)
	{
		return _raw[addr];
	}

	public override long MapVirtualAddressToRaw(ulong addr)
	{
		NsoSegmentHeader nsoSegmentHeader = segments.FirstOrDefault((NsoSegmentHeader x) => addr >= x.MemoryOffset && addr <= x.MemoryOffset + x.DecompressedSize);
		if (nsoSegmentHeader == null)
		{
			throw new InvalidOperationException(string.Format("NSO: Address 0x{0:X} is not present in any of the segments. Known segment ends are (hex) {1}", addr, string.Join(", ", segments.Select((NsoSegmentHeader s) => (s.MemoryOffset + s.DecompressedSize).ToString("X")))));
		}
		return (long)(addr - nsoSegmentHeader.MemoryOffset + nsoSegmentHeader.FileOffset);
	}

	public override ulong MapRawAddressToVirtual(uint offset)
	{
		NsoSegmentHeader nsoSegmentHeader = segments.FirstOrDefault((NsoSegmentHeader x) => offset >= x.FileOffset && offset <= x.FileOffset + x.DecompressedSize);
		if (nsoSegmentHeader == null)
		{
			return 0uL;
		}
		return (ulong)(offset - nsoSegmentHeader.FileOffset) + (ulong)nsoSegmentHeader.MemoryOffset;
	}

	public override ulong GetRVA(ulong pointer)
	{
		return pointer;
	}

	public override byte[] GetRawBinaryContent()
	{
		return _raw;
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
		return _raw.Skip((int)header.TextSegment.FileOffset).Take((int)header.TextSegment.DecompressedSize).ToArray();
	}

	public override ulong GetVirtualAddressOfPrimaryExecutableSection()
	{
		return header.TextSegment.MemoryOffset;
	}
}
