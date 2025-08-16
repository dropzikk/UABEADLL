using System;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp.Formats.Tiff.Compression.Decompressors;

internal readonly ref struct CcittReferenceScanline
{
	private readonly ReadOnlySpan<byte> scanLine;

	private readonly int width;

	private readonly byte whiteByte;

	public bool IsEmpty => scanLine.IsEmpty;

	public CcittReferenceScanline(bool whiteIsZero, ReadOnlySpan<byte> scanLine)
	{
		this.scanLine = scanLine;
		width = scanLine.Length;
		whiteByte = (byte)((!whiteIsZero) ? byte.MaxValue : 0);
	}

	public CcittReferenceScanline(bool whiteIsZero, int width)
	{
		scanLine = default(ReadOnlySpan<byte>);
		this.width = width;
		whiteByte = (byte)((!whiteIsZero) ? byte.MaxValue : 0);
	}

	public int FindB1(int a0, byte a0Byte)
	{
		if (IsEmpty)
		{
			return FindB1ForImaginaryWhiteLine(a0, a0Byte);
		}
		return FindB1ForNormalLine(a0, a0Byte);
	}

	public int FindB2(int b1)
	{
		if (IsEmpty)
		{
			return FindB2ForImaginaryWhiteLine();
		}
		return FindB2ForNormalLine(b1);
	}

	private int FindB1ForImaginaryWhiteLine(int a0, byte a0Byte)
	{
		if (a0 < 0 && a0Byte != whiteByte)
		{
			return 0;
		}
		return width;
	}

	private int FindB1ForNormalLine(int a0, byte a0Byte)
	{
		int num = 0;
		if (a0 < 0)
		{
			if (a0Byte != scanLine[0])
			{
				return 0;
			}
		}
		else
		{
			num = a0;
		}
		ReadOnlySpan<byte> readOnlySpan = scanLine;
		ref ReadOnlySpan<byte> reference = ref readOnlySpan;
		int num2 = num;
		ReadOnlySpan<byte> span = reference.Slice(num2, reference.Length - num2);
		byte value = (byte)(~a0Byte);
		int num3 = span.IndexOf(value);
		if (num3 < 0)
		{
			return scanLine.Length;
		}
		if (num3 != 0)
		{
			return num + num3;
		}
		value = (byte)(~span[0]);
		num3 = span.IndexOf(value);
		if (num3 < 0)
		{
			return scanLine.Length;
		}
		reference = ref span;
		num2 = num3;
		span = reference.Slice(num2, reference.Length - num2);
		num += num3;
		num3 = span.IndexOf((byte)(~value));
		if (num3 < 0)
		{
			return scanLine.Length;
		}
		return num3 + num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private int FindB2ForImaginaryWhiteLine()
	{
		return width;
	}

	private int FindB2ForNormalLine(int b1)
	{
		if (b1 >= scanLine.Length)
		{
			return scanLine.Length;
		}
		byte value = (byte)(~scanLine[b1]);
		int num = b1 + 1;
		ReadOnlySpan<byte> readOnlySpan = scanLine;
		int num2 = num;
		int num3 = readOnlySpan.Slice(num2, readOnlySpan.Length - num2).IndexOf(value);
		if (num3 == -1)
		{
			return scanLine.Length;
		}
		return num + num3;
	}
}
