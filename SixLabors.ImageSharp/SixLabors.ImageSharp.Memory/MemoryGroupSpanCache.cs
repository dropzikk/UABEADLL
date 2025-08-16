using System.Buffers;
using SixLabors.ImageSharp.Memory.Internals;

namespace SixLabors.ImageSharp.Memory;

internal struct MemoryGroupSpanCache
{
	public SpanCacheMode Mode;

	public byte[]? SingleArray;

	public unsafe void* SinglePointer;

	public unsafe void*[] MultiPointer;

	public unsafe static MemoryGroupSpanCache Create<T>(IMemoryOwner<T>[] memoryOwners) where T : struct
	{
		IMemoryOwner<T> memoryOwner = memoryOwners[0];
		MemoryGroupSpanCache result = default(MemoryGroupSpanCache);
		if (memoryOwners.Length == 1)
		{
			if (memoryOwner is SharedArrayPoolBuffer<T> sharedArrayPoolBuffer)
			{
				result.Mode = SpanCacheMode.SingleArray;
				result.SingleArray = sharedArrayPoolBuffer.Array;
			}
			else if (memoryOwner is UnmanagedBuffer<T> unmanagedBuffer)
			{
				result.Mode = SpanCacheMode.SinglePointer;
				result.SinglePointer = unmanagedBuffer.Pointer;
			}
		}
		else if (memoryOwner is UnmanagedBuffer<T>)
		{
			result.Mode = SpanCacheMode.MultiPointer;
			result.MultiPointer = new void*[memoryOwners.Length];
			for (int i = 0; i < memoryOwners.Length; i++)
			{
				result.MultiPointer[i] = ((UnmanagedBuffer<T>)memoryOwners[i]).Pointer;
			}
		}
		return result;
	}
}
