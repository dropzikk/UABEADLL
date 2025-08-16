namespace SixLabors.ImageSharp.Processing.Processors.Convolution;

internal static class ScharrKernels
{
	public static DenseMatrix<float> ScharrX => new float[3, 3]
	{
		{ -3f, 0f, 3f },
		{ -10f, 0f, 10f },
		{ -3f, 0f, 3f }
	};

	public static DenseMatrix<float> ScharrY => new float[3, 3]
	{
		{ 3f, 10f, 3f },
		{ 0f, 0f, 0f },
		{ -3f, -10f, -3f }
	};
}
