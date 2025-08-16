using SixLabors.ImageSharp.Processing.Processors.Convolution;

namespace SixLabors.ImageSharp.Processing;

public static class KnownEdgeDetectorKernels
{
	public static EdgeDetector2DKernel Kayyali { get; } = EdgeDetector2DKernel.KayyaliKernel;

	public static EdgeDetectorCompassKernel Kirsch { get; } = EdgeDetectorCompassKernel.Kirsch;

	public static EdgeDetectorKernel Laplacian3x3 { get; } = EdgeDetectorKernel.Laplacian3x3;

	public static EdgeDetectorKernel Laplacian5x5 { get; } = EdgeDetectorKernel.Laplacian5x5;

	public static EdgeDetectorKernel LaplacianOfGaussian { get; } = EdgeDetectorKernel.LaplacianOfGaussian;

	public static EdgeDetector2DKernel Prewitt { get; } = EdgeDetector2DKernel.PrewittKernel;

	public static EdgeDetector2DKernel RobertsCross { get; } = EdgeDetector2DKernel.RobertsCrossKernel;

	public static EdgeDetectorCompassKernel Robinson { get; } = EdgeDetectorCompassKernel.Robinson;

	public static EdgeDetector2DKernel Scharr { get; } = EdgeDetector2DKernel.ScharrKernel;

	public static EdgeDetector2DKernel Sobel { get; } = EdgeDetector2DKernel.SobelKernel;
}
