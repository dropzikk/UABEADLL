using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Effects;

public sealed class PixelateProcessor : IImageProcessor
{
	public int Size { get; }

	public PixelateProcessor(int size)
	{
		Guard.MustBeGreaterThan(size, 0, "size");
		Size = size;
	}

	public IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle) where TPixel : unmanaged, IPixel<TPixel>
	{
		return new PixelateProcessor<TPixel>(configuration, this, source, sourceRectangle);
	}
}
internal class PixelateProcessor<TPixel> : ImageProcessor<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly struct RowOperation
	{
		private readonly int minX;

		private readonly int maxX;

		private readonly int maxXIndex;

		private readonly int maxY;

		private readonly int maxYIndex;

		private readonly int size;

		private readonly int radius;

		private readonly Buffer2D<TPixel> source;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RowOperation(Rectangle bounds, int size, Buffer2D<TPixel> source)
		{
			minX = bounds.X;
			maxX = bounds.Right;
			maxXIndex = bounds.Right - 1;
			maxY = bounds.Bottom;
			maxYIndex = bounds.Bottom - 1;
			this.size = size;
			radius = size >> 1;
			this.source = source;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Invoke(int y)
		{
			Span<TPixel> span = source.DangerousGetRowSpan(Math.Min(y + radius, maxYIndex));
			for (int i = minX; i < maxX; i += size)
			{
				TPixel val = span[Math.Min(i + radius, maxXIndex)];
				for (int j = y; j < y + size && j < maxY; j++)
				{
					for (int k = i; k < i + size && k < maxX; k++)
					{
						source[k, j] = val;
					}
				}
			}
		}
	}

	private readonly PixelateProcessor definition;

	private int Size => definition.Size;

	public PixelateProcessor(Configuration configuration, PixelateProcessor definition, Image<TPixel> source, Rectangle sourceRectangle)
		: base(configuration, source, sourceRectangle)
	{
		this.definition = definition;
	}

	protected override void OnFrameApply(ImageFrame<TPixel> source)
	{
		Rectangle interest = Rectangle.Intersect(base.SourceRectangle, source.Bounds());
		int size = Size;
		Guard.MustBeBetweenOrEqualTo(size, 0, interest.Width, "size");
		Guard.MustBeBetweenOrEqualTo(size, 0, interest.Height, "size");
		Parallel.ForEach(EnumerableExtensions.SteppedRange(interest.Y, (int i) => i < interest.Bottom, size), base.Configuration.GetParallelOptions(), new RowOperation(interest, size, source.PixelBuffer).Invoke);
	}
}
