using System;
using System.Buffers;
using System.IO;

namespace SixLabors.ImageSharp;

internal static class StreamExtensions
{
	public static void Write(this Stream stream, Span<byte> buffer, int offset, int count)
	{
		stream.Write(buffer.Slice(offset, count));
	}

	public static int Read(this Stream stream, Span<byte> buffer, int offset, int count)
	{
		return stream.Read(buffer.Slice(offset, count));
	}

	public static void Skip(this Stream stream, int count)
	{
		if (count < 1)
		{
			return;
		}
		if (stream.CanSeek)
		{
			stream.Seek(count, SeekOrigin.Current);
			return;
		}
		byte[] array = ArrayPool<byte>.Shared.Rent(count);
		try
		{
			while (count > 0)
			{
				int num = stream.Read(array, 0, count);
				if (num == 0)
				{
					break;
				}
				count -= num;
			}
		}
		finally
		{
			ArrayPool<byte>.Shared.Return(array);
		}
	}
}
