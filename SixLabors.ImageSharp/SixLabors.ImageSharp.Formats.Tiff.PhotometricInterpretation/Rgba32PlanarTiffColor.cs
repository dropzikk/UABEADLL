using System;
using System.Buffers;
using System.Numerics;
using SixLabors.ImageSharp.Formats.Tiff.Utils;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff.PhotometricInterpretation;

internal class Rgba32PlanarTiffColor<TPixel> : TiffBasePlanarColorDecoder<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly bool isBigEndian;

	private readonly TiffExtraSampleType? extraSamplesType;

	public Rgba32PlanarTiffColor(TiffExtraSampleType? extraSamplesType, bool isBigEndian)
	{
		this.extraSamplesType = extraSamplesType;
		this.isBigEndian = isBigEndian;
	}

	public override void Decode(IMemoryOwner<byte>[] data, Buffer2D<TPixel> pixels, int left, int top, int width, int height)
	{
		TPixel color = default(TPixel);
		color.FromScaledVector4(Vector4.Zero);
		Span<byte> span = data[0].GetSpan();
		Span<byte> span2 = data[1].GetSpan();
		Span<byte> span3 = data[2].GetSpan();
		Span<byte> span4 = data[3].GetSpan();
		bool flag = extraSamplesType.HasValue && extraSamplesType == TiffExtraSampleType.AssociatedAlphaData;
		int num = 0;
		for (int i = top; i < top + height; i++)
		{
			Span<TPixel> span5 = pixels.DangerousGetRowSpan(i).Slice(left, width);
			if (isBigEndian)
			{
				for (int j = 0; j < span5.Length; j++)
				{
					ulong r = TiffUtils.ConvertToUIntBigEndian(span.Slice(num, 4));
					ulong g = TiffUtils.ConvertToUIntBigEndian(span2.Slice(num, 4));
					ulong b = TiffUtils.ConvertToUIntBigEndian(span3.Slice(num, 4));
					ulong a = TiffUtils.ConvertToUIntBigEndian(span4.Slice(num, 4));
					num += 4;
					span5[j] = (flag ? TiffUtils.ColorScaleTo32BitPremultiplied(r, g, b, a, color) : TiffUtils.ColorScaleTo32Bit(r, g, b, a, color));
				}
			}
			else
			{
				for (int k = 0; k < span5.Length; k++)
				{
					ulong r2 = TiffUtils.ConvertToUIntLittleEndian(span.Slice(num, 4));
					ulong g2 = TiffUtils.ConvertToUIntLittleEndian(span2.Slice(num, 4));
					ulong b2 = TiffUtils.ConvertToUIntLittleEndian(span3.Slice(num, 4));
					ulong a2 = TiffUtils.ConvertToUIntLittleEndian(span4.Slice(num, 4));
					num += 4;
					span5[k] = (flag ? TiffUtils.ColorScaleTo32BitPremultiplied(r2, g2, b2, a2, color) : TiffUtils.ColorScaleTo32Bit(r2, g2, b2, a2, color));
				}
			}
		}
	}
}
