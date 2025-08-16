using System;

namespace SixLabors.ImageSharp.Memory;

internal delegate void TransformItemsInplaceDelegate<T>(Span<T> data);
