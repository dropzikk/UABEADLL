using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

internal abstract class TransformProcessor<TPixel> : CloningImageProcessor<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	protected TransformProcessor(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle)
		: base(configuration, source, sourceRectangle)
	{
	}

	protected override void AfterImageApply(Image<TPixel> destination)
	{
		TransformProcessorHelpers.UpdateDimensionalMetadata(destination);
		base.AfterImageApply(destination);
	}
}
