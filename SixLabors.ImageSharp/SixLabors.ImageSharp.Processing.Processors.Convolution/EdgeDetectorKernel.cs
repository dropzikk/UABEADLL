using System;

namespace SixLabors.ImageSharp.Processing.Processors.Convolution;

public readonly struct EdgeDetectorKernel : IEquatable<EdgeDetectorKernel>
{
	public static readonly EdgeDetectorKernel Laplacian3x3 = new EdgeDetectorKernel(LaplacianKernels.Laplacian3x3);

	public static readonly EdgeDetectorKernel Laplacian5x5 = new EdgeDetectorKernel(LaplacianKernels.Laplacian5x5);

	public static readonly EdgeDetectorKernel LaplacianOfGaussian = new EdgeDetectorKernel(LaplacianKernels.LaplacianOfGaussianXY);

	public DenseMatrix<float> KernelXY { get; }

	public EdgeDetectorKernel(DenseMatrix<float> kernelXY)
	{
		KernelXY = kernelXY;
	}

	public static bool operator ==(EdgeDetectorKernel left, EdgeDetectorKernel right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(EdgeDetectorKernel left, EdgeDetectorKernel right)
	{
		return !(left == right);
	}

	public override bool Equals(object? obj)
	{
		if (obj is EdgeDetectorKernel other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(EdgeDetectorKernel other)
	{
		return KernelXY.Equals(other.KernelXY);
	}

	public override int GetHashCode()
	{
		return KernelXY.GetHashCode();
	}
}
