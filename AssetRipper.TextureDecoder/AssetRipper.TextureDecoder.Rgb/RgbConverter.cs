using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AssetRipper.TextureDecoder.Rgb;

public static class RgbConverter
{
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int A8ToBGRA32(ReadOnlySpan<byte> input, int width, int height, out byte[] output)
	{
		output = new byte[width * height * 4];
		return A8ToBGRA32(input, width, height, output);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int A8ToBGRA32(ReadOnlySpan<byte> input, int width, int height, Span<byte> output)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				output[num2] = 0;
				output[num2 + 1] = 0;
				output[num2 + 2] = 0;
				output[num2 + 3] = input[num];
				num2 += 4;
				num++;
			}
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int ARGB16ToBGRA32(ReadOnlySpan<byte> input, int width, int height, out byte[] output)
	{
		output = new byte[width * height * 4];
		return ARGB16ToBGRA32(input, width, height, output);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int ARGB16ToBGRA32(ReadOnlySpan<byte> input, int width, int height, Span<byte> output)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				output[num2] = (byte)(input[num] << 4);
				output[num2 + 1] = (byte)(input[num] & 0xF0);
				output[num2 + 2] = (byte)(input[num + 1] << 4);
				output[num2 + 3] = (byte)(input[num + 1] & 0xF0);
				num += 2;
				num2 += 4;
			}
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int RGB24ToBGRA32(ReadOnlySpan<byte> input, int width, int height, out byte[] output)
	{
		output = new byte[width * height * 4];
		return RGB24ToBGRA32(input, width, height, output);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int RGB24ToBGRA32(ReadOnlySpan<byte> input, int width, int height, Span<byte> output)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				output[num2] = input[num + 2];
				output[num2 + 1] = input[num + 1];
				output[num2 + 2] = input[num];
				output[num2 + 3] = byte.MaxValue;
				num += 3;
				num2 += 4;
			}
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int RGBA32ToBGRA32(ReadOnlySpan<byte> input, int width, int height, out byte[] output)
	{
		output = new byte[width * height * 4];
		return RGBA32ToBGRA32(input, width, height, output);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int RGBA32ToBGRA32(ReadOnlySpan<byte> input, int width, int height, Span<byte> output)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				output[num2] = input[num + 2];
				output[num2 + 1] = input[num + 1];
				output[num2 + 2] = input[num];
				output[num2 + 3] = input[num + 3];
				num += 4;
				num2 += 4;
			}
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int ARGB32ToBGRA32(ReadOnlySpan<byte> input, int width, int height, out byte[] output)
	{
		output = new byte[width * height * 4];
		return ARGB32ToBGRA32(input, width, height, output);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int ARGB32ToBGRA32(ReadOnlySpan<byte> input, int width, int height, Span<byte> output)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				output[num2] = input[num + 3];
				output[num2 + 1] = input[num + 2];
				output[num2 + 2] = input[num + 1];
				output[num2 + 3] = input[num];
				num += 4;
				num2 += 4;
			}
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int RGB16ToBGRA32(ReadOnlySpan<byte> input, int width, int height, out byte[] output)
	{
		output = new byte[width * height * 4];
		return RGB16ToBGRA32(input, width, height, output);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int RGB16ToBGRA32(ReadOnlySpan<byte> input, int width, int height, Span<byte> output)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				byte b = (byte)(input[num + 1] & 0xF8);
				byte b2 = (byte)((input[num + 1] << 5) | ((input[num] & 0xE0) >> 3));
				byte b3 = (byte)(input[num] << 3);
				output[num2] = b3;
				output[num2 + 1] = b2;
				output[num2 + 2] = b;
				output[num2 + 3] = byte.MaxValue;
				num += 2;
				num2 += 4;
			}
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int R16ToBGRA32(ReadOnlySpan<byte> input, int width, int height, out byte[] output)
	{
		output = new byte[width * height * 4];
		return R16ToBGRA32(input, width, height, output);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int R16ToBGRA32(ReadOnlySpan<byte> input, int width, int height, Span<byte> output)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				output[num2] = 0;
				output[num2 + 1] = 0;
				output[num2 + 2] = input[num + 1];
				output[num2 + 3] = byte.MaxValue;
				num += 2;
				num2 += 4;
			}
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int RGBA16ToBGRA32(ReadOnlySpan<byte> input, int width, int height, out byte[] output)
	{
		output = new byte[width * height * 4];
		return RGBA16ToBGRA32(input, width, height, output);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int RGBA16ToBGRA32(ReadOnlySpan<byte> input, int width, int height, Span<byte> output)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				output[num2] = (byte)(input[num] & 0xF0);
				output[num2 + 1] = (byte)(input[num + 1] << 4);
				output[num2 + 2] = (byte)(input[num + 1] & 0xF0);
				output[num2 + 3] = (byte)(input[num] << 4);
				num += 2;
				num2 += 4;
			}
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int RG16ToBGRA32(ReadOnlySpan<byte> input, int width, int height, out byte[] output)
	{
		output = new byte[width * height * 4];
		return RG16ToBGRA32(input, width, height, output);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int RG16ToBGRA32(ReadOnlySpan<byte> input, int width, int height, Span<byte> output)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				output[num2] = 0;
				output[num2 + 1] = input[num + 1];
				output[num2 + 2] = input[num];
				output[num2 + 3] = byte.MaxValue;
				num += 2;
				num2 += 4;
			}
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int R8ToBGRA32(ReadOnlySpan<byte> input, int width, int height, out byte[] output)
	{
		output = new byte[width * height * 4];
		return R8ToBGRA32(input, width, height, output);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int R8ToBGRA32(ReadOnlySpan<byte> input, int width, int height, Span<byte> output)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				output[num2] = 0;
				output[num2 + 1] = 0;
				output[num2 + 2] = input[num];
				output[num2 + 3] = byte.MaxValue;
				num++;
				num2 += 4;
			}
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int RHalfToBGRA32(ReadOnlySpan<byte> input, int width, int height, out byte[] output)
	{
		output = new byte[width * height * 4];
		return RHalfToBGRA32(input, width, height, output);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int RHalfToBGRA32(ReadOnlySpan<byte> input, int width, int height, Span<byte> output)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				byte b = ClampByte(ToHalf(input, num) * 255f);
				output[num2] = 0;
				output[num2 + 1] = 0;
				output[num2 + 2] = b;
				output[num2 + 3] = byte.MaxValue;
				num += 2;
				num2 += 4;
			}
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int RGHalfToBGRA32(ReadOnlySpan<byte> input, int width, int height, out byte[] output)
	{
		output = new byte[width * height * 4];
		return RGHalfToBGRA32(input, width, height, output);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int RGHalfToBGRA32(ReadOnlySpan<byte> input, int width, int height, Span<byte> output)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				byte b = ClampByte(ToHalf(input, num) * 255f);
				byte b2 = ClampByte(ToHalf(input, num + 2) * 255f);
				output[num2] = 0;
				output[num2 + 1] = b2;
				output[num2 + 2] = b;
				output[num2 + 3] = byte.MaxValue;
				num += 4;
				num2 += 4;
			}
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int RGBAHalfToBGRA32(ReadOnlySpan<byte> input, int width, int height, out byte[] output)
	{
		output = new byte[width * height * 4];
		return RGBAHalfToBGRA32(input, width, height, output);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int RGBAHalfToBGRA32(ReadOnlySpan<byte> input, int width, int height, Span<byte> output)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				byte b = ClampByte(ToHalf(input, num) * 255f);
				byte b2 = ClampByte(ToHalf(input, num + 2) * 255f);
				byte b3 = ClampByte(ToHalf(input, num + 4) * 255f);
				byte b4 = ClampByte(ToHalf(input, num + 6) * 255f);
				output[num2] = b3;
				output[num2 + 1] = b2;
				output[num2 + 2] = b;
				output[num2 + 3] = b4;
				num += 8;
				num2 += 4;
			}
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int RFloatToBGRA32(ReadOnlySpan<byte> input, int width, int height, out byte[] output)
	{
		output = new byte[width * height * 4];
		return RFloatToBGRA32(input, width, height, output);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int RFloatToBGRA32(ReadOnlySpan<byte> input, int width, int height, Span<byte> output)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				byte b = ClampByte(ToSingle(input, num) * 255f);
				output[num2] = 0;
				output[num2 + 1] = 0;
				output[num2 + 2] = b;
				output[num2 + 3] = byte.MaxValue;
				num += 4;
				num2 += 4;
			}
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int RGFloatToBGRA32(ReadOnlySpan<byte> input, int width, int height, out byte[] output)
	{
		output = new byte[width * height * 4];
		return RGFloatToBGRA32(input, width, height, output);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int RGFloatToBGRA32(ReadOnlySpan<byte> input, int width, int height, Span<byte> output)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				byte b = ClampByte(ToSingle(input, num) * 255f);
				byte b2 = ClampByte(ToSingle(input, num + 4) * 255f);
				output[num2] = 0;
				output[num2 + 1] = b2;
				output[num2 + 2] = b;
				output[num2 + 3] = byte.MaxValue;
				num += 8;
				num2 += 4;
			}
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int RGBAFloatToBGRA32(ReadOnlySpan<byte> input, int width, int height, out byte[] output)
	{
		output = new byte[width * height * 4];
		return RGBAFloatToBGRA32(input, width, height, output);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int RGBAFloatToBGRA32(ReadOnlySpan<byte> input, int width, int height, Span<byte> output)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				byte b = ClampByte(ToSingle(input, num) * 255f);
				byte b2 = ClampByte(ToSingle(input, num + 4) * 255f);
				byte b3 = ClampByte(ToSingle(input, num + 8) * 255f);
				byte b4 = ClampByte(ToSingle(input, num + 12) * 255f);
				output[num2] = b3;
				output[num2 + 1] = b2;
				output[num2 + 2] = b;
				output[num2 + 3] = b4;
				num += 16;
				num2 += 4;
			}
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int RGB9e5FloatToBGRA32(ReadOnlySpan<byte> input, int width, int height, out byte[] output)
	{
		output = new byte[width * height * 4];
		return RGB9e5FloatToBGRA32(input, width, height, output);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int RGB9e5FloatToBGRA32(ReadOnlySpan<byte> input, int width, int height, Span<byte> output)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				uint num3 = BinaryPrimitives.ReadUInt32LittleEndian(input.Slice(num, 4));
				double num4 = Math.Pow(2.0, (int)((num3 >> 27) - 24));
				byte b = ClampByte((double)(num3 & 0x1FF) * num4 * 255.0);
				byte b2 = ClampByte((double)((num3 >> 9) & 0x1FF) * num4 * 255.0);
				byte b3 = ClampByte((double)((num3 >> 18) & 0x1FF) * num4 * 255.0);
				output[num2] = b3;
				output[num2 + 1] = b2;
				output[num2 + 2] = b;
				output[num2 + 3] = byte.MaxValue;
				num += 4;
				num2 += 4;
			}
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int RG32ToBGRA32(ReadOnlySpan<byte> input, int width, int height, out byte[] output)
	{
		output = new byte[width * height * 4];
		return RG32ToBGRA32(input, width, height, output);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int RG32ToBGRA32(ReadOnlySpan<byte> input, int width, int height, Span<byte> output)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				output[num2] = 0;
				output[num2 + 1] = input[num + 3];
				output[num2 + 2] = input[num + 1];
				output[num2 + 3] = byte.MaxValue;
				num += 4;
				num2 += 4;
			}
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int RGB48ToBGRA32(ReadOnlySpan<byte> input, int width, int height, out byte[] output)
	{
		output = new byte[width * height * 4];
		return RGB48ToBGRA32(input, width, height, output);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int RGB48ToBGRA32(ReadOnlySpan<byte> input, int width, int height, Span<byte> output)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				output[num2] = input[num + 5];
				output[num2 + 1] = input[num + 3];
				output[num2 + 2] = input[num + 1];
				output[num2 + 3] = byte.MaxValue;
				num += 6;
				num2 += 4;
			}
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int RGBA64ToBGRA32(ReadOnlySpan<byte> input, int width, int height, out byte[] output)
	{
		output = new byte[width * height * 4];
		return RGBA64ToBGRA32(input, width, height, output);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int RGBA64ToBGRA32(ReadOnlySpan<byte> input, int width, int height, Span<byte> output)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				output[num2] = input[num + 5];
				output[num2 + 1] = input[num + 3];
				output[num2 + 2] = input[num + 1];
				output[num2 + 3] = input[num + 7];
				num += 8;
				num2 += 4;
			}
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int Convert<TSource, TSourceArg, TDestination, TDestinationArg>(ReadOnlySpan<byte> input, int width, int height, out byte[] output) where TSource : unmanaged, IColor<TSourceArg> where TSourceArg : unmanaged where TDestination : unmanaged, IColor<TDestinationArg> where TDestinationArg : unmanaged
	{
		output = new byte[width * height * Unsafe.SizeOf<TDestination>()];
		return Convert<TSource, TSourceArg, TDestination, TDestinationArg>(input, width, height, output);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int Convert<TSource, TSourceArg, TDestination, TDestinationArg>(ReadOnlySpan<byte> input, int width, int height, Span<byte> output) where TSource : unmanaged, IColor<TSourceArg> where TSourceArg : unmanaged where TDestination : unmanaged, IColor<TDestinationArg> where TDestinationArg : unmanaged
	{
		ReadOnlySpan<TSource> sourceSpan = MemoryMarshal.Cast<byte, TSource>(input).Slice(0, width * height);
		Span<TDestination> destinationSpan = MemoryMarshal.Cast<byte, TDestination>(output).Slice(0, width * height);
		Convert<TSource, TSourceArg, TDestination, TDestinationArg>(sourceSpan, destinationSpan);
		return width * height * Unsafe.SizeOf<TSource>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static void Convert<TSource, TSourceArg, TDestination, TDestinationArg>(ReadOnlySpan<TSource> sourceSpan, Span<TDestination> destinationSpan) where TSource : unmanaged, IColor<TSourceArg> where TSourceArg : unmanaged where TDestination : unmanaged, IColor<TDestinationArg> where TDestinationArg : unmanaged
	{
		for (int i = 0; i < sourceSpan.Length; i++)
		{
			TSource val = sourceSpan[i];
			TDestination color = default(TDestination);
			val.GetChannels(out var r, out var g, out var b, out var a);
			color.SetConvertedChannels<TDestination, TDestinationArg, TSourceArg>(r, g, b, a);
			destinationSpan[i] = color;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private static float ToHalf(ReadOnlySpan<byte> input, int offset)
	{
		return (float)BinaryPrimitives.ReadHalfLittleEndian(input.Slice(offset, 2));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private static float ToSingle(ReadOnlySpan<byte> input, int offset)
	{
		return BinaryPrimitives.ReadSingleLittleEndian(input.Slice(offset, 4));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private static byte ClampByte(float x)
	{
		if (!(255f < x))
		{
			if (!(x > 0f))
			{
				return 0;
			}
			return (byte)x;
		}
		return byte.MaxValue;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private static byte ClampByte(double x)
	{
		if (!(255.0 < x))
		{
			if (!(x > 0.0))
			{
				return 0;
			}
			return (byte)x;
		}
		return byte.MaxValue;
	}
}
