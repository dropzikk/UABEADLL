using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnTrayIconProxy : MicroComProxyBase, IAvnTrayIcon, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 3;

	public unsafe void SetIcon(void* data, IntPtr length)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, IntPtr, int>)(*base.PPV)[base.VTableSize])(base.PPV, data, length);
		if (num != 0)
		{
			throw new COMException("SetIcon failed", num);
		}
	}

	public unsafe void SetMenu(IAvnMenu menu)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, MicroComRuntime.GetNativePointer(menu));
		if (num != 0)
		{
			throw new COMException("SetMenu failed", num);
		}
	}

	public unsafe void SetIsVisible(int isVisible)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, isVisible);
		if (num != 0)
		{
			throw new COMException("SetIsVisible failed", num);
		}
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnTrayIcon), new Guid("60992d19-38f0-4141-a0a9-76ac303801f3"), (IntPtr p, bool owns) => new __MicroComIAvnTrayIconProxy(p, owns));
	}

	protected __MicroComIAvnTrayIconProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
