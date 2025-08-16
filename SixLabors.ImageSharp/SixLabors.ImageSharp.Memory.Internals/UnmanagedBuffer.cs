using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace SixLabors.ImageSharp.Memory.Internals;

internal sealed class UnmanagedBuffer<T> : MemoryManager<T>, IRefCounted where T : struct
{
	private readonly int lengthInElements;

	private readonly UnmanagedBufferLifetimeGuard lifetimeGuard;

	private int disposed;

	public unsafe void* Pointer => lifetimeGuard.Handle.Pointer;

	public UnmanagedBuffer(int lengthInElements, UnmanagedBufferLifetimeGuard lifetimeGuard)
	{
		this.lengthInElements = lengthInElements;
		this.lifetimeGuard = lifetimeGuard;
	}

	public unsafe override Span<T> GetSpan()
	{
		return new Span<T>(Pointer, lengthInElements);
	}

	public unsafe override MemoryHandle Pin(int elementIndex = 0)
	{
		lifetimeGuard.AddRef();
		return new MemoryHandle(Unsafe.Add<T>(Pointer, elementIndex), default(GCHandle), this);
	}

	protected override void Dispose(bool disposing)
	{
		if (Interlocked.Exchange(ref disposed, 1) != 1)
		{
			lifetimeGuard.Dispose();
		}
	}

	public override void Unpin()
	{
		lifetimeGuard.ReleaseRef();
	}

	public void AddRef()
	{
		lifetimeGuard.AddRef();
	}

	public void ReleaseRef()
	{
		lifetimeGuard.ReleaseRef();
	}

	public static UnmanagedBuffer<T> Allocate(int lengthInElements)
	{
		return new UnmanagedBuffer<T>(lengthInElements, new UnmanagedBufferLifetimeGuard.FreeHandle(UnmanagedMemoryHandle.Allocate(lengthInElements * Unsafe.SizeOf<T>())));
	}
}
