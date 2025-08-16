using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Nerdbank.Streams;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
internal class Sequence<T> : IBufferWriter<T>, IDisposable
{
	private class SequenceSegment : ReadOnlySequenceSegment<T>
	{
		internal static readonly SequenceSegment Empty = new SequenceSegment();

		private static readonly bool MayContainReferences = !typeof(T).GetTypeInfo().IsPrimitive;

		private T[]? array;

		internal int Start { get; private set; }

		internal int End { get; private set; }

		internal Memory<T> RemainingMemory => AvailableMemory.Slice(End);

		internal Span<T> RemainingSpan => AvailableMemory.Span.Slice(End);

		internal IMemoryOwner<T>? MemoryOwner { get; private set; }

		internal Memory<T> AvailableMemory
		{
			get
			{
				T[] array = this.array;
				if (array == null)
				{
					return MemoryOwner?.Memory ?? default(Memory<T>);
				}
				return array;
			}
		}

		internal int Length => End - Start;

		internal int WritableBytes => AvailableMemory.Length - End;

		internal new SequenceSegment? Next
		{
			get
			{
				return (SequenceSegment)base.Next;
			}
			set
			{
				base.Next = value;
			}
		}

		internal bool IsForeignMemory
		{
			get
			{
				if (array == null)
				{
					return MemoryOwner == null;
				}
				return false;
			}
		}

		internal void Assign(IMemoryOwner<T> memoryOwner)
		{
			MemoryOwner = memoryOwner;
			base.Memory = memoryOwner.Memory;
		}

		internal void Assign(T[] array)
		{
			this.array = array;
			base.Memory = array;
		}

		internal void AssignForeign(ReadOnlyMemory<T> memory)
		{
			base.Memory = memory;
			End = memory.Length;
		}

		internal void ResetMemory(ArrayPool<T>? arrayPool)
		{
			ClearReferences(Start, End - Start);
			base.Memory = default(ReadOnlyMemory<T>);
			Next = null;
			base.RunningIndex = 0L;
			Start = 0;
			End = 0;
			if (array != null)
			{
				arrayPool.Return(array);
				array = null;
			}
			else
			{
				MemoryOwner?.Dispose();
				MemoryOwner = null;
			}
		}

		internal void SetNext(SequenceSegment segment)
		{
			Next = segment;
			segment.RunningIndex = base.RunningIndex + Start + Length;
			if (!IsForeignMemory)
			{
				base.Memory = AvailableMemory.Slice(0, Start + Length);
			}
		}

		internal void Advance(int count)
		{
			if (count < 0 || End + count > base.Memory.Length)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			End += count;
		}

		internal void AdvanceTo(int offset)
		{
			ClearReferences(Start, offset - Start);
			Start = offset;
		}

		private void ClearReferences(int startIndex, int length)
		{
			if (MayContainReferences)
			{
				Span<T> span = AvailableMemory.Span;
				span = span.Slice(startIndex, length);
				span.Clear();
			}
		}
	}

	private const int MaximumAutoGrowSize = 32768;

	private static readonly int DefaultLengthFromArrayPool = 1 + 4095 / Unsafe.SizeOf<T>();

	private static readonly ReadOnlySequence<T> Empty = new ReadOnlySequence<T>(SequenceSegment.Empty, 0, SequenceSegment.Empty, 0);

	private readonly Stack<SequenceSegment> segmentPool = new Stack<SequenceSegment>();

	private readonly MemoryPool<T>? memoryPool;

	private readonly ArrayPool<T>? arrayPool;

	private SequenceSegment? first;

	private SequenceSegment? last;

	public int MinimumSpanLength { get; set; }

	public bool AutoIncreaseMinimumSpanLength { get; set; } = true;

	public ReadOnlySequence<T> AsReadOnlySequence => this;

	public long Length => AsReadOnlySequence.Length;

	private string DebuggerDisplay => $"Length: {AsReadOnlySequence.Length}";

	public Sequence()
		: this(ArrayPool<T>.Create())
	{
	}

	public Sequence(MemoryPool<T> memoryPool)
	{
		this.memoryPool = memoryPool ?? throw new ArgumentNullException("memoryPool");
	}

	public Sequence(ArrayPool<T> arrayPool)
	{
		this.arrayPool = arrayPool ?? throw new ArgumentNullException("arrayPool");
	}

	public static implicit operator ReadOnlySequence<T>(Sequence<T> sequence)
	{
		SequenceSegment sequenceSegment = sequence.first;
		if (sequenceSegment != null)
		{
			SequenceSegment sequenceSegment2 = sequence.last;
			if (sequenceSegment2 != null)
			{
				return new ReadOnlySequence<T>(sequenceSegment, sequenceSegment.Start, sequenceSegment2, sequenceSegment2.End);
			}
		}
		return Empty;
	}

	public void AdvanceTo(SequencePosition position)
	{
		SequenceSegment sequenceSegment = (SequenceSegment)position.GetObject();
		if (sequenceSegment != null && (sequenceSegment != SequenceSegment.Empty || Length != 0L))
		{
			int integer = position.GetInteger();
			SequenceSegment next = first;
			while (next != sequenceSegment && next != null)
			{
				next = next.Next;
			}
			if (next == null)
			{
				throw new ArgumentException("Position does not represent a valid position in this sequence.", "position");
			}
			if (integer < next.Start)
			{
				throw new ArgumentException("Position must not be earlier than current position.", "position");
			}
			for (next = first; next != sequenceSegment; next = RecycleAndGetNext(next))
			{
			}
			sequenceSegment.AdvanceTo(integer);
			first = ((sequenceSegment.Length == 0) ? RecycleAndGetNext(sequenceSegment) : sequenceSegment);
			if (first == null)
			{
				last = null;
			}
		}
	}

	public void Advance(int count)
	{
		(last ?? throw new InvalidOperationException("Cannot advance before acquiring memory.")).Advance(count);
		ConsiderMinimumSizeIncrease();
	}

	public Memory<T> GetMemory(int sizeHint)
	{
		return GetSegment(sizeHint).RemainingMemory;
	}

	public Span<T> GetSpan(int sizeHint)
	{
		return GetSegment(sizeHint).RemainingSpan;
	}

	public void Append(ReadOnlyMemory<T> memory)
	{
		if (memory.Length > 0)
		{
			SequenceSegment sequenceSegment = ((segmentPool.Count > 0) ? segmentPool.Pop() : new SequenceSegment());
			sequenceSegment.AssignForeign(memory);
			Append(sequenceSegment);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public void Dispose()
	{
		Reset();
	}

	public void Reset()
	{
		for (SequenceSegment sequenceSegment = first; sequenceSegment != null; sequenceSegment = RecycleAndGetNext(sequenceSegment))
		{
		}
		first = (last = null);
	}

	private SequenceSegment GetSegment(int sizeHint)
	{
		if (sizeHint < 0)
		{
			throw new ArgumentOutOfRangeException("sizeHint");
		}
		int? num = null;
		if (sizeHint == 0)
		{
			if (last == null || last.WritableBytes == 0)
			{
				num = -1;
			}
		}
		else if (last == null || last.WritableBytes < sizeHint)
		{
			num = Math.Max(MinimumSpanLength, sizeHint);
		}
		if (num.HasValue)
		{
			SequenceSegment sequenceSegment = ((segmentPool.Count > 0) ? segmentPool.Pop() : new SequenceSegment());
			if (arrayPool != null)
			{
				sequenceSegment.Assign(arrayPool.Rent((num.Value == -1) ? DefaultLengthFromArrayPool : num.Value));
			}
			else
			{
				sequenceSegment.Assign(memoryPool.Rent(num.Value));
			}
			Append(sequenceSegment);
		}
		return last;
	}

	private void Append(SequenceSegment segment)
	{
		if (last == null)
		{
			first = (last = segment);
			return;
		}
		if (last.Length > 0)
		{
			last.SetNext(segment);
		}
		else
		{
			SequenceSegment next = first;
			if (first != last)
			{
				while (next.Next != last)
				{
					next = next.Next;
				}
			}
			else
			{
				first = segment;
			}
			next.SetNext(segment);
			RecycleAndGetNext(last);
		}
		last = segment;
	}

	private SequenceSegment? RecycleAndGetNext(SequenceSegment segment)
	{
		SequenceSegment? next = segment.Next;
		segment.ResetMemory(arrayPool);
		segmentPool.Push(segment);
		return next;
	}

	private void ConsiderMinimumSizeIncrease()
	{
		if (AutoIncreaseMinimumSpanLength && MinimumSpanLength < 32768)
		{
			int num = Math.Min(32768, (int)Math.Min(2147483647L, Length / 2));
			if (MinimumSpanLength < num)
			{
				MinimumSpanLength = num;
			}
		}
	}
}
