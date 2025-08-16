using System;
using System.Buffers.Binary;

namespace SixLabors.ImageSharp.Formats.Png.Chunks;

internal readonly struct PngHeader
{
	public const int Size = 13;

	public int Width { get; }

	public int Height { get; }

	public byte BitDepth { get; }

	public PngColorType ColorType { get; }

	public byte CompressionMethod { get; }

	public byte FilterMethod { get; }

	public PngInterlaceMode InterlaceMethod { get; }

	public PngHeader(int width, int height, byte bitDepth, PngColorType colorType, byte compressionMethod, byte filterMethod, PngInterlaceMode interlaceMethod)
	{
		Width = width;
		Height = height;
		BitDepth = bitDepth;
		ColorType = colorType;
		CompressionMethod = compressionMethod;
		FilterMethod = filterMethod;
		InterlaceMethod = interlaceMethod;
	}

	public void Validate()
	{
		if (!PngConstants.ColorTypes.TryGetValue(ColorType, out byte[] value))
		{
			throw new NotSupportedException($"Invalid or unsupported color type. Was '{ColorType}'.");
		}
		if (value.AsSpan().IndexOf(BitDepth) == -1)
		{
			throw new NotSupportedException($"Invalid or unsupported bit depth. Was '{BitDepth}'.");
		}
		if (FilterMethod != 0)
		{
			throw new NotSupportedException($"Invalid filter method. Expected 0. Was '{FilterMethod}'.");
		}
		PngInterlaceMode interlaceMethod = InterlaceMethod;
		if (interlaceMethod != 0 && interlaceMethod != PngInterlaceMode.Adam7)
		{
			throw new NotSupportedException($"Invalid interlace method. Expected 'None' or 'Adam7'. Was '{InterlaceMethod}'.");
		}
	}

	public void WriteTo(Span<byte> buffer)
	{
		BinaryPrimitives.WriteInt32BigEndian(buffer.Slice(0, 4), Width);
		BinaryPrimitives.WriteInt32BigEndian(buffer.Slice(4, 4), Height);
		buffer[8] = BitDepth;
		buffer[9] = (byte)ColorType;
		buffer[10] = CompressionMethod;
		buffer[11] = FilterMethod;
		buffer[12] = (byte)InterlaceMethod;
	}

	public static PngHeader Parse(ReadOnlySpan<byte> data)
	{
		return new PngHeader(BinaryPrimitives.ReadInt32BigEndian(data.Slice(0, 4)), BinaryPrimitives.ReadInt32BigEndian(data.Slice(4, 4)), data[8], (PngColorType)data[9], data[10], data[11], (PngInterlaceMode)data[12]);
	}
}
