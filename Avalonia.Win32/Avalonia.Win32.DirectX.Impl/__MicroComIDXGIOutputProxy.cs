using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX.Impl;

internal class __MicroComIDXGIOutputProxy : __MicroComIDXGIObjectProxy, IDXGIOutput, IDXGIObject, IUnknown, IDisposable
{
	public unsafe DXGI_OUTPUT_DESC Desc
	{
		get
		{
			DXGI_OUTPUT_DESC result = default(DXGI_OUTPUT_DESC);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetDesc failed", num);
			}
			return result;
		}
	}

	public unsafe DXGI_FRAME_STATISTICS FrameStatistics
	{
		get
		{
			DXGI_FRAME_STATISTICS result = default(DXGI_FRAME_STATISTICS);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 11])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetFrameStatistics failed", num);
			}
			return result;
		}
	}

	protected override int VTableSize => base.VTableSize + 12;

	public unsafe DXGI_MODE_DESC GetDisplayModeList(DXGI_FORMAT EnumFormat, ushort Flags, ushort* pNumModes)
	{
		DXGI_MODE_DESC result = default(DXGI_MODE_DESC);
		int num = ((delegate* unmanaged[Stdcall]<void*, DXGI_FORMAT, ushort, void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, EnumFormat, Flags, pNumModes, &result);
		if (num != 0)
		{
			throw new COMException("GetDisplayModeList failed", num);
		}
		return result;
	}

	public unsafe void FindClosestMatchingMode(DXGI_MODE_DESC* pModeToMatch, DXGI_MODE_DESC* pClosestMatch, IUnknown pConcernedDevice)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, pModeToMatch, pClosestMatch, MicroComRuntime.GetNativePointer(pConcernedDevice));
		if (num != 0)
		{
			throw new COMException("FindClosestMatchingMode failed", num);
		}
	}

	public unsafe void WaitForVBlank()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 3])(base.PPV);
		if (num != 0)
		{
			throw new COMException("WaitForVBlank failed", num);
		}
	}

	public unsafe void TakeOwnership(IUnknown pDevice, int Exclusive)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int, int>)(*base.PPV)[base.VTableSize + 4])(base.PPV, MicroComRuntime.GetNativePointer(pDevice), Exclusive);
		if (num != 0)
		{
			throw new COMException("TakeOwnership failed", num);
		}
	}

	public unsafe void ReleaseOwnership()
	{
		((delegate* unmanaged[Stdcall]<void*, void>)(*base.PPV)[base.VTableSize + 5])(base.PPV);
	}

	public unsafe void GetGammaControlCapabilities(IntPtr pGammaCaps)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, int>)(*base.PPV)[base.VTableSize + 6])(base.PPV, pGammaCaps);
		if (num != 0)
		{
			throw new COMException("GetGammaControlCapabilities failed", num);
		}
	}

	public unsafe void SetGammaControl(void* pArray)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 7])(base.PPV, pArray);
		if (num != 0)
		{
			throw new COMException("SetGammaControl failed", num);
		}
	}

	public unsafe void GetGammaControl(IntPtr pArray)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, int>)(*base.PPV)[base.VTableSize + 8])(base.PPV, pArray);
		if (num != 0)
		{
			throw new COMException("GetGammaControl failed", num);
		}
	}

	public unsafe void SetDisplaySurface(IDXGISurface pScanoutSurface)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 9])(base.PPV, MicroComRuntime.GetNativePointer(pScanoutSurface));
		if (num != 0)
		{
			throw new COMException("SetDisplaySurface failed", num);
		}
	}

	public unsafe void GetDisplaySurfaceData(IDXGISurface pDestination)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 10])(base.PPV, MicroComRuntime.GetNativePointer(pDestination));
		if (num != 0)
		{
			throw new COMException("GetDisplaySurfaceData failed", num);
		}
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IDXGIOutput), new Guid("ae02eedb-c735-4690-8d52-5a8dc20213aa"), (IntPtr p, bool owns) => new __MicroComIDXGIOutputProxy(p, owns));
	}

	protected __MicroComIDXGIOutputProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
