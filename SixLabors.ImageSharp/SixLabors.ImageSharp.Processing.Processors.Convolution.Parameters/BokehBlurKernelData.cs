using System.Numerics;

namespace SixLabors.ImageSharp.Processing.Processors.Convolution.Parameters;

internal readonly struct BokehBlurKernelData
{
	public readonly Vector4[] Parameters;

	public readonly Complex64[][] Kernels;

	public BokehBlurKernelData(Vector4[] parameters, Complex64[][] kernels)
	{
		Parameters = parameters;
		Kernels = kernels;
	}
}
