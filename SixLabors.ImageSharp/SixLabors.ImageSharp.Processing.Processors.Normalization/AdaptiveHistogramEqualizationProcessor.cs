using System;
using System.Buffers;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Normalization;

public class AdaptiveHistogramEqualizationProcessor : HistogramEqualizationProcessor
{
	public int NumberOfTiles { get; }

	public AdaptiveHistogramEqualizationProcessor(int luminanceLevels, bool clipHistogram, int clipLimit, int numberOfTiles)
		: base(luminanceLevels, clipHistogram, clipLimit)
	{
		NumberOfTiles = numberOfTiles;
	}

	public override IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle)
	{
		return new AdaptiveHistogramEqualizationProcessor<TPixel>(configuration, base.LuminanceLevels, base.ClipHistogram, base.ClipLimit, NumberOfTiles, source, sourceRectangle);
	}
}
internal class AdaptiveHistogramEqualizationProcessor<TPixel> : HistogramEqualizationProcessor<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly struct RowIntervalOperation : IRowIntervalOperation
	{
		private readonly CdfTileData cdfData;

		private readonly List<(int Y, int CdfY)> tileYStartPositions;

		private readonly int tileWidth;

		private readonly int tileHeight;

		private readonly int tileCount;

		private readonly int halfTileWidth;

		private readonly int luminanceLevels;

		private readonly Buffer2D<TPixel> source;

		private readonly int sourceWidth;

		private readonly int sourceHeight;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RowIntervalOperation(CdfTileData cdfData, List<(int Y, int CdfY)> tileYStartPositions, int tileWidth, int tileHeight, int tileCount, int halfTileWidth, int luminanceLevels, Buffer2D<TPixel> source)
		{
			this.cdfData = cdfData;
			this.tileYStartPositions = tileYStartPositions;
			this.tileWidth = tileWidth;
			this.tileHeight = tileHeight;
			this.tileCount = tileCount;
			this.halfTileWidth = halfTileWidth;
			this.luminanceLevels = luminanceLevels;
			this.source = source;
			sourceWidth = source.Width;
			sourceHeight = source.Height;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Invoke(in RowInterval rows)
		{
			for (int i = rows.Min; i < rows.Max; i++)
			{
				(int Y, int CdfY) tuple = tileYStartPositions[i];
				int item = tuple.Y;
				int item2 = tuple.CdfY;
				int num = 0;
				int num2 = halfTileWidth;
				for (int j = 0; j < tileCount - 1; j++)
				{
					int num3 = 0;
					int num4 = Math.Min(item + tileHeight, sourceHeight);
					int num5 = Math.Min(num2 + tileWidth, sourceWidth);
					for (int k = item; k < num4; k++)
					{
						Span<TPixel> span = source.DangerousGetRowSpan(k);
						int num6 = 0;
						for (int l = num2; l < num5; l++)
						{
							ref TPixel reference = ref span[l];
							float num7 = AdaptiveHistogramEqualizationProcessor<TPixel>.InterpolateBetweenFourTiles(reference, cdfData, tileCount, tileCount, num6, num3, num, item2, tileWidth, tileHeight, luminanceLevels);
							reference.FromVector4(new Vector4(num7, num7, num7, reference.ToVector4().W));
							num6++;
						}
						num3++;
					}
					num++;
					num2 += tileWidth;
				}
			}
		}

		void IRowIntervalOperation.Invoke(in RowInterval rows)
		{
			Invoke(in rows);
		}
	}

	private sealed class CdfTileData : IDisposable
	{
		private readonly struct RowIntervalOperation : IRowIntervalOperation
		{
			private readonly HistogramEqualizationProcessor<TPixel> processor;

			private readonly MemoryAllocator allocator;

			private readonly Buffer2D<int> cdfMinBuffer2D;

			private readonly Buffer2D<int> cdfLutBuffer2D;

			private readonly List<(int Y, int CdfY)> tileYStartPositions;

			private readonly int tileWidth;

			private readonly int tileHeight;

			private readonly int luminanceLevels;

			private readonly Buffer2D<TPixel> source;

			private readonly int sourceWidth;

			private readonly int sourceHeight;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public RowIntervalOperation(HistogramEqualizationProcessor<TPixel> processor, MemoryAllocator allocator, Buffer2D<int> cdfMinBuffer2D, Buffer2D<int> cdfLutBuffer2D, List<(int Y, int CdfY)> tileYStartPositions, int tileWidth, int tileHeight, int luminanceLevels, Buffer2D<TPixel> source)
			{
				this.processor = processor;
				this.allocator = allocator;
				this.cdfMinBuffer2D = cdfMinBuffer2D;
				this.cdfLutBuffer2D = cdfLutBuffer2D;
				this.tileYStartPositions = tileYStartPositions;
				this.tileWidth = tileWidth;
				this.tileHeight = tileHeight;
				this.luminanceLevels = luminanceLevels;
				this.source = source;
				sourceWidth = source.Width;
				sourceHeight = source.Height;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Invoke(in RowInterval rows)
			{
				for (int i = rows.Min; i < rows.Max; i++)
				{
					int num = 0;
					int item = tileYStartPositions[i].CdfY;
					int item2 = tileYStartPositions[i].Y;
					int num2 = Math.Min(item2 + tileHeight, sourceHeight);
					Span<int> span = cdfMinBuffer2D.DangerousGetRowSpan(item);
					span.Clear();
					using IMemoryOwner<int> buffer = allocator.Allocate<int>(luminanceLevels);
					Span<int> span2 = buffer.GetSpan();
					ref int reference = ref MemoryMarshal.GetReference(span2);
					for (int j = 0; j < sourceWidth; j += tileWidth)
					{
						span2.Clear();
						ref int reference2 = ref MemoryMarshal.GetReference(cdfLutBuffer2D.DangerousGetRowSpan(i).Slice(num * luminanceLevels, luminanceLevels));
						int num3 = Math.Min(j + tileWidth, sourceWidth);
						for (int k = item2; k < num2; k++)
						{
							Span<TPixel> span3 = source.DangerousGetRowSpan(k);
							for (int l = j; l < num3; l++)
							{
								span2[HistogramEqualizationProcessor<TPixel>.GetLuminance(span3[l], luminanceLevels)]++;
							}
						}
						if (processor.ClipHistogramEnabled)
						{
							processor.ClipHistogram(span2, processor.ClipLimit);
						}
						span[num] += HistogramEqualizationProcessor<TPixel>.CalculateCdf(ref reference2, ref reference, span2.Length - 1);
						num++;
					}
				}
			}

			void IRowIntervalOperation.Invoke(in RowInterval rows)
			{
				Invoke(in rows);
			}
		}

		private readonly Configuration configuration;

		private readonly MemoryAllocator memoryAllocator;

		private readonly Buffer2D<int> cdfMinBuffer2D;

		private readonly Buffer2D<int> cdfLutBuffer2D;

		private readonly int pixelsInTile;

		private readonly int sourceWidth;

		private readonly int tileWidth;

		private readonly int tileHeight;

		private readonly int luminanceLevels;

		private readonly List<(int Y, int CdfY)> tileYStartPositions;

		public CdfTileData(Configuration configuration, int sourceWidth, int sourceHeight, int tileCountX, int tileCountY, int tileWidth, int tileHeight, int luminanceLevels)
		{
			this.configuration = configuration;
			memoryAllocator = configuration.MemoryAllocator;
			this.luminanceLevels = luminanceLevels;
			cdfMinBuffer2D = memoryAllocator.Allocate2D<int>(tileCountX, tileCountY);
			cdfLutBuffer2D = memoryAllocator.Allocate2D<int>(tileCountX * luminanceLevels, tileCountY);
			this.sourceWidth = sourceWidth;
			this.tileWidth = tileWidth;
			this.tileHeight = tileHeight;
			pixelsInTile = tileWidth * tileHeight;
			tileYStartPositions = new List<(int, int)>();
			int num = 0;
			for (int i = 0; i < sourceHeight; i += tileHeight)
			{
				tileYStartPositions.Add((i, num));
				num++;
			}
		}

		public void CalculateLookupTables(ImageFrame<TPixel> source, HistogramEqualizationProcessor<TPixel> processor)
		{
			RowIntervalOperation operation = new RowIntervalOperation(processor, memoryAllocator, cdfMinBuffer2D, cdfLutBuffer2D, tileYStartPositions, tileWidth, tileHeight, luminanceLevels, source.PixelBuffer);
			ParallelRowIterator.IterateRowIntervals(configuration, new Rectangle(0, 0, sourceWidth, tileYStartPositions.Count), in operation);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Span<int> GetCdfLutSpan(int tileX, int tileY)
		{
			return cdfLutBuffer2D.DangerousGetRowSpan(tileY).Slice(tileX * luminanceLevels, luminanceLevels);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float RemapGreyValue(int tilesX, int tilesY, int luminance)
		{
			int num = cdfMinBuffer2D[tilesX, tilesY];
			Span<int> cdfLutSpan = GetCdfLutSpan(tilesX, tilesY);
			if (pixelsInTile - num != 0)
			{
				return (float)cdfLutSpan[luminance] / (float)(pixelsInTile - num);
			}
			return cdfLutSpan[luminance] / pixelsInTile;
		}

		public void Dispose()
		{
			cdfMinBuffer2D.Dispose();
			cdfLutBuffer2D.Dispose();
		}
	}

	private int Tiles { get; }

	public AdaptiveHistogramEqualizationProcessor(Configuration configuration, int luminanceLevels, bool clipHistogram, int clipLimit, int tiles, Image<TPixel> source, Rectangle sourceRectangle)
		: base(configuration, luminanceLevels, clipHistogram, clipLimit, source, sourceRectangle)
	{
		Guard.MustBeGreaterThanOrEqualTo(tiles, 2, "tiles");
		Guard.MustBeLessThanOrEqualTo(tiles, 100, "tiles");
		Tiles = tiles;
	}

	protected override void OnFrameApply(ImageFrame<TPixel> source)
	{
		int width = source.Width;
		int height = source.Height;
		int num = (int)MathF.Ceiling((float)width / (float)Tiles);
		int num2 = (int)MathF.Ceiling((float)height / (float)Tiles);
		int tiles = Tiles;
		int num3 = (int)((uint)num / 2u);
		int num4 = (int)((uint)num2 / 2u);
		int luminanceLevels = base.LuminanceLevels;
		using CdfTileData cdfTileData = new CdfTileData(base.Configuration, width, height, Tiles, Tiles, num, num2, luminanceLevels);
		cdfTileData.CalculateLookupTables(source, this);
		List<(int, int)> list = new List<(int, int)>();
		int num5 = 0;
		int num6 = num4;
		for (int i = 0; i < tiles - 1; i++)
		{
			list.Add((num6, num5));
			num5++;
			num6 += num2;
		}
		RowIntervalOperation operation = new RowIntervalOperation(cdfTileData, list, num, num2, tiles, num3, luminanceLevels, source.PixelBuffer);
		ParallelRowIterator.IterateRowIntervals(base.Configuration, new Rectangle(0, 0, width, list.Count), in operation);
		ProcessBorderColumn(source.PixelBuffer, cdfTileData, 0, height, Tiles, num2, 0, num3, luminanceLevels);
		int xStart = (Tiles - 1) * num + num3;
		ProcessBorderColumn(source.PixelBuffer, cdfTileData, Tiles - 1, height, Tiles, num2, xStart, width, luminanceLevels);
		ProcessBorderRow(source.PixelBuffer, cdfTileData, 0, width, Tiles, num, 0, num4, luminanceLevels);
		int yStart = (Tiles - 1) * num2 + num4;
		ProcessBorderRow(source.PixelBuffer, cdfTileData, Tiles - 1, width, Tiles, num, yStart, height, luminanceLevels);
		ProcessCornerTile(source.PixelBuffer, cdfTileData, 0, 0, 0, num3, 0, num4, luminanceLevels);
		ProcessCornerTile(source.PixelBuffer, cdfTileData, 0, Tiles - 1, 0, num3, yStart, height, luminanceLevels);
		ProcessCornerTile(source.PixelBuffer, cdfTileData, Tiles - 1, 0, xStart, width, 0, num4, luminanceLevels);
		ProcessCornerTile(source.PixelBuffer, cdfTileData, Tiles - 1, Tiles - 1, xStart, width, yStart, height, luminanceLevels);
	}

	private static void ProcessCornerTile(Buffer2D<TPixel> source, CdfTileData cdfData, int cdfX, int cdfY, int xStart, int xEnd, int yStart, int yEnd, int luminanceLevels)
	{
		for (int i = yStart; i < yEnd; i++)
		{
			Span<TPixel> span = source.DangerousGetRowSpan(i);
			for (int j = xStart; j < xEnd; j++)
			{
				ref TPixel reference = ref span[j];
				float num = cdfData.RemapGreyValue(cdfX, cdfY, HistogramEqualizationProcessor<TPixel>.GetLuminance(reference, luminanceLevels));
				reference.FromVector4(new Vector4(num, num, num, reference.ToVector4().W));
			}
		}
	}

	private static void ProcessBorderColumn(Buffer2D<TPixel> source, CdfTileData cdfData, int cdfX, int sourceHeight, int tileCount, int tileHeight, int xStart, int xEnd, int luminanceLevels)
	{
		uint num = (uint)tileHeight / 2u;
		int num2 = 0;
		int num3 = (int)num;
		for (int i = 0; i < tileCount - 1; i++)
		{
			int num4 = Math.Min(num3 + tileHeight, sourceHeight - 1);
			int num5 = 0;
			for (int j = num3; j < num4; j++)
			{
				Span<TPixel> span = source.DangerousGetRowSpan(j);
				for (int k = xStart; k < xEnd; k++)
				{
					ref TPixel reference = ref span[k];
					float num6 = InterpolateBetweenTwoTiles(reference, cdfData, cdfX, num2, cdfX, num2 + 1, num5, tileHeight, luminanceLevels);
					reference.FromVector4(new Vector4(num6, num6, num6, reference.ToVector4().W));
				}
				num5++;
			}
			num2++;
			num3 += tileHeight;
		}
	}

	private static void ProcessBorderRow(Buffer2D<TPixel> source, CdfTileData cdfData, int cdfY, int sourceWidth, int tileCount, int tileWidth, int yStart, int yEnd, int luminanceLevels)
	{
		uint num = (uint)tileWidth / 2u;
		int num2 = 0;
		int num3 = (int)num;
		for (int i = 0; i < tileCount - 1; i++)
		{
			for (int j = yStart; j < yEnd; j++)
			{
				Span<TPixel> span = source.DangerousGetRowSpan(j);
				int num4 = 0;
				int num5 = Math.Min(num3 + tileWidth, sourceWidth - 1);
				for (int k = num3; k < num5; k++)
				{
					ref TPixel reference = ref span[k];
					float num6 = InterpolateBetweenTwoTiles(reference, cdfData, num2, cdfY, num2 + 1, cdfY, num4, tileWidth, luminanceLevels);
					reference.FromVector4(new Vector4(num6, num6, num6, reference.ToVector4().W));
					num4++;
				}
			}
			num2++;
			num3 += tileWidth;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static float InterpolateBetweenFourTiles(TPixel sourcePixel, CdfTileData cdfData, int tileCountX, int tileCountY, int tileX, int tileY, int cdfX, int cdfY, int tileWidth, int tileHeight, int luminanceLevels)
	{
		int luminance = HistogramEqualizationProcessor<TPixel>.GetLuminance(sourcePixel, luminanceLevels);
		float tx = (float)tileX / (float)(tileWidth - 1);
		float ty = (float)tileY / (float)(tileHeight - 1);
		int tilesY = Math.Min(tileCountY - 1, cdfY + 1);
		int tilesX = Math.Min(tileCountX - 1, cdfX + 1);
		float lt = cdfData.RemapGreyValue(cdfX, cdfY, luminance);
		float rt = cdfData.RemapGreyValue(tilesX, cdfY, luminance);
		float lb = cdfData.RemapGreyValue(cdfX, tilesY, luminance);
		float rb = cdfData.RemapGreyValue(tilesX, tilesY, luminance);
		return BilinearInterpolation(tx, ty, lt, rt, lb, rb);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static float InterpolateBetweenTwoTiles(TPixel sourcePixel, CdfTileData cdfData, int tileX1, int tileY1, int tileX2, int tileY2, int tilePos, int tileWidth, int luminanceLevels)
	{
		int luminance = HistogramEqualizationProcessor<TPixel>.GetLuminance(sourcePixel, luminanceLevels);
		float t = (float)tilePos / (float)(tileWidth - 1);
		float left = cdfData.RemapGreyValue(tileX1, tileY1, luminance);
		float right = cdfData.RemapGreyValue(tileX2, tileY2, luminance);
		return LinearInterpolation(left, right, t);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static float BilinearInterpolation(float tx, float ty, float lt, float rt, float lb, float rb)
	{
		return LinearInterpolation(LinearInterpolation(lt, rt, tx), LinearInterpolation(lb, rb, tx), ty);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static float LinearInterpolation(float left, float right, float t)
	{
		return left + (right - left) * t;
	}
}
