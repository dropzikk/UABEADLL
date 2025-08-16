using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp.Memory;

[EditorBrowsable(EditorBrowsableState.Never)]
public ref struct MemoryGroupEnumerator<T> where T : struct
{
	private readonly IMemoryGroup<T> memoryGroup;

	private readonly int count;

	private int index;

	public Memory<T> Current
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return memoryGroup[index];
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal MemoryGroupEnumerator(MemoryGroup<T>.Owned memoryGroup)
	{
		this.memoryGroup = memoryGroup;
		count = memoryGroup.Count;
		index = -1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal MemoryGroupEnumerator(MemoryGroup<T>.Consumed memoryGroup)
	{
		this.memoryGroup = memoryGroup;
		count = memoryGroup.Count;
		index = -1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal MemoryGroupEnumerator(MemoryGroupView<T> memoryGroup)
	{
		this.memoryGroup = memoryGroup;
		count = memoryGroup.Count;
		index = -1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool MoveNext()
	{
		int num = index + 1;
		if (num < count)
		{
			index = num;
			return true;
		}
		return false;
	}
}
