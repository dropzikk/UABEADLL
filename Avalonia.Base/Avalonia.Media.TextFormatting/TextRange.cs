using System;

namespace Avalonia.Media.TextFormatting;

public readonly record struct TextRange(int start, int length)
{
	public int End => Start + Length - 1;

	public TextRange Take(int length)
	{
		if (length > Length)
		{
			throw new ArgumentOutOfRangeException("length");
		}
		return new TextRange(Start, length);
	}

	public TextRange Skip(int length)
	{
		if (length > Length)
		{
			throw new ArgumentOutOfRangeException("length");
		}
		return new TextRange(Start + length, Length - length);
	}
}
