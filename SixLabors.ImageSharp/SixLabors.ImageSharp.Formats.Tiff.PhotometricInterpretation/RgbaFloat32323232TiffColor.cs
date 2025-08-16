using System;
using System.Numerics;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff.PhotometricInterpretation;

internal class RgbaFloat32323232TiffColor<TPixel> : TiffBaseColorDecoder<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly bool isBigEndian;

	public RgbaFloat32323232TiffColor(bool isBigEndian)
	{
		this.isBigEndian = isBigEndian;
	}

	public override void Decode(ReadOnlySpan<byte> data, Buffer2D<TPixel> pixels, int left, int top, int width, int height)
	{
		TPixel val = default(TPixel);
		val.FromScaledVector4(Vector4.Zero);
		int num = 0;
		Span<byte> span = stackalloc byte[4];
		for (int i = top; i < top + height; i++)
		{
			Span<TPixel> span2 = pixels.DangerousGetRowSpan(i).Slice(left, width);
			if (isBigEndian)
			{
				for (int j = 0; j < span2.Length; j++)
				{
					data.Slice(num, 4).CopyTo(span);
					span.Reverse();
					float x = BitConverter.ToSingle(span);
					num += 4;
					data.Slice(num, 4).CopyTo(span);
					span.Reverse();
					float y = BitConverter.ToSingle(span);
					num += 4;
					data.Slice(num, 4).CopyTo(span);
					span.Reverse();
					float z = BitConverter.ToSingle(span);
					num += 4;
					data.Slice(num, 4).CopyTo(span);
					span.Reverse();
					float w = BitConverter.ToSingle(span);
					num += 4;
					Vector4 vector = new Vector4(x, y, z, w);
					val.FromScaledVector4(vector);
					span2[j] = val;
				}
			}
			else
			{
				for (int k = 0; k < span2.Length; k++)
				{
					float x2 = BitConverter.ToSingle(data.Slice(num, 4));
					num += 4;
					float y2 = BitConverter.ToSingle(data.Slice(num, 4));
					num += 4;
					float z2 = BitConverter.ToSingle(data.Slice(num, 4));
					num += 4;
					float w2 = BitConverter.ToSingle(data.Slice(num, 4));
					num += 4;
					Vector4 vector2 = new Vector4(x2, y2, z2, w2);
					val.FromScaledVector4(vector2);
					span2[k] = val;
				}
			}
		}
	}
}
