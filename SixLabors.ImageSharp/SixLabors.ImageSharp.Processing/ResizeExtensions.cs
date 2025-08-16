using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace SixLabors.ImageSharp.Processing;

public static class ResizeExtensions
{
	public static IImageProcessingContext Resize(this IImageProcessingContext source, Size size)
	{
		return source.Resize(size.Width, size.Height, KnownResamplers.Bicubic, compand: false);
	}

	public static IImageProcessingContext Resize(this IImageProcessingContext source, Size size, bool compand)
	{
		return source.Resize(size.Width, size.Height, KnownResamplers.Bicubic, compand);
	}

	public static IImageProcessingContext Resize(this IImageProcessingContext source, int width, int height)
	{
		return source.Resize(width, height, KnownResamplers.Bicubic, compand: false);
	}

	public static IImageProcessingContext Resize(this IImageProcessingContext source, int width, int height, bool compand)
	{
		return source.Resize(width, height, KnownResamplers.Bicubic, compand);
	}

	public static IImageProcessingContext Resize(this IImageProcessingContext source, int width, int height, IResampler sampler)
	{
		return source.Resize(width, height, sampler, compand: false);
	}

	public static IImageProcessingContext Resize(this IImageProcessingContext source, Size size, IResampler sampler, bool compand)
	{
		return source.Resize(size.Width, size.Height, sampler, new Rectangle(0, 0, size.Width, size.Height), compand);
	}

	public static IImageProcessingContext Resize(this IImageProcessingContext source, int width, int height, IResampler sampler, bool compand)
	{
		return source.Resize(width, height, sampler, new Rectangle(0, 0, width, height), compand);
	}

	public static IImageProcessingContext Resize(this IImageProcessingContext source, int width, int height, IResampler sampler, Rectangle sourceRectangle, Rectangle targetRectangle, bool compand)
	{
		ResizeOptions options = new ResizeOptions
		{
			Size = new Size(width, height),
			Mode = ResizeMode.Manual,
			Sampler = sampler,
			TargetRectangle = targetRectangle,
			Compand = compand
		};
		return source.ApplyProcessor(new ResizeProcessor(options, source.GetCurrentSize()), sourceRectangle);
	}

	public static IImageProcessingContext Resize(this IImageProcessingContext source, int width, int height, IResampler sampler, Rectangle targetRectangle, bool compand)
	{
		ResizeOptions options = new ResizeOptions
		{
			Size = new Size(width, height),
			Mode = ResizeMode.Manual,
			Sampler = sampler,
			TargetRectangle = targetRectangle,
			Compand = compand
		};
		return source.Resize(options);
	}

	public static IImageProcessingContext Resize(this IImageProcessingContext source, ResizeOptions options)
	{
		return source.ApplyProcessor(new ResizeProcessor(options, source.GetCurrentSize()));
	}
}
