using System;
using System.Numerics;
using SixLabors.ImageSharp.Formats.Tiff.Utils;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff.PhotometricInterpretation;

internal class Rgb161616TiffColor<TPixel> : TiffBaseColorDecoder<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly bool isBigEndian;

	private readonly Configuration configuration;

	public Rgb161616TiffColor(Configuration configuration, bool isBigEndian)
	{
		this.configuration = configuration;
		this.isBigEndian = isBigEndian;
	}

	public override void Decode(ReadOnlySpan<byte> data, Buffer2D<TPixel> pixels, int left, int top, int width, int height)
	{
		Rgba64 rgba64Default = TiffUtils.Rgba64Default;
		TPixel color = default(TPixel);
		color.FromScaledVector4(Vector4.Zero);
		int num = 0;
		for (int i = top; i < top + height; i++)
		{
			Span<TPixel> destinationPixels = pixels.DangerousGetRowSpan(i).Slice(left, width);
			if (isBigEndian)
			{
				for (int j = 0; j < destinationPixels.Length; j++)
				{
					ulong r = TiffUtils.ConvertToUShortBigEndian(data.Slice(num, 2));
					num += 2;
					ulong g = TiffUtils.ConvertToUShortBigEndian(data.Slice(num, 2));
					num += 2;
					ulong b = TiffUtils.ConvertToUShortBigEndian(data.Slice(num, 2));
					num += 2;
					destinationPixels[j] = TiffUtils.ColorFromRgb64(rgba64Default, r, g, b, color);
				}
			}
			else
			{
				int num2 = destinationPixels.Length * 6;
				PixelOperations<TPixel>.Instance.FromRgb48Bytes(configuration, data.Slice(num, num2), destinationPixels, destinationPixels.Length);
				num += num2;
			}
		}
	}
}
