using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AssetRipper.TextureDecoder.Rgb;

namespace AssetRipper.TextureDecoder.Yuy2;

public static class Yuy2Decoder
{
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int DecompressYUY2(ReadOnlySpan<byte> input, int width, int height, out byte[] output)
	{
		output = new byte[width * height * 4];
		return DecompressYUY2(input, width, height, output);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int DecompressYUY2(ReadOnlySpan<byte> input, int width, int height, Span<byte> output)
	{
		ThrowHelper.ThrowIfNotEnoughSpace(output, width, height);
		int result = 0;
		int num = 0;
		int num2 = width / 2;
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				int num3 = input[result++];
				int num4 = input[result++];
				byte num5 = input[result++];
				byte num6 = input[result++];
				int num7 = num3 - 16;
				int num8 = num4 - 128;
				int num9 = num6 - 128;
				output[num++] = ClampByte(298 * num7 + 516 * num8 + 128 >> 8);
				output[num++] = ClampByte(298 * num7 - 100 * num8 - 208 * num9 + 128 >> 8);
				output[num++] = ClampByte(298 * num7 + 409 * num9 + 128 >> 8);
				output[num++] = byte.MaxValue;
				num7 = num5 - 16;
				output[num++] = ClampByte(298 * num7 + 516 * num8 + 128 >> 8);
				output[num++] = ClampByte(298 * num7 - 100 * num8 - 208 * num9 + 128 >> 8);
				output[num++] = ClampByte(298 * num7 + 409 * num9 + 128 >> 8);
				output[num++] = byte.MaxValue;
			}
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int DecompressYUY2<TOutput, TOutputArg>(ReadOnlySpan<byte> input, int width, int height, out byte[] output) where TOutput : unmanaged, IColor<TOutputArg> where TOutputArg : unmanaged
	{
		output = new byte[width * height * Unsafe.SizeOf<TOutput>()];
		return DecompressYUY2<TOutput, TOutputArg>(input, width, height, output);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int DecompressYUY2<TOutput, TOutputArg>(ReadOnlySpan<byte> input, int width, int height, Span<byte> output) where TOutput : unmanaged, IColor<TOutputArg> where TOutputArg : unmanaged
	{
		return DecompressYUY2<TOutput, TOutputArg>(input, width, height, MemoryMarshal.Cast<byte, TOutput>(output));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int DecompressYUY2<TOutput, TOutputArg>(ReadOnlySpan<byte> input, int width, int height, Span<TOutput> output) where TOutput : unmanaged, IColor<TOutputArg> where TOutputArg : unmanaged
	{
		ThrowHelper.ThrowIfNotEnoughSpace(output.Length, width * height);
		int result = 0;
		int num = 0;
		int num2 = width / 2;
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				int num3 = input[result++];
				int num4 = input[result++];
				byte num5 = input[result++];
				byte num6 = input[result++];
				int num7 = num3 - 16;
				int num8 = num4 - 128;
				int num9 = num6 - 128;
				ColorExtensions.SetConvertedChannels<TOutput, TOutputArg, byte>(b: ClampByte(298 * num7 + 516 * num8 + 128 >> 8), g: ClampByte(298 * num7 - 100 * num8 - 208 * num9 + 128 >> 8), r: ClampByte(298 * num7 + 409 * num9 + 128 >> 8), color: ref output[num++], a: byte.MaxValue);
				num7 = num5 - 16;
				byte b2 = ClampByte(298 * num7 + 516 * num8 + 128 >> 8);
				byte g2 = ClampByte(298 * num7 - 100 * num8 - 208 * num9 + 128 >> 8);
				byte r2 = ClampByte(298 * num7 + 409 * num9 + 128 >> 8);
				output[num++].SetConvertedChannels<TOutput, TOutputArg, byte>(r2, g2, b2, byte.MaxValue);
			}
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private static byte ClampByte(int x)
	{
		return (byte)((255 < x) ? 255u : ((x > 0) ? ((uint)x) : 0u));
	}
}
