namespace SixLabors.ImageSharp.Processing.Processors.Convolution;

internal static class RobertsCrossKernels
{
	public static DenseMatrix<float> RobertsCrossX => new float[2, 2]
	{
		{ 1f, 0f },
		{ 0f, -1f }
	};

	public static DenseMatrix<float> RobertsCrossY => new float[2, 2]
	{
		{ 0f, 1f },
		{ -1f, 0f }
	};
}
