using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Memory.Internals;

namespace SixLabors.ImageSharp.Memory;

internal sealed class UniformUnmanagedMemoryPoolMemoryAllocator : MemoryAllocator
{
	private const int OneMegabyte = 1048576;

	private const int DefaultContiguousPoolBlockSizeBytes = 4194304;

	private const int DefaultNonPoolBlockSizeBytes = 33554432;

	private readonly int sharedArrayPoolThresholdInBytes;

	private readonly int poolBufferSizeInBytes;

	private readonly int poolCapacity;

	private readonly UniformUnmanagedMemoryPool.TrimSettings trimSettings;

	private readonly UniformUnmanagedMemoryPool pool;

	private readonly UnmanagedMemoryAllocator nonPoolAllocator;

	internal static Func<long> GetTotalAvailableMemoryBytes { get; set; } = () => GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;

	public UniformUnmanagedMemoryPoolMemoryAllocator(int? maxPoolSizeMegabytes)
		: this(4194304, maxPoolSizeMegabytes.HasValue ? ((long)maxPoolSizeMegabytes.Value * 1048576L) : GetDefaultMaxPoolSizeBytes(), 33554432)
	{
	}

	public UniformUnmanagedMemoryPoolMemoryAllocator(int poolBufferSizeInBytes, long maxPoolSizeInBytes, int unmanagedBufferSizeInBytes)
		: this(1048576, poolBufferSizeInBytes, maxPoolSizeInBytes, unmanagedBufferSizeInBytes)
	{
	}

	internal UniformUnmanagedMemoryPoolMemoryAllocator(int sharedArrayPoolThresholdInBytes, int poolBufferSizeInBytes, long maxPoolSizeInBytes, int unmanagedBufferSizeInBytes)
		: this(sharedArrayPoolThresholdInBytes, poolBufferSizeInBytes, maxPoolSizeInBytes, unmanagedBufferSizeInBytes, UniformUnmanagedMemoryPool.TrimSettings.Default)
	{
	}

	internal UniformUnmanagedMemoryPoolMemoryAllocator(int sharedArrayPoolThresholdInBytes, int poolBufferSizeInBytes, long maxPoolSizeInBytes, int unmanagedBufferSizeInBytes, UniformUnmanagedMemoryPool.TrimSettings trimSettings)
	{
		this.sharedArrayPoolThresholdInBytes = sharedArrayPoolThresholdInBytes;
		this.poolBufferSizeInBytes = poolBufferSizeInBytes;
		poolCapacity = (int)(maxPoolSizeInBytes / poolBufferSizeInBytes);
		this.trimSettings = trimSettings;
		pool = new UniformUnmanagedMemoryPool(this.poolBufferSizeInBytes, poolCapacity, this.trimSettings);
		nonPoolAllocator = new UnmanagedMemoryAllocator(unmanagedBufferSizeInBytes);
	}

	protected internal override int GetBufferCapacityInBytes()
	{
		return poolBufferSizeInBytes;
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
		if (num <= (ulong)sharedArrayPoolThresholdInBytes)
		{
			SharedArrayPoolBuffer<T> sharedArrayPoolBuffer = new SharedArrayPoolBuffer<T>(length);
			if (options.Has(AllocationOptions.Clean))
			{
				sharedArrayPoolBuffer.GetSpan().Clear();
			}
			return sharedArrayPoolBuffer;
		}
		if (num <= (ulong)poolBufferSizeInBytes)
		{
			UnmanagedMemoryHandle handle = pool.Rent();
			if (handle.IsValid)
			{
				return pool.CreateGuardedBuffer<T>(handle, length, options.Has(AllocationOptions.Clean));
			}
		}
		return nonPoolAllocator.Allocate<T>(length, options);
	}

	internal override MemoryGroup<T> AllocateGroupCore<T>(long totalLengthInElements, long totalLengthInBytes, int bufferAlignment, AllocationOptions options = AllocationOptions.None)
	{
		if (totalLengthInBytes <= sharedArrayPoolThresholdInBytes)
		{
			return MemoryGroup<T>.CreateContiguous(new SharedArrayPoolBuffer<T>((int)totalLengthInElements), options.Has(AllocationOptions.Clean));
		}
		if (totalLengthInBytes <= poolBufferSizeInBytes)
		{
			UnmanagedMemoryHandle handle = pool.Rent();
			if (handle.IsValid)
			{
				return MemoryGroup<T>.CreateContiguous(pool.CreateGuardedBuffer<T>(handle, (int)totalLengthInElements, options.Has(AllocationOptions.Clean)), options.Has(AllocationOptions.Clean));
			}
		}
		if (MemoryGroup<T>.TryAllocate(pool, totalLengthInElements, bufferAlignment, options, out MemoryGroup<T> memoryGroup))
		{
			return memoryGroup;
		}
		return MemoryGroup<T>.Allocate(nonPoolAllocator, totalLengthInElements, bufferAlignment, options);
	}

	public override void ReleaseRetainedResources()
	{
		pool.Release();
	}

	private static long GetDefaultMaxPoolSizeBytes()
	{
		if (Environment.Is64BitProcess)
		{
			long num = GetTotalAvailableMemoryBytes();
			if (num > 0)
			{
				return (long)((ulong)num / 8uL);
			}
		}
		return 134217728L;
	}
}
