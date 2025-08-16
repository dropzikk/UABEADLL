using System;
using System.Runtime.CompilerServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnPlatformThreadingInterfaceProxy : MicroComProxyBase, IAvnPlatformThreadingInterface, IUnknown, IDisposable
{
	public unsafe int CurrentThreadIsLoopThread => ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize])(base.PPV);

	protected override int VTableSize => base.VTableSize + 7;

	public unsafe void SetEvents(IAvnPlatformThreadingInterfaceEvents cb)
	{
		((delegate* unmanaged[Stdcall]<void*, void*, void>)(*base.PPV)[base.VTableSize + 1])(base.PPV, MicroComRuntime.GetNativePointer(cb));
	}

	public unsafe IAvnLoopCancellation CreateLoopCancellation()
	{
		return MicroComRuntime.CreateProxyOrNullFor<IAvnLoopCancellation>(((delegate* unmanaged[Stdcall]<void*, void*>)(*base.PPV)[base.VTableSize + 2])(base.PPV), ownsHandle: true);
	}

	public unsafe void RunLoop(IAvnLoopCancellation cancel)
	{
		((delegate* unmanaged[Stdcall]<void*, void*, void>)(*base.PPV)[base.VTableSize + 3])(base.PPV, MicroComRuntime.GetNativePointer(cancel));
	}

	public unsafe void Signal()
	{
		((delegate* unmanaged[Stdcall]<void*, void>)(*base.PPV)[base.VTableSize + 4])(base.PPV);
	}

	public unsafe void UpdateTimer(int ms)
	{
		((delegate* unmanaged[Stdcall]<void*, int, void>)(*base.PPV)[base.VTableSize + 5])(base.PPV, ms);
	}

	public unsafe void RequestBackgroundProcessing()
	{
		((delegate* unmanaged[Stdcall]<void*, void>)(*base.PPV)[base.VTableSize + 6])(base.PPV);
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnPlatformThreadingInterface), new Guid("fbc06f3d-7860-42df-83fd-53c4b02dd9c3"), (IntPtr p, bool owns) => new __MicroComIAvnPlatformThreadingInterfaceProxy(p, owns));
	}

	protected __MicroComIAvnPlatformThreadingInterfaceProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
