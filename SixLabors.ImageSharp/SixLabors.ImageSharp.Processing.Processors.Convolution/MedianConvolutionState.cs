using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SixLabors.ImageSharp.Processing.Processors.Convolution;

internal readonly ref struct MedianConvolutionState
{
	private readonly Span<int> rowOffsetMap;

	private readonly Span<int> columnOffsetMap;

	private readonly int kernelHeight;

	private readonly int kernelWidth;

	public Kernel<Vector4> Kernel
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get;
	}

	public MedianConvolutionState(in DenseMatrix<Vector4> kernel, KernelSamplingMap map)
	{
		Kernel = new Kernel<Vector4>(kernel);
		kernelHeight = kernel.Rows;
		kernelWidth = kernel.Columns;
		rowOffsetMap = map.GetRowOffsetSpan();
		columnOffsetMap = map.GetColumnOffsetSpan();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ref int GetSampleRow(int row)
	{
		return ref Unsafe.Add(ref MemoryMarshal.GetReference(rowOffsetMap), (uint)(row * kernelHeight));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ref int GetSampleColumn(int column)
	{
		return ref Unsafe.Add(ref MemoryMarshal.GetReference(columnOffsetMap), (uint)(column * kernelWidth));
	}
}
