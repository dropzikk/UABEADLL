using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Convolution;

internal class ConvolutionProcessor<TPixel> : ImageProcessor<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly struct RowOperation : IRowOperation<Vector4>
	{
		private readonly Rectangle bounds;

		private readonly Buffer2D<TPixel> targetPixels;

		private readonly Buffer2D<TPixel> sourcePixels;

		private readonly KernelSamplingMap map;

		private readonly DenseMatrix<float> kernel;

		private readonly Configuration configuration;

		private readonly bool preserveAlpha;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RowOperation(Rectangle bounds, Buffer2D<TPixel> targetPixels, Buffer2D<TPixel> sourcePixels, KernelSamplingMap map, DenseMatrix<float> kernel, Configuration configuration, bool preserveAlpha)
		{
			this.bounds = bounds;
			this.targetPixels = targetPixels;
			this.sourcePixels = sourcePixels;
			this.map = map;
			this.kernel = kernel;
			this.configuration = configuration;
			this.preserveAlpha = preserveAlpha;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetRequiredBufferLength(Rectangle bounds)
		{
			return 2 * bounds.Width;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Invoke(int y, Span<Vector4> span)
		{
			int x = bounds.X;
			int width = bounds.Width;
			Span<Vector4> span2 = span.Slice(0, bounds.Width);
			int width2 = bounds.Width;
			Span<Vector4> span3 = span.Slice(width2, span.Length - width2);
			MemoryMarshal.GetReference(span);
			Span<TPixel> span4 = targetPixels.DangerousGetRowSpan(y);
			Span<TPixel> destinationPixels = span4.Slice(x, width);
			ConvolutionState convolutionState = new ConvolutionState(in kernel, map);
			int row = y - bounds.Y;
			ref int sampleRow = ref convolutionState.GetSampleRow((uint)row);
			if (preserveAlpha)
			{
				span3.Clear();
				ref Vector4 reference = ref MemoryMarshal.GetReference(span3);
				Span<TPixel> span5;
				for (uint num = 0u; num < convolutionState.Kernel.Rows; num++)
				{
					int y2 = Unsafe.Add(ref sampleRow, num);
					span4 = sourcePixels.DangerousGetRowSpan(y2);
					span5 = span4.Slice(x, width);
					PixelOperations<TPixel>.Instance.ToVector4(configuration, span5, span2);
					ref Vector4 reference2 = ref MemoryMarshal.GetReference(span2);
					for (uint num2 = 0u; num2 < (uint)span2.Length; num2++)
					{
						ref int sampleColumn = ref convolutionState.GetSampleColumn(num2);
						ref Vector4 reference3 = ref Unsafe.Add(ref reference, num2);
						for (uint num3 = 0u; num3 < convolutionState.Kernel.Columns; num3++)
						{
							int num4 = Unsafe.Add(ref sampleColumn, num3) - x;
							Vector4 vector = Unsafe.Add(ref reference2, (uint)num4);
							reference3 += convolutionState.Kernel[num, num3] * vector;
						}
					}
				}
				span4 = sourcePixels.DangerousGetRowSpan(y);
				span5 = span4.Slice(x, width);
				PixelOperations<TPixel>.Instance.ToVector4(configuration, span5, span2);
				for (nuint num5 = 0u; num5 < (uint)span5.Length; num5++)
				{
					Unsafe.Add(ref reference, num5).W = Unsafe.Add(ref MemoryMarshal.GetReference(span2), num5).W;
				}
			}
			else
			{
				span3.Clear();
				ref Vector4 reference4 = ref MemoryMarshal.GetReference(span3);
				for (uint num6 = 0u; num6 < convolutionState.Kernel.Rows; num6++)
				{
					int y3 = Unsafe.Add(ref sampleRow, num6);
					span4 = sourcePixels.DangerousGetRowSpan(y3);
					Span<TPixel> span6 = span4.Slice(x, width);
					PixelOperations<TPixel>.Instance.ToVector4(configuration, span6, span2);
					Numerics.Premultiply(span2);
					ref Vector4 reference5 = ref MemoryMarshal.GetReference(span2);
					for (uint num7 = 0u; num7 < (uint)span2.Length; num7++)
					{
						ref int sampleColumn2 = ref convolutionState.GetSampleColumn(num7);
						ref Vector4 reference6 = ref Unsafe.Add(ref reference4, num7);
						for (uint num8 = 0u; num8 < convolutionState.Kernel.Columns; num8++)
						{
							int num9 = Unsafe.Add(ref sampleColumn2, num8) - x;
							Vector4 vector2 = Unsafe.Add(ref reference5, (uint)num9);
							reference6 += convolutionState.Kernel[num6, num8] * vector2;
						}
					}
				}
				Numerics.UnPremultiply(span3);
			}
			PixelOperations<TPixel>.Instance.FromVector4Destructive(configuration, span3, destinationPixels);
		}
	}

	public DenseMatrix<float> KernelXY { get; }

	public bool PreserveAlpha { get; }

	public ConvolutionProcessor(Configuration configuration, in DenseMatrix<float> kernelXY, bool preserveAlpha, Image<TPixel> source, Rectangle sourceRectangle)
		: base(configuration, source, sourceRectangle)
	{
		KernelXY = kernelXY;
		PreserveAlpha = preserveAlpha;
	}

	protected override void OnFrameApply(ImageFrame<TPixel> source)
	{
		MemoryAllocator memoryAllocator = base.Configuration.MemoryAllocator;
		using Buffer2D<TPixel> buffer2D = memoryAllocator.Allocate2D<TPixel>(source.Size());
		source.CopyTo(buffer2D);
		Rectangle rectangle = Rectangle.Intersect(base.SourceRectangle, source.Bounds());
		using (KernelSamplingMap kernelSamplingMap = new KernelSamplingMap(memoryAllocator))
		{
			kernelSamplingMap.BuildSamplingOffsetMap(KernelXY, rectangle);
			RowOperation operation = new RowOperation(rectangle, buffer2D, source.PixelBuffer, kernelSamplingMap, KernelXY, base.Configuration, PreserveAlpha);
			ParallelRowIterator.IterateRows<RowOperation, Vector4>(base.Configuration, rectangle, in operation);
		}
		Buffer2D<TPixel>.SwapOrCopyContent(source.PixelBuffer, buffer2D);
	}
}
