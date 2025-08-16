using System;
using System.Buffers;
using System.Numerics;
using SixLabors.ImageSharp.Formats.Tiff.Utils;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff.PhotometricInterpretation;

internal class Rgba24PlanarTiffColor<TPixel> : TiffBasePlanarColorDecoder<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly bool isBigEndian;

	private readonly TiffExtraSampleType? extraSamplesType;

	public Rgba24PlanarTiffColor(TiffExtraSampleType? extraSamplesType, bool isBigEndian)
	{
		this.extraSamplesType = extraSamplesType;
		this.isBigEndian = isBigEndian;
	}

	public override void Decode(IMemoryOwner<byte>[] data, Buffer2D<TPixel> pixels, int left, int top, int width, int height)
	{
		TPixel color = default(TPixel);
		color.FromScaledVector4(Vector4.Zero);
		Span<byte> span = stackalloc byte[4];
		bool num = isBigEndian;
		Span<byte> span2 = data[0].GetSpan();
		Span<byte> span3 = data[1].GetSpan();
		Span<byte> span4 = data[2].GetSpan();
		Span<byte> span5 = data[3].GetSpan();
		int num2 = (num ? 1 : 0);
		Span<byte> destination = span.Slice(num2, span.Length - num2);
		bool flag = extraSamplesType.HasValue && extraSamplesType == TiffExtraSampleType.AssociatedAlphaData;
		int num3 = 0;
		for (int i = top; i < top + height; i++)
		{
			Span<TPixel> span6 = pixels.DangerousGetRowSpan(i).Slice(left, width);
			if (isBigEndian)
			{
				for (int j = 0; j < span6.Length; j++)
				{
					span2.Slice(num3, 3).CopyTo(destination);
					ulong r = TiffUtils.ConvertToUIntBigEndian(span);
					span3.Slice(num3, 3).CopyTo(destination);
					ulong g = TiffUtils.ConvertToUIntBigEndian(span);
					span4.Slice(num3, 3).CopyTo(destination);
					ulong b = TiffUtils.ConvertToUIntBigEndian(span);
					span5.Slice(num3, 3).CopyTo(destination);
					ulong a = TiffUtils.ConvertToUIntBigEndian(span);
					num3 += 3;
					span6[j] = (flag ? TiffUtils.ColorScaleTo24BitPremultiplied(r, g, b, a, color) : TiffUtils.ColorScaleTo24Bit(r, g, b, a, color));
				}
			}
			else
			{
				for (int k = 0; k < span6.Length; k++)
				{
					span2.Slice(num3, 3).CopyTo(destination);
					ulong r2 = TiffUtils.ConvertToUIntLittleEndian(span);
					span3.Slice(num3, 3).CopyTo(destination);
					ulong g2 = TiffUtils.ConvertToUIntLittleEndian(span);
					span4.Slice(num3, 3).CopyTo(destination);
					ulong b2 = TiffUtils.ConvertToUIntLittleEndian(span);
					span5.Slice(num3, 3).CopyTo(destination);
					ulong a2 = TiffUtils.ConvertToUIntLittleEndian(span);
					num3 += 3;
					span6[k] = (flag ? TiffUtils.ColorScaleTo24BitPremultiplied(r2, g2, b2, a2, color) : TiffUtils.ColorScaleTo24Bit(r2, g2, b2, a2, color));
				}
			}
		}
	}
}
