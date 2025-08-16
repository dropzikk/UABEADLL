using System.Numerics;

namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

public sealed class SkewProcessor : AffineTransformProcessor
{
	public float DegreesX { get; }

	public float DegreesY { get; }

	public SkewProcessor(float degreesX, float degreesY, Size sourceSize)
		: this(degreesX, degreesY, KnownResamplers.Bicubic, sourceSize)
	{
	}

	public SkewProcessor(float degreesX, float degreesY, IResampler sampler, Size sourceSize)
		: this(TransformUtils.CreateSkewMatrixDegrees(degreesX, degreesY, sourceSize), sampler, sourceSize)
	{
		DegreesX = degreesX;
		DegreesY = degreesY;
	}

	private SkewProcessor(Matrix3x2 skewMatrix, IResampler sampler, Size sourceSize)
		: base(skewMatrix, sampler, TransformUtils.GetTransformedSize(sourceSize, skewMatrix))
	{
	}
}
