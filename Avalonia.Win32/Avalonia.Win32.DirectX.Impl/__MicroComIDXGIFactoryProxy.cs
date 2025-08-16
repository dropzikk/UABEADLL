using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX.Impl;

internal class __MicroComIDXGIFactoryProxy : __MicroComIDXGIObjectProxy, IDXGIFactory, IDXGIObject, IUnknown, IDisposable
{
	public unsafe IntPtr WindowAssociation
	{
		get
		{
			IntPtr result = default(IntPtr);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetWindowAssociation failed", num);
			}
			return result;
		}
	}

	protected override int VTableSize => base.VTableSize + 5;

	public unsafe int EnumAdapters(ushort Adapter, void* ppAdapter)
	{
		return ((delegate* unmanaged[Stdcall]<void*, ushort, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, Adapter, ppAdapter);
	}

	public unsafe void MakeWindowAssociation(IntPtr WindowHandle, ushort Flags)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, ushort, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, WindowHandle, Flags);
		if (num != 0)
		{
			throw new COMException("MakeWindowAssociation failed", num);
		}
	}

	public unsafe IDXGISwapChain CreateSwapChain(IUnknown pDevice, DXGI_SWAP_CHAIN_DESC* pDesc)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 3])(base.PPV, MicroComRuntime.GetNativePointer(pDevice), pDesc, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateSwapChain failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IDXGISwapChain>(pObject, ownsHandle: true);
	}

	public unsafe IDXGIAdapter CreateSoftwareAdapter(void* Module)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 4])(base.PPV, Module, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateSoftwareAdapter failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IDXGIAdapter>(pObject, ownsHandle: true);
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IDXGIFactory), new Guid("7b7166ec-21c7-44ae-b21a-c9ae321ae369"), (IntPtr p, bool owns) => new __MicroComIDXGIFactoryProxy(p, owns));
	}

	protected __MicroComIDXGIFactoryProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
