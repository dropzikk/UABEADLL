using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Processing.Processors.Convolution;

internal sealed class KernelSamplingMap : IDisposable
{
	private readonly MemoryAllocator allocator;

	private bool isDisposed;

	private IMemoryOwner<int>? yOffsets;

	private IMemoryOwner<int>? xOffsets;

	public KernelSamplingMap(MemoryAllocator allocator)
	{
		this.allocator = allocator;
	}

	public void BuildSamplingOffsetMap(DenseMatrix<float> kernel, Rectangle bounds)
	{
		BuildSamplingOffsetMap(kernel.Rows, kernel.Columns, bounds, BorderWrappingMode.Repeat, BorderWrappingMode.Repeat);
	}

	public void BuildSamplingOffsetMap(int kernelHeight, int kernelWidth, Rectangle bounds)
	{
		BuildSamplingOffsetMap(kernelHeight, kernelWidth, bounds, BorderWrappingMode.Repeat, BorderWrappingMode.Repeat);
	}

	public void BuildSamplingOffsetMap(int kernelHeight, int kernelWidth, Rectangle bounds, BorderWrappingMode xBorderMode, BorderWrappingMode yBorderMode)
	{
		yOffsets = allocator.Allocate<int>(bounds.Height * kernelHeight);
		xOffsets = allocator.Allocate<int>(bounds.Width * kernelWidth);
		int y = bounds.Y;
		int max = bounds.Bottom - 1;
		int x = bounds.X;
		int max2 = bounds.Right - 1;
		BuildOffsets(yOffsets, bounds.Height, kernelHeight, y, max, yBorderMode);
		BuildOffsets(xOffsets, bounds.Width, kernelWidth, x, max2, xBorderMode);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Span<int> GetRowOffsetSpan()
	{
		return yOffsets.GetSpan();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Span<int> GetColumnOffsetSpan()
	{
		return xOffsets.GetSpan();
	}

	public void Dispose()
	{
		if (!isDisposed)
		{
			yOffsets?.Dispose();
			xOffsets?.Dispose();
			isDisposed = true;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void BuildOffsets(IMemoryOwner<int> offsets, int boundsSize, int kernelSize, int min, int max, BorderWrappingMode borderMode)
	{
		int num = kernelSize >> 1;
		Span<int> span = offsets.GetSpan();
		ref int reference = ref MemoryMarshal.GetReference(span);
		for (int i = 0; i < boundsSize; i++)
		{
			int num2 = i * kernelSize;
			for (int j = 0; j < kernelSize; j++)
			{
				Unsafe.Add(ref reference, (uint)(num2 + j)) = i + j + min - num;
			}
		}
		CorrectBorder(span, kernelSize, min, max, borderMode);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void CorrectBorder(Span<int> span, int kernelSize, int min, int max, BorderWrappingMode borderMode)
	{
		int num = (kernelSize >> 1) * kernelSize;
		MemoryMarshal.GetReference(span);
		if (num <= 0)
		{
			return;
		}
		switch (borderMode)
		{
		case BorderWrappingMode.Repeat:
		{
			Numerics.Clamp(span.Slice(0, num), min, max);
			int num9 = num;
			int length = span.Length;
			int num10 = length - num9;
			Numerics.Clamp(span.Slice(num10, length - num10), min, max);
			break;
		}
		case BorderWrappingMode.Mirror:
		{
			int num11 = min + min - 1;
			for (int m = 0; m < num; m++)
			{
				int num12 = span[m];
				if (num12 < min)
				{
					span[m] = num11 - num12;
				}
			}
			int num13 = max + max + 1;
			for (int n = span.Length - num; n < span.Length; n++)
			{
				int num14 = span[n];
				if (num14 > max)
				{
					span[n] = num13 - num14;
				}
			}
			break;
		}
		case BorderWrappingMode.Bounce:
		{
			int num5 = min + min;
			for (int k = 0; k < num; k++)
			{
				int num6 = span[k];
				if (num6 < min)
				{
					span[k] = num5 - num6;
				}
			}
			int num7 = max + max;
			for (int l = span.Length - num; l < span.Length; l++)
			{
				int num8 = span[l];
				if (num8 > max)
				{
					span[l] = num7 - num8;
				}
			}
			break;
		}
		case BorderWrappingMode.Wrap:
		{
			int num2 = max - min + 1;
			for (int i = 0; i < num; i++)
			{
				int num3 = span[i];
				if (num3 < min)
				{
					span[i] = num2 + num3;
				}
			}
			for (int j = span.Length - num; j < span.Length; j++)
			{
				int num4 = span[j];
				if (num4 > max)
				{
					span[j] = num4 - num2;
				}
			}
			break;
		}
		}
	}
}
