using System;
using System.Runtime.CompilerServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnPredicateCallbackProxy : MicroComProxyBase, IAvnPredicateCallback, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 1;

	public unsafe int Evaluate()
	{
		return ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize])(base.PPV);
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnPredicateCallback), new Guid("59e0586d-bd1c-4b85-9882-80d448b0fed9"), (IntPtr p, bool owns) => new __MicroComIAvnPredicateCallbackProxy(p, owns));
	}

	protected __MicroComIAvnPredicateCallbackProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
