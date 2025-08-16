using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Convolution;

internal readonly struct Convolution2DRowOperation<TPixel> : IRowOperation<Vector4> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly Rectangle bounds;

	private readonly Buffer2D<TPixel> targetPixels;

	private readonly Buffer2D<TPixel> sourcePixels;

	private readonly KernelSamplingMap map;

	private readonly DenseMatrix<float> kernelMatrixY;

	private readonly DenseMatrix<float> kernelMatrixX;

	private readonly Configuration configuration;

	private readonly bool preserveAlpha;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Convolution2DRowOperation(Rectangle bounds, Buffer2D<TPixel> targetPixels, Buffer2D<TPixel> sourcePixels, KernelSamplingMap map, DenseMatrix<float> kernelMatrixY, DenseMatrix<float> kernelMatrixX, Configuration configuration, bool preserveAlpha)
	{
		this.bounds = bounds;
		this.targetPixels = targetPixels;
		this.sourcePixels = sourcePixels;
		this.map = map;
		this.kernelMatrixY = kernelMatrixY;
		this.kernelMatrixX = kernelMatrixX;
		this.configuration = configuration;
		this.preserveAlpha = preserveAlpha;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetRequiredBufferLength(Rectangle bounds)
	{
		return 3 * bounds.Width;
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
		Span<Vector4> span2 = span.Slice(0, width);
		Span<Vector4> span3 = span.Slice(width, width);
		Span<Vector4> span4 = span.Slice(width * 2, width);
		Convolution2DState convolution2DState = new Convolution2DState(in kernelMatrixY, in kernelMatrixX, map);
		ref int sampleRow = ref convolution2DState.GetSampleRow((uint)(y - bounds.Y));
		span3.Clear();
		span4.Clear();
		ref Vector4 reference = ref MemoryMarshal.GetReference(span3);
		ref Vector4 reference2 = ref MemoryMarshal.GetReference(span4);
		ReadOnlyKernel kernelY = convolution2DState.KernelY;
		ReadOnlyKernel kernelX = convolution2DState.KernelX;
		Span<TPixel> span5;
		Span<TPixel> span6;
		for (uint num = 0u; num < kernelY.Rows; num++)
		{
			int y2 = Unsafe.Add(ref sampleRow, num);
			span5 = sourcePixels.DangerousGetRowSpan(y2);
			span6 = span5.Slice(x, width);
			PixelOperations<TPixel>.Instance.ToVector4(configuration, span6, span2);
			ref Vector4 reference3 = ref MemoryMarshal.GetReference(span2);
			for (uint num2 = 0u; num2 < (uint)span2.Length; num2++)
			{
				ref int sampleColumn = ref convolution2DState.GetSampleColumn(num2);
				ref Vector4 reference4 = ref Unsafe.Add(ref reference, num2);
				ref Vector4 reference5 = ref Unsafe.Add(ref reference2, num2);
				for (uint num3 = 0u; num3 < kernelY.Columns; num3++)
				{
					int num4 = Unsafe.Add(ref sampleColumn, num3) - x;
					Vector4 vector = Unsafe.Add(ref reference3, (uint)num4);
					reference4 += kernelX[num, num3] * vector;
					reference5 += kernelY[num, num3] * vector;
				}
			}
		}
		span5 = sourcePixels.DangerousGetRowSpan(y);
		span6 = span5.Slice(x, width);
		PixelOperations<TPixel>.Instance.ToVector4(configuration, span6, span2);
		for (nuint num5 = 0u; num5 < (uint)span6.Length; num5++)
		{
			ref Vector4 reference6 = ref Unsafe.Add(ref reference, num5);
			Vector4 vector2 = reference6;
			Vector4 vector3 = Unsafe.Add(ref reference2, num5);
			reference6 = Vector4.SquareRoot(vector3 * vector3 + vector2 * vector2);
			reference6.W = Unsafe.Add(ref MemoryMarshal.GetReference(span2), num5).W;
		}
		span5 = targetPixels.DangerousGetRowSpan(y);
		Span<TPixel> destinationPixels = span5.Slice(x, width);
		PixelOperations<TPixel>.Instance.FromVector4Destructive(configuration, span3, destinationPixels);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void Convolve4(int y, Span<Vector4> span)
	{
		int x = bounds.X;
		int width = bounds.Width;
		Span<Vector4> span2 = span.Slice(0, width);
		Span<Vector4> span3 = span.Slice(width, width);
		Span<Vector4> span4 = span.Slice(width * 2, width);
		Convolution2DState convolution2DState = new Convolution2DState(in kernelMatrixY, in kernelMatrixX, map);
		ref int sampleRow = ref convolution2DState.GetSampleRow((uint)(y - bounds.Y));
		span3.Clear();
		span4.Clear();
		ref Vector4 reference = ref MemoryMarshal.GetReference(span3);
		ref Vector4 reference2 = ref MemoryMarshal.GetReference(span4);
		ReadOnlyKernel kernelY = convolution2DState.KernelY;
		ReadOnlyKernel kernelX = convolution2DState.KernelX;
		Span<TPixel> span5;
		for (uint num = 0u; num < kernelY.Rows; num++)
		{
			int y2 = Unsafe.Add(ref sampleRow, num);
			span5 = sourcePixels.DangerousGetRowSpan(y2);
			Span<TPixel> span6 = span5.Slice(x, width);
			PixelOperations<TPixel>.Instance.ToVector4(configuration, span6, span2);
			Numerics.Premultiply(span2);
			ref Vector4 reference3 = ref MemoryMarshal.GetReference(span2);
			for (uint num2 = 0u; num2 < (uint)span2.Length; num2++)
			{
				ref int sampleColumn = ref convolution2DState.GetSampleColumn(num2);
				ref Vector4 reference4 = ref Unsafe.Add(ref reference, num2);
				ref Vector4 reference5 = ref Unsafe.Add(ref reference2, num2);
				for (uint num3 = 0u; num3 < kernelY.Columns; num3++)
				{
					int elementOffset = Unsafe.Add(ref sampleColumn, num3) - x;
					Vector4 vector = Unsafe.Add(ref reference3, elementOffset);
					reference4 += kernelX[num, num3] * vector;
					reference5 += kernelY[num, num3] * vector;
				}
			}
		}
		for (nuint num4 = 0u; num4 < (uint)span3.Length; num4++)
		{
			ref Vector4 reference6 = ref Unsafe.Add(ref reference, num4);
			Vector4 vector2 = reference6;
			Vector4 vector3 = Unsafe.Add(ref reference2, num4);
			reference6 = Vector4.SquareRoot(vector3 * vector3 + vector2 * vector2);
		}
		Numerics.UnPremultiply(span3);
		span5 = targetPixels.DangerousGetRowSpan(y);
		Span<TPixel> destinationPixels = span5.Slice(x, width);
		PixelOperations<TPixel>.Instance.FromVector4Destructive(configuration, span3, destinationPixels);
	}
}
