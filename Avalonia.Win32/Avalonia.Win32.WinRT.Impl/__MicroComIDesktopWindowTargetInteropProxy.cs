using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIDesktopWindowTargetInteropProxy : MicroComProxyBase, IDesktopWindowTargetInterop, IUnknown, IDisposable
{
	public unsafe IntPtr HWnd
	{
		get
		{
			IntPtr result = default(IntPtr);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetHWnd failed", num);
			}
			return result;
		}
	}

	protected override int VTableSize => base.VTableSize + 1;

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IDesktopWindowTargetInterop), new Guid("35DBF59E-E3F9-45B0-81E7-FE75F4145DC9"), (IntPtr p, bool owns) => new __MicroComIDesktopWindowTargetInteropProxy(p, owns));
	}

	protected __MicroComIDesktopWindowTargetInteropProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
