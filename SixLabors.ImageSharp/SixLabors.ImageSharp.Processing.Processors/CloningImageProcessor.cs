using System;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors;

public abstract class CloningImageProcessor : ICloningImageProcessor, IImageProcessor
{
	public abstract ICloningImageProcessor<TPixel> CreatePixelSpecificCloningProcessor<TPixel>(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle) where TPixel : unmanaged, IPixel<TPixel>;

	IImageProcessor<TPixel> IImageProcessor.CreatePixelSpecificProcessor<TPixel>(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle)
	{
		return CreatePixelSpecificCloningProcessor(configuration, source, sourceRectangle);
	}
}
public abstract class CloningImageProcessor<TPixel> : ICloningImageProcessor<TPixel>, IImageProcessor<TPixel>, IDisposable where TPixel : unmanaged, IPixel<TPixel>
{
	protected Image<TPixel> Source { get; }

	protected Rectangle SourceRectangle { get; }

	protected Configuration Configuration { get; }

	protected CloningImageProcessor(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle)
	{
		Configuration = configuration;
		Source = source;
		SourceRectangle = sourceRectangle;
	}

	Image<TPixel> ICloningImageProcessor<TPixel>.CloneAndExecute()
	{
		Image<TPixel> image = CreateTarget();
		CheckFrameCount(Source, image);
		_ = Configuration;
		BeforeImageApply(image);
		for (int i = 0; i < Source.Frames.Count; i++)
		{
			ImageFrame<TPixel> source = Source.Frames[i];
			ImageFrame<TPixel> destination = image.Frames[i];
			BeforeFrameApply(source, destination);
			OnFrameApply(source, destination);
			AfterFrameApply(source, destination);
		}
		AfterImageApply(image);
		return image;
	}

	void IImageProcessor<TPixel>.Execute()
	{
		Image<TPixel> image = null;
		try
		{
			image = ((ICloningImageProcessor<TPixel>)this).CloneAndExecute();
			CheckFrameCount(Source, image);
			Source.SwapOrCopyPixelsBuffersFrom(image);
		}
		finally
		{
			image?.Dispose();
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected abstract Size GetDestinationSize();

	protected virtual void BeforeImageApply(Image<TPixel> destination)
	{
	}

	protected virtual void BeforeFrameApply(ImageFrame<TPixel> source, ImageFrame<TPixel> destination)
	{
	}

	protected abstract void OnFrameApply(ImageFrame<TPixel> source, ImageFrame<TPixel> destination);

	protected virtual void AfterFrameApply(ImageFrame<TPixel> source, ImageFrame<TPixel> destination)
	{
	}

	protected virtual void AfterImageApply(Image<TPixel> destination)
	{
	}

	protected virtual void Dispose(bool disposing)
	{
	}

	private Image<TPixel> CreateTarget()
	{
		Image<TPixel> source = Source;
		Size destinationSize = GetDestinationSize();
		ImageFrame<TPixel>[] array = new ImageFrame<TPixel>[source.Frames.Count];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = new ImageFrame<TPixel>(Configuration, destinationSize.Width, destinationSize.Height, source.Frames[i].Metadata.DeepClone());
		}
		return new Image<TPixel>(Configuration, source.Metadata.DeepClone(), array);
	}

	private void CheckFrameCount(Image<TPixel> a, Image<TPixel> b)
	{
		if (a.Frames.Count != b.Frames.Count)
		{
			throw new ImageProcessingException("An error occurred when processing the image using " + GetType().Name + ". The processor changed the number of frames.");
		}
	}
}
