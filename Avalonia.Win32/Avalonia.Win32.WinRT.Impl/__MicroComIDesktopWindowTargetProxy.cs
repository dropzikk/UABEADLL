using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIDesktopWindowTargetProxy : __MicroComIInspectableProxy, IDesktopWindowTarget, IInspectable, IUnknown, IDisposable
{
	public unsafe int IsTopmost
	{
		get
		{
			int result = 0;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetIsTopmost failed", num);
			}
			return result;
		}
	}

	protected override int VTableSize => base.VTableSize + 1;

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IDesktopWindowTarget), new Guid("6329D6CA-3366-490E-9DB3-25312929AC51"), (IntPtr p, bool owns) => new __MicroComIDesktopWindowTargetProxy(p, owns));
	}

	protected __MicroComIDesktopWindowTargetProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
