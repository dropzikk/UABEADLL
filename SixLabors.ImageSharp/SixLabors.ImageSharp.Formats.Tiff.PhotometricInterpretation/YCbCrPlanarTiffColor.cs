using System;
using System.Buffers;
using SixLabors.ImageSharp.Formats.Tiff.Utils;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff.PhotometricInterpretation;

internal class YCbCrPlanarTiffColor<TPixel> : TiffBasePlanarColorDecoder<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly YCbCrConverter converter;

	private readonly ushort[] ycbcrSubSampling;

	public YCbCrPlanarTiffColor(Rational[] referenceBlackAndWhite, Rational[] coefficients, ushort[] ycbcrSubSampling)
	{
		converter = new YCbCrConverter(referenceBlackAndWhite, coefficients);
		this.ycbcrSubSampling = ycbcrSubSampling;
	}

	public override void Decode(IMemoryOwner<byte>[] data, Buffer2D<TPixel> pixels, int left, int top, int width, int height)
	{
		Span<byte> span = data[0].GetSpan();
		Span<byte> span2 = data[1].GetSpan();
		Span<byte> span3 = data[2].GetSpan();
		if (ycbcrSubSampling != null && (ycbcrSubSampling[0] != 1 || ycbcrSubSampling[1] != 1))
		{
			ReverseChromaSubSampling(width, height, ycbcrSubSampling[0], ycbcrSubSampling[1], span2, span3);
		}
		TPixel val = default(TPixel);
		int num = 0;
		int num2 = 0;
		if (ycbcrSubSampling != null)
		{
			num2 = TiffUtils.PaddingToNextInteger(width, ycbcrSubSampling[0]);
		}
		for (int i = top; i < top + height; i++)
		{
			Span<TPixel> span4 = pixels.DangerousGetRowSpan(i).Slice(left, width);
			for (int j = 0; j < span4.Length; j++)
			{
				Rgba32 source = converter.ConvertToRgba32(span[num], span2[num], span3[num]);
				val.FromRgba32(source);
				span4[j] = val;
				num++;
			}
			num += num2;
		}
	}

	private static void ReverseChromaSubSampling(int width, int height, int horizontalSubSampling, int verticalSubSampling, Span<byte> planarCb, Span<byte> planarCr)
	{
		width += TiffUtils.PaddingToNextInteger(width, horizontalSubSampling);
		height += TiffUtils.PaddingToNextInteger(height, verticalSubSampling);
		for (int num = height - 1; num >= 0; num--)
		{
			for (int num2 = width - 1; num2 >= 0; num2--)
			{
				int index = num * width + num2;
				int index2 = num / verticalSubSampling * (width / horizontalSubSampling) + num2 / horizontalSubSampling;
				planarCb[index] = planarCb[index2];
				planarCr[index] = planarCr[index2];
			}
		}
	}
}
