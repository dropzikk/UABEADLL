using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIAsyncActionProxy : __MicroComIInspectableProxy, IAsyncAction, IInspectable, IUnknown, IDisposable
{
	public unsafe IAsyncActionCompletedHandler Completed
	{
		get
		{
			void* pObject = null;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, &pObject);
			if (num != 0)
			{
				throw new COMException("GetCompleted failed", num);
			}
			return MicroComRuntime.CreateProxyOrNullFor<IAsyncActionCompletedHandler>(pObject, ownsHandle: true);
		}
	}

	protected override int VTableSize => base.VTableSize + 3;

	public unsafe void SetCompleted(IAsyncActionCompletedHandler handler)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, MicroComRuntime.GetNativePointer(handler));
		if (num != 0)
		{
			throw new COMException("SetCompleted failed", num);
		}
	}

	public unsafe void GetResults()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV);
		if (num != 0)
		{
			throw new COMException("GetResults failed", num);
		}
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAsyncAction), new Guid("5A648006-843A-4DA9-865B-9D26E5DFAD7B"), (IntPtr p, bool owns) => new __MicroComIAsyncActionProxy(p, owns));
	}

	protected __MicroComIAsyncActionProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
