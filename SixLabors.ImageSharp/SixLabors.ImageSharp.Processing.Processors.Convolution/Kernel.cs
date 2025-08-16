using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SixLabors.ImageSharp.Processing.Processors.Convolution;

internal readonly ref struct Kernel<T> where T : struct, IEquatable<T>
{
	private readonly Span<T> values;

	public int Columns
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get;
	}

	public int Rows
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get;
	}

	public ReadOnlySpan<T> Span
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return values;
		}
	}

	public T this[int row, int column]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return Unsafe.Add(ref MemoryMarshal.GetReference(values), (uint)(row * Columns + column));
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			Unsafe.Add(ref MemoryMarshal.GetReference(values), (uint)(row * Columns + column)) = value;
		}
	}

	public Kernel(DenseMatrix<T> matrix)
	{
		Columns = matrix.Columns;
		Rows = matrix.Rows;
		values = matrix.Span;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetValue(int index, T value)
	{
		Unsafe.Add(ref MemoryMarshal.GetReference(values), (uint)index) = value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Clear()
	{
		values.Clear();
	}

	[Conditional("DEBUG")]
	private void CheckCoordinates(int row, int column)
	{
		if (row < 0 || row >= Rows)
		{
			throw new ArgumentOutOfRangeException("row", row, $"{row} is outside the matrix bounds.");
		}
		if (column < 0 || column >= Columns)
		{
			throw new ArgumentOutOfRangeException("column", column, $"{column} is outside the matrix bounds.");
		}
	}

	[Conditional("DEBUG")]
	private void CheckIndex(int index)
	{
		if (index < 0 || index >= values.Length)
		{
			throw new ArgumentOutOfRangeException("index", index, $"{index} is outside the matrix bounds.");
		}
	}
}
