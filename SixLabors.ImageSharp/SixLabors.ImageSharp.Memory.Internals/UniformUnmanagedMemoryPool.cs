using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Threading;

namespace SixLabors.ImageSharp.Memory.Internals;

internal class UniformUnmanagedMemoryPool : CriticalFinalizerObject
{
	public class TrimSettings
	{
		public int TrimPeriodMilliseconds { get; set; } = 60000;

		public float Rate { get; set; } = 0.5f;

		public unsafe float HighPressureThresholdRate { get; set; } = (sizeof(IntPtr) == 8) ? 0.9f : 0.6f;

		public bool Enabled => Rate > 0f;

		public static TrimSettings Default => new TrimSettings();
	}

	private sealed class GroupLifetimeGuard : RefCountedMemoryLifetimeGuard
	{
		private readonly UniformUnmanagedMemoryPool pool;

		private readonly UnmanagedMemoryHandle[] handles;

		public GroupLifetimeGuard(UniformUnmanagedMemoryPool pool, UnmanagedMemoryHandle[] handles)
		{
			this.pool = pool;
			this.handles = handles;
		}

		protected override void Release()
		{
			if (!pool.Return(handles))
			{
				UnmanagedMemoryHandle[] array = handles;
				foreach (UnmanagedMemoryHandle unmanagedMemoryHandle in array)
				{
					unmanagedMemoryHandle.Free();
				}
			}
		}
	}

	private sealed class ReturnToPoolBufferLifetimeGuard : UnmanagedBufferLifetimeGuard
	{
		private readonly UniformUnmanagedMemoryPool pool;

		public ReturnToPoolBufferLifetimeGuard(UniformUnmanagedMemoryPool pool, UnmanagedMemoryHandle handle)
			: base(handle)
		{
			this.pool = pool;
		}

		protected override void Release()
		{
			if (!pool.Return(base.Handle))
			{
				base.Handle.Free();
			}
		}
	}

	private static int minTrimPeriodMilliseconds = int.MaxValue;

	private static readonly List<WeakReference<UniformUnmanagedMemoryPool>> AllPools = new List<WeakReference<UniformUnmanagedMemoryPool>>();

	private static Timer? trimTimer;

	private static readonly Stopwatch Stopwatch = System.Diagnostics.Stopwatch.StartNew();

	private readonly TrimSettings trimSettings;

	private readonly UnmanagedMemoryHandle[] buffers;

	private int index;

	private long lastTrimTimestamp;

	private int finalized;

	public int BufferLength { get; }

	public int Capacity { get; }

	private bool Finalized => finalized == 1;

	public UniformUnmanagedMemoryPool(int bufferLength, int capacity)
		: this(bufferLength, capacity, TrimSettings.Default)
	{
	}

	public UniformUnmanagedMemoryPool(int bufferLength, int capacity, TrimSettings trimSettings)
	{
		this.trimSettings = trimSettings;
		Capacity = capacity;
		BufferLength = bufferLength;
		buffers = new UnmanagedMemoryHandle[capacity];
		if (trimSettings.Enabled)
		{
			UpdateTimer(trimSettings, this);
			Gen2GcCallback.Register((object s) => ((UniformUnmanagedMemoryPool)s).Trim(), this);
			lastTrimTimestamp = Stopwatch.ElapsedMilliseconds;
		}
	}

	~UniformUnmanagedMemoryPool()
	{
		Interlocked.Exchange(ref finalized, 1);
		TrimAll(buffers);
	}

	public UnmanagedMemoryHandle Rent()
	{
		UnmanagedMemoryHandle[] array = buffers;
		if (index == array.Length || Finalized)
		{
			return UnmanagedMemoryHandle.NullHandle;
		}
		UnmanagedMemoryHandle result;
		lock (array)
		{
			if (index == array.Length || Finalized)
			{
				return UnmanagedMemoryHandle.NullHandle;
			}
			result = array[index];
			array[index++] = default(UnmanagedMemoryHandle);
		}
		if (result.IsInvalid)
		{
			result = UnmanagedMemoryHandle.Allocate(BufferLength);
		}
		return result;
	}

	public UnmanagedMemoryHandle[]? Rent(int bufferCount)
	{
		UnmanagedMemoryHandle[] array = buffers;
		if (index + bufferCount >= array.Length + 1 || Finalized)
		{
			return null;
		}
		UnmanagedMemoryHandle[] array2;
		lock (array)
		{
			if (index + bufferCount >= array.Length + 1 || Finalized)
			{
				return null;
			}
			array2 = new UnmanagedMemoryHandle[bufferCount];
			for (int i = 0; i < bufferCount; i++)
			{
				array2[i] = array[index];
				array[index++] = UnmanagedMemoryHandle.NullHandle;
			}
		}
		for (int j = 0; j < array2.Length; j++)
		{
			if (array2[j].IsInvalid)
			{
				array2[j] = UnmanagedMemoryHandle.Allocate(BufferLength);
			}
		}
		return array2;
	}

	public bool Return(UnmanagedMemoryHandle bufferHandle)
	{
		Guard.IsTrue(bufferHandle.IsValid, "bufferHandle", "Returning NullHandle to the pool is not allowed.");
		lock (buffers)
		{
			if (Finalized || index == 0)
			{
				return false;
			}
			buffers[--index] = bufferHandle;
		}
		return true;
	}

	public bool Return(Span<UnmanagedMemoryHandle> bufferHandles)
	{
		lock (buffers)
		{
			if (Finalized || index - bufferHandles.Length + 1 <= 0)
			{
				return false;
			}
			for (int num = bufferHandles.Length - 1; num >= 0; num--)
			{
				ref UnmanagedMemoryHandle reference = ref bufferHandles[num];
				Guard.IsTrue(reference.IsValid, "bufferHandles", "Returning NullHandle to the pool is not allowed.");
				buffers[--index] = reference;
			}
		}
		return true;
	}

	public void Release()
	{
		lock (buffers)
		{
			for (int i = index; i < buffers.Length; i++)
			{
				ref UnmanagedMemoryHandle reference = ref buffers[i];
				if (reference.IsInvalid)
				{
					break;
				}
				reference.Free();
			}
		}
	}

	[Conditional("DEBUG")]
	private void DebugThrowInvalidReturn()
	{
		if (Finalized)
		{
			throw new ObjectDisposedException("UniformUnmanagedMemoryPool", "Invalid handle return to the pool! The pool has been finalized.");
		}
		throw new InvalidOperationException("Invalid handle return to the pool! Returning more buffers than rented.");
	}

	private static void UpdateTimer(TrimSettings settings, UniformUnmanagedMemoryPool pool)
	{
		lock (AllPools)
		{
			AllPools.Add(new WeakReference<UniformUnmanagedMemoryPool>(pool));
			int num = settings.TrimPeriodMilliseconds / 4;
			if (trimTimer == null)
			{
				trimTimer = new Timer(delegate
				{
					TimerCallback();
				}, null, num, num);
			}
			else if (settings.TrimPeriodMilliseconds < minTrimPeriodMilliseconds)
			{
				trimTimer.Change(num, num);
			}
			minTrimPeriodMilliseconds = Math.Min(minTrimPeriodMilliseconds, settings.TrimPeriodMilliseconds);
		}
	}

	private static void TimerCallback()
	{
		lock (AllPools)
		{
			for (int num = AllPools.Count - 1; num >= 0; num--)
			{
				if (!AllPools[num].TryGetTarget(out UniformUnmanagedMemoryPool _))
				{
					AllPools.RemoveAt(num);
				}
			}
			foreach (WeakReference<UniformUnmanagedMemoryPool> allPool in AllPools)
			{
				if (allPool.TryGetTarget(out var target2))
				{
					target2.Trim();
				}
			}
		}
	}

	private bool Trim()
	{
		if (Finalized)
		{
			return false;
		}
		UnmanagedMemoryHandle[] buffersLocal = buffers;
		if (IsHighMemoryPressure())
		{
			TrimAll(buffersLocal);
			return true;
		}
		if (Stopwatch.ElapsedMilliseconds - lastTrimTimestamp > trimSettings.TrimPeriodMilliseconds)
		{
			return TrimLowPressure(buffersLocal);
		}
		return true;
	}

	private void TrimAll(UnmanagedMemoryHandle[] buffersLocal)
	{
		lock (buffersLocal)
		{
			for (int i = index; i < buffersLocal.Length && buffersLocal[i].IsValid; i++)
			{
				buffersLocal[i].Free();
			}
		}
	}

	private bool TrimLowPressure(UnmanagedMemoryHandle[] buffersLocal)
	{
		lock (buffersLocal)
		{
			int num = 0;
			for (int i = index; i < buffersLocal.Length && buffersLocal[i].IsValid; i++)
			{
				num++;
			}
			int num2 = (int)Math.Ceiling((float)num * trimSettings.Rate);
			int num3 = index + num - 1;
			int num4 = index + num - num2;
			for (int num5 = num3; num5 >= num4; num5--)
			{
				buffersLocal[num5].Free();
			}
			lastTrimTimestamp = Stopwatch.ElapsedMilliseconds;
		}
		return true;
	}

	private bool IsHighMemoryPressure()
	{
		GCMemoryInfo gCMemoryInfo = GC.GetGCMemoryInfo();
		return (float)gCMemoryInfo.MemoryLoadBytes >= (float)gCMemoryInfo.HighMemoryLoadThresholdBytes * trimSettings.HighPressureThresholdRate;
	}

	public UnmanagedBuffer<T> CreateGuardedBuffer<T>(UnmanagedMemoryHandle handle, int lengthInElements, bool clear) where T : struct
	{
		UnmanagedBuffer<T> unmanagedBuffer = new UnmanagedBuffer<T>(lengthInElements, new ReturnToPoolBufferLifetimeGuard(this, handle));
		if (clear)
		{
			unmanagedBuffer.Clear();
		}
		return unmanagedBuffer;
	}

	public RefCountedMemoryLifetimeGuard CreateGroupLifetimeGuard(UnmanagedMemoryHandle[] handles)
	{
		return new GroupLifetimeGuard(this, handles);
	}
}
