using System;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Advanced;

public readonly struct ParallelExecutionSettings
{
	public const int DefaultMinimumPixelsProcessedPerTask = 4096;

	public MemoryAllocator MemoryAllocator { get; }

	public int MaxDegreeOfParallelism { get; }

	public int MinimumPixelsProcessedPerTask { get; }

	public ParallelExecutionSettings(int maxDegreeOfParallelism, int minimumPixelsProcessedPerTask, MemoryAllocator memoryAllocator)
	{
		if (maxDegreeOfParallelism == 0 || maxDegreeOfParallelism < -1)
		{
			throw new ArgumentOutOfRangeException("maxDegreeOfParallelism");
		}
		Guard.MustBeGreaterThan(minimumPixelsProcessedPerTask, 0, "minimumPixelsProcessedPerTask");
		Guard.NotNull(memoryAllocator, "memoryAllocator");
		MaxDegreeOfParallelism = maxDegreeOfParallelism;
		MinimumPixelsProcessedPerTask = minimumPixelsProcessedPerTask;
		MemoryAllocator = memoryAllocator;
	}

	public ParallelExecutionSettings(int maxDegreeOfParallelism, MemoryAllocator memoryAllocator)
		: this(maxDegreeOfParallelism, 4096, memoryAllocator)
	{
	}

	public ParallelExecutionSettings MultiplyMinimumPixelsPerTask(int multiplier)
	{
		Guard.MustBeGreaterThan(multiplier, 0, "multiplier");
		return new ParallelExecutionSettings(MaxDegreeOfParallelism, MinimumPixelsProcessedPerTask * multiplier, MemoryAllocator);
	}

	public static ParallelExecutionSettings FromConfiguration(Configuration configuration)
	{
		return new ParallelExecutionSettings(configuration.MaxDegreeOfParallelism, configuration.MemoryAllocator);
	}
}
