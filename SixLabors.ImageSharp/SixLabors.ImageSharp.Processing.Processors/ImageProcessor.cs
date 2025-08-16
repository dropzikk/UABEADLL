using System;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors;

public abstract class ImageProcessor<TPixel> : IImageProcessor<TPixel>, IDisposable where TPixel : unmanaged, IPixel<TPixel>
{
	protected Image<TPixel> Source { get; }

	protected Rectangle SourceRectangle { get; }

	protected Configuration Configuration { get; }

	protected ImageProcessor(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle)
	{
		Configuration = configuration;
		Source = source;
		SourceRectangle = sourceRectangle;
	}

	void IImageProcessor<TPixel>.Execute()
	{
		BeforeImageApply();
		foreach (ImageFrame<TPixel> frame in Source.Frames)
		{
			Apply(frame);
		}
		AfterImageApply();
	}

	public void Apply(ImageFrame<TPixel> source)
	{
		BeforeFrameApply(source);
		OnFrameApply(source);
		AfterFrameApply(source);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void BeforeImageApply()
	{
	}

	protected virtual void BeforeFrameApply(ImageFrame<TPixel> source)
	{
	}

	protected abstract void OnFrameApply(ImageFrame<TPixel> source);

	protected virtual void AfterFrameApply(ImageFrame<TPixel> source)
	{
	}

	protected virtual void AfterImageApply()
	{
	}

	protected virtual void Dispose(bool disposing)
	{
	}
}
