using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositorDesktopInteropProxy : MicroComProxyBase, ICompositorDesktopInterop, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 2;

	public unsafe IDesktopWindowTarget CreateDesktopWindowTarget(IntPtr hwndTarget, int isTopmost)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, int, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, hwndTarget, isTopmost, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateDesktopWindowTarget failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IDesktopWindowTarget>(pObject, ownsHandle: true);
	}

	public unsafe void EnsureOnThread(int threadId)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, threadId);
		if (num != 0)
		{
			throw new COMException("EnsureOnThread failed", num);
		}
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(ICompositorDesktopInterop), new Guid("29E691FA-4567-4DCA-B319-D0F207EB6807"), (IntPtr p, bool owns) => new __MicroComICompositorDesktopInteropProxy(p, owns));
	}

	protected __MicroComICompositorDesktopInteropProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
