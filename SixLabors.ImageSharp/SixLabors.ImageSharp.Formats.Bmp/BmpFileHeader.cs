using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SixLabors.ImageSharp.Formats.Bmp;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal readonly struct BmpFileHeader
{
	public const int Size = 14;

	public short Type { get; }

	public int FileSize { get; }

	public int Reserved { get; }

	public int Offset { get; }

	public BmpFileHeader(short type, int fileSize, int reserved, int offset)
	{
		Type = type;
		FileSize = fileSize;
		Reserved = reserved;
		Offset = offset;
	}

	public static BmpFileHeader Parse(Span<byte> data)
	{
		return MemoryMarshal.Cast<byte, BmpFileHeader>(data)[0];
	}

	public void WriteTo(Span<byte> buffer)
	{
		Unsafe.As<byte, BmpFileHeader>(ref MemoryMarshal.GetReference(buffer)) = this;
	}
}
