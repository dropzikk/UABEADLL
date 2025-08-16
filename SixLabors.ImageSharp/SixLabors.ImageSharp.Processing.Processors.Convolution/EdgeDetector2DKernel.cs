using System;

namespace SixLabors.ImageSharp.Processing.Processors.Convolution;

public readonly struct EdgeDetector2DKernel : IEquatable<EdgeDetector2DKernel>
{
	public static readonly EdgeDetector2DKernel KayyaliKernel = new EdgeDetector2DKernel(KayyaliKernels.KayyaliX, KayyaliKernels.KayyaliY);

	public static readonly EdgeDetector2DKernel PrewittKernel = new EdgeDetector2DKernel(PrewittKernels.PrewittX, PrewittKernels.PrewittY);

	public static readonly EdgeDetector2DKernel RobertsCrossKernel = new EdgeDetector2DKernel(RobertsCrossKernels.RobertsCrossX, RobertsCrossKernels.RobertsCrossY);

	public static readonly EdgeDetector2DKernel ScharrKernel = new EdgeDetector2DKernel(ScharrKernels.ScharrX, ScharrKernels.ScharrY);

	public static readonly EdgeDetector2DKernel SobelKernel = new EdgeDetector2DKernel(SobelKernels.SobelX, SobelKernels.SobelY);

	public DenseMatrix<float> KernelX { get; }

	public DenseMatrix<float> KernelY { get; }

	public EdgeDetector2DKernel(DenseMatrix<float> kernelX, DenseMatrix<float> kernelY)
	{
		Guard.IsTrue(kernelX.Size.Equals(kernelY.Size), "kernelX kernelY", "Kernel sizes must be the same.");
		KernelX = kernelX;
		KernelY = kernelY;
	}

	public static bool operator ==(EdgeDetector2DKernel left, EdgeDetector2DKernel right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(EdgeDetector2DKernel left, EdgeDetector2DKernel right)
	{
		return !(left == right);
	}

	public override bool Equals(object? obj)
	{
		if (obj is EdgeDetector2DKernel other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(EdgeDetector2DKernel other)
	{
		if (KernelX.Equals(other.KernelX))
		{
			return KernelY.Equals(other.KernelY);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(KernelX, KernelY);
	}
}
