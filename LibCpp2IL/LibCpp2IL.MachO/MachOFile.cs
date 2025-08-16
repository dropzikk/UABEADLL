using System;
using System.IO;
using System.Linq;
using LibCpp2IL.Logging;

namespace LibCpp2IL.MachO;

public class MachOFile : Il2CppBinary
{
	private byte[] _raw;

	private readonly MachOHeader _header;

	private readonly MachOLoadCommand[] _loadCommands;

	private readonly MachOSegmentCommand[] Segments64;

	private readonly MachOSection[] Sections64;

	public override long RawLength => _raw.Length;

	public MachOFile(MemoryStream input, long maxMetadataUsages)
		: base(input, maxMetadataUsages)
	{
		_raw = input.GetBuffer();
		LibLogger.Verbose("\tReading Mach-O header...");
		_header = new MachOHeader();
		_header.Read(this);
		switch (_header.Magic)
		{
		case 4277009102u:
			LibLogger.Verbose("Mach-O is 32-bit...");
			is32Bit = true;
			break;
		case 4277009103u:
			LibLogger.Verbose("Mach-O is 64-bit...");
			is32Bit = false;
			break;
		default:
			throw new Exception($"Unknown Mach-O Magic: {_header.Magic}");
		}
		switch (_header.CpuType)
		{
		case MachOCpuType.CPU_TYPE_I386:
			LibLogger.VerboseNewline("Mach-O contains x86_32 instructions.");
			InstructionSet = InstructionSet.X86_32;
			break;
		case MachOCpuType.CPU_TYPE_X86_64:
			LibLogger.VerboseNewline("Mach-O contains x86_64 instructions.");
			InstructionSet = InstructionSet.X86_64;
			break;
		case MachOCpuType.CPU_TYPE_ARM:
			LibLogger.VerboseNewline("Mach-O contains ARM (32-bit) instructions.");
			InstructionSet = InstructionSet.ARM32;
			break;
		case MachOCpuType.CPU_TYPE_ARM64:
			LibLogger.VerboseNewline("Mach-O contains ARM64 instructions.");
			InstructionSet = InstructionSet.ARM64;
			break;
		default:
			throw new Exception($"Don't know how to handle a Mach-O CPU Type of {_header.CpuType}");
		}
		if (_header.Magic == 4277009102u)
		{
			LibLogger.ErrorNewline("32-bit MACH-O files have not been tested! Please report any issues.");
		}
		else
		{
			LibLogger.WarnNewline("Mach-O Support is experimental. Please open an issue if anything seems incorrect.");
		}
		LibLogger.Verbose("\tReading Mach-O load commands...");
		_loadCommands = new MachOLoadCommand[_header.NumLoadCommands];
		for (int i = 0; i < _header.NumLoadCommands; i++)
		{
			_loadCommands[i] = new MachOLoadCommand();
			_loadCommands[i].Read(this);
		}
		LibLogger.VerboseNewline($"Read {_loadCommands.Length} load commands.");
		Segments64 = (from c in _loadCommands
			where c.Command == LoadCommandId.LC_SEGMENT_64
			select c.CommandData).Cast<MachOSegmentCommand>().ToArray();
		Sections64 = Segments64.SelectMany((MachOSegmentCommand s) => s.Sections).ToArray();
		LibLogger.VerboseNewline($"\tMach-O contains {Segments64.Length} segments, split into {Sections64.Length} sections.");
	}

	public override byte GetByteAtRawAddress(ulong addr)
	{
		return _raw[addr];
	}

	public override long MapVirtualAddressToRaw(ulong uiAddr)
	{
		MachOSection machOSection = Sections64.FirstOrDefault((MachOSection s) => s.Address <= uiAddr && uiAddr < s.Address + s.Size);
		if (machOSection == null)
		{
			throw new Exception($"Could not find section for virtual address 0x{uiAddr:X}. Lowest section address is 0x{Sections64.Min((MachOSection s) => s.Address):X}, highest section address is 0x{Sections64.Max((MachOSection s) => s.Address + s.Size):X}");
		}
		return (long)(machOSection.Offset + (uiAddr - machOSection.Address));
	}

	public override ulong MapRawAddressToVirtual(uint offset)
	{
		MachOSection machOSection = Sections64.FirstOrDefault((MachOSection s) => s.Offset <= offset && offset < s.Offset + s.Size);
		if (machOSection == null)
		{
			throw new Exception($"Could not find section for raw address 0x{offset:X}");
		}
		return machOSection.Address + (offset - machOSection.Offset);
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

	private MachOSection GetTextSection64()
	{
		return Sections64.FirstOrDefault((MachOSection s) => s.SectionName == "__text") ?? throw new Exception("Could not find __text section");
	}

	public override byte[] GetEntirePrimaryExecutableSection()
	{
		MachOSection textSection = GetTextSection64();
		return _raw.SubArray((int)textSection.Offset, (int)textSection.Size);
	}

	public override ulong GetVirtualAddressOfPrimaryExecutableSection()
	{
		return GetTextSection64().Address;
	}
}
