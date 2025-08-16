using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvaloniaNativeFactoryProxy : MicroComProxyBase, IAvaloniaNativeFactory, IUnknown, IDisposable
{
	public unsafe IAvnMacOptions MacOptions => MicroComRuntime.CreateProxyOrNullFor<IAvnMacOptions>(((delegate* unmanaged[Stdcall]<void*, void*>)(*base.PPV)[base.VTableSize + 1])(base.PPV), ownsHandle: true);

	protected override int VTableSize => base.VTableSize + 21;

	public unsafe void Initialize(IAvnGCHandleDeallocatorCallback deallocator, IAvnApplicationEvents appCb, IAvnDispatcher dispatcher)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, MicroComRuntime.GetNativePointer(deallocator), MicroComRuntime.GetNativePointer(appCb), MicroComRuntime.GetNativePointer(dispatcher));
		if (num != 0)
		{
			throw new COMException("Initialize failed", num);
		}
	}

	public unsafe IAvnWindow CreateWindow(IAvnWindowEvents cb)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, MicroComRuntime.GetNativePointer(cb), &pObject);
		if (num != 0)
		{
			throw new COMException("CreateWindow failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IAvnWindow>(pObject, ownsHandle: true);
	}

	public unsafe IAvnPopup CreatePopup(IAvnWindowEvents cb)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 3])(base.PPV, MicroComRuntime.GetNativePointer(cb), &pObject);
		if (num != 0)
		{
			throw new COMException("CreatePopup failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IAvnPopup>(pObject, ownsHandle: true);
	}

	public unsafe IAvnPlatformThreadingInterface CreatePlatformThreadingInterface()
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 4])(base.PPV, &pObject);
		if (num != 0)
		{
			throw new COMException("CreatePlatformThreadingInterface failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IAvnPlatformThreadingInterface>(pObject, ownsHandle: true);
	}

	public unsafe IAvnSystemDialogs CreateSystemDialogs()
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 5])(base.PPV, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateSystemDialogs failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IAvnSystemDialogs>(pObject, ownsHandle: true);
	}

	public unsafe IAvnScreens CreateScreens()
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 6])(base.PPV, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateScreens failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IAvnScreens>(pObject, ownsHandle: true);
	}

	public unsafe IAvnClipboard CreateClipboard()
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 7])(base.PPV, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateClipboard failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IAvnClipboard>(pObject, ownsHandle: true);
	}

	public unsafe IAvnClipboard CreateDndClipboard()
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 8])(base.PPV, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateDndClipboard failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IAvnClipboard>(pObject, ownsHandle: true);
	}

	public unsafe IAvnCursorFactory CreateCursorFactory()
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 9])(base.PPV, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateCursorFactory failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IAvnCursorFactory>(pObject, ownsHandle: true);
	}

	public unsafe IAvnGlDisplay ObtainGlDisplay()
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 10])(base.PPV, &pObject);
		if (num != 0)
		{
			throw new COMException("ObtainGlDisplay failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IAvnGlDisplay>(pObject, ownsHandle: true);
	}

	public unsafe IAvnMetalDisplay ObtainMetalDisplay()
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 11])(base.PPV, &pObject);
		if (num != 0)
		{
			throw new COMException("ObtainMetalDisplay failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IAvnMetalDisplay>(pObject, ownsHandle: true);
	}

	public unsafe void SetAppMenu(IAvnMenu menu)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 12])(base.PPV, MicroComRuntime.GetNativePointer(menu));
		if (num != 0)
		{
			throw new COMException("SetAppMenu failed", num);
		}
	}

	public unsafe void SetServicesMenu(IAvnMenu menu)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 13])(base.PPV, MicroComRuntime.GetNativePointer(menu));
		if (num != 0)
		{
			throw new COMException("SetServicesMenu failed", num);
		}
	}

	public unsafe IAvnMenu CreateMenu(IAvnMenuEvents cb)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 14])(base.PPV, MicroComRuntime.GetNativePointer(cb), &pObject);
		if (num != 0)
		{
			throw new COMException("CreateMenu failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IAvnMenu>(pObject, ownsHandle: true);
	}

	public unsafe IAvnMenuItem CreateMenuItem()
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 15])(base.PPV, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateMenuItem failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IAvnMenuItem>(pObject, ownsHandle: true);
	}

	public unsafe IAvnMenuItem CreateMenuItemSeparator()
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 16])(base.PPV, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateMenuItemSeparator failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IAvnMenuItem>(pObject, ownsHandle: true);
	}

	public unsafe IAvnTrayIcon CreateTrayIcon()
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 17])(base.PPV, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateTrayIcon failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IAvnTrayIcon>(pObject, ownsHandle: true);
	}

	public unsafe IAvnApplicationCommands CreateApplicationCommands()
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 18])(base.PPV, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateApplicationCommands failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IAvnApplicationCommands>(pObject, ownsHandle: true);
	}

	public unsafe IAvnPlatformSettings CreatePlatformSettings()
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 19])(base.PPV, &pObject);
		if (num != 0)
		{
			throw new COMException("CreatePlatformSettings failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IAvnPlatformSettings>(pObject, ownsHandle: true);
	}

	public unsafe IAvnPlatformBehaviorInhibition CreatePlatformBehaviorInhibition()
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 20])(base.PPV, &pObject);
		if (num != 0)
		{
			throw new COMException("CreatePlatformBehaviorInhibition failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IAvnPlatformBehaviorInhibition>(pObject, ownsHandle: true);
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvaloniaNativeFactory), new Guid("809c652e-7396-11d2-9771-00a0c9b4d50c"), (IntPtr p, bool owns) => new __MicroComIAvaloniaNativeFactoryProxy(p, owns));
	}

	protected __MicroComIAvaloniaNativeFactoryProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
