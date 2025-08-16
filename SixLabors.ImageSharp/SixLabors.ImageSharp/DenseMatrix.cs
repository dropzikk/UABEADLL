using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp;

public readonly struct DenseMatrix<T> : IEquatable<DenseMatrix<T>> where T : struct, IEquatable<T>
{
	public T[] Data { get; }

	public int Columns { get; }

	public int Rows { get; }

	public Size Size { get; }

	public int Count { get; }

	public Span<T> Span => new Span<T>(Data);

	public ref T this[int row, int column]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return ref Data[row * Columns + column];
		}
	}

	public DenseMatrix(int length)
		: this(length, length)
	{
	}

	public DenseMatrix(int columns, int rows)
	{
		Guard.MustBeGreaterThan(columns, 0, "columns");
		Guard.MustBeGreaterThan(rows, 0, "rows");
		Rows = rows;
		Columns = columns;
		Size = new Size(columns, rows);
		Count = columns * rows;
		Data = new T[Columns * Rows];
	}

	public DenseMatrix(T[,] data)
	{
		Guard.NotNull(data, "data");
		int length = data.GetLength(0);
		int length2 = data.GetLength(1);
		Guard.MustBeGreaterThan(length, 0, "Rows");
		Guard.MustBeGreaterThan(length2, 0, "Columns");
		Rows = length;
		Columns = length2;
		Size = new Size(length2, length);
		Count = Columns * Rows;
		Data = new T[Columns * Rows];
		for (int i = 0; i < Rows; i++)
		{
			for (int j = 0; j < Columns; j++)
			{
				this[i, j] = data[i, j];
			}
		}
	}

	public DenseMatrix(int columns, int rows, Span<T> data)
	{
		Guard.MustBeGreaterThan(rows, 0, "Rows");
		Guard.MustBeGreaterThan(columns, 0, "Columns");
		Guard.IsTrue(rows * columns == data.Length, "data", "Length should be equal to ros * columns");
		Rows = rows;
		Columns = columns;
		Size = new Size(columns, rows);
		Count = Columns * Rows;
		Data = new T[Columns * Rows];
		data.CopyTo(Data);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator DenseMatrix<T>(T[,] data)
	{
		return new DenseMatrix<T>(data);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator T[,](in DenseMatrix<T> data)
	{
		T[,] array = new T[data.Rows, data.Columns];
		for (int i = 0; i < data.Rows; i++)
		{
			for (int j = 0; j < data.Columns; j++)
			{
				ref T reference = ref array[i, j];
				reference = data[i, j];
			}
		}
		return array;
	}

	public static bool operator ==(DenseMatrix<T> left, DenseMatrix<T> right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(DenseMatrix<T> left, DenseMatrix<T> right)
	{
		return !(left == right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DenseMatrix<T> Transpose()
	{
		DenseMatrix<T> result = new DenseMatrix<T>(Rows, Columns);
		for (int i = 0; i < Rows; i++)
		{
			for (int j = 0; j < Columns; j++)
			{
				result[j, i] = this[i, j];
			}
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Fill(T value)
	{
		Span.Fill(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Clear()
	{
		Span.Clear();
	}

	[Conditional("DEBUG")]
	private void CheckCoordinates(int row, int column)
	{
		if (row < 0 || row >= Rows)
		{
			throw new ArgumentOutOfRangeException("row", row, $"{row} is outwith the matrix bounds.");
		}
		if (column < 0 || column >= Columns)
		{
			throw new ArgumentOutOfRangeException("column", column, $"{column} is outwith the matrix bounds.");
		}
	}

	public override bool Equals(object? obj)
	{
		if (obj is DenseMatrix<T> other)
		{
			return Equals(other);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(DenseMatrix<T> other)
	{
		if (Columns == other.Columns && Rows == other.Rows)
		{
			return Span.SequenceEqual(other.Span);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode()
	{
		HashCode hashCode = default(HashCode);
		hashCode.Add(Columns);
		hashCode.Add(Rows);
		Span<T> span = Span;
		for (int i = 0; i < span.Length; i++)
		{
			hashCode.Add(span[i]);
		}
		return hashCode.ToHashCode();
	}
}
