using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace SixLabors.ImageSharp.Processing;

public static class SwizzleExtensions
{
	public static IImageProcessingContext Swizzle<TSwizzler>(this IImageProcessingContext source, TSwizzler swizzler) where TSwizzler : struct, ISwizzler
	{
		return source.ApplyProcessor(new SwizzleProcessor<TSwizzler>(swizzler));
	}
}
