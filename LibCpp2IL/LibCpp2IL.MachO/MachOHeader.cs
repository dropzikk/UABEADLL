namespace LibCpp2IL.MachO;

public class MachOHeader
{
	public const uint MAGIC_32_BIT = 4277009102u;

	public const uint MAGIC_64_BIT = 4277009103u;

	public uint Magic;

	public MachOCpuType CpuType;

	public MachOCpuSubtype CpuSubtype;

	public MachOFileType FileType;

	public uint NumLoadCommands;

	public uint SizeOfLoadCommands;

	public MachOHeaderFlags Flags;

	public uint Reserved;

	public void Read(ClassReadingBinaryReader reader)
	{
		Magic = reader.ReadUInt32();
		CpuType = (MachOCpuType)reader.ReadUInt32();
		CpuSubtype = (MachOCpuSubtype)reader.ReadUInt32();
		FileType = (MachOFileType)reader.ReadUInt32();
		NumLoadCommands = reader.ReadUInt32();
		SizeOfLoadCommands = reader.ReadUInt32();
		Flags = (MachOHeaderFlags)reader.ReadUInt32();
		if (Magic == 4277009103u)
		{
			Reserved = reader.ReadUInt32();
		}
	}
}
