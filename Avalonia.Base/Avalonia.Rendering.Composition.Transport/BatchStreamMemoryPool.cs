using System;
using System.Runtime.InteropServices;

namespace Avalonia.Rendering.Composition.Transport;

internal sealed class BatchStreamMemoryPool : BatchStreamPoolBase<IntPtr>
{
	public int BufferSize { get; }

	public BatchStreamMemoryPool(bool reclaimImmediately, int bufferSize = 1024, Action<Func<bool>>? startTimer = null)
		: base(needsFinalize: true, reclaimImmediately, startTimer)
	{
		BufferSize = bufferSize;
	}

	protected override IntPtr CreateItem()
	{
		return Marshal.AllocHGlobal(BufferSize);
	}

	protected override void DestroyItem(IntPtr item)
	{
		Marshal.FreeHGlobal(item);
	}
}
