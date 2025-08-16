namespace LibCpp2IL.Elf;

public enum ElfFileType : ushort
{
	ET_NONE = 0,
	ET_REL = 1,
	ET_EXEC = 2,
	ET_DYN = 3,
	ET_CORE = 4,
	ET_LOOS = 65024,
	ET_HIOS = 65279,
	ET_LOPROC = 65280,
	ET_HIPROC = ushort.MaxValue
}
