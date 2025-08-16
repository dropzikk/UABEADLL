using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositorInteropProxy : MicroComProxyBase, ICompositorInterop, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 3;

	public unsafe ICompositionSurface CreateCompositionSurfaceForHandle(IntPtr swapChain)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, swapChain, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateCompositionSurfaceForHandle failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<ICompositionSurface>(pObject, ownsHandle: true);
	}

	public unsafe ICompositionSurface CreateCompositionSurfaceForSwapChain(IUnknown swapChain)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, MicroComRuntime.GetNativePointer(swapChain), &pObject);
		if (num != 0)
		{
			throw new COMException("CreateCompositionSurfaceForSwapChain failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<ICompositionSurface>(pObject, ownsHandle: true);
	}

	public unsafe ICompositionGraphicsDevice CreateGraphicsDevice(IUnknown renderingDevice)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, MicroComRuntime.GetNativePointer(renderingDevice), &pObject);
		if (num != 0)
		{
			throw new COMException("CreateGraphicsDevice failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<ICompositionGraphicsDevice>(pObject, ownsHandle: true);
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(ICompositorInterop), new Guid("25297D5C-3AD4-4C9C-B5CF-E36A38512330"), (IntPtr p, bool owns) => new __MicroComICompositorInteropProxy(p, owns));
	}

	protected __MicroComICompositorInteropProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
