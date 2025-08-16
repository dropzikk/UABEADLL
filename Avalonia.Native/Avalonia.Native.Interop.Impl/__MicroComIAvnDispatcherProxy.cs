using System;
using System.Runtime.CompilerServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnDispatcherProxy : MicroComProxyBase, IAvnDispatcher, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 1;

	public unsafe void Post(IAvnActionCallback cb)
	{
		((delegate* unmanaged[Stdcall]<void*, void*, void>)(*base.PPV)[base.VTableSize])(base.PPV, MicroComRuntime.GetNativePointer(cb));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnDispatcher), new Guid("96688589-5dc7-41ec-9ce3-d481942454ee"), (IntPtr p, bool owns) => new __MicroComIAvnDispatcherProxy(p, owns));
	}

	protected __MicroComIAvnDispatcherProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
