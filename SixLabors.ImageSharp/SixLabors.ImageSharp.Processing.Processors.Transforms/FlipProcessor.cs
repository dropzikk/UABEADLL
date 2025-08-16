using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

public sealed class FlipProcessor : IImageProcessor
{
	public FlipMode FlipMode { get; }

	public FlipProcessor(FlipMode flipMode)
	{
		FlipMode = flipMode;
	}

	public IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle) where TPixel : unmanaged, IPixel<TPixel>
	{
		return new FlipProcessor<TPixel>(configuration, this, source, sourceRectangle);
	}
}
internal class FlipProcessor<TPixel> : ImageProcessor<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly struct RowOperation : IRowOperation
	{
		private readonly Buffer2D<TPixel> source;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RowOperation(Buffer2D<TPixel> source)
		{
			this.source = source;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Invoke(int y)
		{
			source.DangerousGetRowSpan(y).Reverse();
		}
	}

	private readonly FlipProcessor definition;

	public FlipProcessor(Configuration configuration, FlipProcessor definition, Image<TPixel> source, Rectangle sourceRectangle)
		: base(configuration, source, sourceRectangle)
	{
		this.definition = definition;
	}

	protected override void OnFrameApply(ImageFrame<TPixel> source)
	{
		switch (definition.FlipMode)
		{
		case FlipMode.Vertical:
			FlipX(source.PixelBuffer, base.Configuration);
			break;
		case FlipMode.Horizontal:
			FlipY(source, base.Configuration);
			break;
		}
	}

	private static void FlipX(Buffer2D<TPixel> source, Configuration configuration)
	{
		int height = source.Height;
		using IMemoryOwner<TPixel> memoryOwner = configuration.MemoryAllocator.Allocate<TPixel>(source.Width);
		Span<TPixel> span = memoryOwner.Memory.Span;
		for (int i = 0; i < (int)((uint)height / 2u); i++)
		{
			int y = height - i - 1;
			Span<TPixel> destination = source.DangerousGetRowSpan(y);
			Span<TPixel> destination2 = source.DangerousGetRowSpan(i);
			destination.CopyTo(span);
			destination2.CopyTo(destination);
			span.CopyTo(destination2);
		}
	}

	private static void FlipY(ImageFrame<TPixel> source, Configuration configuration)
	{
		RowOperation operation = new RowOperation(source.PixelBuffer);
		ParallelRowIterator.IterateRows(configuration, source.Bounds(), in operation);
	}
}
