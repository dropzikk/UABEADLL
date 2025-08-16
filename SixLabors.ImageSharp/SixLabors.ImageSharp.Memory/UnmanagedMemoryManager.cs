using System;
using System.Buffers;
using System.Runtime.InteropServices;

namespace SixLabors.ImageSharp.Memory;

internal sealed class UnmanagedMemoryManager<T> : MemoryManager<T> where T : unmanaged
{
	private unsafe readonly void* pointer;

	private readonly int length;

	public unsafe UnmanagedMemoryManager(void* pointer, int length)
	{
		this.pointer = pointer;
		this.length = length;
	}

	protected override void Dispose(bool disposing)
	{
	}

	public unsafe override Span<T> GetSpan()
	{
		return new Span<T>(pointer, length);
	}

	public unsafe override MemoryHandle Pin(int elementIndex = 0)
	{
		return new MemoryHandle((byte*)pointer + (nint)elementIndex * (nint)sizeof(T), default(GCHandle), this);
	}

	public override void Unpin()
	{
	}
}
