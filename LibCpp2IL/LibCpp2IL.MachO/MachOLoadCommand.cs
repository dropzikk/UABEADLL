using System.Text;

namespace LibCpp2IL.MachO;

public class MachOLoadCommand
{
	public LoadCommandId Command;

	public uint CommandSize;

	public object? CommandData;

	public byte[]? UnknownCommandData;

	public string? UnknownDataAsString
	{
		get
		{
			if (UnknownCommandData != null)
			{
				return Encoding.UTF8.GetString(UnknownCommandData);
			}
			return null;
		}
	}

	public void Read(ClassReadingBinaryReader reader)
	{
		Command = (LoadCommandId)reader.ReadUInt32();
		CommandSize = reader.ReadUInt32();
		LoadCommandId command = Command;
		if (command == LoadCommandId.LC_SEGMENT || command == LoadCommandId.LC_SEGMENT_64)
		{
			MachOSegmentCommand machOSegmentCommand = new MachOSegmentCommand();
			machOSegmentCommand.Read(reader);
			CommandData = machOSegmentCommand;
		}
		else
		{
			UnknownCommandData = reader.ReadBytes((int)(CommandSize - 8));
		}
	}
}
