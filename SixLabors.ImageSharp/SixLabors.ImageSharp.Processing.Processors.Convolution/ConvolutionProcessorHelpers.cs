using System;
using System.Diagnostics.CodeAnalysis;

namespace SixLabors.ImageSharp.Processing.Processors.Convolution;

internal static class ConvolutionProcessorHelpers
{
	internal static int GetDefaultGaussianRadius(float sigma)
	{
		return (int)MathF.Ceiling(sigma * 3f);
	}

	internal static float[] CreateGaussianBlurKernel(int size, float weight)
	{
		float[] array = new float[size];
		float num = 0f;
		float num2 = (float)(size - 1) / 2f;
		for (int i = 0; i < size; i++)
		{
			float num3 = Numerics.Gaussian((float)i - num2, weight);
			num += num3;
			array[i] = num3;
		}
		for (int j = 0; j < size; j++)
		{
			array[j] /= num;
		}
		return array;
	}

	internal static float[] CreateGaussianSharpenKernel(int size, float weight)
	{
		float[] array = new float[size];
		float num = 0f;
		float num2 = (float)(size - 1) / 2f;
		for (int i = 0; i < size; i++)
		{
			float num3 = Numerics.Gaussian((float)i - num2, weight);
			num += num3;
			array[i] = num3;
		}
		int num4 = (int)num2;
		for (int j = 0; j < size; j++)
		{
			if (j == num4)
			{
				array[j] = 2f * num - array[j];
			}
			else
			{
				array[j] = 0f - array[j];
			}
		}
		for (int k = 0; k < size; k++)
		{
			array[k] /= num;
		}
		return array;
	}

	public static bool TryGetLinearlySeparableComponents(this DenseMatrix<float> matrix, [NotNullWhen(true)] out float[]? row, [NotNullWhen(true)] out float[]? column)
	{
		int rows = matrix.Rows;
		int columns = matrix.Columns;
		float[] array = new float[columns];
		float[] array2 = new float[rows];
		for (int i = 1; i < rows; i++)
		{
			float num = matrix[i, 0] / matrix[0, 0];
			for (int j = 1; j < columns; j++)
			{
				if (Math.Abs(num - matrix[i, j] / matrix[0, j]) > 0.0001f)
				{
					row = null;
					column = null;
					return false;
				}
			}
			array2[i] = num;
		}
		array2[0] = 1f;
		for (int k = 0; k < columns; k++)
		{
			array[k] = matrix[0, k];
		}
		row = array;
		column = array2;
		return true;
	}
}
