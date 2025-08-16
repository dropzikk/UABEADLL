using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Effects;

internal sealed class PositionAwarePixelRowDelegateProcessor : IImageProcessor
{
	public readonly struct PixelRowDelegate : IPixelRowDelegate
	{
		private readonly PixelRowOperation<Point> pixelRowOperation;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public PixelRowDelegate(PixelRowOperation<Point> pixelRowOperation)
		{
			this.pixelRowOperation = pixelRowOperation;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Invoke(Span<Vector4> span, Point offset)
		{
			pixelRowOperation(span, offset);
		}
	}

	public PixelRowOperation<Point> PixelRowOperation { get; }

	public PixelConversionModifiers Modifiers { get; }

	public PositionAwarePixelRowDelegateProcessor(PixelRowOperation<Point> pixelRowOperation, PixelConversionModifiers modifiers)
	{
		PixelRowOperation = pixelRowOperation;
		Modifiers = modifiers;
	}

	public IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle) where TPixel : unmanaged, IPixel<TPixel>
	{
		return new PixelRowDelegateProcessor<TPixel, PixelRowDelegate>(new PixelRowDelegate(PixelRowOperation), configuration, Modifiers, source, sourceRectangle);
	}
}
