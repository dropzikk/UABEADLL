using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Threading;

namespace MicroCom.Runtime;

public class MicroComProxyBase : CriticalFinalizerObject, IUnknown, IDisposable
{
	private IntPtr _nativePointer;

	private bool _ownsHandle;

	private SynchronizationContext _synchronizationContext;

	private static readonly SendOrPostCallback _disposeDelegate = DisposeOnContext;

	public IntPtr NativePointer
	{
		get
		{
			if (_nativePointer == IntPtr.Zero)
			{
				throw new ObjectDisposedException(GetType().FullName);
			}
			return _nativePointer;
		}
	}

	public unsafe void*** PPV => (void***)(void*)NativePointer;

	protected virtual int VTableSize => 3;

	public bool IsDisposed => _nativePointer == IntPtr.Zero;

	public bool OwnsHandle => _ownsHandle;

	public MicroComProxyBase(IntPtr nativePointer, bool ownsHandle)
	{
		_nativePointer = nativePointer;
		_ownsHandle = ownsHandle;
		_synchronizationContext = SynchronizationContext.Current;
		if (!_ownsHandle)
		{
			GC.SuppressFinalize(this);
		}
	}

	public unsafe void AddRef()
	{
		((delegate* unmanaged[Stdcall]<void*, int>)(*PPV)[1])(PPV);
	}

	public unsafe void Release()
	{
		((delegate* unmanaged[Stdcall]<void*, int>)(*PPV)[2])(PPV);
	}

	public unsafe int QueryInterface(Guid guid, out IntPtr ppv)
	{
		IntPtr intPtr = default(IntPtr);
		int result = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*(*PPV)))(PPV, &guid, &intPtr);
		ppv = intPtr;
		return result;
	}

	public T QueryInterface<T>() where T : IUnknown
	{
		Guid guidFor = MicroComRuntime.GetGuidFor(typeof(T));
		IntPtr ppv;
		int num = QueryInterface(guidFor, out ppv);
		if (num == 0)
		{
			return (T)MicroComRuntime.CreateProxyFor(typeof(T), ppv, ownsHandle: true);
		}
		throw new COMException("QueryInterface failed", num);
	}

	protected virtual void Dispose(bool disposing)
	{
		_ = _nativePointer;
		if (_ownsHandle)
		{
			Release();
			_ownsHandle = false;
		}
		_nativePointer = IntPtr.Zero;
		GC.SuppressFinalize(this);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}

	public void EnsureOwned()
	{
		if (!_ownsHandle)
		{
			GC.ReRegisterForFinalize(true);
			AddRef();
			_ownsHandle = true;
		}
	}

	private static void DisposeOnContext(object state)
	{
		(state as MicroComProxyBase)?.Dispose(disposing: false);
	}

	~MicroComProxyBase()
	{
		if (_ownsHandle)
		{
			if (_synchronizationContext == null)
			{
				Dispose();
			}
			else
			{
				_synchronizationContext.Post(_disposeDelegate, this);
			}
		}
	}
}
