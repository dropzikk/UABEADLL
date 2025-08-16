namespace SixLabors.ImageSharp.Processing.Processors.Convolution;

internal static class LaplacianKernelFactory
{
	public static DenseMatrix<float> CreateKernel(uint length)
	{
		Guard.MustBeGreaterThanOrEqualTo(length, 3u, "length");
		Guard.IsFalse(length % 2 == 0, "length", "The kernel length must be an odd number.");
		DenseMatrix<float> result = new DenseMatrix<float>((int)length);
		result.Fill(-1f);
		int num = (int)(length / 2);
		result[num, num] = length * length - 1;
		return result;
	}
}
