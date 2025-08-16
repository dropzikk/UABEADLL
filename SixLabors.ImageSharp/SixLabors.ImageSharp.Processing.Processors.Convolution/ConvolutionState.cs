using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SixLabors.ImageSharp.Processing.Processors.Convolution;

internal readonly ref struct ConvolutionState
{
	private readonly Span<int> rowOffsetMap;

	private readonly Span<int> columnOffsetMap;

	private readonly uint kernelHeight;

	private readonly uint kernelWidth;

	public ReadOnlyKernel Kernel
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get;
	}

	public ConvolutionState(in DenseMatrix<float> kernel, KernelSamplingMap map)
	{
		Kernel = new ReadOnlyKernel(kernel);
		kernelHeight = (uint)kernel.Rows;
		kernelWidth = (uint)kernel.Columns;
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
