using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp;

internal static class ColorNumerics
{
	private static readonly Vector4 Bt709 = new Vector4(0.2126f, 0.7152f, 0.0722f, 0f);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetBT709Luminance(ref Vector4 vector, int luminanceLevels)
	{
		return (int)MathF.Round(Vector4.Dot(vector, Bt709) * (float)(luminanceLevels - 1));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte Get8BitBT709Luminance(byte r, byte g, byte b)
	{
		return (byte)((float)(int)r * 0.2126f + (float)(int)g * 0.7152f + (float)(int)b * 0.0722f + 0.5f);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ushort Get16BitBT709Luminance(ushort r, ushort g, ushort b)
	{
		return (ushort)((float)(int)r * 0.2126f + (float)(int)g * 0.7152f + (float)(int)b * 0.0722f + 0.5f);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ushort Get16BitBT709Luminance(float r, float g, float b)
	{
		return (ushort)(r * 0.2126f + g * 0.7152f + b * 0.0722f + 0.5f);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte DownScaleFrom16BitTo8Bit(ushort component)
	{
		return (byte)(component * 255 + 32895 >> 16);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ushort UpscaleFrom8BitTo16Bit(byte component)
	{
		return (ushort)(component * 257);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetBitsNeededForColorDepth(int colors)
	{
		return Math.Max(1, (int)Math.Ceiling(Math.Log(colors, 2.0)));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetColorCountForBitDepth(int bitDepth)
	{
		return 1 << bitDepth;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Vector4 Transform(Vector4 vector, in ColorMatrix.Impl matrix)
	{
		return matrix.X * vector.X + matrix.Y * vector.Y + matrix.Z * vector.Z + matrix.W * vector.W + matrix.V;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Transform(ref Vector4 vector, ref ColorMatrix matrix)
	{
		vector = Transform(vector, in matrix.AsImpl());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Transform(Span<Vector4> vectors, ref ColorMatrix matrix)
	{
		for (int i = 0; i < vectors.Length; i++)
		{
			Transform(ref vectors[i], ref matrix);
		}
	}
}
