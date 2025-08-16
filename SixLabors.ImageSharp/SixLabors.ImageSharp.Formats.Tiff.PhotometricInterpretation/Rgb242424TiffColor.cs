using System;
using System.Numerics;
using SixLabors.ImageSharp.Formats.Tiff.Utils;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff.PhotometricInterpretation;

internal class Rgb242424TiffColor<TPixel> : TiffBaseColorDecoder<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly bool isBigEndian;

	public Rgb242424TiffColor(bool isBigEndian)
	{
		this.isBigEndian = isBigEndian;
	}

	public override void Decode(ReadOnlySpan<byte> data, Buffer2D<TPixel> pixels, int left, int top, int width, int height)
	{
		TPixel color = default(TPixel);
		color.FromScaledVector4(Vector4.Zero);
		int num = 0;
		Span<byte> span = stackalloc byte[4];
		int num2 = (isBigEndian ? 1 : 0);
		Span<byte> destination = span.Slice(num2, span.Length - num2);
		for (int i = top; i < top + height; i++)
		{
			Span<TPixel> span2 = pixels.DangerousGetRowSpan(i).Slice(left, width);
			if (isBigEndian)
			{
				for (int j = 0; j < span2.Length; j++)
				{
					data.Slice(num, 3).CopyTo(destination);
					ulong r = TiffUtils.ConvertToUIntBigEndian(span);
					num += 3;
					data.Slice(num, 3).CopyTo(destination);
					ulong g = TiffUtils.ConvertToUIntBigEndian(span);
					num += 3;
					data.Slice(num, 3).CopyTo(destination);
					ulong b = TiffUtils.ConvertToUIntBigEndian(span);
					num += 3;
					span2[j] = TiffUtils.ColorScaleTo24Bit(r, g, b, color);
				}
			}
			else
			{
				for (int k = 0; k < span2.Length; k++)
				{
					data.Slice(num, 3).CopyTo(destination);
					ulong r2 = TiffUtils.ConvertToUIntLittleEndian(span);
					num += 3;
					data.Slice(num, 3).CopyTo(destination);
					ulong g2 = TiffUtils.ConvertToUIntLittleEndian(span);
					num += 3;
					data.Slice(num, 3).CopyTo(destination);
					ulong b2 = TiffUtils.ConvertToUIntLittleEndian(span);
					num += 3;
					span2[k] = TiffUtils.ColorScaleTo24Bit(r2, g2, b2, color);
				}
			}
		}
	}
}
