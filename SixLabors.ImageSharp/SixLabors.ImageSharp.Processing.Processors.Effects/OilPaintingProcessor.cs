using System;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Effects;

public sealed class OilPaintingProcessor : IImageProcessor
{
	public int Levels { get; }

	public int BrushSize { get; }

	public OilPaintingProcessor(int levels, int brushSize)
	{
		Guard.MustBeGreaterThan(levels, 0, "levels");
		Guard.MustBeGreaterThan(brushSize, 0, "brushSize");
		Levels = levels;
		BrushSize = brushSize;
	}

	public IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle) where TPixel : unmanaged, IPixel<TPixel>
	{
		return new OilPaintingProcessor<TPixel>(configuration, this, source, sourceRectangle);
	}
}
internal class OilPaintingProcessor<TPixel> : ImageProcessor<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly struct RowIntervalOperation : IRowIntervalOperation
	{
		private readonly Rectangle bounds;

		private readonly Buffer2D<TPixel> targetPixels;

		private readonly Buffer2D<TPixel> source;

		private readonly Configuration configuration;

		private readonly int radius;

		private readonly int levels;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RowIntervalOperation(Rectangle bounds, Buffer2D<TPixel> targetPixels, Buffer2D<TPixel> source, Configuration configuration, int radius, int levels)
		{
			this.bounds = bounds;
			this.targetPixels = targetPixels;
			this.source = source;
			this.configuration = configuration;
			this.radius = radius;
			this.levels = levels;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Invoke(in RowInterval rows)
		{
			int max = bounds.Bottom - 1;
			int max2 = bounds.Right - 1;
			using IMemoryOwner<Vector4> memoryOwner = configuration.MemoryAllocator.Allocate<Vector4>(source.Width);
			using IMemoryOwner<Vector4> memoryOwner2 = configuration.MemoryAllocator.Allocate<Vector4>(source.Width);
			using IMemoryOwner<float> memoryOwner3 = configuration.MemoryAllocator.Allocate<float>(levels * 4);
			Memory<Vector4> memory = memoryOwner.Memory;
			Span<Vector4> span = memory.Span;
			Span<Vector4> destinationVectors = span.Slice(bounds.X, bounds.Width);
			memory = memoryOwner2.Memory;
			Span<Vector4> span2 = memory.Span;
			Span<Vector4> sourceVectors = span2.Slice(bounds.X, bounds.Width);
			Span<float> span3 = memoryOwner3.GetSpan();
			Span<int> span4 = MemoryMarshal.Cast<float, int>(span3);
			ref Span<float> reference = ref span3;
			int num = levels;
			Span<float> span5 = reference.Slice(num, reference.Length - num);
			reference = ref span5;
			num = levels;
			Span<float> span6 = reference.Slice(num, reference.Length - num);
			reference = ref span6;
			num = levels;
			Span<float> span7 = reference.Slice(num, reference.Length - num);
			for (int i = rows.Min; i < rows.Max; i++)
			{
				Span<TPixel> span8 = source.DangerousGetRowSpan(i).Slice(bounds.X, bounds.Width);
				PixelOperations<TPixel>.Instance.ToVector4(configuration, span8, destinationVectors, PixelConversionModifiers.Scale);
				for (int j = bounds.X; j < bounds.Right; j++)
				{
					int num2 = 0;
					int index = 0;
					memoryOwner3.Memory.Span.Clear();
					for (int k = 0; k <= radius; k++)
					{
						int num3 = k - radius;
						int value = i + num3;
						value = Numerics.Clamp(value, 0, max);
						Span<TPixel> span9 = source.DangerousGetRowSpan(value);
						for (int l = 0; l <= radius; l++)
						{
							int num4 = l - radius;
							int value2 = j + num4;
							value2 = Numerics.Clamp(value2, 0, max2);
							Vector4 vector = span9[value2].ToScaledVector4();
							float x = vector.X;
							float z = vector.Z;
							float y = vector.Y;
							int num5 = (int)MathF.Round((z + y + x) / 3f * (float)(levels - 1));
							span4[num5]++;
							span5[num5] += x;
							span6[num5] += z;
							span7[num5] += y;
							if (span4[num5] > num2)
							{
								num2 = span4[num5];
								index = num5;
							}
						}
						float x2 = span5[index] / (float)num2;
						float z2 = span6[index] / (float)num2;
						float y2 = span7[index] / (float)num2;
						float w = span[j].W;
						span2[j] = new Vector4(x2, y2, z2, w);
					}
				}
				Span<TPixel> destinationPixels = targetPixels.DangerousGetRowSpan(i).Slice(bounds.X, bounds.Width);
				PixelOperations<TPixel>.Instance.FromVector4Destructive(configuration, sourceVectors, destinationPixels, PixelConversionModifiers.Scale);
			}
		}

		void IRowIntervalOperation.Invoke(in RowInterval rows)
		{
			Invoke(in rows);
		}
	}

	private readonly OilPaintingProcessor definition;

	public OilPaintingProcessor(Configuration configuration, OilPaintingProcessor definition, Image<TPixel> source, Rectangle sourceRectangle)
		: base(configuration, source, sourceRectangle)
	{
		this.definition = definition;
	}

	protected override void OnFrameApply(ImageFrame<TPixel> source)
	{
		int levels = Math.Clamp(definition.Levels, 1, 255);
		int num = Math.Clamp(definition.BrushSize, 1, Math.Min(source.Width, source.Height));
		using Buffer2D<TPixel> buffer2D = base.Configuration.MemoryAllocator.Allocate2D<TPixel>(source.Size());
		source.CopyTo(buffer2D);
		RowIntervalOperation operation = new RowIntervalOperation(base.SourceRectangle, buffer2D, source.PixelBuffer, base.Configuration, num >> 1, levels);
		try
		{
			ParallelRowIterator.IterateRowIntervals(base.Configuration, base.SourceRectangle, in operation);
		}
		catch (Exception innerException)
		{
			throw new ImageProcessingException("The OilPaintProcessor failed. The most likely reason is that a pixel component was outside of its' allowed range.", innerException);
		}
		Buffer2D<TPixel>.SwapOrCopyContent(source.PixelBuffer, buffer2D);
	}
}
