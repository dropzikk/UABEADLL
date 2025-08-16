namespace SixLabors.ImageSharp.Processing.Processors.Convolution;

internal static class LaplacianKernels
{
	public static DenseMatrix<float> Laplacian3x3 => LaplacianKernelFactory.CreateKernel(3u);

	public static DenseMatrix<float> Laplacian5x5 => LaplacianKernelFactory.CreateKernel(5u);

	public static DenseMatrix<float> LaplacianOfGaussianXY => new float[5, 5]
	{
		{ 0f, 0f, -1f, 0f, 0f },
		{ 0f, -1f, -2f, -1f, 0f },
		{ -1f, -2f, 16f, -2f, -1f },
		{ 0f, -1f, -2f, -1f, 0f },
		{ 0f, 0f, -1f, 0f, 0f }
	};
}
