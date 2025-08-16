using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIActivationFactoryProxy : __MicroComIInspectableProxy, IActivationFactory, IInspectable, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 1;

	public unsafe IntPtr ActivateInstance()
	{
		IntPtr result = default(IntPtr);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("ActivateInstance failed", num);
		}
		return result;
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IActivationFactory), new Guid("00000035-0000-0000-C000-000000000046"), (IntPtr p, bool owns) => new __MicroComIActivationFactoryProxy(p, owns));
	}

	protected __MicroComIActivationFactoryProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
