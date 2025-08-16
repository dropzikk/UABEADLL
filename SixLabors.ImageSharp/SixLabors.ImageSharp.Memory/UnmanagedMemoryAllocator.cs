using System.Buffers;
using SixLabors.ImageSharp.Memory.Internals;

namespace SixLabors.ImageSharp.Memory;

internal class UnmanagedMemoryAllocator : MemoryAllocator
{
	private readonly int bufferCapacityInBytes;

	public UnmanagedMemoryAllocator(int bufferCapacityInBytes)
	{
		this.bufferCapacityInBytes = bufferCapacityInBytes;
	}

	protected internal override int GetBufferCapacityInBytes()
	{
		return bufferCapacityInBytes;
	}

	public override IMemoryOwner<T> Allocate<T>(int length, AllocationOptions options = AllocationOptions.None)
	{
		UnmanagedBuffer<T> unmanagedBuffer = UnmanagedBuffer<T>.Allocate(length);
		if (options.Has(AllocationOptions.Clean))
		{
			unmanagedBuffer.GetSpan().Clear();
		}
		return unmanagedBuffer;
	}
}
