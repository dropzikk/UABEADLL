using System;
using System.Buffers.Binary;
using System.IO;

namespace SixLabors.ImageSharp.Formats.Tiff.Writers;

internal sealed class TiffStreamWriter : IDisposable
{
	public static bool IsLittleEndian => BitConverter.IsLittleEndian;

	public long Position => BaseStream.Position;

	public Stream BaseStream { get; }

	public TiffStreamWriter(Stream output)
	{
		BaseStream = output;
	}

	public long PlaceMarker(Span<byte> buffer)
	{
		long position = BaseStream.Position;
		Write(0u, buffer);
		return position;
	}

	public void Write(byte[] value)
	{
		BaseStream.Write(value, 0, value.Length);
	}

	public void Write(ReadOnlySpan<byte> value)
	{
		BaseStream.Write(value);
	}

	public void Write(byte value)
	{
		BaseStream.WriteByte(value);
	}

	public void Write(ushort value, Span<byte> buffer)
	{
		if (IsLittleEndian)
		{
			BinaryPrimitives.WriteUInt16LittleEndian(buffer, value);
		}
		else
		{
			BinaryPrimitives.WriteUInt16BigEndian(buffer, value);
		}
		BaseStream.Write(buffer.Slice(0, 2));
	}

	public void Write(uint value, Span<byte> buffer)
	{
		if (IsLittleEndian)
		{
			BinaryPrimitives.WriteUInt32LittleEndian(buffer, value);
		}
		else
		{
			BinaryPrimitives.WriteUInt32BigEndian(buffer, value);
		}
		BaseStream.Write(buffer.Slice(0, 4));
	}

	public void WritePadded(Span<byte> value)
	{
		BaseStream.Write(value);
		if (value.Length % 4 != 0)
		{
			ReadOnlySpan<byte> buffer = new byte[4] { 0, 0, 0, 0 };
			buffer = buffer.Slice(0, 4 - value.Length % 4);
			BaseStream.Write(buffer);
		}
	}

	public void WriteMarker(long offset, uint value, Span<byte> buffer)
	{
		long position = BaseStream.Position;
		BaseStream.Seek(offset, SeekOrigin.Begin);
		Write(value, buffer);
		BaseStream.Seek(position, SeekOrigin.Begin);
	}

	public void WriteMarkerFast(long offset, uint value, Span<byte> buffer)
	{
		BaseStream.Seek(offset, SeekOrigin.Begin);
		Write(value, buffer);
	}

	public void Dispose()
	{
		BaseStream.Flush();
	}
}
