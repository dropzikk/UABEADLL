using System;
using System.Runtime.CompilerServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnWindowEventsProxy : __MicroComIAvnWindowBaseEventsProxy, IAvnWindowEvents, IAvnWindowBaseEvents, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 3;

	public unsafe int Closing()
	{
		return ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize])(base.PPV);
	}

	public unsafe void WindowStateChanged(AvnWindowState state)
	{
		((delegate* unmanaged[Stdcall]<void*, AvnWindowState, void>)(*base.PPV)[base.VTableSize + 1])(base.PPV, state);
	}

	public unsafe void GotInputWhenDisabled()
	{
		((delegate* unmanaged[Stdcall]<void*, void>)(*base.PPV)[base.VTableSize + 2])(base.PPV);
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnWindowEvents), new Guid("1ae178ee-1fcc-447f-b6dd-b7bb727f934c"), (IntPtr p, bool owns) => new __MicroComIAvnWindowEventsProxy(p, owns));
	}

	protected __MicroComIAvnWindowEventsProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
