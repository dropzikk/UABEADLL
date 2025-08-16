namespace SixLabors.ImageSharp.Processing.Processors.Convolution;

internal static class SobelKernels
{
	public static DenseMatrix<float> SobelX => new float[3, 3]
	{
		{ -1f, 0f, 1f },
		{ -2f, 0f, 2f },
		{ -1f, 0f, 1f }
	};

	public static DenseMatrix<float> SobelY => new float[3, 3]
	{
		{ -1f, -2f, -1f },
		{ 0f, 0f, 0f },
		{ 1f, 2f, 1f }
	};
}
