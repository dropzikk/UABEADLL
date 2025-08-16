using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositor3Proxy : __MicroComIInspectableProxy, ICompositor3, IInspectable, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 1;

	public unsafe ICompositionBackdropBrush CreateHostBackdropBrush()
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateHostBackdropBrush failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<ICompositionBackdropBrush>(pObject, ownsHandle: true);
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(ICompositor3), new Guid("C9DD8EF0-6EB1-4E3C-A658-675D9C64D4AB"), (IntPtr p, bool owns) => new __MicroComICompositor3Proxy(p, owns));
	}

	protected __MicroComICompositor3Proxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
