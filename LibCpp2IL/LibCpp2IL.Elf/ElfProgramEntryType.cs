namespace LibCpp2IL.Elf;

public enum ElfProgramEntryType : uint
{
	PT_NONE = 0u,
	PT_LOAD = 1u,
	PT_DYNAMIC = 2u,
	PT_INTERP = 3u,
	PT_NOTE = 4u,
	PT_SHLIB = 5u,
	PT_PHDR = 6u,
	PT_TLS = 7u,
	PT_LOOS = 1610612736u,
	PT_HIOS = 1879048191u,
	PT_LOPROC = 1879048192u,
	PT_HIPROC = 2147483647u
}
