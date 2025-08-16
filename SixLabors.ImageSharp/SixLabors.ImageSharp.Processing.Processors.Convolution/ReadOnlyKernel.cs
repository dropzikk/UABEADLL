using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SixLabors.ImageSharp.Processing.Processors.Convolution;

internal readonly ref struct ReadOnlyKernel
{
	private readonly ReadOnlySpan<float> values;

	public uint Columns
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get;
	}

	public uint Rows
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get;
	}

	public float this[uint row, uint column]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return Unsafe.Add(ref MemoryMarshal.GetReference(values), row * Columns + column);
		}
	}

	public ReadOnlyKernel(DenseMatrix<float> matrix)
	{
		Columns = (uint)matrix.Columns;
		Rows = (uint)matrix.Rows;
		values = matrix.Span;
	}

	[Conditional("DEBUG")]
	private void CheckCoordinates(uint row, uint column)
	{
		if (row >= Rows)
		{
			throw new ArgumentOutOfRangeException("row", row, $"{row} is outwith the matrix bounds.");
		}
		if (column >= Columns)
		{
			throw new ArgumentOutOfRangeException("column", column, $"{column} is outwith the matrix bounds.");
		}
	}
}
