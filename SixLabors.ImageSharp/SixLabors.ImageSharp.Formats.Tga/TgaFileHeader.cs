using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SixLabors.ImageSharp.Formats.Tga;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal readonly struct TgaFileHeader
{
	public const int Size = 18;

	public byte IdLength { get; }

	public byte ColorMapType { get; }

	public TgaImageType ImageType { get; }

	public short CMapStart { get; }

	public short CMapLength { get; }

	public byte CMapDepth { get; }

	public short XOffset { get; }

	public short YOffset { get; }

	public short Width { get; }

	public short Height { get; }

	public byte PixelDepth { get; }

	public byte ImageDescriptor { get; }

	public TgaFileHeader(byte idLength, byte colorMapType, TgaImageType imageType, short cMapStart, short cMapLength, byte cMapDepth, short xOffset, short yOffset, short width, short height, byte pixelDepth, byte imageDescriptor)
	{
		IdLength = idLength;
		ColorMapType = colorMapType;
		ImageType = imageType;
		CMapStart = cMapStart;
		CMapLength = cMapLength;
		CMapDepth = cMapDepth;
		XOffset = xOffset;
		YOffset = yOffset;
		Width = width;
		Height = height;
		PixelDepth = pixelDepth;
		ImageDescriptor = imageDescriptor;
	}

	public static TgaFileHeader Parse(Span<byte> data)
	{
		return MemoryMarshal.Cast<byte, TgaFileHeader>(data)[0];
	}

	public void WriteTo(Span<byte> buffer)
	{
		Unsafe.As<byte, TgaFileHeader>(ref MemoryMarshal.GetReference(buffer)) = this;
	}
}
