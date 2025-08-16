using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace MicroCom.Runtime;

public static class MicroComRuntime
{
	private static ConcurrentDictionary<Type, IntPtr> _vtables;

	private static ConcurrentDictionary<Type, Func<IntPtr, bool, object>> _factories;

	private static ConcurrentDictionary<Type, Guid> _guids;

	private static ConcurrentDictionary<Guid, Type> _guidsToTypes;

	internal static readonly Guid ManagedObjectInterfaceGuid;

	static MicroComRuntime()
	{
		_vtables = new ConcurrentDictionary<Type, IntPtr>();
		_factories = new ConcurrentDictionary<Type, Func<IntPtr, bool, object>>();
		_guids = new ConcurrentDictionary<Type, Guid>();
		_guidsToTypes = new ConcurrentDictionary<Guid, Type>();
		ManagedObjectInterfaceGuid = Guid.Parse("cd7687c0-a9c2-4563-b08e-a399df50c633");
		Register(typeof(IUnknown), new Guid("00000000-0000-0000-C000-000000000046"), (IntPtr ppv, bool owns) => new MicroComProxyBase(ppv, owns));
		RegisterVTable(typeof(IUnknown), MicroComVtblBase.Vtable);
	}

	public static void RegisterVTable(Type t, IntPtr vtable)
	{
		_vtables[t] = vtable;
	}

	public static void Register(Type t, Guid guid, Func<IntPtr, bool, object> proxyFactory)
	{
		_factories[t] = proxyFactory;
		_guids[t] = guid;
		_guidsToTypes[guid] = t;
	}

	public static Guid GetGuidFor(Type type)
	{
		return _guids[type];
	}

	public unsafe static T CreateProxyFor<T>(void* pObject, bool ownsHandle)
	{
		return (T)CreateProxyFor(typeof(T), new IntPtr(pObject), ownsHandle);
	}

	public static T CreateProxyFor<T>(IntPtr pObject, bool ownsHandle)
	{
		return (T)CreateProxyFor(typeof(T), pObject, ownsHandle);
	}

	public unsafe static T CreateProxyOrNullFor<T>(void* pObject, bool ownsHandle) where T : class
	{
		if (pObject != null)
		{
			return (T)CreateProxyFor(typeof(T), new IntPtr(pObject), ownsHandle);
		}
		return null;
	}

	public static T CreateProxyOrNullFor<T>(IntPtr pObject, bool ownsHandle) where T : class
	{
		if (!(pObject == IntPtr.Zero))
		{
			return (T)CreateProxyFor(typeof(T), pObject, ownsHandle);
		}
		return null;
	}

	public static object CreateProxyFor(Type type, IntPtr pObject, bool ownsHandle)
	{
		if (pObject == IntPtr.Zero)
		{
			throw new ArgumentNullException("pObject");
		}
		return _factories[type](pObject, ownsHandle);
	}

	public unsafe static IntPtr GetNativeIntPtr<T>(this T obj, bool owned = false) where T : IUnknown
	{
		return new IntPtr(GetNativePointer(obj, owned));
	}

	public unsafe static void* GetNativePointer<T>(T obj, bool owned = false) where T : IUnknown
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is MicroComProxyBase microComProxyBase)
		{
			if (owned)
			{
				microComProxyBase.AddRef();
			}
			return (void*)microComProxyBase.NativePointer;
		}
		if (obj is IMicroComShadowContainer microComShadowContainer)
		{
			IMicroComShadowContainer microComShadowContainer2 = microComShadowContainer;
			if (microComShadowContainer2.Shadow == null)
			{
				MicroComShadow microComShadow2 = (microComShadowContainer2.Shadow = new MicroComShadow(microComShadowContainer));
			}
			void* ptr = null;
			int orCreateNativePointer = microComShadowContainer.Shadow.GetOrCreateNativePointer(typeof(T), &ptr);
			if (orCreateNativePointer != 0)
			{
				throw new COMException("Unable to create native callable wrapper for type " + typeof(T)?.ToString() + " for instance of type " + obj.GetType(), orCreateNativePointer);
			}
			if (owned)
			{
				microComShadowContainer.Shadow.AddRef((Ccw*)ptr);
			}
			return ptr;
		}
		T val = obj;
		throw new ArgumentException("Unable to get a native pointer for " + val);
	}

	public unsafe static object GetObjectFromCcw(IntPtr ccw)
	{
		Ccw* ptr = (Ccw*)(void*)ccw;
		return ((MicroComShadow)GCHandle.FromIntPtr(ptr->GcShadowHandle).Target).Target;
	}

	public static bool IsComWrapper(IUnknown obj)
	{
		return obj is MicroComProxyBase;
	}

	public static object TryUnwrapManagedObject(IUnknown obj)
	{
		if (!(obj is MicroComProxyBase microComProxyBase))
		{
			return null;
		}
		if (microComProxyBase.QueryInterface(ManagedObjectInterfaceGuid, out var _) != 0)
		{
			return null;
		}
		microComProxyBase.Release();
		return GetObjectFromCcw(microComProxyBase.NativePointer);
	}

	public static bool TryGetTypeForGuid(Guid guid, out Type t)
	{
		return _guidsToTypes.TryGetValue(guid, out t);
	}

	public static bool GetVtableFor(Type type, out IntPtr ptr)
	{
		return _vtables.TryGetValue(type, out ptr);
	}

	public static void UnhandledException(object target, Exception e)
	{
		if (target is IMicroComExceptionCallback microComExceptionCallback)
		{
			try
			{
				microComExceptionCallback.RaiseException(e);
			}
			catch
			{
			}
		}
	}

	public unsafe static T CloneReference<T>(this T iface) where T : IUnknown
	{
		_ = (MicroComProxyBase)(object)iface;
		return CreateProxyFor<T>(GetNativePointer(iface, owned: true), ownsHandle: true);
	}

	public static T QueryInterface<T>(this IUnknown unknown) where T : IUnknown
	{
		return ((MicroComProxyBase)unknown).QueryInterface<T>();
	}

	public static void UnsafeAddRef(this IUnknown unknown)
	{
		((MicroComProxyBase)unknown).AddRef();
	}

	public static void UnsafeRelease(this IUnknown unknown)
	{
		((MicroComProxyBase)unknown).Release();
	}
}
