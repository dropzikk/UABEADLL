using System;
using System.Runtime.InteropServices;

namespace SixLabors.ImageSharp.Formats.Bmp;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal readonly struct BmpArrayFileHeader
{
	public short Type { get; }

	public int Size { get; }

	public int OffsetToNext { get; }

	public short ScreenWidth { get; }

	public short ScreenHeight { get; }

	public BmpArrayFileHeader(short type, int size, int offsetToNext, short width, short height)
	{
		Type = type;
		Size = size;
		OffsetToNext = offsetToNext;
		ScreenWidth = width;
		ScreenHeight = height;
	}

	public static BmpArrayFileHeader Parse(Span<byte> data)
	{
		return MemoryMarshal.Cast<byte, BmpArrayFileHeader>(data)[0];
	}
}
