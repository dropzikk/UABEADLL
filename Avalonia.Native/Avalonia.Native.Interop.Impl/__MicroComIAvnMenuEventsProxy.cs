using System;
using System.Runtime.CompilerServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnMenuEventsProxy : MicroComProxyBase, IAvnMenuEvents, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 3;

	public unsafe void NeedsUpdate()
	{
		((delegate* unmanaged[Stdcall]<void*, void>)(*base.PPV)[base.VTableSize])(base.PPV);
	}

	public unsafe void Opening()
	{
		((delegate* unmanaged[Stdcall]<void*, void>)(*base.PPV)[base.VTableSize + 1])(base.PPV);
	}

	public unsafe void Closed()
	{
		((delegate* unmanaged[Stdcall]<void*, void>)(*base.PPV)[base.VTableSize + 2])(base.PPV);
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnMenuEvents), new Guid("0af7df53-7632-42f4-a650-0992c361b477"), (IntPtr p, bool owns) => new __MicroComIAvnMenuEventsProxy(p, owns));
	}

	protected __MicroComIAvnMenuEventsProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
