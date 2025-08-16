using System;
using System.Runtime.CompilerServices;
using AssetRipper.TextureDecoder.Rgb;
using AssetRipper.TextureDecoder.Rgb.Formats;

namespace AssetRipper.TextureDecoder.Bc;

public static class BcDecoder
{
	public static int DecompressBC1(ReadOnlySpan<byte> input, int width, int height, out byte[] output)
	{
		output = new byte[width * height * Unsafe.SizeOf<ColorBGRA32>()];
		return DecompressBC1(input, width, height, output);
	}

	public static int DecompressBC1(ReadOnlySpan<byte> input, int width, int height, Span<byte> output)
	{
		int num = 0;
		for (int i = 0; i < height; i += 4)
		{
			for (int j = 0; j < width; j += 4)
			{
				int start = (i * width + j) * Unsafe.SizeOf<ColorRGBA32>();
				BcHelpers.DecompressBc1(input.Slice(num, 8), output.Slice(start), width * 4);
				num += 8;
			}
		}
		RgbConverter.Convert<ColorRGBA32, byte, ColorBGRA32, byte>(output, width, height, output);
		return num;
	}

	public static int DecompressBC2(ReadOnlySpan<byte> input, int width, int height, out byte[] output)
	{
		output = new byte[width * height * Unsafe.SizeOf<ColorBGRA32>()];
		return DecompressBC2(input, width, height, output);
	}

	public static int DecompressBC2(ReadOnlySpan<byte> input, int width, int height, Span<byte> output)
	{
		int num = 0;
		for (int i = 0; i < height; i += 4)
		{
			for (int j = 0; j < width; j += 4)
			{
				int start = (i * width + j) * Unsafe.SizeOf<ColorRGBA32>();
				BcHelpers.DecompressBc2(input.Slice(num), output.Slice(start), width * 4);
				num += 16;
			}
		}
		RgbConverter.Convert<ColorRGBA32, byte, ColorBGRA32, byte>(output, width, height, output);
		return num;
	}

	public static int DecompressBC3(ReadOnlySpan<byte> input, int width, int height, out byte[] output)
	{
		output = new byte[width * height * Unsafe.SizeOf<ColorBGRA32>()];
		return DecompressBC3(input, width, height, output);
	}

	public static int DecompressBC3(ReadOnlySpan<byte> input, int width, int height, Span<byte> output)
	{
		int num = 0;
		for (int i = 0; i < height; i += 4)
		{
			for (int j = 0; j < width; j += 4)
			{
				int start = (i * width + j) * Unsafe.SizeOf<ColorRGBA32>();
				BcHelpers.DecompressBc3(input.Slice(num), output.Slice(start), width * 4);
				num += 16;
			}
		}
		RgbConverter.Convert<ColorRGBA32, byte, ColorBGRA32, byte>(output, width, height, output);
		return num;
	}

	public static int DecompressBC4(ReadOnlySpan<byte> input, int width, int height, out byte[] output)
	{
		output = new byte[width * height * Unsafe.SizeOf<ColorBGRA32>()];
		return DecompressBC4(input, width, height, output);
	}

	public static int DecompressBC4(ReadOnlySpan<byte> input, int width, int height, Span<byte> output)
	{
		int num = 0;
		Span<byte> span = new byte[width * height * Unsafe.SizeOf<ColorR8>()];
		for (int i = 0; i < height; i += 4)
		{
			for (int j = 0; j < width; j += 4)
			{
				int start = (i * width + j) * Unsafe.SizeOf<ColorR8>();
				BcHelpers.DecompressBc4(input.Slice(num), span.Slice(start), width);
				num += 8;
			}
		}
		RgbConverter.Convert<ColorR8, byte, ColorBGRA32, byte>(span, width, height, output);
		return num;
	}

	public static int DecompressBC5(ReadOnlySpan<byte> input, int width, int height, out byte[] output)
	{
		output = new byte[width * height * Unsafe.SizeOf<ColorBGRA32>()];
		return DecompressBC5(input, width, height, output);
	}

	public static int DecompressBC5(ReadOnlySpan<byte> input, int width, int height, Span<byte> output)
	{
		int num = 0;
		Span<byte> span = new byte[width * height * Unsafe.SizeOf<ColorRG16>()];
		for (int i = 0; i < height; i += 4)
		{
			for (int j = 0; j < width; j += 4)
			{
				int start = (i * width + j) * Unsafe.SizeOf<ColorRG16>();
				BcHelpers.DecompressBc5(input.Slice(num), span.Slice(start), width * 2);
				num += 16;
			}
		}
		RgbConverter.Convert<ColorRG16, byte, ColorBGRA32, byte>(span, width, height, output);
		return num;
	}

	public static int DecompressBC6H(ReadOnlySpan<byte> input, int width, int height, bool isSigned, out byte[] output)
	{
		output = new byte[width * height * Unsafe.SizeOf<ColorBGRA32>()];
		return DecompressBC6H(input, width, height, isSigned, output);
	}

	public unsafe static int DecompressBC6H(ReadOnlySpan<byte> input, int width, int height, bool isSigned, Span<byte> output)
	{
		byte[] array = new byte[width * height * Unsafe.SizeOf<ColorRGBSingle>()];
		int result;
		fixed (byte* input2 = input)
		{
			fixed (byte* output2 = array)
			{
				result = DecompressBC6H(input2, width, height, isSigned, output2);
			}
		}
		RgbConverter.Convert<ColorRGBSingle, float, ColorBGRA32, byte>(array, width, height, output);
		return result;
	}

	private unsafe static int DecompressBC6H(byte* input, int width, int height, bool isSigned, byte* output)
	{
		int num = 0;
		for (int i = 0; i < height; i += 4)
		{
			for (int j = 0; j < width; j += 4)
			{
				int num2 = (i * width + j) * Unsafe.SizeOf<ColorRGBSingle>();
				BcHelpers.DecompressBc6h_Float(input + num, output + num2, width * 3, isSigned ? 1 : 0);
				num += 16;
			}
		}
		return num;
	}

	public static int DecompressBC7(ReadOnlySpan<byte> input, int width, int height, out byte[] output)
	{
		output = new byte[width * height * Unsafe.SizeOf<ColorBGRA32>()];
		return DecompressBC7(input, width, height, output);
	}

	public unsafe static int DecompressBC7(ReadOnlySpan<byte> input, int width, int height, Span<byte> output)
	{
		int result;
		fixed (byte* input2 = input)
		{
			fixed (byte* output2 = output)
			{
				result = DecompressBC7(input2, width, height, output2);
			}
		}
		RgbConverter.Convert<ColorRGBA32, byte, ColorBGRA32, byte>(output, width, height, output);
		return result;
	}

	private unsafe static int DecompressBC7(byte* input, int width, int height, byte* output)
	{
		int num = 0;
		for (int i = 0; i < height; i += 4)
		{
			for (int j = 0; j < width; j += 4)
			{
				int num2 = (i * width + j) * Unsafe.SizeOf<ColorRGBA32>();
				BcHelpers.DecompressBc7(input + num, output + num2, width * 4);
				num += 16;
			}
		}
		return num;
	}
}
