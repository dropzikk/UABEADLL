namespace SixLabors.ImageSharp.Processing.Processors.Convolution;

internal static class PrewittKernels
{
	public static DenseMatrix<float> PrewittX => new float[3, 3]
	{
		{ -1f, 0f, 1f },
		{ -1f, 0f, 1f },
		{ -1f, 0f, 1f }
	};

	public static DenseMatrix<float> PrewittY => new float[3, 3]
	{
		{ 1f, 1f, 1f },
		{ 0f, 0f, 0f },
		{ -1f, -1f, -1f }
	};
}
