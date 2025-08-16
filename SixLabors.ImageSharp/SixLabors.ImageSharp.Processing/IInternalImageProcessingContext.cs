using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing;

internal interface IInternalImageProcessingContext<TPixel> : IImageProcessingContext where TPixel : unmanaged, IPixel<TPixel>
{
	Image<TPixel> GetResultImage();
}
