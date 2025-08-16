using System;

namespace SixLabors.ImageSharp.Memory;

internal static class MemoryGroupExtensions
{
	private struct MemoryGroupCursor<T> where T : struct
	{
		private readonly IMemoryGroup<T> memoryGroup;

		private int bufferIndex;

		private int elementIndex;

		private bool IsAtLastBuffer => bufferIndex == memoryGroup.Count - 1;

		private int CurrentBufferLength => memoryGroup[bufferIndex].Length;

		public MemoryGroupCursor(IMemoryGroup<T> memoryGroup)
		{
			this.memoryGroup = memoryGroup;
			bufferIndex = 0;
			elementIndex = 0;
		}

		public Span<T> GetSpan(int length)
		{
			return memoryGroup[bufferIndex].Span.Slice(elementIndex, length);
		}

		public int LookAhead()
		{
			return CurrentBufferLength - elementIndex;
		}

		public void Forward(int steps)
		{
			int num = elementIndex + steps;
			int currentBufferLength = CurrentBufferLength;
			if (num < currentBufferLength)
			{
				elementIndex = num;
				return;
			}
			if (num == currentBufferLength)
			{
				bufferIndex++;
				elementIndex = 0;
				return;
			}
			throw new ArgumentException("Can't forward multiple buffers!", "steps");
		}
	}

	internal static void Fill<T>(this IMemoryGroup<T> group, T value) where T : struct
	{
		MemoryGroupEnumerator<T> enumerator = group.GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator.Current.Span.Fill(value);
		}
	}

	internal static void Clear<T>(this IMemoryGroup<T> group) where T : struct
	{
		MemoryGroupEnumerator<T> enumerator = group.GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator.Current.Span.Clear();
		}
	}

	internal static Memory<T> GetBoundedMemorySlice<T>(this IMemoryGroup<T> group, long start, int length) where T : struct
	{
		Guard.NotNull(group, "group");
		Guard.IsTrue(group.IsValid, "group", "Group must be valid!");
		Guard.MustBeGreaterThanOrEqualTo(length, 0, "length");
		Guard.MustBeLessThan(start, group.TotalLength, "start");
		long result;
		int num = (int)Math.DivRem(start, group.BufferLength, out result);
		int num2 = (int)result;
		if ((uint)num >= group.Count)
		{
			throw new ArgumentOutOfRangeException("start");
		}
		int num3 = num2 + length;
		Memory<T> memory = group[num];
		if (num3 > memory.Length)
		{
			throw new ArgumentOutOfRangeException("length");
		}
		return memory.Slice(num2, length);
	}

	internal static void CopyTo<T>(this IMemoryGroup<T> source, Span<T> target) where T : struct
	{
		Guard.NotNull(source, "source");
		Guard.MustBeGreaterThanOrEqualTo(target.Length, source.TotalLength, "target");
		MemoryGroupCursor<T> memoryGroupCursor = new MemoryGroupCursor<T>(source);
		int num2;
		for (long num = 0L; num < source.TotalLength; num += num2)
		{
			num2 = Math.Min(memoryGroupCursor.LookAhead(), target.Length);
			memoryGroupCursor.GetSpan(num2).CopyTo(target);
			memoryGroupCursor.Forward(num2);
			int num3 = num2;
			target = target.Slice(num3, target.Length - num3);
		}
	}

	internal static void CopyTo<T>(this Span<T> source, IMemoryGroup<T> target) where T : struct
	{
		((ReadOnlySpan<T>)source).CopyTo(target);
	}

	internal static void CopyTo<T>(this ReadOnlySpan<T> source, IMemoryGroup<T> target) where T : struct
	{
		Guard.NotNull(target, "target");
		Guard.MustBeGreaterThanOrEqualTo(target.TotalLength, source.Length, "target");
		MemoryGroupCursor<T> memoryGroupCursor = new MemoryGroupCursor<T>(target);
		while (!source.IsEmpty)
		{
			int num = Math.Min(memoryGroupCursor.LookAhead(), source.Length);
			source.Slice(0, num).CopyTo(memoryGroupCursor.GetSpan(num));
			memoryGroupCursor.Forward(num);
			int num2 = num;
			source = source.Slice(num2, source.Length - num2);
		}
	}

	internal static void CopyTo<T>(this IMemoryGroup<T>? source, IMemoryGroup<T>? target) where T : struct
	{
		Guard.NotNull(source, "source");
		Guard.NotNull(target, "target");
		Guard.IsTrue(source.IsValid, "source", "Source group must be valid.");
		Guard.IsTrue(target.IsValid, "target", "Target group must be valid.");
		Guard.MustBeLessThanOrEqualTo(source.TotalLength, target.TotalLength, "Destination buffer too short!");
		if (!source.IsEmpty())
		{
			long num = 0L;
			MemoryGroupCursor<T> memoryGroupCursor = new MemoryGroupCursor<T>(source);
			MemoryGroupCursor<T> memoryGroupCursor2 = new MemoryGroupCursor<T>(target);
			int num2;
			for (; num < source.TotalLength; num += num2)
			{
				num2 = Math.Min(memoryGroupCursor.LookAhead(), memoryGroupCursor2.LookAhead());
				Span<T> span = memoryGroupCursor.GetSpan(num2);
				Span<T> span2 = memoryGroupCursor2.GetSpan(num2);
				span.CopyTo(span2);
				memoryGroupCursor.Forward(num2);
				memoryGroupCursor2.Forward(num2);
			}
		}
	}

	internal static void TransformTo<TSource, TTarget>(this IMemoryGroup<TSource> source, IMemoryGroup<TTarget> target, TransformItemsDelegate<TSource, TTarget> transform) where TSource : struct where TTarget : struct
	{
		Guard.NotNull(source, "source");
		Guard.NotNull(target, "target");
		Guard.NotNull(transform, "transform");
		Guard.IsTrue(source.IsValid, "source", "Source group must be valid.");
		Guard.IsTrue(target.IsValid, "target", "Target group must be valid.");
		Guard.MustBeLessThanOrEqualTo(source.TotalLength, target.TotalLength, "Destination buffer too short!");
		if (!source.IsEmpty())
		{
			long num = 0L;
			MemoryGroupCursor<TSource> memoryGroupCursor = new MemoryGroupCursor<TSource>(source);
			MemoryGroupCursor<TTarget> memoryGroupCursor2 = new MemoryGroupCursor<TTarget>(target);
			int num2;
			for (; num < source.TotalLength; num += num2)
			{
				num2 = Math.Min(memoryGroupCursor.LookAhead(), memoryGroupCursor2.LookAhead());
				Span<TSource> span = memoryGroupCursor.GetSpan(num2);
				Span<TTarget> span2 = memoryGroupCursor2.GetSpan(num2);
				transform(span, span2);
				memoryGroupCursor.Forward(num2);
				memoryGroupCursor2.Forward(num2);
			}
		}
	}

	internal static void TransformInplace<T>(this IMemoryGroup<T> memoryGroup, TransformItemsInplaceDelegate<T> transform) where T : struct
	{
		MemoryGroupEnumerator<T> enumerator = memoryGroup.GetEnumerator();
		while (enumerator.MoveNext())
		{
			transform(enumerator.Current.Span);
		}
	}

	internal static bool IsEmpty<T>(this IMemoryGroup<T> group) where T : struct
	{
		return group.Count == 0;
	}
}
