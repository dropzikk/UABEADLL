using System;
using System.Runtime.CompilerServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnSystemDialogEventsProxy : MicroComProxyBase, IAvnSystemDialogEvents, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 1;

	public unsafe void OnCompleted(int numResults, void* ptrFirstResult)
	{
		((delegate* unmanaged[Stdcall]<void*, int, void*, void>)(*base.PPV)[base.VTableSize])(base.PPV, numResults, ptrFirstResult);
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnSystemDialogEvents), new Guid("6c621a6e-e4c1-4ae3-9749-83eeeffa09b6"), (IntPtr p, bool owns) => new __MicroComIAvnSystemDialogEventsProxy(p, owns));
	}

	protected __MicroComIAvnSystemDialogEventsProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
