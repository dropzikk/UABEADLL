using System;

namespace SixLabors.ImageSharp.Processing;

public static class PadExtensions
{
	public static IImageProcessingContext Pad(this IImageProcessingContext source, int width, int height)
	{
		return source.Pad(width, height, default(Color));
	}

	public static IImageProcessingContext Pad(this IImageProcessingContext source, int width, int height, Color color)
	{
		Size currentSize = source.GetCurrentSize();
		ResizeOptions options = new ResizeOptions
		{
			Size = new Size(Math.Max(width, currentSize.Width), Math.Max(height, currentSize.Height)),
			Mode = ResizeMode.BoxPad,
			Sampler = KnownResamplers.NearestNeighbor,
			PadColor = color
		};
		return source.Resize(options);
	}
}
