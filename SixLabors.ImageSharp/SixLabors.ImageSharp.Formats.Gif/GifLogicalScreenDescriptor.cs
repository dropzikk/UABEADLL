using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SixLabors.ImageSharp.Formats.Gif;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal readonly struct GifLogicalScreenDescriptor
{
	public const int Size = 7;

	public ushort Width { get; }

	public ushort Height { get; }

	public byte Packed { get; }

	public byte BackgroundColorIndex { get; }

	public byte PixelAspectRatio { get; }

	public bool GlobalColorTableFlag => (Packed & 0x80) >> 7 == 1;

	public int GlobalColorTableSize => 2 << (Packed & 7);

	public int BitsPerPixel => (Packed & 7) + 1;

	public GifLogicalScreenDescriptor(ushort width, ushort height, byte packed, byte backgroundColorIndex, byte pixelAspectRatio = 0)
	{
		Width = width;
		Height = height;
		Packed = packed;
		BackgroundColorIndex = backgroundColorIndex;
		PixelAspectRatio = pixelAspectRatio;
	}

	public void WriteTo(Span<byte> buffer)
	{
		Unsafe.As<byte, GifLogicalScreenDescriptor>(ref MemoryMarshal.GetReference(buffer)) = this;
	}

	public static GifLogicalScreenDescriptor Parse(ReadOnlySpan<byte> buffer)
	{
		GifLogicalScreenDescriptor result = MemoryMarshal.Cast<byte, GifLogicalScreenDescriptor>(buffer)[0];
		if (result.GlobalColorTableSize > 1020)
		{
			throw new ImageFormatException($"Invalid gif colormap size '{result.GlobalColorTableSize}'");
		}
		return result;
	}

	public static byte GetPackedValue(bool globalColorTableFlag, int colorResolution, bool sortFlag, int globalColorTableSize)
	{
		byte b = 0;
		if (globalColorTableFlag)
		{
			b |= 0x80;
		}
		b |= (byte)(colorResolution << 4);
		if (sortFlag)
		{
			b |= 8;
		}
		return (byte)(b | (byte)globalColorTableSize);
	}
}
