using System.Collections.Concurrent;
using System.Collections.Generic;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors;

namespace SixLabors.ImageSharp.Processing;

internal class DefaultImageProcessorContext<TPixel> : IInternalImageProcessingContext<TPixel>, IImageProcessingContext where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly bool mutate;

	private readonly Image<TPixel> source;

	private Image<TPixel>? destination;

	public Configuration Configuration { get; }

	public IDictionary<object, object> Properties { get; } = new ConcurrentDictionary<object, object>();

	public DefaultImageProcessorContext(Configuration configuration, Image<TPixel> source, bool mutate)
	{
		Configuration = configuration;
		this.mutate = mutate;
		this.source = source;
		if (this.mutate)
		{
			destination = source;
		}
	}

	public Image<TPixel> GetResultImage()
	{
		if (!mutate && destination == null)
		{
			destination = source.Clone();
		}
		return destination;
	}

	public Size GetCurrentSize()
	{
		return GetCurrentBounds().Size;
	}

	public IImageProcessingContext ApplyProcessor(IImageProcessor processor)
	{
		return ApplyProcessor(processor, GetCurrentBounds());
	}

	public IImageProcessingContext ApplyProcessor(IImageProcessor processor, Rectangle rectangle)
	{
		if (!mutate && destination == null)
		{
			if (processor is ICloningImageProcessor cloningImageProcessor)
			{
				using ICloningImageProcessor<TPixel> cloningImageProcessor2 = cloningImageProcessor.CreatePixelSpecificCloningProcessor(Configuration, source, rectangle);
				destination = cloningImageProcessor2.CloneAndExecute();
				return this;
			}
			destination = source.Clone();
		}
		using IImageProcessor<TPixel> imageProcessor = processor.CreatePixelSpecificProcessor(Configuration, destination, rectangle);
		imageProcessor.Execute();
		return this;
	}

	private Rectangle GetCurrentBounds()
	{
		return destination?.Bounds ?? source.Bounds;
	}
}
