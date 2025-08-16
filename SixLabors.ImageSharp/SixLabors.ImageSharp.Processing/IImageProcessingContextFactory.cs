using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing;

internal interface IImageProcessingContextFactory
{
	IInternalImageProcessingContext<TPixel> CreateImageProcessingContext<TPixel>(Configuration configuration, Image<TPixel> source, bool mutate) where TPixel : unmanaged, IPixel<TPixel>;
}
