using LibCpp2IL.PE;

namespace LibCpp2IL.Elf;

public interface IElfProgramHeaderEntry
{
	ElfProgramHeaderFlags Flags { get; }

	ElfProgramEntryType Type { get; }

	ulong RawAddress { get; }

	ulong VirtualAddress { get; }

	ulong PhysicalAddr { get; }

	ulong RawSize { get; }

	ulong VirtualSize { get; }

	long Align { get; }
}
