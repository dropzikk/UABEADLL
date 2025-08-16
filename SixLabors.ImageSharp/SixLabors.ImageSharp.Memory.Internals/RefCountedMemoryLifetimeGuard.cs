using System;
using System.Threading;
using SixLabors.ImageSharp.Diagnostics;

namespace SixLabors.ImageSharp.Memory.Internals;

internal abstract class RefCountedMemoryLifetimeGuard : IDisposable
{
	private int refCount = 1;

	private int disposed;

	private int released;

	private string? allocationStackTrace;

	public bool IsDisposed => disposed == 1;

	protected RefCountedMemoryLifetimeGuard()
	{
		if (MemoryDiagnostics.UndisposedAllocationSubscribed)
		{
			allocationStackTrace = Environment.StackTrace;
		}
		MemoryDiagnostics.IncrementTotalUndisposedAllocationCount();
	}

	~RefCountedMemoryLifetimeGuard()
	{
		Interlocked.Exchange(ref disposed, 1);
		ReleaseRef(finalizing: true);
	}

	public void AddRef()
	{
		Interlocked.Increment(ref refCount);
	}

	public void ReleaseRef()
	{
		ReleaseRef(finalizing: false);
	}

	public void Dispose()
	{
		if (Interlocked.Exchange(ref disposed, 1) == 0)
		{
			ReleaseRef();
			GC.SuppressFinalize(this);
		}
	}

	protected abstract void Release();

	private void ReleaseRef(bool finalizing)
	{
		Interlocked.Decrement(ref refCount);
		if (refCount == 0 && Interlocked.Exchange(ref released, 1) == 0)
		{
			if (!finalizing)
			{
				MemoryDiagnostics.DecrementTotalUndisposedAllocationCount();
			}
			else if (allocationStackTrace != null)
			{
				MemoryDiagnostics.RaiseUndisposedMemoryResource(allocationStackTrace);
			}
			Release();
		}
	}
}
