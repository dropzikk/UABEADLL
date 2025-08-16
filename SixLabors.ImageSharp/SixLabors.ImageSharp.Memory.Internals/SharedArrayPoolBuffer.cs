using System;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SixLabors.ImageSharp.Memory.Internals;

internal class SharedArrayPoolBuffer<T> : ManagedBufferBase<T>, IRefCounted where T : struct
{
	private sealed class LifetimeGuard : RefCountedMemoryLifetimeGuard
	{
		private byte[]? array;

		public LifetimeGuard(byte[] array)
		{
			this.array = array;
		}

		protected override void Release()
		{
			ArrayPool<byte>.Shared.Return(array);
			array = null;
		}
	}

	private readonly int lengthInBytes;

	private LifetimeGuard lifetimeGuard;

	public byte[]? Array { get; private set; }

	public SharedArrayPoolBuffer(int lengthInElements)
	{
		lengthInBytes = lengthInElements * Unsafe.SizeOf<T>();
		Array = ArrayPool<byte>.Shared.Rent(lengthInBytes);
		lifetimeGuard = new LifetimeGuard(Array);
	}

	protected override void Dispose(bool disposing)
	{
		if (Array != null)
		{
			lifetimeGuard.Dispose();
			Array = null;
		}
	}

	public override Span<T> GetSpan()
	{
		return MemoryMarshal.Cast<byte, T>(Array.AsSpan(0, lengthInBytes));
	}

	protected override object GetPinnableObject()
	{
		return Array;
	}

	public void AddRef()
	{
		lifetimeGuard.AddRef();
	}

	public void ReleaseRef()
	{
		lifetimeGuard.ReleaseRef();
	}

	[Conditional("DEBUG")]
	[MemberNotNull("Array")]
	private void CheckDisposed()
	{
		if (Array == null)
		{
			throw new ObjectDisposedException("SharedArrayPoolBuffer");
		}
	}
}
