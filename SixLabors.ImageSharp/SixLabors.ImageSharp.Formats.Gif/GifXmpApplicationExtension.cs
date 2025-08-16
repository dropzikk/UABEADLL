using System;
using System.IO;
using SixLabors.ImageSharp.IO;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Formats.Gif;

internal readonly struct GifXmpApplicationExtension : IGifExtension
{
	public byte Label => byte.MaxValue;

	public int ContentLength
	{
		get
		{
			if (Data.Length == 0)
			{
				return 0;
			}
			return Data.Length + 269;
		}
	}

	public byte[] Data { get; }

	public GifXmpApplicationExtension(byte[] data)
	{
		Data = data;
	}

	public static GifXmpApplicationExtension Read(Stream stream, MemoryAllocator allocator)
	{
		byte[] array = ReadXmpData(stream, allocator);
		int num = array.Length - 256;
		byte[] array2 = Array.Empty<byte>();
		if (num > 0)
		{
			array2 = new byte[num];
			array.AsSpan(0, num).CopyTo(array2);
			stream.Skip(1);
		}
		return new GifXmpApplicationExtension(array2);
	}

	public int WriteTo(Span<byte> buffer)
	{
		int num = 0;
		buffer[num++] = 11;
		ReadOnlySpan<byte> xmpApplicationIdentificationBytes = GifConstants.XmpApplicationIdentificationBytes;
		ref Span<byte> reference = ref buffer;
		int num2 = num;
		xmpApplicationIdentificationBytes.CopyTo(reference.Slice(num2, reference.Length - num2));
		num += xmpApplicationIdentificationBytes.Length;
		byte[] data = Data;
		reference = ref buffer;
		num2 = num;
		data.CopyTo(reference.Slice(num2, reference.Length - num2));
		num += Data.Length;
		buffer[num++] = 1;
		for (byte b = byte.MaxValue; b > 0; b--)
		{
			buffer[num++] = b;
		}
		buffer[num++] = 0;
		return ContentLength;
	}

	private static byte[] ReadXmpData(Stream stream, MemoryAllocator allocator)
	{
		using ChunkedMemoryStream chunkedMemoryStream = new ChunkedMemoryStream(allocator);
		while (true)
		{
			int num = stream.ReadByte();
			if (num <= 0)
			{
				break;
			}
			chunkedMemoryStream.WriteByte((byte)num);
		}
		return chunkedMemoryStream.ToArray();
	}
}
