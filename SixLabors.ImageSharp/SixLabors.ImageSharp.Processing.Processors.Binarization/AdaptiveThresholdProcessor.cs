using System;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Binarization;

public class AdaptiveThresholdProcessor : IImageProcessor
{
	public Color Upper { get; set; }

	public Color Lower { get; set; }

	public float ThresholdLimit { get; set; }

	public AdaptiveThresholdProcessor()
		: this(Color.White, Color.Black, 0.85f)
	{
	}

	public AdaptiveThresholdProcessor(float thresholdLimit)
		: this(Color.White, Color.Black, thresholdLimit)
	{
	}

	public AdaptiveThresholdProcessor(Color upper, Color lower)
		: this(upper, lower, 0.85f)
	{
	}

	public AdaptiveThresholdProcessor(Color upper, Color lower, float thresholdLimit)
	{
		Upper = upper;
		Lower = lower;
		ThresholdLimit = thresholdLimit;
	}

	public IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle) where TPixel : unmanaged, IPixel<TPixel>
	{
		return new AdaptiveThresholdProcessor<TPixel>(configuration, this, source, sourceRectangle);
	}
}
internal class AdaptiveThresholdProcessor<TPixel> : ImageProcessor<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly struct RowOperation : IRowOperation<L8>
	{
		private readonly Configuration configuration;

		private readonly Rectangle bounds;

		private readonly Buffer2D<TPixel> source;

		private readonly Buffer2D<ulong> intImage;

		private readonly TPixel upper;

		private readonly TPixel lower;

		private readonly float thresholdLimit;

		private readonly int startX;

		private readonly int startY;

		private readonly byte clusterSize;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RowOperation(Configuration configuration, Rectangle bounds, Buffer2D<TPixel> source, Buffer2D<ulong> intImage, TPixel upper, TPixel lower, float thresholdLimit, byte clusterSize)
		{
			this.configuration = configuration;
			this.bounds = bounds;
			startX = bounds.X;
			startY = bounds.Y;
			this.source = source;
			this.intImage = intImage;
			this.upper = upper;
			this.lower = lower;
			this.thresholdLimit = thresholdLimit;
			this.clusterSize = clusterSize;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetRequiredBufferLength(Rectangle bounds)
		{
			return bounds.Width;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Invoke(int y, Span<L8> span)
		{
			Span<TPixel> span2 = source.DangerousGetRowSpan(y).Slice(startX, span.Length);
			PixelOperations<TPixel>.Instance.ToL8(configuration, span2, span);
			int num = bounds.Width - 1;
			int num2 = bounds.Height - 1;
			for (int i = 0; i < span2.Length; i++)
			{
				int num3 = Math.Clamp(i - clusterSize + 1, 0, num);
				int num4 = Math.Min(i + clusterSize + 1, num);
				int num5 = Math.Clamp(y - startY - clusterSize + 1, 0, num2);
				int num6 = Math.Min(y - startY + clusterSize + 1, num2);
				uint num7 = (uint)((num4 - num3) * (num6 - num5));
				ulong num8 = Math.Min(intImage[num4, num6] - intImage[num3, num6] - intImage[num4, num5] + intImage[num3, num5], ulong.MaxValue);
				if ((float)(span[i].PackedValue * num7) <= (float)num8 * thresholdLimit)
				{
					span2[i] = lower;
				}
				else
				{
					span2[i] = upper;
				}
			}
		}
	}

	private readonly AdaptiveThresholdProcessor definition;

	public AdaptiveThresholdProcessor(Configuration configuration, AdaptiveThresholdProcessor definition, Image<TPixel> source, Rectangle sourceRectangle)
		: base(configuration, source, sourceRectangle)
	{
		this.definition = definition;
	}

	protected override void OnFrameApply(ImageFrame<TPixel> source)
	{
		Rectangle rectangle = Rectangle.Intersect(base.SourceRectangle, source.Bounds());
		Configuration configuration = base.Configuration;
		TPixel upper = definition.Upper.ToPixel<TPixel>();
		TPixel lower = definition.Lower.ToPixel<TPixel>();
		float thresholdLimit = definition.ThresholdLimit;
		byte clusterSize = (byte)Math.Clamp((float)rectangle.Width / 16f, 0f, 255f);
		using Buffer2D<ulong> intImage = source.CalculateIntegralImage(rectangle);
		RowOperation operation = new RowOperation(configuration, rectangle, source.PixelBuffer, intImage, upper, lower, thresholdLimit, clusterSize);
		ParallelRowIterator.IterateRows<RowOperation, L8>(configuration, rectangle, in operation);
	}
}
