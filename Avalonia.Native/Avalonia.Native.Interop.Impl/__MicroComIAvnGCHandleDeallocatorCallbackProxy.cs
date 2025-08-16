using System;
using System.Runtime.CompilerServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnGCHandleDeallocatorCallbackProxy : MicroComProxyBase, IAvnGCHandleDeallocatorCallback, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 1;

	public unsafe void FreeGCHandle(IntPtr handle)
	{
		((delegate* unmanaged[Stdcall]<void*, IntPtr, void>)(*base.PPV)[base.VTableSize])(base.PPV, handle);
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnGCHandleDeallocatorCallback), new Guid("f07c608e-52e9-422d-836e-c70f6e9b80f5"), (IntPtr p, bool owns) => new __MicroComIAvnGCHandleDeallocatorCallbackProxy(p, owns));
	}

	protected __MicroComIAvnGCHandleDeallocatorCallbackProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
