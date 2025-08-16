using System;
using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff.Utils;

internal static class TiffUtils
{
	private const float Scale24Bit = 5.960465E-08f;

	private const float Scale32Bit = 2.3283064E-10f;

	public static Rgba64 Rgba64Default { get; } = new Rgba64(0, 0, 0, 0);

	public static L16 L16Default { get; } = new L16(0);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ushort ConvertToUShortBigEndian(ReadOnlySpan<byte> buffer)
	{
		return BinaryPrimitives.ReadUInt16BigEndian(buffer);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ushort ConvertToUShortLittleEndian(ReadOnlySpan<byte> buffer)
	{
		return BinaryPrimitives.ReadUInt16LittleEndian(buffer);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint ConvertToUIntBigEndian(ReadOnlySpan<byte> buffer)
	{
		return BinaryPrimitives.ReadUInt32BigEndian(buffer);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint ConvertToUIntLittleEndian(ReadOnlySpan<byte> buffer)
	{
		return BinaryPrimitives.ReadUInt32LittleEndian(buffer);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TPixel ColorFromL8<TPixel>(L8 l8, byte intensity, TPixel color) where TPixel : unmanaged, IPixel<TPixel>
	{
		l8.PackedValue = intensity;
		color.FromL8(l8);
		return color;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TPixel ColorFromRgb64<TPixel>(Rgba64 rgba, ulong r, ulong g, ulong b, TPixel color) where TPixel : unmanaged, IPixel<TPixel>
	{
		rgba.PackedValue = r | (g << 16) | (b << 32) | 0xFFFF000000000000uL;
		color.FromRgba64(rgba);
		return color;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TPixel ColorFromRgba64<TPixel>(Rgba64 rgba, ulong r, ulong g, ulong b, ulong a, TPixel color) where TPixel : unmanaged, IPixel<TPixel>
	{
		rgba.PackedValue = r | (g << 16) | (b << 32) | (a << 48);
		color.FromRgba64(rgba);
		return color;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TPixel ColorFromRgba64Premultiplied<TPixel>(Rgba64 rgba, ulong r, ulong g, ulong b, ulong a, TPixel color) where TPixel : unmanaged, IPixel<TPixel>
	{
		rgba.PackedValue = r | (g << 16) | (b << 32) | (a << 48);
		Vector4 vector = rgba.ToVector4();
		return UnPremultiply(ref vector, color);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TPixel ColorScaleTo24Bit<TPixel>(ulong r, ulong g, ulong b, TPixel color) where TPixel : unmanaged, IPixel<TPixel>
	{
		Vector4 vector = new Vector4((float)r * 5.960465E-08f, (float)g * 5.960465E-08f, (float)b * 5.960465E-08f, 1f);
		color.FromScaledVector4(vector);
		return color;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TPixel ColorScaleTo24Bit<TPixel>(ulong r, ulong g, ulong b, ulong a, TPixel color) where TPixel : unmanaged, IPixel<TPixel>
	{
		Vector4 vector = new Vector4(r, g, b, a) * 5.960465E-08f;
		color.FromScaledVector4(vector);
		return color;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TPixel ColorScaleTo24BitPremultiplied<TPixel>(ulong r, ulong g, ulong b, ulong a, TPixel color) where TPixel : unmanaged, IPixel<TPixel>
	{
		Vector4 vector = new Vector4(r, g, b, a) * 5.960465E-08f;
		return UnPremultiply(ref vector, color);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TPixel ColorScaleTo32Bit<TPixel>(ulong r, ulong g, ulong b, TPixel color) where TPixel : unmanaged, IPixel<TPixel>
	{
		Vector4 vector = new Vector4((float)r * 2.3283064E-10f, (float)g * 2.3283064E-10f, (float)b * 2.3283064E-10f, 1f);
		color.FromScaledVector4(vector);
		return color;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TPixel ColorScaleTo32Bit<TPixel>(ulong r, ulong g, ulong b, ulong a, TPixel color) where TPixel : unmanaged, IPixel<TPixel>
	{
		Vector4 vector = new Vector4(r, g, b, a) * 2.3283064E-10f;
		color.FromScaledVector4(vector);
		return color;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TPixel ColorScaleTo32BitPremultiplied<TPixel>(ulong r, ulong g, ulong b, ulong a, TPixel color) where TPixel : unmanaged, IPixel<TPixel>
	{
		Vector4 vector = new Vector4(r, g, b, a) * 2.3283064E-10f;
		return UnPremultiply(ref vector, color);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TPixel ColorFromL16<TPixel>(L16 l16, ushort intensity, TPixel color) where TPixel : unmanaged, IPixel<TPixel>
	{
		l16.PackedValue = intensity;
		color.FromL16(l16);
		return color;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TPixel ColorScaleTo24Bit<TPixel>(ulong intensity, TPixel color) where TPixel : unmanaged, IPixel<TPixel>
	{
		Vector4 vector = new Vector4((float)intensity * 5.960465E-08f, (float)intensity * 5.960465E-08f, (float)intensity * 5.960465E-08f, 1f);
		color.FromScaledVector4(vector);
		return color;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TPixel ColorScaleTo32Bit<TPixel>(ulong intensity, TPixel color) where TPixel : unmanaged, IPixel<TPixel>
	{
		Vector4 vector = new Vector4((float)intensity * 2.3283064E-10f, (float)intensity * 2.3283064E-10f, (float)intensity * 2.3283064E-10f, 1f);
		color.FromScaledVector4(vector);
		return color;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TPixel UnPremultiply<TPixel>(ref Vector4 vector, TPixel color) where TPixel : unmanaged, IPixel<TPixel>
	{
		Numerics.UnPremultiply(ref vector);
		color.FromScaledVector4(vector);
		return color;
	}

	public static int PaddingToNextInteger(int valueToRoundUp, int subSampling)
	{
		if (valueToRoundUp % subSampling == 0)
		{
			return 0;
		}
		return subSampling - valueToRoundUp % subSampling;
	}
}
