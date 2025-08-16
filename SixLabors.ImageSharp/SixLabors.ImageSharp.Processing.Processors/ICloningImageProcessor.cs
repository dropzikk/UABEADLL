using System;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors;

public interface ICloningImageProcessor : IImageProcessor
{
	ICloningImageProcessor<TPixel> CreatePixelSpecificCloningProcessor<TPixel>(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle) where TPixel : unmanaged, IPixel<TPixel>;
}
public interface ICloningImageProcessor<TPixel> : IImageProcessor<TPixel>, IDisposable where TPixel : unmanaged, IPixel<TPixel>
{
	Image<TPixel> CloneAndExecute();
}
