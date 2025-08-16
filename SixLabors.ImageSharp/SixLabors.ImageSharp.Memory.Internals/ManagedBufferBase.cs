using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SixLabors.ImageSharp.Memory.Internals;

internal abstract class ManagedBufferBase<T> : MemoryManager<T> where T : struct
{
	private GCHandle pinHandle;

	public unsafe override MemoryHandle Pin(int elementIndex = 0)
	{
		if (!pinHandle.IsAllocated)
		{
			pinHandle = GCHandle.Alloc(GetPinnableObject(), GCHandleType.Pinned);
		}
		return new MemoryHandle(Unsafe.Add<T>((void*)pinHandle.AddrOfPinnedObject(), elementIndex), default(GCHandle), this);
	}

	public override void Unpin()
	{
		if (pinHandle.IsAllocated)
		{
			pinHandle.Free();
		}
	}

	protected abstract object GetPinnableObject();
}
