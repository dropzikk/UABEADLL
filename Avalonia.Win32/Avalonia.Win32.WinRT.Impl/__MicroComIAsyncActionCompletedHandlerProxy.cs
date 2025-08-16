using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIAsyncActionCompletedHandlerProxy : MicroComProxyBase, IAsyncActionCompletedHandler, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 1;

	public unsafe void Invoke(IAsyncAction asyncInfo, AsyncStatus asyncStatus)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, AsyncStatus, int>)(*base.PPV)[base.VTableSize])(base.PPV, MicroComRuntime.GetNativePointer(asyncInfo), asyncStatus);
		if (num != 0)
		{
			throw new COMException("Invoke failed", num);
		}
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAsyncActionCompletedHandler), new Guid("A4ED5C81-76C9-40BD-8BE6-B1D90FB20AE7"), (IntPtr p, bool owns) => new __MicroComIAsyncActionCompletedHandlerProxy(p, owns));
	}

	protected __MicroComIAsyncActionCompletedHandlerProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
