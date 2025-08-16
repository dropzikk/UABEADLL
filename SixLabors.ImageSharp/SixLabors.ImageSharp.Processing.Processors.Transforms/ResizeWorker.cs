using System;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

internal sealed class ResizeWorker<TPixel> : IDisposable where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly Buffer2D<Vector4> transposedFirstPassBuffer;

	private readonly Configuration configuration;

	private readonly PixelConversionModifiers conversionModifiers;

	private readonly ResizeKernelMap horizontalKernelMap;

	private readonly Buffer2DRegion<TPixel> source;

	private readonly Rectangle sourceRectangle;

	private readonly IMemoryOwner<Vector4> tempRowBuffer;

	private readonly IMemoryOwner<Vector4> tempColumnBuffer;

	private readonly ResizeKernelMap verticalKernelMap;

	private readonly Rectangle targetWorkingRect;

	private readonly Point targetOrigin;

	private readonly int windowBandHeight;

	private readonly int workerHeight;

	private RowInterval currentWindow;

	public ResizeWorker(Configuration configuration, Buffer2DRegion<TPixel> source, PixelConversionModifiers conversionModifiers, ResizeKernelMap horizontalKernelMap, ResizeKernelMap verticalKernelMap, Rectangle targetWorkingRect, Point targetOrigin)
	{
		this.configuration = configuration;
		this.source = source;
		sourceRectangle = source.Rectangle;
		this.conversionModifiers = conversionModifiers;
		this.horizontalKernelMap = horizontalKernelMap;
		this.verticalKernelMap = verticalKernelMap;
		this.targetWorkingRect = targetWorkingRect;
		this.targetOrigin = targetOrigin;
		windowBandHeight = verticalKernelMap.MaxDiameter;
		int sizeLimitHintInBytes = Math.Min(configuration.WorkingBufferSizeHintInBytes, configuration.MemoryAllocator.GetBufferCapacityInBytes());
		int num = ResizeHelper.CalculateResizeWorkerHeightInWindowBands(windowBandHeight, targetWorkingRect.Width, sizeLimitHintInBytes);
		workerHeight = Math.Min(sourceRectangle.Height, num * windowBandHeight);
		transposedFirstPassBuffer = configuration.MemoryAllocator.Allocate2D<Vector4>(workerHeight, targetWorkingRect.Width, preferContiguosImageBuffers: true, AllocationOptions.Clean);
		tempRowBuffer = configuration.MemoryAllocator.Allocate<Vector4>(sourceRectangle.Width);
		tempColumnBuffer = configuration.MemoryAllocator.Allocate<Vector4>(targetWorkingRect.Width);
		currentWindow = new RowInterval(0, workerHeight);
	}

	public void Dispose()
	{
		transposedFirstPassBuffer.Dispose();
		tempRowBuffer.Dispose();
		tempColumnBuffer.Dispose();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Span<Vector4> GetColumnSpan(int x, int startY)
	{
		Span<Vector4> span = transposedFirstPassBuffer.DangerousGetRowSpan(x);
		int num = startY - currentWindow.Min;
		return span.Slice(num, span.Length - num);
	}

	public void Initialize()
	{
		CalculateFirstPassValues(currentWindow);
	}

	public void FillDestinationPixels(RowInterval rowInterval, Buffer2D<TPixel> destination)
	{
		Span<Vector4> span = tempColumnBuffer.GetSpan();
		Span<Vector4> span2 = transposedFirstPassBuffer.DangerousGetSingleSpan();
		int left = targetWorkingRect.Left;
		int right = targetWorkingRect.Right;
		int width = targetWorkingRect.Width;
		for (int i = rowInterval.Min; i < rowInterval.Max; i++)
		{
			ResizeKernel kernel = verticalKernelMap.GetKernel((uint)(i - targetOrigin.Y));
			while (kernel.StartIndex + kernel.Length > currentWindow.Max)
			{
				Slide();
			}
			ref Vector4 reference = ref MemoryMarshal.GetReference(span);
			int index = kernel.StartIndex - currentWindow.Min;
			ref Vector4 reference2 = ref span2[index];
			for (nuint num = 0u; num < (uint)(right - left); num++)
			{
				ref Vector4 rowStartRef = ref Unsafe.Add(ref reference2, num * (uint)workerHeight);
				Unsafe.Add(ref reference, num) = kernel.ConvolveCore(ref rowStartRef);
			}
			Span<TPixel> destinationPixels = destination.DangerousGetRowSpan(i).Slice(left, width);
			PixelOperations<TPixel>.Instance.FromVector4Destructive(configuration, span, destinationPixels, conversionModifiers);
		}
	}

	private void Slide()
	{
		int num = currentWindow.Max - windowBandHeight;
		int max = Math.Min(num + workerHeight, sourceRectangle.Height);
		transposedFirstPassBuffer.DangerousCopyColumns(workerHeight - windowBandHeight, 0, windowBandHeight);
		currentWindow = new RowInterval(num, max);
		CalculateFirstPassValues(currentWindow.Slice(windowBandHeight));
	}

	private void CalculateFirstPassValues(RowInterval calculationInterval)
	{
		Span<Vector4> span = tempRowBuffer.GetSpan();
		Span<Vector4> span2 = transposedFirstPassBuffer.DangerousGetSingleSpan();
		nuint num = (uint)targetWorkingRect.Left;
		nuint num2 = (uint)targetWorkingRect.Right;
		nuint num3 = (uint)targetOrigin.X;
		for (int i = calculationInterval.Min; i < calculationInterval.Max; i++)
		{
			Span<TPixel> span3 = source.DangerousGetRowSpan(i);
			PixelOperations<TPixel>.Instance.ToVector4(configuration, span3, span, conversionModifiers);
			ref Vector4 reference = ref span2[i - currentWindow.Min];
			nuint num4 = num;
			nuint num5 = 0u;
			while (num4 < num2)
			{
				ResizeKernel kernel = horizontalKernelMap.GetKernel(num4 - num3);
				Unsafe.Add(ref reference, num5 * (uint)workerHeight) = kernel.Convolve(span);
				num4++;
				num5++;
			}
		}
	}
}
