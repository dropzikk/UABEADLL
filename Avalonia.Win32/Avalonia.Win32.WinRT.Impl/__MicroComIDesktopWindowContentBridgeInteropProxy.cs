using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIDesktopWindowContentBridgeInteropProxy : MicroComProxyBase, IDesktopWindowContentBridgeInterop, IUnknown, IDisposable
{
	public unsafe IntPtr HWnd
	{
		get
		{
			IntPtr result = default(IntPtr);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetHWnd failed", num);
			}
			return result;
		}
	}

	public unsafe float AppliedScaleFactor
	{
		get
		{
			float result = 0f;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetAppliedScaleFactor failed", num);
			}
			return result;
		}
	}

	protected override int VTableSize => base.VTableSize + 3;

	public unsafe void Initialize(ICompositor compositor, IntPtr parentHwnd)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, IntPtr, int>)(*base.PPV)[base.VTableSize])(base.PPV, MicroComRuntime.GetNativePointer(compositor), parentHwnd);
		if (num != 0)
		{
			throw new COMException("Initialize failed", num);
		}
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IDesktopWindowContentBridgeInterop), new Guid("37642806-F421-4FD0-9F82-23AE7C776182"), (IntPtr p, bool owns) => new __MicroComIDesktopWindowContentBridgeInteropProxy(p, owns));
	}

	protected __MicroComIDesktopWindowContentBridgeInteropProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
