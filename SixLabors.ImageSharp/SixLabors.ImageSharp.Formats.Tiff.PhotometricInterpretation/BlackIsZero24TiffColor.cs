using System;
using System.Numerics;
using SixLabors.ImageSharp.Formats.Tiff.Utils;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff.PhotometricInterpretation;

internal class BlackIsZero24TiffColor<TPixel> : TiffBaseColorDecoder<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly bool isBigEndian;

	public BlackIsZero24TiffColor(bool isBigEndian)
	{
		this.isBigEndian = isBigEndian;
	}

	public override void Decode(ReadOnlySpan<byte> data, Buffer2D<TPixel> pixels, int left, int top, int width, int height)
	{
		TPixel color = default(TPixel);
		color.FromScaledVector4(Vector4.Zero);
		Span<byte> span = stackalloc byte[4];
		int num = (isBigEndian ? 1 : 0);
		Span<byte> destination = span.Slice(num, span.Length - num);
		int num2 = 0;
		for (int i = top; i < top + height; i++)
		{
			Span<TPixel> span2 = pixels.DangerousGetRowSpan(i).Slice(left, width);
			if (isBigEndian)
			{
				for (int j = 0; j < span2.Length; j++)
				{
					data.Slice(num2, 3).CopyTo(destination);
					ulong intensity = TiffUtils.ConvertToUIntBigEndian(span);
					num2 += 3;
					span2[j] = TiffUtils.ColorScaleTo24Bit(intensity, color);
				}
			}
			else
			{
				for (int k = 0; k < span2.Length; k++)
				{
					data.Slice(num2, 3).CopyTo(destination);
					ulong intensity2 = TiffUtils.ConvertToUIntLittleEndian(span);
					num2 += 3;
					span2[k] = TiffUtils.ColorScaleTo24Bit(intensity2, color);
				}
			}
		}
	}
}
