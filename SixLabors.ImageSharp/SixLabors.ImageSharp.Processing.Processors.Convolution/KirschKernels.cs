namespace SixLabors.ImageSharp.Processing.Processors.Convolution;

internal static class KirschKernels
{
	public static DenseMatrix<float> North => new float[3, 3]
	{
		{ 5f, 5f, 5f },
		{ -3f, 0f, -3f },
		{ -3f, -3f, -3f }
	};

	public static DenseMatrix<float> NorthWest => new float[3, 3]
	{
		{ 5f, 5f, -3f },
		{ 5f, 0f, -3f },
		{ -3f, -3f, -3f }
	};

	public static DenseMatrix<float> West => new float[3, 3]
	{
		{ 5f, -3f, -3f },
		{ 5f, 0f, -3f },
		{ 5f, -3f, -3f }
	};

	public static DenseMatrix<float> SouthWest => new float[3, 3]
	{
		{ -3f, -3f, -3f },
		{ 5f, 0f, -3f },
		{ 5f, 5f, -3f }
	};

	public static DenseMatrix<float> South => new float[3, 3]
	{
		{ -3f, -3f, -3f },
		{ -3f, 0f, -3f },
		{ 5f, 5f, 5f }
	};

	public static DenseMatrix<float> SouthEast => new float[3, 3]
	{
		{ -3f, -3f, -3f },
		{ -3f, 0f, 5f },
		{ -3f, 5f, 5f }
	};

	public static DenseMatrix<float> East => new float[3, 3]
	{
		{ -3f, -3f, 5f },
		{ -3f, 0f, 5f },
		{ -3f, -3f, 5f }
	};

	public static DenseMatrix<float> NorthEast => new float[3, 3]
	{
		{ -3f, 5f, 5f },
		{ -3f, 0f, 5f },
		{ -3f, -3f, -3f }
	};
}
