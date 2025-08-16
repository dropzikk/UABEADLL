using System;
using System.Runtime.CompilerServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.Win32Com.Impl;

internal class __MicroComIModalWindowProxy : MicroComProxyBase, IModalWindow, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 1;

	public unsafe int Show(IntPtr hwndOwner)
	{
		return ((delegate* unmanaged[Stdcall]<void*, IntPtr, int>)(*base.PPV)[base.VTableSize])(base.PPV, hwndOwner);
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IModalWindow), new Guid("B4DB1657-70D7-485E-8E3E-6FCB5A5C1802"), (IntPtr p, bool owns) => new __MicroComIModalWindowProxy(p, owns));
	}

	protected __MicroComIModalWindowProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
