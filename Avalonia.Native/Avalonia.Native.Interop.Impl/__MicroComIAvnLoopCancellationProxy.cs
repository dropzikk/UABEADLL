using System;
using System.Runtime.CompilerServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnLoopCancellationProxy : MicroComProxyBase, IAvnLoopCancellation, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 1;

	public unsafe void Cancel()
	{
		((delegate* unmanaged[Stdcall]<void*, void>)(*base.PPV)[base.VTableSize])(base.PPV);
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnLoopCancellation), new Guid("97330f88-c22b-4a8e-a130-201520091b01"), (IntPtr p, bool owns) => new __MicroComIAvnLoopCancellationProxy(p, owns));
	}

	protected __MicroComIAvnLoopCancellationProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
