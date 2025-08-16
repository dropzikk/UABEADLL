using System;

namespace SixLabors.ImageSharp.Memory.Internals;

internal class BasicArrayBuffer<T> : ManagedBufferBase<T> where T : struct
{
	public T[] Array { get; }

	public int Length { get; }

	public BasicArrayBuffer(T[] array, int length)
	{
		Array = array;
		Length = length;
	}

	public BasicArrayBuffer(T[] array)
		: this(array, array.Length)
	{
	}

	public override Span<T> GetSpan()
	{
		return Array.AsSpan(0, Length);
	}

	protected override void Dispose(bool disposing)
	{
	}

	protected override object GetPinnableObject()
	{
		return Array;
	}
}
