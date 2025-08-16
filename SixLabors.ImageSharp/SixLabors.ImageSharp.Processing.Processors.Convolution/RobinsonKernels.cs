namespace SixLabors.ImageSharp.Processing.Processors.Convolution;

internal static class RobinsonKernels
{
	public static DenseMatrix<float> North => new float[3, 3]
	{
		{ 1f, 2f, 1f },
		{ 0f, 0f, 0f },
		{ -1f, -2f, -1f }
	};

	public static DenseMatrix<float> NorthWest => new float[3, 3]
	{
		{ 2f, 1f, 0f },
		{ 1f, 0f, -1f },
		{ 0f, -1f, -2f }
	};

	public static DenseMatrix<float> West => new float[3, 3]
	{
		{ 1f, 0f, -1f },
		{ 2f, 0f, -2f },
		{ 1f, 0f, -1f }
	};

	public static DenseMatrix<float> SouthWest => new float[3, 3]
	{
		{ 0f, -1f, -2f },
		{ 1f, 0f, -1f },
		{ 2f, 1f, 0f }
	};

	public static DenseMatrix<float> South => new float[3, 3]
	{
		{ -1f, -2f, -1f },
		{ 0f, 0f, 0f },
		{ 1f, 2f, 1f }
	};

	public static DenseMatrix<float> SouthEast => new float[3, 3]
	{
		{ -2f, -1f, 0f },
		{ -1f, 0f, 1f },
		{ 0f, 1f, 2f }
	};

	public static DenseMatrix<float> East => new float[3, 3]
	{
		{ -1f, 0f, 1f },
		{ -2f, 0f, 2f },
		{ -1f, 0f, 1f }
	};

	public static DenseMatrix<float> NorthEast => new float[3, 3]
	{
		{ 0f, 1f, 2f },
		{ -1f, 0f, 1f },
		{ -2f, -1f, 0f }
	};
}
