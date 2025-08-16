using System;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff.PhotometricInterpretation;

internal class Rgb888TiffColor<TPixel> : TiffBaseColorDecoder<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly Configuration configuration;

	public Rgb888TiffColor(Configuration configuration)
	{
		this.configuration = configuration;
	}

	public override void Decode(ReadOnlySpan<byte> data, Buffer2D<TPixel> pixels, int left, int top, int width, int height)
	{
		int num = 0;
		for (int i = top; i < top + height; i++)
		{
			Span<TPixel> destinationPixels = pixels.DangerousGetRowSpan(i).Slice(left, width);
			int num2 = destinationPixels.Length * 3;
			PixelOperations<TPixel>.Instance.FromRgb24Bytes(configuration, data.Slice(num, num2), destinationPixels, destinationPixels.Length);
			num += num2;
		}
	}
}
