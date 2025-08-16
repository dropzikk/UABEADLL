using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Filters;

internal sealed class OpaqueProcessor<TPixel> : ImageProcessor<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly struct OpaqueRowOperation : IRowOperation<Vector4>
	{
		private readonly Configuration configuration;

		private readonly Buffer2D<TPixel> target;

		private readonly Rectangle bounds;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public OpaqueRowOperation(Configuration configuration, Buffer2D<TPixel> target, Rectangle bounds)
		{
			this.configuration = configuration;
			this.target = target;
			this.bounds = bounds;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetRequiredBufferLength(Rectangle bounds)
		{
			return bounds.Width;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Invoke(int y, Span<Vector4> span)
		{
			Span<TPixel> span2 = target.DangerousGetRowSpan(y);
			int x = bounds.X;
			Span<TPixel> destinationPixels = span2.Slice(x, span2.Length - x);
			PixelOperations<TPixel>.Instance.ToVector4(configuration, destinationPixels.Slice(0, span.Length), span, PixelConversionModifiers.Scale);
			ref Vector4 reference = ref MemoryMarshal.GetReference(span);
			for (int i = 0; i < bounds.Width; i++)
			{
				Unsafe.Add(ref reference, (uint)i).W = 1f;
			}
			PixelOperations<TPixel>.Instance.FromVector4Destructive(configuration, span, destinationPixels, PixelConversionModifiers.Scale);
		}
	}

	public OpaqueProcessor(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle)
		: base(configuration, source, sourceRectangle)
	{
	}

	protected override void OnFrameApply(ImageFrame<TPixel> source)
	{
		Rectangle rectangle = Rectangle.Intersect(base.SourceRectangle, source.Bounds());
		OpaqueRowOperation operation = new OpaqueRowOperation(base.Configuration, source.PixelBuffer, rectangle);
		ParallelRowIterator.IterateRows<OpaqueRowOperation, Vector4>(base.Configuration, rectangle, in operation);
	}
}
