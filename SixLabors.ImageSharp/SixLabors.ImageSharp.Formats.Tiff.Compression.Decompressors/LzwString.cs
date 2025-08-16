using System;

namespace SixLabors.ImageSharp.Formats.Tiff.Compression.Decompressors;

public class LzwString
{
	private static readonly LzwString Empty = new LzwString(0, 0, 0, null);

	private readonly LzwString previous;

	private readonly byte value;

	public int Length { get; }

	public byte FirstChar { get; }

	public LzwString(byte code)
		: this(code, code, 1, null)
	{
	}

	private LzwString(byte value, byte firstChar, int length, LzwString previous)
	{
		this.value = value;
		FirstChar = firstChar;
		Length = length;
		this.previous = previous;
	}

	public LzwString Concatenate(byte other)
	{
		if (this == Empty)
		{
			return new LzwString(other);
		}
		return new LzwString(other, FirstChar, Length + 1, this);
	}

	public int WriteTo(Span<byte> buffer, int offset)
	{
		if (Length == 0)
		{
			return 0;
		}
		if (Length == 1)
		{
			buffer[offset] = value;
			return 1;
		}
		LzwString lzwString = this;
		int num = Length - 1;
		if (num >= buffer.Length)
		{
			TiffThrowHelper.ThrowImageFormatException("Error reading lzw compressed stream. Either pixel buffer to write to is to small or code length is invalid!");
		}
		for (int num2 = num; num2 >= 0; num2--)
		{
			buffer[offset + num2] = lzwString.value;
			lzwString = lzwString.previous;
		}
		return Length;
	}
}
