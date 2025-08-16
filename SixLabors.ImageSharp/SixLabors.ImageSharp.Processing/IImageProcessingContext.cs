using System.Collections.Generic;
using SixLabors.ImageSharp.Processing.Processors;

namespace SixLabors.ImageSharp.Processing;

public interface IImageProcessingContext
{
	Configuration Configuration { get; }

	IDictionary<object, object> Properties { get; }

	Size GetCurrentSize();

	IImageProcessingContext ApplyProcessor(IImageProcessor processor, Rectangle rectangle);

	IImageProcessingContext ApplyProcessor(IImageProcessor processor);
}
