using System;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

public interface IResamplingTransformImageProcessor<TPixel> : IImageProcessor<TPixel>, IDisposable where TPixel : unmanaged, IPixel<TPixel>
{
	void ApplyTransform<TResampler>(in TResampler sampler) where TResampler : struct, IResampler;
}
