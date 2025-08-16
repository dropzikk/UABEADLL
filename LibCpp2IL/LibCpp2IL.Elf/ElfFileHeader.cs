namespace LibCpp2IL.Elf;

public class ElfFileHeader
{
	public ElfFileType Type;

	public short Machine;

	public int Version;

	public long pEntryPoint;

	public long pProgramHeader;

	public long pSectionHeader;

	public int Flags;

	public short HeaderSize;

	public short ProgramHeaderEntrySize;

	public short ProgramHeaderEntryCount;

	public short SectionHeaderEntrySize;

	public short SectionHeaderEntryCount;

	public short SectionNameSectionOffset;
}
