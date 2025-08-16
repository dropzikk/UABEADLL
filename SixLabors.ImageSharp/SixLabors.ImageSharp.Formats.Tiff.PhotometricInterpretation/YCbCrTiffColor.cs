using System;
using System.Buffers;
using SixLabors.ImageSharp.Formats.Tiff.Utils;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff.PhotometricInterpretation;

internal class YCbCrTiffColor<TPixel> : TiffBaseColorDecoder<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly MemoryAllocator memoryAllocator;

	private readonly YCbCrConverter converter;

	private readonly ushort[] ycbcrSubSampling;

	public YCbCrTiffColor(MemoryAllocator memoryAllocator, Rational[] referenceBlackAndWhite, Rational[] coefficients, ushort[] ycbcrSubSampling)
	{
		this.memoryAllocator = memoryAllocator;
		converter = new YCbCrConverter(referenceBlackAndWhite, coefficients);
		this.ycbcrSubSampling = ycbcrSubSampling;
	}

	public override void Decode(ReadOnlySpan<byte> data, Buffer2D<TPixel> pixels, int left, int top, int width, int height)
	{
		ReadOnlySpan<byte> ycbcrData = data;
		if (ycbcrSubSampling != null && (ycbcrSubSampling[0] != 1 || ycbcrSubSampling[1] != 1))
		{
			int num = width + 4;
			int num2 = height + 4;
			int length = num * num2 * 3;
			using IMemoryOwner<byte> buffer = memoryAllocator.Allocate<byte>(length);
			Span<byte> span = buffer.GetSpan();
			ReverseChromaSubSampling(width, height, ycbcrSubSampling[0], ycbcrSubSampling[1], data, span);
			ycbcrData = span;
			DecodeYCbCrData(pixels, left, top, width, height, ycbcrData);
			return;
		}
		DecodeYCbCrData(pixels, left, top, width, height, ycbcrData);
	}

	private void DecodeYCbCrData(Buffer2D<TPixel> pixels, int left, int top, int width, int height, ReadOnlySpan<byte> ycbcrData)
	{
		TPixel val = default(TPixel);
		int num = 0;
		int num2 = 0;
		if (ycbcrSubSampling != null)
		{
			num2 = TiffUtils.PaddingToNextInteger(width, ycbcrSubSampling[0]);
		}
		for (int i = top; i < top + height; i++)
		{
			Span<TPixel> span = pixels.DangerousGetRowSpan(i).Slice(left, width);
			for (int j = 0; j < span.Length; j++)
			{
				Rgba32 source = converter.ConvertToRgba32(ycbcrData[num], ycbcrData[num + 1], ycbcrData[num + 2]);
				val.FromRgba32(source);
				span[j] = val;
				num += 3;
			}
			num += num2 * 3;
		}
	}

	private static void ReverseChromaSubSampling(int width, int height, int horizontalSubSampling, int verticalSubSampling, ReadOnlySpan<byte> source, Span<byte> destination)
	{
		width += TiffUtils.PaddingToNextInteger(width, horizontalSubSampling);
		height += TiffUtils.PaddingToNextInteger(height, verticalSubSampling);
		int num = width / horizontalSubSampling;
		int num2 = height / verticalSubSampling;
		int num3 = horizontalSubSampling * verticalSubSampling;
		int num4 = num3 + 2;
		for (int num5 = num2 - 1; num5 >= 0; num5--)
		{
			for (int num6 = num - 1; num6 >= 0; num6--)
			{
				int num7 = num5 * num + num6;
				ReadOnlySpan<byte> readOnlySpan = source.Slice(num7 * num4, num4);
				byte b = readOnlySpan[num3 + 1];
				byte b2 = readOnlySpan[num3];
				for (int num8 = verticalSubSampling - 1; num8 >= 0; num8--)
				{
					for (int num9 = horizontalSubSampling - 1; num9 >= 0; num9--)
					{
						int num10 = 3 * ((num5 * verticalSubSampling + num8) * width + num6 * horizontalSubSampling + num9);
						destination[num10 + 2] = b;
						destination[num10 + 1] = b2;
						destination[num10] = readOnlySpan[num8 * horizontalSubSampling + num9];
					}
				}
			}
		}
	}
}
