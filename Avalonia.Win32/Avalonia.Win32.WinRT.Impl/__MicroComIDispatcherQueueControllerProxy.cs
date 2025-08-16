using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIDispatcherQueueControllerProxy : __MicroComIInspectableProxy, IDispatcherQueueController, IInspectable, IUnknown, IDisposable
{
	public unsafe IDispatcherQueue DispatcherQueue
	{
		get
		{
			void* pObject = null;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &pObject);
			if (num != 0)
			{
				throw new COMException("GetDispatcherQueue failed", num);
			}
			return MicroComRuntime.CreateProxyOrNullFor<IDispatcherQueue>(pObject, ownsHandle: true);
		}
	}

	protected override int VTableSize => base.VTableSize + 2;

	public unsafe IAsyncAction ShutdownQueueAsync()
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, &pObject);
		if (num != 0)
		{
			throw new COMException("ShutdownQueueAsync failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IAsyncAction>(pObject, ownsHandle: true);
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IDispatcherQueueController), new Guid("22F34E66-50DB-4E36-A98D-61C01B384D20"), (IntPtr p, bool owns) => new __MicroComIDispatcherQueueControllerProxy(p, owns));
	}

	protected __MicroComIDispatcherQueueControllerProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
