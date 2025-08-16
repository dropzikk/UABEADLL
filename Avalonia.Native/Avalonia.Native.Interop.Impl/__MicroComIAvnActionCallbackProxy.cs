using System;
using System.Runtime.CompilerServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnActionCallbackProxy : MicroComProxyBase, IAvnActionCallback, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 1;

	public unsafe void Run()
	{
		((delegate* unmanaged[Stdcall]<void*, void>)(*base.PPV)[base.VTableSize])(base.PPV);
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnActionCallback), new Guid("04c1b049-1f43-418a-9159-cae627ec1367"), (IntPtr p, bool owns) => new __MicroComIAvnActionCallbackProxy(p, owns));
	}

	protected __MicroComIAvnActionCallbackProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
