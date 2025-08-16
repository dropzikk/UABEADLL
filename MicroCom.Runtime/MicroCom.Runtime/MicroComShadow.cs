using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace MicroCom.Runtime;

public class MicroComShadow : IDisposable
{
	private readonly object _lock = new object();

	private readonly Dictionary<Type, IntPtr> _shadows = new Dictionary<Type, IntPtr>();

	private readonly Dictionary<IntPtr, Type> _backShadows = new Dictionary<IntPtr, Type>();

	private GCHandle? _handle;

	private volatile int _refCount;

	internal IMicroComShadowContainer Target { get; }

	internal MicroComShadow(IMicroComShadowContainer target)
	{
		Target = target;
		Target.Shadow = this;
	}

	internal unsafe int QueryInterface(Ccw* ccw, Guid* guid, void** ppv)
	{
		if (MicroComRuntime.TryGetTypeForGuid(*guid, out var t))
		{
			return QueryInterface(t, ppv);
		}
		if (*guid == MicroComRuntime.ManagedObjectInterfaceGuid)
		{
			ccw->RefCount++;
			*ppv = ccw;
			return 0;
		}
		return -2147467262;
	}

	internal unsafe int QueryInterface(Type type, void** ppv)
	{
		if (!type.IsInstanceOfType(Target))
		{
			return -2147467262;
		}
		int orCreateNativePointer = GetOrCreateNativePointer(type, ppv);
		if (orCreateNativePointer == 0)
		{
			AddRef((Ccw*)(*ppv));
		}
		return orCreateNativePointer;
	}

	internal unsafe int GetOrCreateNativePointer(Type type, void** ppv)
	{
		if (!MicroComRuntime.GetVtableFor(type, out var ptr))
		{
			return -2147467262;
		}
		lock (_lock)
		{
			if (_shadows.TryGetValue(type, out var value))
			{
				Ccw* ptr2 = (Ccw*)(void*)value;
				AddRef(ptr2);
				*ppv = ptr2;
				return 0;
			}
			IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf<Ccw>());
			Ccw* ptr3 = (Ccw*)(void*)intPtr;
			*ptr3 = default(Ccw);
			ptr3->RefCount = 0;
			ptr3->VTable = ptr;
			if (!_handle.HasValue)
			{
				_handle = GCHandle.Alloc(this);
			}
			ptr3->GcShadowHandle = GCHandle.ToIntPtr(_handle.Value);
			_shadows[type] = intPtr;
			_backShadows[intPtr] = type;
			*ppv = ptr3;
			return 0;
		}
	}

	internal unsafe int AddRef(Ccw* ccw)
	{
		if (Interlocked.Increment(ref _refCount) == 1)
		{
			try
			{
				Target.OnReferencedFromNative();
			}
			catch (Exception e)
			{
				MicroComRuntime.UnhandledException(Target, e);
			}
		}
		return Interlocked.Increment(ref ccw->RefCount);
	}

	internal unsafe int Release(Ccw* ccw)
	{
		Interlocked.Decrement(ref _refCount);
		int num = Interlocked.Decrement(ref ccw->RefCount);
		if (num == 0)
		{
			return FreeCcw(ccw);
		}
		return num;
	}

	private unsafe int FreeCcw(Ccw* ccw)
	{
		lock (_lock)
		{
			if (ccw->RefCount != 0)
			{
				return ccw->RefCount;
			}
			IntPtr intPtr = new IntPtr(ccw);
			Type key = _backShadows[intPtr];
			_backShadows.Remove(intPtr);
			_shadows.Remove(key);
			Marshal.FreeHGlobal(intPtr);
			if (_shadows.Count == 0)
			{
				_handle?.Free();
				_handle = null;
				try
				{
					Target.OnUnreferencedFromNative();
				}
				catch (Exception e)
				{
					MicroComRuntime.UnhandledException(Target, e);
				}
			}
		}
		return 0;
	}

	public unsafe void Dispose()
	{
		lock (_lock)
		{
			List<IntPtr> list = null;
			foreach (KeyValuePair<IntPtr, Type> backShadow in _backShadows)
			{
				Ccw* ptr = (Ccw*)(void*)backShadow.Key;
				if (ptr->RefCount == 0)
				{
					if (list == null)
					{
						list = new List<IntPtr>();
					}
					list.Add(backShadow.Key);
				}
			}
			if (list == null)
			{
				return;
			}
			foreach (IntPtr item in list)
			{
				FreeCcw((Ccw*)(void*)item);
			}
		}
	}
}
