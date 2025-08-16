using System;
using System.Collections.Generic;
using System.IO;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;

namespace SixLabors.ImageSharp.Formats.Tiff;

internal class DirectoryReader
{
	private const int DirectoryMax = 65534;

	private readonly Stream stream;

	private readonly MemoryAllocator allocator;

	private ulong nextIfdOffset;

	public ByteOrder ByteOrder { get; private set; }

	public bool IsBigTiff { get; private set; }

	public DirectoryReader(Stream stream, MemoryAllocator allocator)
	{
		this.stream = stream;
		this.allocator = allocator;
	}

	public IList<ExifProfile> Read()
	{
		ByteOrder = ReadByteOrder(stream);
		HeaderReader headerReader = new HeaderReader(stream, ByteOrder);
		headerReader.ReadFileHeader();
		nextIfdOffset = headerReader.FirstIfdOffset;
		IsBigTiff = headerReader.IsBigTiff;
		return ReadIfds(headerReader.IsBigTiff);
	}

	private static ByteOrder ReadByteOrder(Stream stream)
	{
		Span<byte> buffer = stackalloc byte[2];
		if (stream.Read(buffer) != 2)
		{
			throw TiffThrowHelper.ThrowInvalidHeader();
		}
		if (buffer[0] == 73 && buffer[1] == 73)
		{
			return ByteOrder.LittleEndian;
		}
		if (buffer[0] == 77 && buffer[1] == 77)
		{
			return ByteOrder.BigEndian;
		}
		throw TiffThrowHelper.ThrowInvalidHeader();
	}

	private IList<ExifProfile> ReadIfds(bool isBigTiff)
	{
		List<EntryReader> list = new List<EntryReader>();
		while (nextIfdOffset != 0L && nextIfdOffset < (ulong)stream.Length)
		{
			EntryReader entryReader = new EntryReader(stream, ByteOrder, allocator);
			entryReader.ReadTags(isBigTiff, nextIfdOffset);
			if (entryReader.BigValues.Count > 0)
			{
				entryReader.BigValues.Sort(((ulong Offset, ExifDataType DataType, ulong NumberOfComponents, ExifValue Exif) t1, (ulong Offset, ExifDataType DataType, ulong NumberOfComponents, ExifValue Exif) t2) => t1.Offset.CompareTo(t2.Offset));
				if (entryReader.BigValues[0].Offset < entryReader.NextIfdOffset)
				{
					entryReader.ReadBigValues();
				}
			}
			if (nextIfdOffset >= entryReader.NextIfdOffset && entryReader.NextIfdOffset != 0L)
			{
				TiffThrowHelper.ThrowImageFormatException("TIFF image contains circular directory offsets");
			}
			nextIfdOffset = entryReader.NextIfdOffset;
			list.Add(entryReader);
			if (list.Count >= 65534)
			{
				TiffThrowHelper.ThrowImageFormatException("TIFF image contains too many directories");
			}
		}
		List<ExifProfile> list2 = new List<ExifProfile>(list.Count);
		foreach (EntryReader item2 in list)
		{
			item2.ReadBigValues();
			ExifProfile item = new ExifProfile(item2.Values, item2.InvalidTags);
			list2.Add(item);
		}
		return list2;
	}
}
