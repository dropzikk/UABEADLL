using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX.Impl;

internal class __MicroComIDXGIDeviceProxy : __MicroComIDXGIObjectProxy, IDXGIDevice, IDXGIObject, IUnknown, IDisposable
{
	public unsafe IDXGIAdapter Adapter
	{
		get
		{
			void* pObject = null;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &pObject);
			if (num != 0)
			{
				throw new COMException("GetAdapter failed", num);
			}
			return MicroComRuntime.CreateProxyOrNullFor<IDXGIAdapter>(pObject, ownsHandle: true);
		}
	}

	public unsafe int GPUThreadPriority
	{
		get
		{
			int result = 0;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 4])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetGPUThreadPriority failed", num);
			}
			return result;
		}
	}

	protected override int VTableSize => base.VTableSize + 5;

	public unsafe IDXGISurface CreateSurface(DXGI_SURFACE_DESC* pDesc, ushort NumSurfaces, uint Usage, void** pSharedResource)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, ushort, uint, void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, pDesc, NumSurfaces, Usage, pSharedResource, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateSurface failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IDXGISurface>(pObject, ownsHandle: true);
	}

	public unsafe void QueryResourceResidency(IUnknown ppResources, DXGI_RESIDENCY* pResidencyStatus, ushort NumResources)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, ushort, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, MicroComRuntime.GetNativePointer(ppResources), pResidencyStatus, NumResources);
		if (num != 0)
		{
			throw new COMException("QueryResourceResidency failed", num);
		}
	}

	public unsafe void SetGPUThreadPriority(int Priority)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int, int>)(*base.PPV)[base.VTableSize + 3])(base.PPV, Priority);
		if (num != 0)
		{
			throw new COMException("SetGPUThreadPriority failed", num);
		}
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IDXGIDevice), new Guid("54ec77fa-1377-44e6-8c32-88fd5f44c84c"), (IntPtr p, bool owns) => new __MicroComIDXGIDeviceProxy(p, owns));
	}

	protected __MicroComIDXGIDeviceProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
