using System;

namespace SixLabors.ImageSharp.Memory;

internal delegate void TransformItemsDelegate<TSource, TTarget>(ReadOnlySpan<TSource> source, Span<TTarget> target);
