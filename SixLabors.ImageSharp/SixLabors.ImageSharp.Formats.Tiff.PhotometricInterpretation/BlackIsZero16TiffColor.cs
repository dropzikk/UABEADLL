using System;
using System.Numerics;
using SixLabors.ImageSharp.Formats.Tiff.Utils;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff.PhotometricInterpretation;

internal class BlackIsZero16TiffColor<TPixel> : TiffBaseColorDecoder<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly bool isBigEndian;

	private readonly Configuration configuration;

	public BlackIsZero16TiffColor(Configuration configuration, bool isBigEndian)
	{
		this.configuration = configuration;
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
			Span<TPixel> destinationPixels = pixels.DangerousGetRowSpan(i).Slice(left, width);
			if (isBigEndian)
			{
				for (int j = 0; j < destinationPixels.Length; j++)
				{
					ushort intensity = TiffUtils.ConvertToUShortBigEndian(data.Slice(num, 2));
					num += 2;
					destinationPixels[j] = TiffUtils.ColorFromL16(l16Default, intensity, color);
				}
			}
			else
			{
				int num2 = destinationPixels.Length * 2;
				PixelOperations<TPixel>.Instance.FromL16Bytes(configuration, data.Slice(num, num2), destinationPixels, destinationPixels.Length);
				num += num2;
			}
		}
	}
}
