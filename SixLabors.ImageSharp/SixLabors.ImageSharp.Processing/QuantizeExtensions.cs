using SixLabors.ImageSharp.Processing.Processors.Quantization;

namespace SixLabors.ImageSharp.Processing;

public static class QuantizeExtensions
{
	public static IImageProcessingContext Quantize(this IImageProcessingContext source)
	{
		return source.Quantize(KnownQuantizers.Octree);
	}

	public static IImageProcessingContext Quantize(this IImageProcessingContext source, IQuantizer quantizer)
	{
		return source.ApplyProcessor(new QuantizeProcessor(quantizer));
	}

	public static IImageProcessingContext Quantize(this IImageProcessingContext source, Rectangle rectangle)
	{
		return source.Quantize(KnownQuantizers.Octree, rectangle);
	}

	public static IImageProcessingContext Quantize(this IImageProcessingContext source, IQuantizer quantizer, Rectangle rectangle)
	{
		return source.ApplyProcessor(new QuantizeProcessor(quantizer), rectangle);
	}
}
