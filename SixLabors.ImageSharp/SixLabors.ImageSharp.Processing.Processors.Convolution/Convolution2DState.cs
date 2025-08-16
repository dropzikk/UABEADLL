using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SixLabors.ImageSharp.Processing.Processors.Convolution;

internal readonly ref struct Convolution2DState
{
	private readonly Span<int> rowOffsetMap;

	private readonly Span<int> columnOffsetMap;

	private readonly uint kernelHeight;

	private readonly uint kernelWidth;

	public ReadOnlyKernel KernelY
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get;
	}

	public ReadOnlyKernel KernelX
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get;
	}

	public Convolution2DState(in DenseMatrix<float> kernelY, in DenseMatrix<float> kernelX, KernelSamplingMap map)
	{
		KernelY = new ReadOnlyKernel(kernelY);
		KernelX = new ReadOnlyKernel(kernelX);
		kernelHeight = (uint)kernelY.Rows;
		kernelWidth = (uint)kernelY.Columns;
		rowOffsetMap = map.GetRowOffsetSpan();
		columnOffsetMap = map.GetColumnOffsetSpan();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ref int GetSampleRow(uint row)
	{
		return ref Unsafe.Add(ref MemoryMarshal.GetReference(rowOffsetMap), row * kernelHeight);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ref int GetSampleColumn(uint column)
	{
		return ref Unsafe.Add(ref MemoryMarshal.GetReference(columnOffsetMap), column * kernelWidth);
	}
}
