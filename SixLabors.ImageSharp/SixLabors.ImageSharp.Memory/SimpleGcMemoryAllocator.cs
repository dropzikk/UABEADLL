using System.Buffers;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Memory.Internals;

namespace SixLabors.ImageSharp.Memory;

public sealed class SimpleGcMemoryAllocator : MemoryAllocator
{
	protected internal override int GetBufferCapacityInBytes()
	{
		return int.MaxValue;
	}

	public override IMemoryOwner<T> Allocate<T>(int length, AllocationOptions options = AllocationOptions.None)
	{
		if (length < 0)
		{
			InvalidMemoryOperationException.ThrowNegativeAllocationException(length);
		}
		ulong num = (ulong)length * (ulong)Unsafe.SizeOf<T>();
		if (num > (ulong)base.SingleBufferAllocationLimitBytes)
		{
			InvalidMemoryOperationException.ThrowAllocationOverLimitException(num, base.SingleBufferAllocationLimitBytes);
		}
		return new BasicArrayBuffer<T>(new T[length]);
	}
}
