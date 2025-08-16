using System;
using System.Collections.Generic;

namespace Avalonia.Rendering.Composition.Transport;

internal class BatchStreamData
{
	public Queue<BatchStreamSegment<object?[]>> Objects { get; } = new Queue<BatchStreamSegment<object[]>>();

	public Queue<BatchStreamSegment<IntPtr>> Structs { get; } = new Queue<BatchStreamSegment<IntPtr>>();
}
