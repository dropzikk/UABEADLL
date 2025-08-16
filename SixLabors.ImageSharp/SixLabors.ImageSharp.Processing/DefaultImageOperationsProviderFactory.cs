using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing;

internal class DefaultImageOperationsProviderFactory : IImageProcessingContextFactory
{
	public IInternalImageProcessingContext<TPixel> CreateImageProcessingContext<TPixel>(Configuration configuration, Image<TPixel> source, bool mutate) where TPixel : unmanaged, IPixel<TPixel>
	{
		return new DefaultImageProcessorContext<TPixel>(configuration, source, mutate);
	}
}
