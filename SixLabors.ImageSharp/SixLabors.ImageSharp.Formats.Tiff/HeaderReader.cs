using System.IO;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;

namespace SixLabors.ImageSharp.Formats.Tiff;

internal class HeaderReader : BaseExifReader
{
	public bool IsBigTiff { get; private set; }

	public ulong FirstIfdOffset { get; private set; }

	public HeaderReader(Stream stream, ByteOrder byteOrder)
		: base(stream, null)
	{
		base.IsBigEndian = byteOrder == ByteOrder.BigEndian;
	}

	public void ReadFileHeader()
	{
		switch (ReadUInt16())
		{
		case 42:
			IsBigTiff = false;
			FirstIfdOffset = ReadUInt32();
			return;
		case 43:
		{
			IsBigTiff = true;
			ushort num = ReadUInt16();
			ushort num2 = ReadUInt16();
			if (num == 8 && num2 == 0)
			{
				FirstIfdOffset = ReadUInt64();
				return;
			}
			break;
		}
		}
		TiffThrowHelper.ThrowInvalidHeader();
	}
}
