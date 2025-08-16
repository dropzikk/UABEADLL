using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Effects;

internal sealed class PixelRowDelegateProcessor : IImageProcessor
{
	public readonly struct PixelRowDelegate : IPixelRowDelegate
	{
		private readonly PixelRowOperation pixelRowOperation;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public PixelRowDelegate(PixelRowOperation pixelRowOperation)
		{
			this.pixelRowOperation = pixelRowOperation;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Invoke(Span<Vector4> span, Point offset)
		{
			pixelRowOperation(span);
		}
	}

	public PixelRowOperation PixelRowOperation { get; }

	public PixelConversionModifiers Modifiers { get; }

	public PixelRowDelegateProcessor(PixelRowOperation pixelRowOperation, PixelConversionModifiers modifiers)
	{
		PixelRowOperation = pixelRowOperation;
		Modifiers = modifiers;
	}

	public IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle) where TPixel : unmanaged, IPixel<TPixel>
	{
		return new PixelRowDelegateProcessor<TPixel, PixelRowDelegate>(new PixelRowDelegate(PixelRowOperation), configuration, Modifiers, source, sourceRectangle);
	}
}
internal sealed class PixelRowDelegateProcessor<TPixel, TDelegate> : ImageProcessor<TPixel> where TPixel : unmanaged, IPixel<TPixel> where TDelegate : struct, IPixelRowDelegate
{
	private readonly struct RowOperation : IRowOperation<Vector4>
	{
		private readonly int startX;

		private readonly Buffer2D<TPixel> source;

		private readonly Configuration configuration;

		private readonly PixelConversionModifiers modifiers;

		private readonly TDelegate rowProcessor;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RowOperation(int startX, Buffer2D<TPixel> source, Configuration configuration, PixelConversionModifiers modifiers, in TDelegate rowProcessor)
		{
			this.startX = startX;
			this.source = source;
			this.configuration = configuration;
			this.modifiers = modifiers;
			this.rowProcessor = rowProcessor;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetRequiredBufferLength(Rectangle bounds)
		{
			return bounds.Width;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Invoke(int y, Span<Vector4> span)
		{
			Span<TPixel> span2 = source.DangerousGetRowSpan(y).Slice(startX, span.Length);
			PixelOperations<TPixel>.Instance.ToVector4(configuration, span2, span, modifiers);
			Unsafe.AsRef(in rowProcessor).Invoke(span, new Point(startX, y));
			PixelOperations<TPixel>.Instance.FromVector4Destructive(configuration, span, span2, modifiers);
		}
	}

	private readonly TDelegate rowDelegate;

	private readonly PixelConversionModifiers modifiers;

	public PixelRowDelegateProcessor(in TDelegate rowDelegate, Configuration configuration, PixelConversionModifiers modifiers, Image<TPixel> source, Rectangle sourceRectangle)
		: base(configuration, source, sourceRectangle)
	{
		this.rowDelegate = rowDelegate;
		this.modifiers = modifiers;
	}

	protected override void OnFrameApply(ImageFrame<TPixel> source)
	{
		Rectangle rectangle = Rectangle.Intersect(base.SourceRectangle, source.Bounds());
		RowOperation operation = new RowOperation(rectangle.X, source.PixelBuffer, base.Configuration, modifiers, in rowDelegate);
		ParallelRowIterator.IterateRows<RowOperation, Vector4>(base.Configuration, rectangle, in operation);
	}
}
