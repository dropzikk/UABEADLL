using System;
using System.Numerics;
using SixLabors.ImageSharp.Formats.Tiff.Utils;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff.PhotometricInterpretation;

internal class WhiteIsZero16TiffColor<TPixel> : TiffBaseColorDecoder<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly bool isBigEndian;

	public WhiteIsZero16TiffColor(bool isBigEndian)
	{
		this.isBigEndian = isBigEndian;
	}

	public override void Decode(ReadOnlySpan<byte> data, Buffer2D<TPixel> pixels, int left, int top, int width, int height)
	{
		L16 l16Default = TiffUtils.L16Default;
		TPixel color = default(TPixel);
		color.FromScaledVector4(Vector4.Zero);
		int num = 0;
		for (int i = top; i < top + height; i++)
		{
			Span<TPixel> span = pixels.DangerousGetRowSpan(i).Slice(left, width);
			if (isBigEndian)
			{
				for (int j = 0; j < span.Length; j++)
				{
					ushort intensity = (ushort)(65535 - TiffUtils.ConvertToUShortBigEndian(data.Slice(num, 2)));
					num += 2;
					span[j] = TiffUtils.ColorFromL16(l16Default, intensity, color);
				}
			}
			else
			{
				for (int k = 0; k < span.Length; k++)
				{
					ushort intensity2 = (ushort)(65535 - TiffUtils.ConvertToUShortLittleEndian(data.Slice(num, 2)));
					num += 2;
					span[k] = TiffUtils.ColorFromL16(l16Default, intensity2, color);
				}
			}
		}
	}
}
