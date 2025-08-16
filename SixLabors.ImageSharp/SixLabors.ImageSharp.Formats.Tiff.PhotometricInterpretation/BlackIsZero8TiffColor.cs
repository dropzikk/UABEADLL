using System;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff.PhotometricInterpretation;

internal class BlackIsZero8TiffColor<TPixel> : TiffBaseColorDecoder<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly Configuration configuration;

	public BlackIsZero8TiffColor(Configuration configuration)
	{
		this.configuration = configuration;
	}

	public override void Decode(ReadOnlySpan<byte> data, Buffer2D<TPixel> pixels, int left, int top, int width, int height)
	{
		int num = 0;
		for (int i = top; i < top + height; i++)
		{
			Span<TPixel> destinationPixels = pixels.DangerousGetRowSpan(i).Slice(left, width);
			int length = destinationPixels.Length;
			PixelOperations<TPixel>.Instance.FromL8Bytes(configuration, data.Slice(num, length), destinationPixels, destinationPixels.Length);
			num += length;
		}
	}
}
