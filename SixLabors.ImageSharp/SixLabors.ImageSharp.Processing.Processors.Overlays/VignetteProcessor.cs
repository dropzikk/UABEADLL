using System;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Overlays;

public sealed class VignetteProcessor : IImageProcessor
{
	public GraphicsOptions GraphicsOptions { get; }

	public Color VignetteColor { get; }

	internal ValueSize RadiusX { get; }

	internal ValueSize RadiusY { get; }

	public VignetteProcessor(GraphicsOptions options, Color color)
	{
		VignetteColor = color;
		GraphicsOptions = options;
	}

	internal VignetteProcessor(GraphicsOptions options, Color color, ValueSize radiusX, ValueSize radiusY)
	{
		VignetteColor = color;
		RadiusX = radiusX;
		RadiusY = radiusY;
		GraphicsOptions = options;
	}

	public IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle) where TPixel : unmanaged, IPixel<TPixel>
	{
		return new VignetteProcessor<TPixel>(configuration, this, source, sourceRectangle);
	}
}
internal class VignetteProcessor<TPixel> : ImageProcessor<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly struct RowOperation : IRowOperation<float>
	{
		private readonly Configuration configuration;

		private readonly Rectangle bounds;

		private readonly PixelBlender<TPixel> blender;

		private readonly Vector2 center;

		private readonly float maxDistance;

		private readonly float blendPercent;

		private readonly IMemoryOwner<TPixel> colors;

		private readonly Buffer2D<TPixel> source;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RowOperation(Configuration configuration, Rectangle bounds, IMemoryOwner<TPixel> colors, PixelBlender<TPixel> blender, Vector2 center, float maxDistance, float blendPercent, Buffer2D<TPixel> source)
		{
			this.configuration = configuration;
			this.bounds = bounds;
			this.colors = colors;
			this.blender = blender;
			this.center = center;
			this.maxDistance = maxDistance;
			this.blendPercent = blendPercent;
			this.source = source;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetRequiredBufferLength(Rectangle bounds)
		{
			return bounds.Width;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Invoke(int y, Span<float> span)
		{
			Span<TPixel> span2 = colors.GetSpan();
			for (int i = 0; i < bounds.Width; i++)
			{
				float num = Vector2.Distance(center, new Vector2(i + bounds.X, y));
				span[i] = Numerics.Clamp(blendPercent * (0.9f * (num / maxDistance)), 0f, 1f);
			}
			Span<TPixel> span3 = source.DangerousGetRowSpan(y).Slice(bounds.X, bounds.Width);
			blender.Blend(configuration, span3, span3, span2, span);
		}
	}

	private readonly PixelBlender<TPixel> blender;

	private readonly VignetteProcessor definition;

	public VignetteProcessor(Configuration configuration, VignetteProcessor definition, Image<TPixel> source, Rectangle sourceRectangle)
		: base(configuration, source, sourceRectangle)
	{
		this.definition = definition;
		blender = PixelOperations<TPixel>.Instance.GetPixelBlender(definition.GraphicsOptions);
	}

	protected override void OnFrameApply(ImageFrame<TPixel> source)
	{
		TPixel value = definition.VignetteColor.ToPixel<TPixel>();
		float blendPercentage = definition.GraphicsOptions.BlendPercentage;
		Rectangle rectangle = Rectangle.Intersect(base.SourceRectangle, source.Bounds());
		Vector2 center = Rectangle.Center(rectangle);
		float num = definition.RadiusX.Calculate(rectangle.Size);
		float num2 = definition.RadiusY.Calculate(rectangle.Size);
		float num3 = ((num > 0f) ? MathF.Min(num, (float)rectangle.Width * 0.5f) : ((float)rectangle.Width * 0.5f));
		float num4 = ((num2 > 0f) ? MathF.Min(num2, (float)rectangle.Height * 0.5f) : ((float)rectangle.Height * 0.5f));
		float maxDistance = MathF.Sqrt(num3 * num3 + num4 * num4);
		Configuration configuration = base.Configuration;
		using IMemoryOwner<TPixel> memoryOwner = configuration.MemoryAllocator.Allocate<TPixel>(rectangle.Width);
		memoryOwner.GetSpan().Fill(value);
		RowOperation operation = new RowOperation(configuration, rectangle, memoryOwner, blender, center, maxDistance, blendPercentage, source.PixelBuffer);
		ParallelRowIterator.IterateRows<RowOperation, float>(configuration, rectangle, in operation);
	}
}
