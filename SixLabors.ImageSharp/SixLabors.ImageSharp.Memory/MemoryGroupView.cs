using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp.Memory;

internal class MemoryGroupView<T> : IMemoryGroup<T>, IReadOnlyList<Memory<T>>, IEnumerable<Memory<T>>, IEnumerable, IReadOnlyCollection<Memory<T>> where T : struct
{
	private class MemoryOwnerWrapper : MemoryManager<T>
	{
		private readonly MemoryGroupView<T> view;

		private readonly int index;

		public MemoryOwnerWrapper(MemoryGroupView<T> view, int index)
		{
			this.view = view;
			this.index = index;
		}

		protected override void Dispose(bool disposing)
		{
		}

		public override Span<T> GetSpan()
		{
			view.EnsureIsValid();
			return view.owner[index].Span;
		}

		public override MemoryHandle Pin(int elementIndex = 0)
		{
			view.EnsureIsValid();
			return view.owner[index].Pin();
		}

		public override void Unpin()
		{
			throw new NotSupportedException();
		}
	}

	private MemoryGroup<T>? owner;

	private readonly MemoryOwnerWrapper[] memoryWrappers;

	public int Count
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			EnsureIsValid();
			return owner.Count;
		}
	}

	public int BufferLength
	{
		get
		{
			EnsureIsValid();
			return owner.BufferLength;
		}
	}

	public long TotalLength
	{
		get
		{
			EnsureIsValid();
			return owner.TotalLength;
		}
	}

	[MemberNotNullWhen(true, "owner")]
	public bool IsValid
	{
		[MemberNotNullWhen(true, "owner")]
		get
		{
			return owner != null;
		}
	}

	public Memory<T> this[int index]
	{
		get
		{
			EnsureIsValid();
			return memoryWrappers[index].Memory;
		}
	}

	public MemoryGroupView(MemoryGroup<T> owner)
	{
		this.owner = owner;
		memoryWrappers = new MemoryOwnerWrapper[owner.Count];
		for (int i = 0; i < owner.Count; i++)
		{
			memoryWrappers[i] = new MemoryOwnerWrapper(this, i);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public MemoryGroupEnumerator<T> GetEnumerator()
	{
		return new MemoryGroupEnumerator<T>(this);
	}

	IEnumerator<Memory<T>> IEnumerable<Memory<T>>.GetEnumerator()
	{
		EnsureIsValid();
		for (int i = 0; i < Count; i++)
		{
			yield return memoryWrappers[i].Memory;
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable<Memory<T>>)this).GetEnumerator();
	}

	internal void Invalidate()
	{
		owner = null;
	}

	[MemberNotNull("owner")]
	private void EnsureIsValid()
	{
		if (!IsValid)
		{
			throw new InvalidMemoryOperationException("Can not access an invalidated MemoryGroupView!");
		}
	}
}
