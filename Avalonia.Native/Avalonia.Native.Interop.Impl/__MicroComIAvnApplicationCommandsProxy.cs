using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnApplicationCommandsProxy : MicroComProxyBase, IAvnApplicationCommands, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 3;

	public unsafe void HideApp()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize])(base.PPV);
		if (num != 0)
		{
			throw new COMException("HideApp failed", num);
		}
	}

	public unsafe void ShowAll()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV);
		if (num != 0)
		{
			throw new COMException("ShowAll failed", num);
		}
	}

	public unsafe void HideOthers()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV);
		if (num != 0)
		{
			throw new COMException("HideOthers failed", num);
		}
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnApplicationCommands), new Guid("b4284791-055b-4313-8c2e-50f0a8c72ce9"), (IntPtr p, bool owns) => new __MicroComIAvnApplicationCommandsProxy(p, owns));
	}

	protected __MicroComIAvnApplicationCommandsProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
