using System;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Normalization;

public class GlobalHistogramEqualizationProcessor : HistogramEqualizationProcessor
{
	public GlobalHistogramEqualizationProcessor(int luminanceLevels, bool clipHistogram, int clipLimit)
		: base(luminanceLevels, clipHistogram, clipLimit)
	{
	}

	public override IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle)
	{
		return new GlobalHistogramEqualizationProcessor<TPixel>(configuration, base.LuminanceLevels, base.ClipHistogram, base.ClipLimit, source, sourceRectangle);
	}
}
internal class GlobalHistogramEqualizationProcessor<TPixel> : HistogramEqualizationProcessor<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly struct CdfApplicationRowOperation : IRowOperation<Vector4>
	{
		private readonly Configuration configuration;

		private readonly Rectangle bounds;

		private readonly IMemoryOwner<int> cdfBuffer;

		private readonly Buffer2D<TPixel> source;

		private readonly int luminanceLevels;

		private readonly float numberOfPixelsMinusCdfMin;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CdfApplicationRowOperation(Configuration configuration, Rectangle bounds, IMemoryOwner<int> cdfBuffer, Buffer2D<TPixel> source, int luminanceLevels, float numberOfPixelsMinusCdfMin)
		{
			this.configuration = configuration;
			this.bounds = bounds;
			this.cdfBuffer = cdfBuffer;
			this.source = source;
			this.luminanceLevels = luminanceLevels;
			this.numberOfPixelsMinusCdfMin = numberOfPixelsMinusCdfMin;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetRequiredBufferLength(Rectangle bounds)
		{
			return bounds.Width;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Invoke(int y, Span<Vector4> span)
		{
			Span<Vector4> span2 = span.Slice(0, bounds.Width);
			ref Vector4 reference = ref MemoryMarshal.GetReference(span2);
			ref int reference2 = ref MemoryMarshal.GetReference(cdfBuffer.GetSpan());
			int num = luminanceLevels;
			float num2 = numberOfPixelsMinusCdfMin;
			Span<TPixel> span3 = source.DangerousGetRowSpan(y);
			PixelOperations<TPixel>.Instance.ToVector4(configuration, span3, span2);
			for (int i = 0; i < bounds.Width; i++)
			{
				Vector4 vector = Unsafe.Add(ref reference, (uint)i);
				int bT709Luminance = ColorNumerics.GetBT709Luminance(ref vector, num);
				float num3 = (float)Unsafe.Add(ref reference2, (uint)bT709Luminance) / num2;
				Unsafe.Add(ref reference, (uint)i) = new Vector4(num3, num3, num3, vector.W);
			}
			PixelOperations<TPixel>.Instance.FromVector4Destructive(configuration, span2, span3);
		}
	}

	public GlobalHistogramEqualizationProcessor(Configuration configuration, int luminanceLevels, bool clipHistogram, int clipLimit, Image<TPixel> source, Rectangle sourceRectangle)
		: base(configuration, luminanceLevels, clipHistogram, clipLimit, source, sourceRectangle)
	{
	}

	protected override void OnFrameApply(ImageFrame<TPixel> source)
	{
		MemoryAllocator memoryAllocator = base.Configuration.MemoryAllocator;
		int num = source.Width * source.Height;
		Rectangle rectangle = Rectangle.Intersect(base.SourceRectangle, source.Bounds());
		using IMemoryOwner<int> memoryOwner = memoryAllocator.Allocate<int>(base.LuminanceLevels, AllocationOptions.Clean);
		GrayscaleLevelsRowOperation<TPixel> operation = new GrayscaleLevelsRowOperation<TPixel>(base.Configuration, rectangle, memoryOwner, source.PixelBuffer, base.LuminanceLevels);
		ParallelRowIterator.IterateRows<GrayscaleLevelsRowOperation<TPixel>, Vector4>(base.Configuration, rectangle, in operation);
		Span<int> span = memoryOwner.GetSpan();
		if (base.ClipHistogramEnabled)
		{
			ClipHistogram(span, base.ClipLimit);
		}
		using IMemoryOwner<int> memoryOwner2 = memoryAllocator.Allocate<int>(base.LuminanceLevels, AllocationOptions.Clean);
		int num2 = HistogramEqualizationProcessor<TPixel>.CalculateCdf(ref MemoryMarshal.GetReference(memoryOwner2.GetSpan()), ref MemoryMarshal.GetReference(span), span.Length - 1);
		float numberOfPixelsMinusCdfMin = num - num2;
		CdfApplicationRowOperation operation2 = new CdfApplicationRowOperation(base.Configuration, rectangle, memoryOwner2, source.PixelBuffer, base.LuminanceLevels, numberOfPixelsMinusCdfMin);
		ParallelRowIterator.IterateRows<CdfApplicationRowOperation, Vector4>(base.Configuration, rectangle, in operation2);
	}
}
