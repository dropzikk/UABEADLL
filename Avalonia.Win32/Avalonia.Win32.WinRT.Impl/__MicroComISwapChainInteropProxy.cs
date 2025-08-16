using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComISwapChainInteropProxy : MicroComProxyBase, ISwapChainInterop, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 1;

	public unsafe void SetSwapChain(IUnknown swapChain)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, MicroComRuntime.GetNativePointer(swapChain));
		if (num != 0)
		{
			throw new COMException("SetSwapChain failed", num);
		}
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(ISwapChainInterop), new Guid("26f496a0-7f38-45fb-88f7-faaabe67dd59"), (IntPtr p, bool owns) => new __MicroComISwapChainInteropProxy(p, owns));
	}

	protected __MicroComISwapChainInteropProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
