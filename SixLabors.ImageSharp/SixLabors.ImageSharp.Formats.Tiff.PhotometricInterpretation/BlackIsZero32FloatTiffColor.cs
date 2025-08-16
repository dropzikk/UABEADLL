using System;
using System.Numerics;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff.PhotometricInterpretation;

internal class BlackIsZero32FloatTiffColor<TPixel> : TiffBaseColorDecoder<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly bool isBigEndian;

	public BlackIsZero32FloatTiffColor(bool isBigEndian)
	{
		this.isBigEndian = isBigEndian;
	}

	public override void Decode(ReadOnlySpan<byte> data, Buffer2D<TPixel> pixels, int left, int top, int width, int height)
	{
		TPixel val = default(TPixel);
		val.FromScaledVector4(Vector4.Zero);
		Span<byte> span = stackalloc byte[4];
		int num = 0;
		for (int i = top; i < top + height; i++)
		{
			Span<TPixel> span2 = pixels.DangerousGetRowSpan(i).Slice(left, width);
			if (isBigEndian)
			{
				for (int j = 0; j < span2.Length; j++)
				{
					data.Slice(num, 4).CopyTo(span);
					span.Reverse();
					float num2 = BitConverter.ToSingle(span);
					num += 4;
					Vector4 vector = new Vector4(num2, num2, num2, 1f);
					val.FromScaledVector4(vector);
					span2[j] = val;
				}
			}
			else
			{
				for (int k = 0; k < span2.Length; k++)
				{
					float num3 = BitConverter.ToSingle(data.Slice(num, 4));
					num += 4;
					Vector4 vector2 = new Vector4(num3, num3, num3, 1f);
					val.FromScaledVector4(vector2);
					span2[k] = val;
				}
			}
		}
	}
}
