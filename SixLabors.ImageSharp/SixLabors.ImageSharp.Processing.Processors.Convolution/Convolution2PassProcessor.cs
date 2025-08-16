using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Convolution;

internal class Convolution2PassProcessor<TPixel> : ImageProcessor<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	internal readonly struct HorizontalConvolutionRowOperation : IRowOperation<Vector4>
	{
		private readonly Rectangle bounds;

		private readonly Buffer2D<TPixel> targetPixels;

		private readonly Buffer2D<TPixel> sourcePixels;

		private readonly KernelSamplingMap map;

		private readonly float[] kernel;

		private readonly Configuration configuration;

		private readonly bool preserveAlpha;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public HorizontalConvolutionRowOperation(Rectangle bounds, Buffer2D<TPixel> targetPixels, Buffer2D<TPixel> sourcePixels, KernelSamplingMap map, float[] kernel, Configuration configuration, bool preserveAlpha)
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
			if (preserveAlpha)
			{
				Convolve3(y, span);
			}
			else
			{
				Convolve4(y, span);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Convolve3(int y, Span<Vector4> span)
		{
			int x = bounds.X;
			int width = bounds.Width;
			int num = kernel.Length;
			Span<Vector4> span2 = span.Slice(0, bounds.Width);
			int width2 = bounds.Width;
			Span<Vector4> span3 = span.Slice(width2, span.Length - width2);
			span3.Clear();
			Span<TPixel> span4 = sourcePixels.DangerousGetRowSpan(y);
			Span<TPixel> span5 = span4.Slice(x, width);
			PixelOperations<TPixel>.Instance.ToVector4(configuration, span5, span2);
			ref Vector4 reference = ref MemoryMarshal.GetReference(span2);
			ref Vector4 reference2 = ref MemoryMarshal.GetReference(span3);
			ref Vector4 right = ref Unsafe.Add(ref reference2, (uint)span2.Length);
			ref float arrayDataReference = ref MemoryMarshal.GetArrayDataReference(kernel);
			ref float right2 = ref Unsafe.Add(ref arrayDataReference, (uint)num);
			ref int reference3 = ref MemoryMarshal.GetReference(map.GetColumnOffsetSpan());
			while (Unsafe.IsAddressLessThan(ref reference2, ref right))
			{
				ref float reference4 = ref arrayDataReference;
				ref int reference5 = ref reference3;
				while (Unsafe.IsAddressLessThan(ref reference4, ref right2))
				{
					Vector4 vector = Unsafe.Add(ref reference, (uint)(reference5 - x));
					reference2 += reference4 * vector;
					reference4 = ref Unsafe.Add(ref reference4, 1);
					reference5 = ref Unsafe.Add(ref reference5, 1);
				}
				reference2 = ref Unsafe.Add(ref reference2, 1);
				reference3 = ref Unsafe.Add(ref reference3, (uint)num);
			}
			span4 = sourcePixels.DangerousGetRowSpan(y);
			span5 = span4.Slice(x, width);
			PixelOperations<TPixel>.Instance.ToVector4(configuration, span5, span2);
			reference2 = ref MemoryMarshal.GetReference(span3);
			while (Unsafe.IsAddressLessThan(ref reference2, ref right))
			{
				reference2.W = reference.W;
				reference2 = ref Unsafe.Add(ref reference2, 1);
				reference = ref Unsafe.Add(ref reference, 1);
			}
			span4 = targetPixels.DangerousGetRowSpan(y);
			Span<TPixel> destinationPixels = span4.Slice(x, width);
			PixelOperations<TPixel>.Instance.FromVector4Destructive(configuration, span3, destinationPixels);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Convolve4(int y, Span<Vector4> span)
		{
			int x = bounds.X;
			int width = bounds.Width;
			int num = kernel.Length;
			Span<Vector4> span2 = span.Slice(0, bounds.Width);
			int width2 = bounds.Width;
			Span<Vector4> span3 = span.Slice(width2, span.Length - width2);
			span3.Clear();
			Span<TPixel> span4 = sourcePixels.DangerousGetRowSpan(y);
			Span<TPixel> span5 = span4.Slice(x, width);
			PixelOperations<TPixel>.Instance.ToVector4(configuration, span5, span2);
			Numerics.Premultiply(span2);
			ref Vector4 reference = ref MemoryMarshal.GetReference(span2);
			ref Vector4 reference2 = ref MemoryMarshal.GetReference(span3);
			ref Vector4 right = ref Unsafe.Add(ref reference2, (uint)span2.Length);
			ref float arrayDataReference = ref MemoryMarshal.GetArrayDataReference(kernel);
			ref float right2 = ref Unsafe.Add(ref arrayDataReference, (uint)num);
			ref int reference3 = ref MemoryMarshal.GetReference(map.GetColumnOffsetSpan());
			while (Unsafe.IsAddressLessThan(ref reference2, ref right))
			{
				ref float reference4 = ref arrayDataReference;
				ref int reference5 = ref reference3;
				while (Unsafe.IsAddressLessThan(ref reference4, ref right2))
				{
					Vector4 vector = Unsafe.Add(ref reference, (uint)(reference5 - x));
					reference2 += reference4 * vector;
					reference4 = ref Unsafe.Add(ref reference4, 1);
					reference5 = ref Unsafe.Add(ref reference5, 1);
				}
				reference2 = ref Unsafe.Add(ref reference2, 1);
				reference3 = ref Unsafe.Add(ref reference3, (uint)num);
			}
			Numerics.UnPremultiply(span3);
			span4 = targetPixels.DangerousGetRowSpan(y);
			Span<TPixel> destinationPixels = span4.Slice(x, width);
			PixelOperations<TPixel>.Instance.FromVector4Destructive(configuration, span3, destinationPixels);
		}
	}

	internal readonly struct VerticalConvolutionRowOperation : IRowOperation<Vector4>
	{
		private readonly Rectangle bounds;

		private readonly Buffer2D<TPixel> targetPixels;

		private readonly Buffer2D<TPixel> sourcePixels;

		private readonly KernelSamplingMap map;

		private readonly float[] kernel;

		private readonly Configuration configuration;

		private readonly bool preserveAlpha;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public VerticalConvolutionRowOperation(Rectangle bounds, Buffer2D<TPixel> targetPixels, Buffer2D<TPixel> sourcePixels, KernelSamplingMap map, float[] kernel, Configuration configuration, bool preserveAlpha)
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
			if (preserveAlpha)
			{
				Convolve3(y, span);
			}
			else
			{
				Convolve4(y, span);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Convolve3(int y, Span<Vector4> span)
		{
			int x = bounds.X;
			int width = bounds.Width;
			int num = kernel.Length;
			Span<Vector4> span2 = span.Slice(0, bounds.Width);
			int width2 = bounds.Width;
			Span<Vector4> span3 = span.Slice(width2, span.Length - width2);
			ref int reference = ref Unsafe.Add(ref MemoryMarshal.GetReference(map.GetRowOffsetSpan()), (uint)((y - bounds.Y) * num));
			span3.Clear();
			ref Vector4 reference2 = ref MemoryMarshal.GetReference(span3);
			ref float reference3 = ref MemoryMarshal.GetArrayDataReference(kernel);
			ref float right = ref Unsafe.Add(ref reference3, (uint)num);
			Span<TPixel> span4;
			Span<TPixel> span5;
			while (Unsafe.IsAddressLessThan(ref reference3, ref right))
			{
				span4 = sourcePixels.DangerousGetRowSpan(reference);
				span5 = span4.Slice(x, width);
				PixelOperations<TPixel>.Instance.ToVector4(configuration, span5, span2);
				ref Vector4 reference4 = ref MemoryMarshal.GetReference(span2);
				ref Vector4 right2 = ref Unsafe.Add(ref reference4, (uint)span2.Length);
				ref Vector4 reference5 = ref reference2;
				float num2 = reference3;
				while (Unsafe.IsAddressLessThan(ref reference4, ref right2))
				{
					reference5 += num2 * reference4;
					reference4 = ref Unsafe.Add(ref reference4, 1);
					reference5 = ref Unsafe.Add(ref reference5, 1);
				}
				reference3 = ref Unsafe.Add(ref reference3, 1);
				reference = ref Unsafe.Add(ref reference, 1);
			}
			span4 = sourcePixels.DangerousGetRowSpan(y);
			span5 = span4.Slice(x, width);
			PixelOperations<TPixel>.Instance.ToVector4(configuration, span5, span2);
			ref Vector4 reference6 = ref MemoryMarshal.GetReference(span2);
			ref Vector4 right3 = ref Unsafe.Add(ref reference6, (uint)span2.Length);
			while (Unsafe.IsAddressLessThan(ref reference6, ref right3))
			{
				reference2.W = reference6.W;
				reference2 = ref Unsafe.Add(ref reference2, 1);
				reference6 = ref Unsafe.Add(ref reference6, 1);
			}
			span4 = targetPixels.DangerousGetRowSpan(y);
			Span<TPixel> destinationPixels = span4.Slice(x, width);
			PixelOperations<TPixel>.Instance.FromVector4Destructive(configuration, span3, destinationPixels);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Convolve4(int y, Span<Vector4> span)
		{
			int x = bounds.X;
			int width = bounds.Width;
			int num = kernel.Length;
			Span<Vector4> span2 = span.Slice(0, bounds.Width);
			int width2 = bounds.Width;
			Span<Vector4> span3 = span.Slice(width2, span.Length - width2);
			ref int reference = ref Unsafe.Add(ref MemoryMarshal.GetReference(map.GetRowOffsetSpan()), (uint)((y - bounds.Y) * num));
			span3.Clear();
			ref Vector4 reference2 = ref MemoryMarshal.GetReference(span3);
			ref float reference3 = ref MemoryMarshal.GetArrayDataReference(kernel);
			ref float right = ref Unsafe.Add(ref reference3, (uint)num);
			Span<TPixel> span4;
			while (Unsafe.IsAddressLessThan(ref reference3, ref right))
			{
				span4 = sourcePixels.DangerousGetRowSpan(reference);
				Span<TPixel> span5 = span4.Slice(x, width);
				PixelOperations<TPixel>.Instance.ToVector4(configuration, span5, span2);
				Numerics.Premultiply(span2);
				ref Vector4 reference4 = ref MemoryMarshal.GetReference(span2);
				ref Vector4 right2 = ref Unsafe.Add(ref reference4, (uint)span2.Length);
				ref Vector4 reference5 = ref reference2;
				float num2 = reference3;
				while (Unsafe.IsAddressLessThan(ref reference4, ref right2))
				{
					reference5 += num2 * reference4;
					reference4 = ref Unsafe.Add(ref reference4, 1);
					reference5 = ref Unsafe.Add(ref reference5, 1);
				}
				reference3 = ref Unsafe.Add(ref reference3, 1);
				reference = ref Unsafe.Add(ref reference, 1);
			}
			Numerics.UnPremultiply(span3);
			span4 = targetPixels.DangerousGetRowSpan(y);
			Span<TPixel> destinationPixels = span4.Slice(x, width);
			PixelOperations<TPixel>.Instance.FromVector4Destructive(configuration, span3, destinationPixels);
		}
	}

	public float[] Kernel { get; }

	public bool PreserveAlpha { get; }

	public BorderWrappingMode BorderWrapModeX { get; }

	public BorderWrappingMode BorderWrapModeY { get; }

	public Convolution2PassProcessor(Configuration configuration, float[] kernel, bool preserveAlpha, Image<TPixel> source, Rectangle sourceRectangle, BorderWrappingMode borderWrapModeX, BorderWrappingMode borderWrapModeY)
		: base(configuration, source, sourceRectangle)
	{
		Kernel = kernel;
		PreserveAlpha = preserveAlpha;
		BorderWrapModeX = borderWrapModeX;
		BorderWrapModeY = borderWrapModeY;
	}

	protected override void OnFrameApply(ImageFrame<TPixel> source)
	{
		using Buffer2D<TPixel> buffer2D = base.Configuration.MemoryAllocator.Allocate2D<TPixel>(source.Size());
		Rectangle rectangle = Rectangle.Intersect(base.SourceRectangle, source.Bounds());
		using KernelSamplingMap kernelSamplingMap = new KernelSamplingMap(base.Configuration.MemoryAllocator);
		kernelSamplingMap.BuildSamplingOffsetMap(Kernel.Length, Kernel.Length, rectangle, BorderWrapModeX, BorderWrapModeY);
		HorizontalConvolutionRowOperation operation = new HorizontalConvolutionRowOperation(rectangle, buffer2D, source.PixelBuffer, kernelSamplingMap, Kernel, base.Configuration, PreserveAlpha);
		ParallelRowIterator.IterateRows<HorizontalConvolutionRowOperation, Vector4>(base.Configuration, rectangle, in operation);
		VerticalConvolutionRowOperation operation2 = new VerticalConvolutionRowOperation(rectangle, source.PixelBuffer, buffer2D, kernelSamplingMap, Kernel, base.Configuration, PreserveAlpha);
		ParallelRowIterator.IterateRows<VerticalConvolutionRowOperation, Vector4>(base.Configuration, rectangle, in operation2);
	}
}
