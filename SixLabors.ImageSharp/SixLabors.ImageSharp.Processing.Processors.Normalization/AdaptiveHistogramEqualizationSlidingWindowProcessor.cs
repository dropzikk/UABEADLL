using System;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Normalization;

public class AdaptiveHistogramEqualizationSlidingWindowProcessor : HistogramEqualizationProcessor
{
	public int NumberOfTiles { get; }

	public AdaptiveHistogramEqualizationSlidingWindowProcessor(int luminanceLevels, bool clipHistogram, int clipLimit, int numberOfTiles)
		: base(luminanceLevels, clipHistogram, clipLimit)
	{
		NumberOfTiles = numberOfTiles;
	}

	public override IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle)
	{
		return new AdaptiveHistogramEqualizationSlidingWindowProcessor<TPixel>(configuration, base.LuminanceLevels, base.ClipHistogram, base.ClipLimit, NumberOfTiles, source, sourceRectangle);
	}
}
internal class AdaptiveHistogramEqualizationSlidingWindowProcessor<TPixel> : HistogramEqualizationProcessor<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly struct SlidingWindowOperation
	{
		private readonly Configuration configuration;

		private readonly AdaptiveHistogramEqualizationSlidingWindowProcessor<TPixel> processor;

		private readonly ImageFrame<TPixel> source;

		private readonly MemoryAllocator memoryAllocator;

		private readonly Buffer2D<TPixel> targetPixels;

		private readonly SlidingWindowInfos swInfos;

		private readonly int yStart;

		private readonly int yEnd;

		private readonly bool useFastPath;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public SlidingWindowOperation(Configuration configuration, AdaptiveHistogramEqualizationSlidingWindowProcessor<TPixel> processor, ImageFrame<TPixel> source, MemoryAllocator memoryAllocator, Buffer2D<TPixel> targetPixels, SlidingWindowInfos swInfos, int yStart, int yEnd, bool useFastPath)
		{
			this.configuration = configuration;
			this.processor = processor;
			this.source = source;
			this.memoryAllocator = memoryAllocator;
			this.targetPixels = targetPixels;
			this.swInfos = swInfos;
			this.yStart = yStart;
			this.yEnd = yEnd;
			this.useFastPath = useFastPath;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Invoke(int x)
		{
			using IMemoryOwner<int> buffer = memoryAllocator.Allocate<int>(processor.LuminanceLevels, AllocationOptions.Clean);
			using IMemoryOwner<int> buffer2 = memoryAllocator.Allocate<int>(processor.LuminanceLevels, AllocationOptions.Clean);
			using IMemoryOwner<int> buffer3 = memoryAllocator.Allocate<int>(processor.LuminanceLevels, AllocationOptions.Clean);
			using IMemoryOwner<Vector4> buffer4 = memoryAllocator.Allocate<Vector4>(swInfos.TileWidth, AllocationOptions.Clean);
			Span<int> span = buffer.GetSpan();
			ref int reference = ref MemoryMarshal.GetReference(span);
			Span<int> span2 = buffer2.GetSpan();
			ref int reference2 = ref MemoryMarshal.GetReference(span2);
			ref int reference3 = ref MemoryMarshal.GetReference(buffer3.GetSpan());
			Span<Vector4> span3 = buffer4.GetSpan();
			ref Vector4 reference4 = ref MemoryMarshal.GetReference(span3);
			for (int i = yStart - swInfos.HalfTileHeight; i < yStart + swInfos.HalfTileHeight; i++)
			{
				if (useFastPath)
				{
					AdaptiveHistogramEqualizationSlidingWindowProcessor<TPixel>.CopyPixelRowFast(source.PixelBuffer, span3, x - swInfos.HalfTileWidth, i, swInfos.TileWidth, configuration);
				}
				else
				{
					AdaptiveHistogramEqualizationSlidingWindowProcessor<TPixel>.CopyPixelRow(source, span3, x - swInfos.HalfTileWidth, i, swInfos.TileWidth, configuration);
				}
				AdaptiveHistogramEqualizationSlidingWindowProcessor<TPixel>.AddPixelsToHistogram(ref reference4, ref reference, processor.LuminanceLevels, span3.Length);
			}
			for (int j = yStart; j < yEnd; j++)
			{
				if (processor.ClipHistogramEnabled)
				{
					span.CopyTo(span2);
					processor.ClipHistogram(span2, processor.ClipLimit);
				}
				int num = (processor.ClipHistogramEnabled ? HistogramEqualizationProcessor<TPixel>.CalculateCdf(ref reference3, ref reference2, span.Length - 1) : HistogramEqualizationProcessor<TPixel>.CalculateCdf(ref reference3, ref reference, span.Length - 1));
				float num2 = swInfos.PixelInTile - num;
				int luminance = HistogramEqualizationProcessor<TPixel>.GetLuminance(source[x, j], processor.LuminanceLevels);
				float num3 = (float)Unsafe.Add(ref reference3, (uint)luminance) / num2;
				targetPixels[x, j].FromVector4(new Vector4(num3, num3, num3, source[x, j].ToVector4().W));
				if (useFastPath)
				{
					AdaptiveHistogramEqualizationSlidingWindowProcessor<TPixel>.CopyPixelRowFast(source.PixelBuffer, span3, x - swInfos.HalfTileWidth, j - swInfos.HalfTileWidth, swInfos.TileWidth, configuration);
				}
				else
				{
					AdaptiveHistogramEqualizationSlidingWindowProcessor<TPixel>.CopyPixelRow(source, span3, x - swInfos.HalfTileWidth, j - swInfos.HalfTileWidth, swInfos.TileWidth, configuration);
				}
				AdaptiveHistogramEqualizationSlidingWindowProcessor<TPixel>.RemovePixelsFromHistogram(ref reference4, ref reference, processor.LuminanceLevels, span3.Length);
				if (useFastPath)
				{
					AdaptiveHistogramEqualizationSlidingWindowProcessor<TPixel>.CopyPixelRowFast(source.PixelBuffer, span3, x - swInfos.HalfTileWidth, j + swInfos.HalfTileWidth, swInfos.TileWidth, configuration);
				}
				else
				{
					AdaptiveHistogramEqualizationSlidingWindowProcessor<TPixel>.CopyPixelRow(source, span3, x - swInfos.HalfTileWidth, j + swInfos.HalfTileWidth, swInfos.TileWidth, configuration);
				}
				AdaptiveHistogramEqualizationSlidingWindowProcessor<TPixel>.AddPixelsToHistogram(ref reference4, ref reference, processor.LuminanceLevels, span3.Length);
			}
		}
	}

	private class SlidingWindowInfos
	{
		public int TileWidth { get; }

		public int TileHeight { get; }

		public int PixelInTile { get; }

		public int HalfTileWidth { get; }

		public int HalfTileHeight { get; }

		public SlidingWindowInfos(int tileWidth, int tileHeight, int halfTileWidth, int halfTileHeight, int pixelInTile)
		{
			TileWidth = tileWidth;
			TileHeight = tileHeight;
			HalfTileWidth = halfTileWidth;
			HalfTileHeight = halfTileHeight;
			PixelInTile = pixelInTile;
		}
	}

	private int Tiles { get; }

	public AdaptiveHistogramEqualizationSlidingWindowProcessor(Configuration configuration, int luminanceLevels, bool clipHistogram, int clipLimit, int tiles, Image<TPixel> source, Rectangle sourceRectangle)
		: base(configuration, luminanceLevels, clipHistogram, clipLimit, source, sourceRectangle)
	{
		Guard.MustBeGreaterThanOrEqualTo(tiles, 2, "tiles");
		Guard.MustBeLessThanOrEqualTo(tiles, 100, "tiles");
		Tiles = tiles;
	}

	protected override void OnFrameApply(ImageFrame<TPixel> source)
	{
		MemoryAllocator memoryAllocator = base.Configuration.MemoryAllocator;
		ParallelOptions parallelOptions = new ParallelOptions
		{
			MaxDegreeOfParallelism = base.Configuration.MaxDegreeOfParallelism
		};
		int num;
		int num2 = (num = source.Width / Tiles);
		int pixelInTile = num2 * num;
		int num3 = (int)((uint)num / 2u);
		int num4 = num3;
		SlidingWindowInfos swInfos = new SlidingWindowInfos(num2, num, num4, num3, pixelInTile);
		using Buffer2D<TPixel> buffer2D = base.Configuration.MemoryAllocator.Allocate2D<TPixel>(source.Width, source.Height);
		SlidingWindowOperation slidingWindowOperation = new SlidingWindowOperation(base.Configuration, this, source, memoryAllocator, buffer2D, swInfos, num3, source.Height - num3, useFastPath: true);
		Parallel.For(num4, source.Width - num4, parallelOptions, ((SlidingWindowOperation)slidingWindowOperation).Invoke);
		SlidingWindowOperation slidingWindowOperation2 = new SlidingWindowOperation(base.Configuration, this, source, memoryAllocator, buffer2D, swInfos, 0, source.Height, useFastPath: false);
		Parallel.For(0, num4, parallelOptions, ((SlidingWindowOperation)slidingWindowOperation2).Invoke);
		SlidingWindowOperation slidingWindowOperation3 = new SlidingWindowOperation(base.Configuration, this, source, memoryAllocator, buffer2D, swInfos, 0, source.Height, useFastPath: false);
		Parallel.For(source.Width - num4, source.Width, parallelOptions, ((SlidingWindowOperation)slidingWindowOperation3).Invoke);
		SlidingWindowOperation slidingWindowOperation4 = new SlidingWindowOperation(base.Configuration, this, source, memoryAllocator, buffer2D, swInfos, 0, num3, useFastPath: false);
		Parallel.For(num4, source.Width - num4, parallelOptions, ((SlidingWindowOperation)slidingWindowOperation4).Invoke);
		SlidingWindowOperation slidingWindowOperation5 = new SlidingWindowOperation(base.Configuration, this, source, memoryAllocator, buffer2D, swInfos, source.Height - num3, source.Height, useFastPath: false);
		Parallel.For(num4, source.Width - num4, parallelOptions, ((SlidingWindowOperation)slidingWindowOperation5).Invoke);
		Buffer2D<TPixel>.SwapOrCopyContent(source.PixelBuffer, buffer2D);
	}

	private static void CopyPixelRow(ImageFrame<TPixel> source, Span<Vector4> rowPixels, int x, int y, int tileWidth, Configuration configuration)
	{
		if (y < 0)
		{
			y = Numerics.Abs(y);
		}
		else if (y >= source.Height)
		{
			int num = y - source.Height;
			y = source.Height - num - 1;
		}
		if (x < 0)
		{
			rowPixels.Clear();
			int num2 = 0;
			for (int i = x; i < x + tileWidth; i++)
			{
				rowPixels[num2] = source[Numerics.Abs(i), y].ToVector4();
				num2++;
			}
		}
		else if (x + tileWidth > source.Width)
		{
			rowPixels.Clear();
			int num3 = 0;
			for (int j = x; j < x + tileWidth; j++)
			{
				if (j >= source.Width)
				{
					int num4 = j - source.Width;
					rowPixels[num3] = source[j - num4 - 1, y].ToVector4();
				}
				else
				{
					rowPixels[num3] = source[j, y].ToVector4();
				}
				num3++;
			}
		}
		else
		{
			CopyPixelRowFast(source.PixelBuffer, rowPixels, x, y, tileWidth, configuration);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void CopyPixelRowFast(Buffer2D<TPixel> source, Span<Vector4> rowPixels, int x, int y, int tileWidth, Configuration configuration)
	{
		PixelOperations<TPixel>.Instance.ToVector4(configuration, source.DangerousGetRowSpan(y).Slice(x, tileWidth), rowPixels);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void AddPixelsToHistogram(ref Vector4 greyValuesBase, ref int histogramBase, int luminanceLevels, int length)
	{
		for (nuint num = 0u; num < (uint)length; num++)
		{
			int bT709Luminance = ColorNumerics.GetBT709Luminance(ref Unsafe.Add(ref greyValuesBase, num), luminanceLevels);
			Unsafe.Add(ref histogramBase, (uint)bT709Luminance)++;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void RemovePixelsFromHistogram(ref Vector4 greyValuesBase, ref int histogramBase, int luminanceLevels, int length)
	{
		for (nuint num = 0u; num < (uint)length; num++)
		{
			int bT709Luminance = ColorNumerics.GetBT709Luminance(ref Unsafe.Add(ref greyValuesBase, num), luminanceLevels);
			Unsafe.Add(ref histogramBase, (uint)bT709Luminance)--;
		}
	}
}
