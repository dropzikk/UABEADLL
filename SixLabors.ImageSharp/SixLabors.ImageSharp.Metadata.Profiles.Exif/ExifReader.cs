using System;
using System.Collections.Generic;
using System.IO;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal class ExifReader : BaseExifReader
{
	public ExifReader(byte[] exifData)
		: this(exifData, null)
	{
	}

	public ExifReader(byte[] exifData, MemoryAllocator? allocator)
		: base(new MemoryStream(exifData ?? throw new ArgumentNullException("exifData")), allocator)
	{
	}

	public List<IExifValue> ReadValues()
	{
		List<IExifValue> list = new List<IExifValue>();
		base.IsBigEndian = ReadUInt16() != 18761;
		if (ReadUInt16() != 42)
		{
			return list;
		}
		uint offset = ReadUInt32();
		ReadValues(list, offset);
		uint offset2 = ReadUInt32();
		GetThumbnail(offset2);
		ReadSubIfd(list);
		ReadBigValues(list);
		return list;
	}

	private void GetThumbnail(uint offset)
	{
		if (offset == 0)
		{
			return;
		}
		List<IExifValue> list = new List<IExifValue>();
		ReadValues(list, offset);
		for (int i = 0; i < list.Count; i++)
		{
			ExifValue exifValue = (ExifValue)list[i];
			if (exifValue == ExifTag.JPEGInterchangeFormat)
			{
				base.ThumbnailOffset = ((ExifLong)exifValue).Value;
			}
			else if (exifValue == ExifTag.JPEGInterchangeFormatLength)
			{
				base.ThumbnailLength = ((ExifLong)exifValue).Value;
			}
		}
	}
}
