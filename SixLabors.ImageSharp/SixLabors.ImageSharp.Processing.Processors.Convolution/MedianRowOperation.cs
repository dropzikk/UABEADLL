using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Convolution;

internal readonly struct MedianRowOperation<TPixel> : IRowOperation<Vector4> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly int yChannelStart;

	private readonly int zChannelStart;

	private readonly int wChannelStart;

	private readonly Configuration configuration;

	private readonly Rectangle bounds;

	private readonly Buffer2D<TPixel> targetPixels;

	private readonly Buffer2D<TPixel> sourcePixels;

	private readonly KernelSamplingMap map;

	private readonly int kernelSize;

	private readonly bool preserveAlpha;

	public MedianRowOperation(Rectangle bounds, Buffer2D<TPixel> targetPixels, Buffer2D<TPixel> sourcePixels, KernelSamplingMap map, int kernelSize, Configuration configuration, bool preserveAlpha)
	{
		this.bounds = bounds;
		this.configuration = configuration;
		this.targetPixels = targetPixels;
		this.sourcePixels = sourcePixels;
		this.map = map;
		this.kernelSize = kernelSize;
		this.preserveAlpha = preserveAlpha;
		int num = (yChannelStart = this.kernelSize * this.kernelSize);
		zChannelStart = yChannelStart + num;
		wChannelStart = zChannelStart + num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetRequiredBufferLength(Rectangle bounds)
	{
		return 2 * kernelSize * kernelSize + bounds.Width + kernelSize * bounds.Width;
	}

	public void Invoke(int y, Span<Vector4> span)
	{
		int x = bounds.X;
		int width = bounds.Width;
		int num = kernelSize * kernelSize;
		Span<Vector4> data = span.Slice(0, num);
		Span<Vector4> span2 = span.Slice(num, num);
		Span<Vector4> span3 = span.Slice(num << 1, kernelSize * width);
		Span<Vector4> span4 = span.Slice((num << 1) + span3.Length, width);
		Span<float> span5 = MemoryMarshal.Cast<Vector4, float>(span2);
		Span<float> xChannel = span5.Slice(0, num);
		Span<float> yChannel = span5.Slice(yChannelStart, num);
		Span<float> zChannel = span5.Slice(zChannelStart, num);
		DenseMatrix<Vector4> kernel = new DenseMatrix<Vector4>(kernelSize, kernelSize, data);
		int row = y - bounds.Y;
		MedianConvolutionState medianConvolutionState = new MedianConvolutionState(in kernel, map);
		ref int sampleRow = ref medianConvolutionState.GetSampleRow(row);
		ref Vector4 reference = ref MemoryMarshal.GetReference(span4);
		Span<TPixel> span6;
		for (int i = 0; i < kernelSize; i++)
		{
			int y2 = Unsafe.Add(ref sampleRow, (uint)i);
			span6 = sourcePixels.DangerousGetRowSpan(y2);
			Span<TPixel> span7 = span6.Slice(x, width);
			Span<Vector4> destinationVectors = span3.Slice(i * width, width);
			PixelOperations<TPixel>.Instance.ToVector4(configuration, span7, destinationVectors);
		}
		if (preserveAlpha)
		{
			for (int j = 0; j < width; j++)
			{
				int num2 = 0;
				ref int sampleColumn = ref medianConvolutionState.GetSampleColumn(j);
				ref Vector4 reference2 = ref Unsafe.Add(ref reference, (uint)j);
				for (int k = 0; k < medianConvolutionState.Kernel.Rows; k++)
				{
					ref Span<Vector4> reference3 = ref span3;
					int num3 = k * width;
					ref Vector4 reference4 = ref MemoryMarshal.GetReference(reference3.Slice(num3, reference3.Length - num3));
					for (int l = 0; l < medianConvolutionState.Kernel.Columns; l++)
					{
						int num4 = Unsafe.Add(ref sampleColumn, (uint)l) - x;
						Vector4 value = Unsafe.Add(ref reference4, (uint)num4);
						medianConvolutionState.Kernel.SetValue(num2, value);
						num2++;
					}
				}
				reference2 = FindMedian3(medianConvolutionState.Kernel.Span, xChannel, yChannel, zChannel);
			}
		}
		else
		{
			Span<float> wChannel = span5.Slice(wChannelStart, num);
			for (int m = 0; m < width; m++)
			{
				int num5 = 0;
				ref int sampleColumn2 = ref medianConvolutionState.GetSampleColumn(m);
				ref Vector4 reference5 = ref Unsafe.Add(ref reference, (uint)m);
				for (int n = 0; n < medianConvolutionState.Kernel.Rows; n++)
				{
					ref Span<Vector4> reference3 = ref span3;
					int num3 = n * width;
					ref Vector4 reference6 = ref MemoryMarshal.GetReference(reference3.Slice(num3, reference3.Length - num3));
					for (int num6 = 0; num6 < medianConvolutionState.Kernel.Columns; num6++)
					{
						int num7 = Unsafe.Add(ref sampleColumn2, (uint)num6) - x;
						Vector4 value2 = Unsafe.Add(ref reference6, (uint)num7);
						medianConvolutionState.Kernel.SetValue(num5, value2);
						num5++;
					}
				}
				reference5 = FindMedian4(medianConvolutionState.Kernel.Span, xChannel, yChannel, zChannel, wChannel);
			}
		}
		span6 = targetPixels.DangerousGetRowSpan(y);
		Span<TPixel> destinationPixels = span6.Slice(x, width);
		PixelOperations<TPixel>.Instance.FromVector4Destructive(configuration, span4, destinationPixels);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Vector4 FindMedian3(ReadOnlySpan<Vector4> kernelSpan, Span<float> xChannel, Span<float> yChannel, Span<float> zChannel)
	{
		int index = kernelSpan.Length + 1 >> 1;
		for (int i = 0; i < xChannel.Length; i++)
		{
			xChannel[i] = kernelSpan[i].X;
			yChannel[i] = kernelSpan[i].Y;
			zChannel[i] = kernelSpan[i].Z;
		}
		xChannel.Sort();
		yChannel.Sort();
		zChannel.Sort();
		return new Vector4(xChannel[index], yChannel[index], zChannel[index], kernelSpan[index].W);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Vector4 FindMedian4(ReadOnlySpan<Vector4> kernelSpan, Span<float> xChannel, Span<float> yChannel, Span<float> zChannel, Span<float> wChannel)
	{
		int index = kernelSpan.Length + 1 >> 1;
		for (int i = 0; i < xChannel.Length; i++)
		{
			xChannel[i] = kernelSpan[i].X;
			yChannel[i] = kernelSpan[i].Y;
			zChannel[i] = kernelSpan[i].Z;
			wChannel[i] = kernelSpan[i].W;
		}
		xChannel.Sort();
		yChannel.Sort();
		zChannel.Sort();
		wChannel.Sort();
		return new Vector4(xChannel[index], yChannel[index], zChannel[index], wChannel[index]);
	}
}
