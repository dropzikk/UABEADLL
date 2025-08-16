using System;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

public sealed class CropProcessor : CloningImageProcessor
{
	public Rectangle CropRectangle { get; }

	public CropProcessor(Rectangle cropRectangle, Size sourceSize)
	{
		Guard.IsTrue(new Rectangle(Point.Empty, sourceSize).Contains(cropRectangle), "cropRectangle", "Crop rectangle should be smaller than the source bounds.");
		CropRectangle = cropRectangle;
	}

	public override ICloningImageProcessor<TPixel> CreatePixelSpecificCloningProcessor<TPixel>(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle)
	{
		return new CropProcessor<TPixel>(configuration, this, source, sourceRectangle);
	}
}
internal class CropProcessor<TPixel> : TransformProcessor<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly struct RowOperation : IRowOperation
	{
		private readonly Rectangle bounds;

		private readonly Buffer2D<TPixel> source;

		private readonly Buffer2D<TPixel> destination;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RowOperation(Rectangle bounds, Buffer2D<TPixel> source, Buffer2D<TPixel> destination)
		{
			this.bounds = bounds;
			this.source = source;
			this.destination = destination;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Invoke(int y)
		{
			Span<TPixel> span = source.DangerousGetRowSpan(y);
			int left = bounds.Left;
			Span<TPixel> span2 = span.Slice(left, span.Length - left);
			Span<TPixel> span3 = destination.DangerousGetRowSpan(y - bounds.Top);
			span = span2.Slice(0, bounds.Width);
			span.CopyTo(span3);
		}
	}

	private readonly Rectangle cropRectangle;

	public CropProcessor(Configuration configuration, CropProcessor definition, Image<TPixel> source, Rectangle sourceRectangle)
		: base(configuration, source, sourceRectangle)
	{
		cropRectangle = definition.CropRectangle;
	}

	protected override Size GetDestinationSize()
	{
		return new Size(cropRectangle.Width, cropRectangle.Height);
	}

	protected override void OnFrameApply(ImageFrame<TPixel> source, ImageFrame<TPixel> destination)
	{
		if (source.Width == destination.Width && source.Height == destination.Height && base.SourceRectangle == cropRectangle)
		{
			source.GetPixelMemoryGroup().CopyTo(destination.GetPixelMemoryGroup());
			return;
		}
		Rectangle rectangle = cropRectangle;
		ParallelRowIterator.IterateRows<RowOperation>(rectangle, ParallelExecutionSettings.FromConfiguration(base.Configuration).MultiplyMinimumPixelsPerTask(4), new RowOperation(rectangle, source.PixelBuffer, destination.PixelBuffer));
	}
}
