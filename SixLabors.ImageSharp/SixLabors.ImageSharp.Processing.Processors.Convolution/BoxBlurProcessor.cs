using System;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Convolution;

public sealed class BoxBlurProcessor : IImageProcessor
{
	public const int DefaultRadius = 7;

	public int Radius { get; }

	public BorderWrappingMode BorderWrapModeX { get; }

	public BorderWrappingMode BorderWrapModeY { get; }

	public BoxBlurProcessor(int radius, BorderWrappingMode borderWrapModeX, BorderWrappingMode borderWrapModeY)
	{
		Radius = radius;
		BorderWrapModeX = borderWrapModeX;
		BorderWrapModeY = borderWrapModeY;
	}

	public BoxBlurProcessor(int radius)
		: this(radius, BorderWrappingMode.Repeat, BorderWrappingMode.Repeat)
	{
	}

	public BoxBlurProcessor()
		: this(7)
	{
	}

	public IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle) where TPixel : unmanaged, IPixel<TPixel>
	{
		return new BoxBlurProcessor<TPixel>(configuration, this, source, sourceRectangle, BorderWrapModeX, BorderWrapModeY);
	}
}
internal class BoxBlurProcessor<TPixel> : ImageProcessor<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	public float[] Kernel { get; }

	public BorderWrappingMode BorderWrapModeX { get; }

	public BorderWrappingMode BorderWrapModeY { get; }

	public BoxBlurProcessor(Configuration configuration, BoxBlurProcessor definition, Image<TPixel> source, Rectangle sourceRectangle)
		: base(configuration, source, sourceRectangle)
	{
		int kernelSize = definition.Radius * 2 + 1;
		Kernel = CreateBoxKernel(kernelSize);
	}

	public BoxBlurProcessor(Configuration configuration, BoxBlurProcessor definition, Image<TPixel> source, Rectangle sourceRectangle, BorderWrappingMode borderWrapModeX, BorderWrappingMode borderWrapModeY)
		: base(configuration, source, sourceRectangle)
	{
		int kernelSize = definition.Radius * 2 + 1;
		Kernel = CreateBoxKernel(kernelSize);
		BorderWrapModeX = borderWrapModeX;
		BorderWrapModeY = borderWrapModeY;
	}

	protected override void OnFrameApply(ImageFrame<TPixel> source)
	{
		using Convolution2PassProcessor<TPixel> convolution2PassProcessor = new Convolution2PassProcessor<TPixel>(base.Configuration, Kernel, preserveAlpha: false, base.Source, base.SourceRectangle, BorderWrapModeX, BorderWrapModeY);
		convolution2PassProcessor.Apply(source);
	}

	private static float[] CreateBoxKernel(int kernelSize)
	{
		float[] array = new float[kernelSize];
		array.AsSpan().Fill(1f / (float)kernelSize);
		return array;
	}
}
