using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp.Memory;

public abstract class MemoryAllocator
{
	private const int OneGigabyte = 1073741824;

	public static MemoryAllocator Default { get; } = Create();

	internal long MemoryGroupAllocationLimitBytes { get; private set; } = Environment.Is64BitProcess ? 4294967296L : 1073741824;

	internal int SingleBufferAllocationLimitBytes { get; private set; } = 1073741824;

	protected internal abstract int GetBufferCapacityInBytes();

	public static MemoryAllocator Create()
	{
		return Create(default(MemoryAllocatorOptions));
	}

	public static MemoryAllocator Create(MemoryAllocatorOptions options)
	{
		UniformUnmanagedMemoryPoolMemoryAllocator uniformUnmanagedMemoryPoolMemoryAllocator = new UniformUnmanagedMemoryPoolMemoryAllocator(options.MaximumPoolSizeMegabytes);
		if (options.AllocationLimitMegabytes.HasValue)
		{
			uniformUnmanagedMemoryPoolMemoryAllocator.MemoryGroupAllocationLimitBytes = options.AllocationLimitMegabytes.Value * 1024 * 1024;
			uniformUnmanagedMemoryPoolMemoryAllocator.SingleBufferAllocationLimitBytes = (int)Math.Min(uniformUnmanagedMemoryPoolMemoryAllocator.SingleBufferAllocationLimitBytes, uniformUnmanagedMemoryPoolMemoryAllocator.MemoryGroupAllocationLimitBytes);
		}
		return uniformUnmanagedMemoryPoolMemoryAllocator;
	}

	public abstract IMemoryOwner<T> Allocate<T>(int length, AllocationOptions options = AllocationOptions.None) where T : struct;

	public virtual void ReleaseRetainedResources()
	{
	}

	internal MemoryGroup<T> AllocateGroup<T>(long totalLength, int bufferAlignment, AllocationOptions options = AllocationOptions.None) where T : struct
	{
		if (totalLength < 0)
		{
			InvalidMemoryOperationException.ThrowNegativeAllocationException(totalLength);
		}
		ulong num = (ulong)(totalLength * Unsafe.SizeOf<T>());
		if (num > (ulong)MemoryGroupAllocationLimitBytes)
		{
			InvalidMemoryOperationException.ThrowAllocationOverLimitException(num, MemoryGroupAllocationLimitBytes);
		}
		return AllocateGroupCore<T>(totalLength, (long)num, bufferAlignment, options);
	}

	internal virtual MemoryGroup<T> AllocateGroupCore<T>(long totalLengthInElements, long totalLengthInBytes, int bufferAlignment, AllocationOptions options) where T : struct
	{
		return MemoryGroup<T>.Allocate(this, totalLengthInElements, bufferAlignment, options);
	}
}
