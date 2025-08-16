using System;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Binarization;

public class BinaryThresholdProcessor : IImageProcessor
{
	public float Threshold { get; }

	public Color UpperColor { get; }

	public Color LowerColor { get; }

	public BinaryThresholdMode Mode { get; }

	public BinaryThresholdProcessor(float threshold, BinaryThresholdMode mode)
		: this(threshold, Color.White, Color.Black, mode)
	{
	}

	public BinaryThresholdProcessor(float threshold)
		: this(threshold, Color.White, Color.Black, BinaryThresholdMode.Luminance)
	{
	}

	public BinaryThresholdProcessor(float threshold, Color upperColor, Color lowerColor, BinaryThresholdMode mode)
	{
		Guard.MustBeBetweenOrEqualTo(threshold, 0f, 1f, "threshold");
		Threshold = threshold;
		UpperColor = upperColor;
		LowerColor = lowerColor;
		Mode = mode;
	}

	public BinaryThresholdProcessor(float threshold, Color upperColor, Color lowerColor)
		: this(threshold, upperColor, lowerColor, BinaryThresholdMode.Luminance)
	{
	}

	public IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle) where TPixel : unmanaged, IPixel<TPixel>
	{
		return new BinaryThresholdProcessor<TPixel>(configuration, this, source, sourceRectangle);
	}
}
internal class BinaryThresholdProcessor<TPixel> : ImageProcessor<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly struct RowOperation : IRowOperation<Rgb24>
	{
		private readonly Buffer2D<TPixel> source;

		private readonly TPixel upper;

		private readonly TPixel lower;

		private readonly byte threshold;

		private readonly BinaryThresholdMode mode;

		private readonly int startX;

		private readonly Configuration configuration;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RowOperation(int startX, Buffer2D<TPixel> source, TPixel upper, TPixel lower, byte threshold, BinaryThresholdMode mode, Configuration configuration)
		{
			this.startX = startX;
			this.source = source;
			this.upper = upper;
			this.lower = lower;
			this.threshold = threshold;
			this.mode = mode;
			this.configuration = configuration;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetRequiredBufferLength(Rectangle bounds)
		{
			return bounds.Width;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Invoke(int y, Span<Rgb24> span)
		{
			TPixel val = upper;
			TPixel val2 = lower;
			Span<TPixel> span2 = source.DangerousGetRowSpan(y).Slice(startX, span.Length);
			PixelOperations<TPixel>.Instance.ToRgb24(configuration, span2, span);
			switch (mode)
			{
			case BinaryThresholdMode.Luminance:
			{
				byte b = threshold;
				for (int j = 0; j < span2.Length; j++)
				{
					Rgb24 rgb = span[j];
					byte b2 = ColorNumerics.Get8BitBT709Luminance(rgb.R, rgb.G, rgb.B);
					span2[j] = ((b2 >= b) ? val : val2);
				}
				break;
			}
			case BinaryThresholdMode.Saturation:
			{
				float num2 = (float)(int)threshold / 255f;
				for (int k = 0; k < span2.Length; k++)
				{
					float saturation = GetSaturation(span[k]);
					span2[k] = ((saturation >= num2) ? val : val2);
				}
				break;
			}
			case BinaryThresholdMode.MaxChroma:
			{
				float num = (float)(int)threshold * 0.5f;
				for (int i = 0; i < span2.Length; i++)
				{
					float maxChroma = GetMaxChroma(span[i]);
					span2[i] = ((maxChroma >= num) ? val : val2);
				}
				break;
			}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static float GetSaturation(Rgb24 rgb)
		{
			float x = (float)(int)rgb.R * 0.003921569f;
			float x2 = (float)(int)rgb.G * 0.003921569f;
			float y = (float)(int)rgb.B * 0.003921569f;
			float num = MathF.Max(x, MathF.Max(x2, y));
			float num2 = MathF.Min(x, MathF.Min(x2, y));
			float num3 = num - num2;
			if (MathF.Abs(num3) < Constants.Epsilon)
			{
				return 0f;
			}
			if ((num + num2) * 0.5f <= 0.5f)
			{
				return num3 / (num + num2);
			}
			return num3 / (2f - num - num2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static float GetMaxChroma(Rgb24 rgb)
		{
			float num = (int)rgb.R;
			float num2 = (int)rgb.G;
			float num3 = (int)rgb.B;
			float num4 = 128f + (-0.168736f * num - 0.331264f * num2 + 0.5f * num3);
			float num5 = 128f + (0.5f * num - 0.418688f * num2 - 0.081312f * num3);
			return MathF.Max(MathF.Abs(num4 - 127.5f), MathF.Abs(num5 - 127.5f));
		}
	}

	private readonly BinaryThresholdProcessor definition;

	public BinaryThresholdProcessor(Configuration configuration, BinaryThresholdProcessor definition, Image<TPixel> source, Rectangle sourceRectangle)
		: base(configuration, source, sourceRectangle)
	{
		this.definition = definition;
	}

	protected override void OnFrameApply(ImageFrame<TPixel> source)
	{
		byte threshold = (byte)MathF.Round(definition.Threshold * 255f);
		TPixel upper = definition.UpperColor.ToPixel<TPixel>();
		TPixel lower = definition.LowerColor.ToPixel<TPixel>();
		Rectangle sourceRectangle = base.SourceRectangle;
		Configuration configuration = base.Configuration;
		Rectangle rectangle = Rectangle.Intersect(sourceRectangle, source.Bounds());
		RowOperation operation = new RowOperation(rectangle.X, source.PixelBuffer, upper, lower, threshold, definition.Mode, configuration);
		ParallelRowIterator.IterateRows<RowOperation, Rgb24>(configuration, rectangle, in operation);
	}
}
