using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Filters;

public class FilterProcessor : IImageProcessor
{
	public ColorMatrix Matrix { get; }

	public FilterProcessor(ColorMatrix matrix)
	{
		Matrix = matrix;
	}

	public virtual IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle) where TPixel : unmanaged, IPixel<TPixel>
	{
		return new FilterProcessor<TPixel>(configuration, this, source, sourceRectangle);
	}
}
internal class FilterProcessor<TPixel> : ImageProcessor<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly struct RowOperation : IRowOperation<Vector4>
	{
		private readonly int startX;

		private readonly Buffer2D<TPixel> source;

		private readonly ColorMatrix matrix;

		private readonly Configuration configuration;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RowOperation(int startX, Buffer2D<TPixel> source, ColorMatrix matrix, Configuration configuration)
		{
			this.startX = startX;
			this.source = source;
			this.matrix = matrix;
			this.configuration = configuration;
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
			PixelOperations<TPixel>.Instance.ToVector4(configuration, span2, span, PixelConversionModifiers.Scale);
			ColorNumerics.Transform(span, ref Unsafe.AsRef(in matrix));
			PixelOperations<TPixel>.Instance.FromVector4Destructive(configuration, span, span2, PixelConversionModifiers.Scale);
		}
	}

	private readonly FilterProcessor definition;

	public FilterProcessor(Configuration configuration, FilterProcessor definition, Image<TPixel> source, Rectangle sourceRectangle)
		: base(configuration, source, sourceRectangle)
	{
		this.definition = definition;
	}

	protected override void OnFrameApply(ImageFrame<TPixel> source)
	{
		Rectangle rectangle = Rectangle.Intersect(base.SourceRectangle, source.Bounds());
		RowOperation operation = new RowOperation(rectangle.X, source.PixelBuffer, definition.Matrix, base.Configuration);
		ParallelRowIterator.IterateRows<RowOperation, Vector4>(base.Configuration, rectangle, in operation);
	}
}
