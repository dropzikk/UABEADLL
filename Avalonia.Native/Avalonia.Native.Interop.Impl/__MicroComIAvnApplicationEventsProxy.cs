using System;
using System.Runtime.CompilerServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnApplicationEventsProxy : MicroComProxyBase, IAvnApplicationEvents, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 2;

	public unsafe void FilesOpened(IAvnStringArray urls)
	{
		((delegate* unmanaged[Stdcall]<void*, void*, void>)(*base.PPV)[base.VTableSize])(base.PPV, MicroComRuntime.GetNativePointer(urls));
	}

	public unsafe int TryShutdown()
	{
		return ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV);
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnApplicationEvents), new Guid("6575b5af-f27a-4609-866c-f1f014c20f79"), (IntPtr p, bool owns) => new __MicroComIAvnApplicationEventsProxy(p, owns));
	}

	protected __MicroComIAvnApplicationEventsProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
