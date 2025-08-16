using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnScreensProxy : MicroComProxyBase, IAvnScreens, IUnknown, IDisposable
{
	public unsafe int ScreenCount
	{
		get
		{
			int result = 0;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetScreenCount failed", num);
			}
			return result;
		}
	}

	protected override int VTableSize => base.VTableSize + 2;

	public unsafe AvnScreen GetScreen(int index)
	{
		AvnScreen result = default(AvnScreen);
		int num = ((delegate* unmanaged[Stdcall]<void*, int, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, index, &result);
		if (num != 0)
		{
			throw new COMException("GetScreen failed", num);
		}
		return result;
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnScreens), new Guid("9a52bc7a-d8c7-4230-8d34-704a0b70a933"), (IntPtr p, bool owns) => new __MicroComIAvnScreensProxy(p, owns));
	}

	protected __MicroComIAvnScreensProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
