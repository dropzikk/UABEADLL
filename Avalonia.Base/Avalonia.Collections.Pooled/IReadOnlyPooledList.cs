using System;
using System.Collections;
using System.Collections.Generic;

namespace Avalonia.Collections.Pooled;

internal interface IReadOnlyPooledList<T> : IReadOnlyList<T>, IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>
{
	ReadOnlySpan<T> Span { get; }
}
