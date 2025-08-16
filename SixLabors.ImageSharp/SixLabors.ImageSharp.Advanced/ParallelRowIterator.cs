using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Advanced;

public static class ParallelRowIterator
{
	private readonly struct RowOperationWrapper<T> where T : struct, IRowOperation
	{
		private readonly int minY;

		private readonly int maxY;

		private readonly int stepY;

		private readonly T action;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RowOperationWrapper(int minY, int maxY, int stepY, in T action)
		{
			this.minY = minY;
			this.maxY = maxY;
			this.stepY = stepY;
			this.action = action;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Invoke(int i)
		{
			int num = minY + i * stepY;
			if (num < maxY)
			{
				int num2 = Math.Min(num + stepY, maxY);
				for (int j = num; j < num2; j++)
				{
					Unsafe.AsRef(in action).Invoke(j);
				}
			}
		}
	}

	private readonly struct RowOperationWrapper<T, TBuffer> where T : struct, IRowOperation<TBuffer> where TBuffer : unmanaged
	{
		private readonly int minY;

		private readonly int maxY;

		private readonly int stepY;

		private readonly int bufferLength;

		private readonly MemoryAllocator allocator;

		private readonly T action;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RowOperationWrapper(int minY, int maxY, int stepY, int bufferLength, MemoryAllocator allocator, in T action)
		{
			this.minY = minY;
			this.maxY = maxY;
			this.stepY = stepY;
			this.bufferLength = bufferLength;
			this.allocator = allocator;
			this.action = action;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Invoke(int i)
		{
			int num = minY + i * stepY;
			if (num >= maxY)
			{
				return;
			}
			int num2 = Math.Min(num + stepY, maxY);
			using IMemoryOwner<TBuffer> memoryOwner = allocator.Allocate<TBuffer>(bufferLength);
			Span<TBuffer> span = memoryOwner.Memory.Span;
			for (int j = num; j < num2; j++)
			{
				Unsafe.AsRef(in action).Invoke(j, span);
			}
		}
	}

	private readonly struct RowIntervalOperationWrapper<T> where T : struct, IRowIntervalOperation
	{
		private readonly int minY;

		private readonly int maxY;

		private readonly int stepY;

		private readonly T operation;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RowIntervalOperationWrapper(int minY, int maxY, int stepY, in T operation)
		{
			this.minY = minY;
			this.maxY = maxY;
			this.stepY = stepY;
			this.operation = operation;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Invoke(int i)
		{
			int num = minY + i * stepY;
			if (num < maxY)
			{
				int max = Math.Min(num + stepY, maxY);
				RowInterval rows = new RowInterval(num, max);
				Unsafe.AsRef(in operation).Invoke(in rows);
			}
		}
	}

	private readonly struct RowIntervalOperationWrapper<T, TBuffer> where T : struct, IRowIntervalOperation<TBuffer> where TBuffer : unmanaged
	{
		private readonly int minY;

		private readonly int maxY;

		private readonly int stepY;

		private readonly int bufferLength;

		private readonly MemoryAllocator allocator;

		private readonly T operation;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RowIntervalOperationWrapper(int minY, int maxY, int stepY, int bufferLength, MemoryAllocator allocator, in T operation)
		{
			this.minY = minY;
			this.maxY = maxY;
			this.stepY = stepY;
			this.bufferLength = bufferLength;
			this.allocator = allocator;
			this.operation = operation;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Invoke(int i)
		{
			int num = minY + i * stepY;
			if (num >= maxY)
			{
				return;
			}
			int max = Math.Min(num + stepY, maxY);
			RowInterval rows = new RowInterval(num, max);
			using IMemoryOwner<TBuffer> memoryOwner = allocator.Allocate<TBuffer>(bufferLength);
			Unsafe.AsRef(in operation).Invoke(in rows, memoryOwner.Memory.Span);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void IterateRows<T>(Configuration configuration, Rectangle rectangle, in T operation) where T : struct, IRowOperation
	{
		IterateRows(rectangle, ParallelExecutionSettings.FromConfiguration(configuration), in operation);
	}

	public static void IterateRows<T>(Rectangle rectangle, in ParallelExecutionSettings parallelSettings, in T operation) where T : struct, IRowOperation
	{
		ValidateRectangle(rectangle);
		int top = rectangle.Top;
		int bottom = rectangle.Bottom;
		int width = rectangle.Width;
		int height = rectangle.Height;
		int val = DivideCeil((long)width * (long)height, parallelSettings.MinimumPixelsProcessedPerTask);
		int num = Math.Min(parallelSettings.MaxDegreeOfParallelism, val);
		if (num == 1)
		{
			for (int i = top; i < bottom; i++)
			{
				Unsafe.AsRef(in operation).Invoke(i);
			}
		}
		else
		{
			int stepY = DivideCeil(rectangle.Height, num);
			ParallelOptions parallelOptions = new ParallelOptions
			{
				MaxDegreeOfParallelism = num
			};
			RowOperationWrapper<T> rowOperationWrapper = new RowOperationWrapper<T>(top, bottom, stepY, in operation);
			Parallel.For(0, num, parallelOptions, ((RowOperationWrapper<T>)rowOperationWrapper).Invoke);
		}
	}

	public static void IterateRows<T, TBuffer>(Configuration configuration, Rectangle rectangle, in T operation) where T : struct, IRowOperation<TBuffer> where TBuffer : unmanaged
	{
		IterateRows<T, TBuffer>(rectangle, ParallelExecutionSettings.FromConfiguration(configuration), in operation);
	}

	public static void IterateRows<T, TBuffer>(Rectangle rectangle, in ParallelExecutionSettings parallelSettings, in T operation) where T : struct, IRowOperation<TBuffer> where TBuffer : unmanaged
	{
		ValidateRectangle(rectangle);
		int top = rectangle.Top;
		int bottom = rectangle.Bottom;
		int width = rectangle.Width;
		int height = rectangle.Height;
		int val = DivideCeil((long)width * (long)height, parallelSettings.MinimumPixelsProcessedPerTask);
		int num = Math.Min(parallelSettings.MaxDegreeOfParallelism, val);
		MemoryAllocator memoryAllocator = parallelSettings.MemoryAllocator;
		int requiredBufferLength = Unsafe.AsRef(in operation).GetRequiredBufferLength(rectangle);
		if (num == 1)
		{
			using (IMemoryOwner<TBuffer> memoryOwner = memoryAllocator.Allocate<TBuffer>(requiredBufferLength))
			{
				Span<TBuffer> span = memoryOwner.Memory.Span;
				for (int i = top; i < bottom; i++)
				{
					Unsafe.AsRef(in operation).Invoke(i, span);
				}
				return;
			}
		}
		int stepY = DivideCeil(height, num);
		ParallelOptions parallelOptions = new ParallelOptions
		{
			MaxDegreeOfParallelism = num
		};
		RowOperationWrapper<T, TBuffer> rowOperationWrapper = new RowOperationWrapper<T, TBuffer>(top, bottom, stepY, requiredBufferLength, memoryAllocator, in operation);
		Parallel.For(0, num, parallelOptions, ((RowOperationWrapper<T, TBuffer>)rowOperationWrapper).Invoke);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void IterateRowIntervals<T>(Configuration configuration, Rectangle rectangle, in T operation) where T : struct, IRowIntervalOperation
	{
		IterateRowIntervals(rectangle, ParallelExecutionSettings.FromConfiguration(configuration), in operation);
	}

	public static void IterateRowIntervals<T>(Rectangle rectangle, in ParallelExecutionSettings parallelSettings, in T operation) where T : struct, IRowIntervalOperation
	{
		ValidateRectangle(rectangle);
		int top = rectangle.Top;
		int bottom = rectangle.Bottom;
		int width = rectangle.Width;
		int height = rectangle.Height;
		int val = DivideCeil((long)width * (long)height, parallelSettings.MinimumPixelsProcessedPerTask);
		int num = Math.Min(parallelSettings.MaxDegreeOfParallelism, val);
		if (num == 1)
		{
			RowInterval rows = new RowInterval(top, bottom);
			Unsafe.AsRef(in operation).Invoke(in rows);
			return;
		}
		int stepY = DivideCeil(rectangle.Height, num);
		ParallelOptions parallelOptions = new ParallelOptions
		{
			MaxDegreeOfParallelism = num
		};
		RowIntervalOperationWrapper<T> rowIntervalOperationWrapper = new RowIntervalOperationWrapper<T>(top, bottom, stepY, in operation);
		Parallel.For(0, num, parallelOptions, ((RowIntervalOperationWrapper<T>)rowIntervalOperationWrapper).Invoke);
	}

	public static void IterateRowIntervals<T, TBuffer>(Configuration configuration, Rectangle rectangle, in T operation) where T : struct, IRowIntervalOperation<TBuffer> where TBuffer : unmanaged
	{
		IterateRowIntervals<T, TBuffer>(rectangle, ParallelExecutionSettings.FromConfiguration(configuration), in operation);
	}

	public static void IterateRowIntervals<T, TBuffer>(Rectangle rectangle, in ParallelExecutionSettings parallelSettings, in T operation) where T : struct, IRowIntervalOperation<TBuffer> where TBuffer : unmanaged
	{
		ValidateRectangle(rectangle);
		int top = rectangle.Top;
		int bottom = rectangle.Bottom;
		int width = rectangle.Width;
		int height = rectangle.Height;
		int val = DivideCeil((long)width * (long)height, parallelSettings.MinimumPixelsProcessedPerTask);
		int num = Math.Min(parallelSettings.MaxDegreeOfParallelism, val);
		MemoryAllocator memoryAllocator = parallelSettings.MemoryAllocator;
		int requiredBufferLength = Unsafe.AsRef(in operation).GetRequiredBufferLength(rectangle);
		if (num == 1)
		{
			RowInterval rows = new RowInterval(top, bottom);
			using IMemoryOwner<TBuffer> memoryOwner = memoryAllocator.Allocate<TBuffer>(requiredBufferLength);
			Unsafe.AsRef(in operation).Invoke(in rows, memoryOwner.Memory.Span);
			return;
		}
		int stepY = DivideCeil(height, num);
		ParallelOptions parallelOptions = new ParallelOptions
		{
			MaxDegreeOfParallelism = num
		};
		RowIntervalOperationWrapper<T, TBuffer> rowIntervalOperationWrapper = new RowIntervalOperationWrapper<T, TBuffer>(top, bottom, stepY, requiredBufferLength, memoryAllocator, in operation);
		Parallel.For(0, num, parallelOptions, ((RowIntervalOperationWrapper<T, TBuffer>)rowIntervalOperationWrapper).Invoke);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int DivideCeil(long dividend, int divisor)
	{
		return (int)Math.Min(1 + (dividend - 1) / divisor, 2147483647L);
	}

	private static void ValidateRectangle(Rectangle rectangle)
	{
		Guard.MustBeGreaterThan(rectangle.Width, 0, "rectangle.Width");
		Guard.MustBeGreaterThan(rectangle.Height, 0, "rectangle.Height");
	}
}
