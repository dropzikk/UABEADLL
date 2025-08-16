using System.Numerics;
using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace SixLabors.ImageSharp.Processing;

public static class TransformExtensions
{
	public static IImageProcessingContext Transform(this IImageProcessingContext source, AffineTransformBuilder builder)
	{
		return source.Transform(builder, KnownResamplers.Bicubic);
	}

	public static IImageProcessingContext Transform(this IImageProcessingContext source, AffineTransformBuilder builder, IResampler sampler)
	{
		return source.Transform(new Rectangle(Point.Empty, source.GetCurrentSize()), builder, sampler);
	}

	public static IImageProcessingContext Transform(this IImageProcessingContext source, Rectangle sourceRectangle, AffineTransformBuilder builder, IResampler sampler)
	{
		Matrix3x2 matrix3x = builder.BuildMatrix(sourceRectangle);
		Size transformedSize = TransformUtils.GetTransformedSize(sourceRectangle.Size, matrix3x);
		return source.Transform(sourceRectangle, matrix3x, transformedSize, sampler);
	}

	public static IImageProcessingContext Transform(this IImageProcessingContext source, Rectangle sourceRectangle, Matrix3x2 transform, Size targetDimensions, IResampler sampler)
	{
		return source.ApplyProcessor(new AffineTransformProcessor(transform, sampler, targetDimensions), sourceRectangle);
	}

	public static IImageProcessingContext Transform(this IImageProcessingContext source, ProjectiveTransformBuilder builder)
	{
		return source.Transform(builder, KnownResamplers.Bicubic);
	}

	public static IImageProcessingContext Transform(this IImageProcessingContext source, ProjectiveTransformBuilder builder, IResampler sampler)
	{
		return source.Transform(new Rectangle(Point.Empty, source.GetCurrentSize()), builder, sampler);
	}

	public static IImageProcessingContext Transform(this IImageProcessingContext source, Rectangle sourceRectangle, ProjectiveTransformBuilder builder, IResampler sampler)
	{
		Matrix4x4 matrix4x = builder.BuildMatrix(sourceRectangle);
		Size transformedSize = TransformUtils.GetTransformedSize(sourceRectangle.Size, matrix4x);
		return source.Transform(sourceRectangle, matrix4x, transformedSize, sampler);
	}

	public static IImageProcessingContext Transform(this IImageProcessingContext source, Rectangle sourceRectangle, Matrix4x4 transform, Size targetDimensions, IResampler sampler)
	{
		return source.ApplyProcessor(new ProjectiveTransformProcessor(transform, sampler, targetDimensions), sourceRectangle);
	}
}
