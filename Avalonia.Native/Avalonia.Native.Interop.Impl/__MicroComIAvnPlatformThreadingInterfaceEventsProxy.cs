using System;
using System.Runtime.CompilerServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnPlatformThreadingInterfaceEventsProxy : MicroComProxyBase, IAvnPlatformThreadingInterfaceEvents, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 3;

	public unsafe void Signaled()
	{
		((delegate* unmanaged[Stdcall]<void*, void>)(*base.PPV)[base.VTableSize])(base.PPV);
	}

	public unsafe void Timer()
	{
		((delegate* unmanaged[Stdcall]<void*, void>)(*base.PPV)[base.VTableSize + 1])(base.PPV);
	}

	public unsafe void ReadyForBackgroundProcessing()
	{
		((delegate* unmanaged[Stdcall]<void*, void>)(*base.PPV)[base.VTableSize + 2])(base.PPV);
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnPlatformThreadingInterfaceEvents), new Guid("6df4d2db-0b80-4f59-ad88-0baa5e21eb14"), (IntPtr p, bool owns) => new __MicroComIAvnPlatformThreadingInterfaceEventsProxy(p, owns));
	}

	protected __MicroComIAvnPlatformThreadingInterfaceEventsProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
