using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Formats.Webp.Lossless;

internal sealed class Vp8LHistogramSet : IEnumerable<Vp8LHistogram>, IEnumerable, IDisposable
{
	private sealed class MemberVp8LHistogram : Vp8LHistogram
	{
		public unsafe MemberVp8LHistogram(uint* basePointer, int paletteCodeBits)
			: base(basePointer, paletteCodeBits)
		{
		}

		public unsafe MemberVp8LHistogram(uint* basePointer, Vp8LBackwardRefs refs, int paletteCodeBits)
			: base(basePointer, refs, paletteCodeBits)
		{
		}
	}

	private readonly IMemoryOwner<uint> buffer;

	private MemoryHandle bufferHandle;

	private readonly List<Vp8LHistogram> items;

	private bool isDisposed;

	public int Count => items.Count;

	public Vp8LHistogram this[int index]
	{
		get
		{
			return items[index];
		}
		set
		{
			items[index] = value;
		}
	}

	public unsafe Vp8LHistogramSet(MemoryAllocator memoryAllocator, int capacity, int cacheBits)
	{
		buffer = memoryAllocator.Allocate<uint>(2118 * capacity, AllocationOptions.Clean);
		bufferHandle = buffer.Memory.Pin();
		uint* pointer = (uint*)bufferHandle.Pointer;
		items = new List<Vp8LHistogram>(capacity);
		for (int i = 0; i < capacity; i++)
		{
			items.Add(new MemberVp8LHistogram(pointer + 2118 * i, cacheBits));
		}
	}

	public unsafe Vp8LHistogramSet(MemoryAllocator memoryAllocator, Vp8LBackwardRefs refs, int capacity, int cacheBits)
	{
		buffer = memoryAllocator.Allocate<uint>(2118 * capacity, AllocationOptions.Clean);
		bufferHandle = buffer.Memory.Pin();
		uint* pointer = (uint*)bufferHandle.Pointer;
		items = new List<Vp8LHistogram>(capacity);
		for (int i = 0; i < capacity; i++)
		{
			items.Add(new MemberVp8LHistogram(pointer + 2118 * i, refs, cacheBits));
		}
	}

	public Vp8LHistogramSet(int capacity)
	{
		items = new List<Vp8LHistogram>(capacity);
	}

	public Vp8LHistogramSet()
	{
		items = new List<Vp8LHistogram>();
	}

	public void RemoveAt(int index)
	{
		items.RemoveAt(index);
	}

	public void Dispose()
	{
		if (!isDisposed)
		{
			buffer.Dispose();
			bufferHandle.Dispose();
			items.Clear();
			isDisposed = true;
		}
	}

	public IEnumerator<Vp8LHistogram> GetEnumerator()
	{
		return ((IEnumerable<Vp8LHistogram>)items).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable)items).GetEnumerator();
	}

	[Conditional("DEBUG")]
	private void CheckDisposed()
	{
		if (isDisposed)
		{
			ThrowDisposed();
		}
	}

	private static void ThrowDisposed()
	{
		throw new ObjectDisposedException("Vp8LHistogramSet");
	}
}
