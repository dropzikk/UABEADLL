using System;
using System.Runtime.CompilerServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnDndResultCallbackProxy : MicroComProxyBase, IAvnDndResultCallback, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 1;

	public unsafe void OnDragAndDropComplete(AvnDragDropEffects effecct)
	{
		((delegate* unmanaged[Stdcall]<void*, AvnDragDropEffects, void>)(*base.PPV)[base.VTableSize])(base.PPV, effecct);
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnDndResultCallback), new Guid("a13d2382-3b3a-4d1c-9b27-8f34653d3f01"), (IntPtr p, bool owns) => new __MicroComIAvnDndResultCallbackProxy(p, owns));
	}

	protected __MicroComIAvnDndResultCallbackProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
