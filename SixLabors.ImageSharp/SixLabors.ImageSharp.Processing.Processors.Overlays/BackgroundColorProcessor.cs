using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Overlays;

public sealed class BackgroundColorProcessor : IImageProcessor
{
	public GraphicsOptions GraphicsOptions { get; }

	public Color Color { get; }

	public BackgroundColorProcessor(GraphicsOptions options, Color color)
	{
		Color = color;
		GraphicsOptions = options;
	}

	public IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle) where TPixel : unmanaged, IPixel<TPixel>
	{
		return new BackgroundColorProcessor<TPixel>(configuration, this, source, sourceRectangle);
	}
}
internal class BackgroundColorProcessor<TPixel> : ImageProcessor<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly struct RowOperation : IRowOperation
	{
		private readonly Configuration configuration;

		private readonly Rectangle bounds;

		private readonly PixelBlender<TPixel> blender;

		private readonly IMemoryOwner<float> amount;

		private readonly IMemoryOwner<TPixel> colors;

		private readonly Buffer2D<TPixel> source;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RowOperation(Configuration configuration, Rectangle bounds, PixelBlender<TPixel> blender, IMemoryOwner<float> amount, IMemoryOwner<TPixel> colors, Buffer2D<TPixel> source)
		{
			this.configuration = configuration;
			this.bounds = bounds;
			this.blender = blender;
			this.amount = amount;
			this.colors = colors;
			this.source = source;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Invoke(int y)
		{
			Span<TPixel> span = source.DangerousGetRowSpan(y).Slice(bounds.X, bounds.Width);
			blender.Blend(configuration, span, colors.GetSpan(), span, amount.GetSpan());
		}
	}

	private readonly BackgroundColorProcessor definition;

	public BackgroundColorProcessor(Configuration configuration, BackgroundColorProcessor definition, Image<TPixel> source, Rectangle sourceRectangle)
		: base(configuration, source, sourceRectangle)
	{
		this.definition = definition;
	}

	protected override void OnFrameApply(ImageFrame<TPixel> source)
	{
		TPixel value = definition.Color.ToPixel<TPixel>();
		GraphicsOptions graphicsOptions = definition.GraphicsOptions;
		Rectangle rectangle = Rectangle.Intersect(base.SourceRectangle, source.Bounds());
		Configuration configuration = base.Configuration;
		MemoryAllocator memoryAllocator = configuration.MemoryAllocator;
		using IMemoryOwner<TPixel> memoryOwner = memoryAllocator.Allocate<TPixel>(rectangle.Width);
		using IMemoryOwner<float> memoryOwner2 = memoryAllocator.Allocate<float>(rectangle.Width);
		memoryOwner.GetSpan().Fill(value);
		memoryOwner2.GetSpan().Fill(graphicsOptions.BlendPercentage);
		PixelBlender<TPixel> pixelBlender = PixelOperations<TPixel>.Instance.GetPixelBlender(graphicsOptions);
		RowOperation operation = new RowOperation(configuration, rectangle, pixelBlender, memoryOwner2, memoryOwner, source.PixelBuffer);
		ParallelRowIterator.IterateRows(configuration, rectangle, in operation);
	}
}
