using System.Collections.Generic;
using System.IO;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;

namespace SixLabors.ImageSharp.Formats.Tiff;

internal class EntryReader : BaseExifReader
{
	public List<IExifValue> Values { get; } = new List<IExifValue>();

	public ulong NextIfdOffset { get; private set; }

	public EntryReader(Stream stream, ByteOrder byteOrder, MemoryAllocator allocator)
		: base(stream, allocator)
	{
		base.IsBigEndian = byteOrder == ByteOrder.BigEndian;
	}

	public void ReadTags(bool isBigTiff, ulong ifdOffset)
	{
		if (!isBigTiff)
		{
			ReadValues(Values, (uint)ifdOffset);
			NextIfdOffset = ReadUInt32();
			ReadSubIfd(Values);
		}
		else
		{
			ReadValues64(Values, ifdOffset);
			NextIfdOffset = ReadUInt64();
		}
	}

	public void ReadBigValues()
	{
		ReadBigValues(Values);
	}
}
