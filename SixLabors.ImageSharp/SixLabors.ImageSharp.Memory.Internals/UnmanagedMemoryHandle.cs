using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace SixLabors.ImageSharp.Memory.Internals;

internal struct UnmanagedMemoryHandle : IEquatable<UnmanagedMemoryHandle>
{
	private const int MaxAllocationAttempts = 10;

	private static int totalOutstandingHandles;

	private static long totalOomRetries;

	private static object? lowMemoryMonitor;

	public static readonly UnmanagedMemoryHandle NullHandle;

	private IntPtr handle;

	private int lengthInBytes;

	public IntPtr Handle => handle;

	public bool IsInvalid => Handle == IntPtr.Zero;

	public bool IsValid => Handle != IntPtr.Zero;

	public unsafe void* Pointer => (void*)Handle;

	internal static int TotalOutstandingHandles => totalOutstandingHandles;

	internal static long TotalOomRetries => totalOomRetries;

	private UnmanagedMemoryHandle(IntPtr handle, int lengthInBytes)
	{
		this.handle = handle;
		this.lengthInBytes = lengthInBytes;
		if (lengthInBytes > 0)
		{
			GC.AddMemoryPressure(lengthInBytes);
		}
		Interlocked.Increment(ref totalOutstandingHandles);
	}

	public static bool operator ==(UnmanagedMemoryHandle a, UnmanagedMemoryHandle b)
	{
		return a.Equals(b);
	}

	public static bool operator !=(UnmanagedMemoryHandle a, UnmanagedMemoryHandle b)
	{
		return !a.Equals(b);
	}

	public static UnmanagedMemoryHandle Allocate(int lengthInBytes)
	{
		return new UnmanagedMemoryHandle(AllocateHandle(lengthInBytes), lengthInBytes);
	}

	private static IntPtr AllocateHandle(int lengthInBytes)
	{
		int num = 0;
		IntPtr intPtr = IntPtr.Zero;
		while (intPtr == IntPtr.Zero)
		{
			try
			{
				intPtr = Marshal.AllocHGlobal(lengthInBytes);
			}
			catch (OutOfMemoryException) when (num < 10)
			{
				num++;
				Interlocked.Increment(ref totalOomRetries);
				Interlocked.CompareExchange(ref lowMemoryMonitor, new object(), null);
				Monitor.Enter(lowMemoryMonitor);
				Monitor.Wait(lowMemoryMonitor, 1);
				Monitor.Exit(lowMemoryMonitor);
			}
		}
		return intPtr;
	}

	public void Free()
	{
		IntPtr intPtr = Interlocked.Exchange(ref handle, IntPtr.Zero);
		if (!(intPtr == IntPtr.Zero))
		{
			Marshal.FreeHGlobal(intPtr);
			Interlocked.Decrement(ref totalOutstandingHandles);
			if (lengthInBytes > 0)
			{
				GC.RemoveMemoryPressure(lengthInBytes);
			}
			if (Volatile.Read(ref lowMemoryMonitor) != null)
			{
				Monitor.Enter(lowMemoryMonitor);
				Monitor.PulseAll(lowMemoryMonitor);
				Monitor.Exit(lowMemoryMonitor);
			}
			lengthInBytes = 0;
		}
	}

	public bool Equals(UnmanagedMemoryHandle other)
	{
		return handle.Equals(other.handle);
	}

	public override bool Equals(object? obj)
	{
		if (obj is UnmanagedMemoryHandle other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return handle.GetHashCode();
	}
}
