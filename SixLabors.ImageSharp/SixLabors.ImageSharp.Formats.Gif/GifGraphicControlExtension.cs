using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SixLabors.ImageSharp.Formats.Gif;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal readonly struct GifGraphicControlExtension : IGifExtension, IEquatable<GifGraphicControlExtension>
{
	public byte BlockSize { get; }

	public byte Packed { get; }

	public ushort DelayTime { get; }

	public byte TransparencyIndex { get; }

	public GifDisposalMethod DisposalMethod => (GifDisposalMethod)((Packed & 0x1C) >> 2);

	public bool TransparencyFlag => (Packed & 1) == 1;

	byte IGifExtension.Label => 249;

	int IGifExtension.ContentLength => 5;

	public GifGraphicControlExtension(byte packed, ushort delayTime, byte transparencyIndex)
	{
		BlockSize = 4;
		Packed = packed;
		DelayTime = delayTime;
		TransparencyIndex = transparencyIndex;
	}

	public static bool operator ==(GifGraphicControlExtension left, GifGraphicControlExtension right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(GifGraphicControlExtension left, GifGraphicControlExtension right)
	{
		return !(left == right);
	}

	public int WriteTo(Span<byte> buffer)
	{
		Unsafe.As<byte, GifGraphicControlExtension>(ref MemoryMarshal.GetReference(buffer)) = this;
		return ((IGifExtension)this).ContentLength;
	}

	public static GifGraphicControlExtension Parse(ReadOnlySpan<byte> buffer)
	{
		return MemoryMarshal.Cast<byte, GifGraphicControlExtension>(buffer)[0];
	}

	public static byte GetPackedValue(GifDisposalMethod disposalMethod, bool userInputFlag = false, bool transparencyFlag = false)
	{
		byte b = 0;
		b |= (byte)((int)disposalMethod << 2);
		if (userInputFlag)
		{
			b |= 2;
		}
		if (transparencyFlag)
		{
			b |= 1;
		}
		return b;
	}

	public override bool Equals(object? obj)
	{
		if (obj is GifGraphicControlExtension other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(GifGraphicControlExtension other)
	{
		if (BlockSize == other.BlockSize && Packed == other.Packed && DelayTime == other.DelayTime && TransparencyIndex == other.TransparencyIndex && DisposalMethod == other.DisposalMethod && TransparencyFlag == other.TransparencyFlag && ((IGifExtension)this).Label == ((IGifExtension)other).Label)
		{
			return ((IGifExtension)this).ContentLength == ((IGifExtension)other).ContentLength;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(BlockSize, Packed, DelayTime, TransparencyIndex, DisposalMethod, TransparencyFlag, ((IGifExtension)this).Label, ((IGifExtension)this).ContentLength);
	}
}
