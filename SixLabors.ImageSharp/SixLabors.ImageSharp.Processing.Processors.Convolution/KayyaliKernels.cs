namespace SixLabors.ImageSharp.Processing.Processors.Convolution;

internal static class KayyaliKernels
{
	public static DenseMatrix<float> KayyaliX => new float[3, 3]
	{
		{ 6f, 0f, -6f },
		{ 0f, 0f, 0f },
		{ -6f, 0f, 6f }
	};

	public static DenseMatrix<float> KayyaliY => new float[3, 3]
	{
		{ -6f, 0f, 6f },
		{ 0f, 0f, 0f },
		{ 6f, 0f, -6f }
	};
}
