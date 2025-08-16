using System;
using System.Numerics;
using SixLabors.ImageSharp.Formats.Tiff.Utils;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff.PhotometricInterpretation;

internal class Rgba32323232TiffColor<TPixel> : TiffBaseColorDecoder<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly bool isBigEndian;

	private readonly TiffExtraSampleType? extraSamplesType;

	public Rgba32323232TiffColor(TiffExtraSampleType? extraSamplesType, bool isBigEndian)
	{
		this.extraSamplesType = extraSamplesType;
		this.isBigEndian = isBigEndian;
	}

	public override void Decode(ReadOnlySpan<byte> data, Buffer2D<TPixel> pixels, int left, int top, int width, int height)
	{
		TPixel color = default(TPixel);
		color.FromScaledVector4(Vector4.Zero);
		bool flag = extraSamplesType.HasValue && extraSamplesType == TiffExtraSampleType.AssociatedAlphaData;
		int num = 0;
		for (int i = top; i < top + height; i++)
		{
			Span<TPixel> span = pixels.DangerousGetRowSpan(i).Slice(left, width);
			if (isBigEndian)
			{
				for (int j = 0; j < span.Length; j++)
				{
					ulong r = TiffUtils.ConvertToUIntBigEndian(data.Slice(num, 4));
					num += 4;
					ulong g = TiffUtils.ConvertToUIntBigEndian(data.Slice(num, 4));
					num += 4;
					ulong b = TiffUtils.ConvertToUIntBigEndian(data.Slice(num, 4));
					num += 4;
					ulong a = TiffUtils.ConvertToUIntBigEndian(data.Slice(num, 4));
					num += 4;
					span[j] = (flag ? TiffUtils.ColorScaleTo32BitPremultiplied(r, g, b, a, color) : TiffUtils.ColorScaleTo32Bit(r, g, b, a, color));
				}
			}
			else
			{
				for (int k = 0; k < span.Length; k++)
				{
					ulong r2 = TiffUtils.ConvertToUIntLittleEndian(data.Slice(num, 4));
					num += 4;
					ulong g2 = TiffUtils.ConvertToUIntLittleEndian(data.Slice(num, 4));
					num += 4;
					ulong b2 = TiffUtils.ConvertToUIntLittleEndian(data.Slice(num, 4));
					num += 4;
					ulong a2 = TiffUtils.ConvertToUIntLittleEndian(data.Slice(num, 4));
					num += 4;
					span[k] = (flag ? TiffUtils.ColorScaleTo32BitPremultiplied(r2, g2, b2, a2, color) : TiffUtils.ColorScaleTo32Bit(r2, g2, b2, a2, color));
				}
			}
		}
	}
}
