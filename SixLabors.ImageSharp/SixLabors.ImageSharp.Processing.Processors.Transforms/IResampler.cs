using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

public interface IResampler
{
	float Radius { get; }

	float GetValue(float x);

	void ApplyTransform<TPixel>(IResamplingTransformImageProcessor<TPixel> processor) where TPixel : unmanaged, IPixel<TPixel>;
}
