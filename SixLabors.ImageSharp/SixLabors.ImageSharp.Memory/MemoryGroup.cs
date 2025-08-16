using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.Memory.Internals;

namespace SixLabors.ImageSharp.Memory;

internal abstract class MemoryGroup<T> : IMemoryGroup<T>, IReadOnlyList<Memory<T>>, IEnumerable<Memory<T>>, IEnumerable, IReadOnlyCollection<Memory<T>>, IDisposable where T : struct
{
	public sealed class Consumed : MemoryGroup<T>, IEnumerable<Memory<T>>, IEnumerable
	{
		private readonly Memory<T>[] source;

		public override int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return source.Length;
			}
		}

		public override Memory<T> this[int index] => source[index];

		public Consumed(Memory<T>[] source, int bufferLength, long totalLength)
			: base(bufferLength, totalLength)
		{
			this.source = source;
			base.View = new MemoryGroupView<T>(this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override MemoryGroupEnumerator<T> GetEnumerator()
		{
			return new MemoryGroupEnumerator<T>(this);
		}

		IEnumerator<Memory<T>> IEnumerable<Memory<T>>.GetEnumerator()
		{
			return ((IEnumerable<Memory<T>>)source).GetEnumerator();
		}

		public override void Dispose()
		{
			base.View.Invalidate();
		}
	}

	public sealed class Owned : MemoryGroup<T>, IEnumerable<Memory<T>>, IEnumerable
	{
		private sealed class ObservedBuffer : MemoryManager<T>
		{
			private readonly UnmanagedMemoryHandle handle;

			private readonly int lengthInElements;

			private ObservedBuffer(UnmanagedMemoryHandle handle, int lengthInElements)
			{
				this.handle = handle;
				this.lengthInElements = lengthInElements;
			}

			public static ObservedBuffer Create(UnmanagedMemoryHandle handle, int lengthInElements, AllocationOptions options)
			{
				ObservedBuffer observedBuffer = new ObservedBuffer(handle, lengthInElements);
				if (options.Has(AllocationOptions.Clean))
				{
					observedBuffer.GetSpan().Clear();
				}
				return observedBuffer;
			}

			protected override void Dispose(bool disposing)
			{
			}

			public unsafe override Span<T> GetSpan()
			{
				return new Span<T>(handle.Pointer, lengthInElements);
			}

			public unsafe override MemoryHandle Pin(int elementIndex = 0)
			{
				return new MemoryHandle(Unsafe.Add<T>(handle.Pointer, elementIndex));
			}

			public override void Unpin()
			{
			}
		}

		private IMemoryOwner<T>[]? memoryOwners;

		private RefCountedMemoryLifetimeGuard? groupLifetimeGuard;

		public bool Swappable { get; }

		private bool IsDisposed => memoryOwners == null;

		public override int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				EnsureNotDisposed();
				return memoryOwners.Length;
			}
		}

		public override Memory<T> this[int index]
		{
			get
			{
				EnsureNotDisposed();
				return memoryOwners[index].Memory;
			}
		}

		public Owned(IMemoryOwner<T>[] memoryOwners, int bufferLength, long totalLength, bool swappable)
			: base(bufferLength, totalLength)
		{
			this.memoryOwners = memoryOwners;
			Swappable = swappable;
			base.View = new MemoryGroupView<T>(this);
			memoryGroupSpanCache = MemoryGroupSpanCache.Create(memoryOwners);
		}

		public Owned(UniformUnmanagedMemoryPool pool, UnmanagedMemoryHandle[] pooledHandles, int bufferLength, long totalLength, int sizeOfLastBuffer, AllocationOptions options)
			: this(CreateBuffers(pooledHandles, bufferLength, sizeOfLastBuffer, options), bufferLength, totalLength, swappable: true)
		{
			groupLifetimeGuard = pool.CreateGroupLifetimeGuard(pooledHandles);
		}

		private static IMemoryOwner<T>[] CreateBuffers(UnmanagedMemoryHandle[] pooledBuffers, int bufferLength, int sizeOfLastBuffer, AllocationOptions options)
		{
			IMemoryOwner<T>[] array = new IMemoryOwner<T>[pooledBuffers.Length];
			for (int i = 0; i < pooledBuffers.Length - 1; i++)
			{
				ObservedBuffer observedBuffer = ObservedBuffer.Create(pooledBuffers[i], bufferLength, options);
				array[i] = observedBuffer;
			}
			ObservedBuffer observedBuffer2 = ObservedBuffer.Create(pooledBuffers[^1], sizeOfLastBuffer, options);
			array[^1] = observedBuffer2;
			return array;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override MemoryGroupEnumerator<T> GetEnumerator()
		{
			return new MemoryGroupEnumerator<T>(this);
		}

		public override void IncreaseRefCounts()
		{
			EnsureNotDisposed();
			if (groupLifetimeGuard != null)
			{
				groupLifetimeGuard.AddRef();
				return;
			}
			IMemoryOwner<T>[] array = memoryOwners;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] is IRefCounted refCounted)
				{
					refCounted.AddRef();
				}
			}
		}

		public override void DecreaseRefCounts()
		{
			EnsureNotDisposed();
			if (groupLifetimeGuard != null)
			{
				groupLifetimeGuard.ReleaseRef();
				return;
			}
			IMemoryOwner<T>[] array = memoryOwners;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] is IRefCounted refCounted)
				{
					refCounted.ReleaseRef();
				}
			}
		}

		public override void RecreateViewAfterSwap()
		{
			base.View.Invalidate();
			base.View = new MemoryGroupView<T>(this);
		}

		IEnumerator<Memory<T>> IEnumerable<Memory<T>>.GetEnumerator()
		{
			EnsureNotDisposed();
			return memoryOwners.Select((IMemoryOwner<T> mo) => mo.Memory).GetEnumerator();
		}

		public override void Dispose()
		{
			if (IsDisposed)
			{
				return;
			}
			base.View.Invalidate();
			if (groupLifetimeGuard != null)
			{
				groupLifetimeGuard.Dispose();
			}
			else
			{
				IMemoryOwner<T>[] array = memoryOwners;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Dispose();
				}
			}
			memoryOwners = null;
			base.IsValid = false;
			groupLifetimeGuard = null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[MemberNotNull("memoryOwners")]
		private void EnsureNotDisposed()
		{
			if (memoryOwners == null)
			{
				ThrowObjectDisposedException();
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		[DoesNotReturn]
		private static void ThrowObjectDisposedException()
		{
			throw new ObjectDisposedException("MemoryGroup");
		}
	}

	private static readonly int ElementSize = Unsafe.SizeOf<T>();

	private MemoryGroupSpanCache memoryGroupSpanCache;

	public abstract int Count { get; }

	public int BufferLength { get; }

	public long TotalLength { get; }

	public bool IsValid { get; private set; } = true;

	public MemoryGroupView<T> View { get; private set; }

	public abstract Memory<T> this[int index] { get; }

	private MemoryGroup(int bufferLength, long totalLength)
	{
		BufferLength = bufferLength;
		TotalLength = totalLength;
	}

	public abstract void Dispose();

	public abstract MemoryGroupEnumerator<T> GetEnumerator();

	IEnumerator<Memory<T>> IEnumerable<Memory<T>>.GetEnumerator()
	{
		throw new NotImplementedException($"The type {GetType()} needs to override IEnumerable<Memory<T>>.GetEnumerator()");
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable<Memory<T>>)this).GetEnumerator();
	}

	public static MemoryGroup<T> Allocate(MemoryAllocator allocator, long totalLengthInElements, int bufferAlignmentInElements, AllocationOptions options = AllocationOptions.None)
	{
		int bufferCapacityInBytes = allocator.GetBufferCapacityInBytes();
		Guard.NotNull(allocator, "allocator");
		if (totalLengthInElements < 0)
		{
			InvalidMemoryOperationException.ThrowNegativeAllocationException(totalLengthInElements);
		}
		int num = bufferCapacityInBytes / ElementSize;
		if (bufferAlignmentInElements < 0 || bufferAlignmentInElements > num)
		{
			InvalidMemoryOperationException.ThrowInvalidAlignmentException(bufferAlignmentInElements);
		}
		if (totalLengthInElements == 0L)
		{
			return new Owned(new IMemoryOwner<T>[1] { allocator.Allocate<T>(0, options) }, 0, 0L, swappable: true);
		}
		int num2 = num / bufferAlignmentInElements * bufferAlignmentInElements;
		if (totalLengthInElements > 0 && totalLengthInElements < num2)
		{
			num2 = (int)totalLengthInElements;
		}
		int num3 = (int)(totalLengthInElements % num2);
		long num4 = totalLengthInElements / num2;
		if (num3 == 0)
		{
			num3 = num2;
		}
		else
		{
			num4++;
		}
		IMemoryOwner<T>[] array = new IMemoryOwner<T>[num4];
		for (int i = 0; i < array.Length - 1; i++)
		{
			array[i] = allocator.Allocate<T>(num2, options);
		}
		if (num4 > 0)
		{
			array[^1] = allocator.Allocate<T>(num3, options);
		}
		return new Owned(array, num2, totalLengthInElements, swappable: true);
	}

	public static MemoryGroup<T> CreateContiguous(IMemoryOwner<T> buffer, bool clear)
	{
		if (clear)
		{
			buffer.GetSpan().Clear();
		}
		int length = buffer.Memory.Length;
		return new Owned(new IMemoryOwner<T>[1] { buffer }, length, length, swappable: true);
	}

	public static bool TryAllocate(UniformUnmanagedMemoryPool pool, long totalLengthInElements, int bufferAlignmentInElements, AllocationOptions options, [NotNullWhen(true)] out MemoryGroup<T>? memoryGroup)
	{
		Guard.NotNull(pool, "pool");
		Guard.MustBeGreaterThanOrEqualTo(totalLengthInElements, 0L, "totalLengthInElements");
		Guard.MustBeGreaterThanOrEqualTo(bufferAlignmentInElements, 0, "bufferAlignmentInElements");
		int num = pool.BufferLength / ElementSize;
		if (bufferAlignmentInElements > num)
		{
			memoryGroup = null;
			return false;
		}
		if (totalLengthInElements == 0L)
		{
			throw new InvalidMemoryOperationException("Allocating 0 length buffer from UniformByteArrayPool is disallowed");
		}
		int num2 = num / bufferAlignmentInElements * bufferAlignmentInElements;
		if (totalLengthInElements > 0 && totalLengthInElements < num2)
		{
			num2 = (int)totalLengthInElements;
		}
		int num3 = (int)(totalLengthInElements % num2);
		int num4 = (int)(totalLengthInElements / num2);
		if (num3 == 0)
		{
			num3 = num2;
		}
		else
		{
			num4++;
		}
		UnmanagedMemoryHandle[] array = pool.Rent(num4);
		if (array == null)
		{
			memoryGroup = null;
			return false;
		}
		memoryGroup = new Owned(pool, array, num2, totalLengthInElements, num3, options);
		return true;
	}

	public static MemoryGroup<T> Wrap(params Memory<T>[] source)
	{
		int num = ((source.Length != 0) ? source[0].Length : 0);
		for (int i = 1; i < source.Length - 1; i++)
		{
			if (source[i].Length != num)
			{
				throw new InvalidMemoryOperationException("Wrap: buffers should be uniformly sized!");
			}
		}
		if (source.Length != 0 && source[^1].Length > num)
		{
			throw new InvalidMemoryOperationException("Wrap: the last buffer is too large!");
		}
		long totalLength = ((num > 0) ? ((long)num * (long)(source.Length - 1) + source[^1].Length) : 0);
		return new Consumed(source, num, totalLength);
	}

	public static MemoryGroup<T> Wrap(params IMemoryOwner<T>[] source)
	{
		int num = ((source.Length != 0) ? source[0].Memory.Length : 0);
		for (int i = 1; i < source.Length - 1; i++)
		{
			if (source[i].Memory.Length != num)
			{
				throw new InvalidMemoryOperationException("Wrap: buffers should be uniformly sized!");
			}
		}
		if (source.Length != 0 && source[^1].Memory.Length > num)
		{
			throw new InvalidMemoryOperationException("Wrap: the last buffer is too large!");
		}
		long totalLength = ((num > 0) ? ((long)num * (long)(source.Length - 1) + source[^1].Memory.Length) : 0);
		return new Owned(source, num, totalLength, swappable: false);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe Span<T> GetRowSpanCoreUnsafe(int y, int width)
	{
		switch (memoryGroupSpanCache.Mode)
		{
		case SpanCacheMode.SingleArray:
			return MemoryMarshal.CreateSpan(ref Unsafe.Add(ref Unsafe.As<byte, T>(ref MemoryMarshal.GetReference<byte>(memoryGroupSpanCache.SingleArray)), (uint)(y * width)), width);
		case SpanCacheMode.SinglePointer:
			return new Span<T>(Unsafe.Add<T>(memoryGroupSpanCache.SinglePointer, y * width), width);
		case SpanCacheMode.MultiPointer:
		{
			GetMultiBufferPosition(y, width, out var bufferIdx2, out var bufferStart2);
			return new Span<T>(Unsafe.Add<T>(memoryGroupSpanCache.MultiPointer[bufferIdx2], bufferStart2), width);
		}
		default:
		{
			GetMultiBufferPosition(y, width, out var bufferIdx, out var bufferStart);
			return this[bufferIdx].Span.Slice(bufferStart, width);
		}
		}
	}

	public Span<T> GetRemainingSliceOfBuffer(long start)
	{
		long result;
		long num = Math.DivRem(start, BufferLength, out result);
		Span<T> span = this[(int)num].Span;
		int num2 = (int)result;
		return span.Slice(num2, span.Length - num2);
	}

	public static bool CanSwapContent(MemoryGroup<T> target, MemoryGroup<T> source)
	{
		if (source is Owned { Swappable: not false })
		{
			if (target is Owned owned2)
			{
				return owned2.Swappable;
			}
			return false;
		}
		return false;
	}

	public virtual void RecreateViewAfterSwap()
	{
	}

	public virtual void IncreaseRefCounts()
	{
	}

	public virtual void DecreaseRefCounts()
	{
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void GetMultiBufferPosition(int y, int width, out int bufferIdx, out int bufferStart)
	{
		long result;
		long num = Math.DivRem((long)y * (long)width, BufferLength, out result);
		bufferIdx = (int)num;
		bufferStart = (int)result;
	}
}
