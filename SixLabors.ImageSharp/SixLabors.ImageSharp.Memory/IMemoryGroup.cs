using System;
using System.Collections;
using System.Collections.Generic;

namespace SixLabors.ImageSharp.Memory;

public interface IMemoryGroup<T> : IReadOnlyList<Memory<T>>, IEnumerable<Memory<T>>, IEnumerable, IReadOnlyCollection<Memory<T>> where T : struct
{
	int BufferLength { get; }

	long TotalLength { get; }

	bool IsValid { get; }

	new MemoryGroupEnumerator<T> GetEnumerator();
}
