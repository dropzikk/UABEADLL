using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SixLabors.ImageSharp.Formats.Gif;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal readonly struct GifImageDescriptor
{
	public const int Size = 10;

	public ushort Left { get; }

	public ushort Top { get; }

	public ushort Width { get; }

	public ushort Height { get; }

	public byte Packed { get; }

	public bool LocalColorTableFlag => (Packed & 0x80) >> 7 == 1;

	public int LocalColorTableSize => 2 << (Packed & 7);

	public bool InterlaceFlag => (Packed & 0x40) >> 6 == 1;

	public GifImageDescriptor(ushort left, ushort top, ushort width, ushort height, byte packed)
	{
		Left = left;
		Top = top;
		Width = width;
		Height = height;
		Packed = packed;
	}

	public void WriteTo(Span<byte> buffer)
	{
		buffer[0] = 44;
		Unsafe.As<byte, GifImageDescriptor>(ref MemoryMarshal.GetReference(buffer.Slice(1, buffer.Length - 1))) = this;
	}

	public static GifImageDescriptor Parse(ReadOnlySpan<byte> buffer)
	{
		return MemoryMarshal.Cast<byte, GifImageDescriptor>(buffer)[0];
	}

	public static byte GetPackedValue(bool localColorTableFlag, bool interfaceFlag, bool sortFlag, int localColorTableSize)
	{
		byte b = 0;
		if (localColorTableFlag)
		{
			b |= 0x80;
		}
		if (interfaceFlag)
		{
			b |= 0x40;
		}
		if (sortFlag)
		{
			b |= 0x20;
		}
		return (byte)(b | (byte)localColorTableSize);
	}
}
