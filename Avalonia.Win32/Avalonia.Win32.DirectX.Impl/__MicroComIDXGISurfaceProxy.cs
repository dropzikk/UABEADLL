using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX.Impl;

internal class __MicroComIDXGISurfaceProxy : __MicroComIDXGIDeviceSubObjectProxy, IDXGISurface, IDXGIDeviceSubObject, IDXGIObject, IUnknown, IDisposable
{
	public unsafe DXGI_SURFACE_DESC Desc
	{
		get
		{
			DXGI_SURFACE_DESC result = default(DXGI_SURFACE_DESC);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetDesc failed", num);
			}
			return result;
		}
	}

	protected override int VTableSize => base.VTableSize + 3;

	public unsafe void Map(DXGI_MAPPED_RECT* pLockedRect, ushort MapFlags)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, ushort, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, pLockedRect, MapFlags);
		if (num != 0)
		{
			throw new COMException("Map failed", num);
		}
	}

	public unsafe void Unmap()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV);
		if (num != 0)
		{
			throw new COMException("Unmap failed", num);
		}
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IDXGISurface), new Guid("cafcb56c-6ac3-4889-bf47-9e23bbd260ec"), (IntPtr p, bool owns) => new __MicroComIDXGISurfaceProxy(p, owns));
	}

	protected __MicroComIDXGISurfaceProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
