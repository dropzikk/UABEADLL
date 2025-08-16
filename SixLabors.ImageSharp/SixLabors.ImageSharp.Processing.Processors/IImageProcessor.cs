using System;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors;

public interface IImageProcessor
{
	IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle) where TPixel : unmanaged, IPixel<TPixel>;
}
public interface IImageProcessor<TPixel> : IDisposable where TPixel : unmanaged, IPixel<TPixel>
{
	void Execute();
}
