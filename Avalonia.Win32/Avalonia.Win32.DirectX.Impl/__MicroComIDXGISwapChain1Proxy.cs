using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX.Impl;

internal class __MicroComIDXGISwapChain1Proxy : __MicroComIDXGISwapChainProxy, IDXGISwapChain1, IDXGISwapChain, IDXGIDeviceSubObject, IDXGIObject, IUnknown, IDisposable
{
	public unsafe DXGI_SWAP_CHAIN_DESC1 Desc1
	{
		get
		{
			DXGI_SWAP_CHAIN_DESC1 result = default(DXGI_SWAP_CHAIN_DESC1);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetDesc1 failed", num);
			}
			return result;
		}
	}

	public unsafe DXGI_SWAP_CHAIN_FULLSCREEN_DESC FullscreenDesc
	{
		get
		{
			DXGI_SWAP_CHAIN_FULLSCREEN_DESC result = default(DXGI_SWAP_CHAIN_FULLSCREEN_DESC);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetFullscreenDesc failed", num);
			}
			return result;
		}
	}

	public unsafe IntPtr Hwnd
	{
		get
		{
			IntPtr result = default(IntPtr);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetHwnd failed", num);
			}
			return result;
		}
	}

	public unsafe IDXGIOutput RestrictToOutput
	{
		get
		{
			void* pObject = null;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 6])(base.PPV, &pObject);
			if (num != 0)
			{
				throw new COMException("GetRestrictToOutput failed", num);
			}
			return MicroComRuntime.CreateProxyOrNullFor<IDXGIOutput>(pObject, ownsHandle: true);
		}
	}

	public unsafe DXGI_RGBA BackgroundColor
	{
		get
		{
			DXGI_RGBA result = default(DXGI_RGBA);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 8])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetBackgroundColor failed", num);
			}
			return result;
		}
	}

	public unsafe DXGI_MODE_ROTATION Rotation
	{
		get
		{
			DXGI_MODE_ROTATION result = DXGI_MODE_ROTATION.DXGI_MODE_ROTATION_UNSPECIFIED;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 10])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetRotation failed", num);
			}
			return result;
		}
	}

	protected override int VTableSize => base.VTableSize + 11;

	public unsafe void* GetCoreWindow(Guid* refiid)
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 3])(base.PPV, refiid, &result);
		if (num != 0)
		{
			throw new COMException("GetCoreWindow failed", num);
		}
		return result;
	}

	public unsafe void Present1(ushort SyncInterval, ushort PresentFlags, DXGI_PRESENT_PARAMETERS* pPresentParameters)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, ushort, ushort, void*, int>)(*base.PPV)[base.VTableSize + 4])(base.PPV, SyncInterval, PresentFlags, pPresentParameters);
		if (num != 0)
		{
			throw new COMException("Present1 failed", num);
		}
	}

	public unsafe int IsTemporaryMonoSupported()
	{
		return ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 5])(base.PPV);
	}

	public unsafe void SetBackgroundColor(DXGI_RGBA* pColor)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 7])(base.PPV, pColor);
		if (num != 0)
		{
			throw new COMException("SetBackgroundColor failed", num);
		}
	}

	public unsafe void SetRotation(DXGI_MODE_ROTATION Rotation)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, DXGI_MODE_ROTATION, int>)(*base.PPV)[base.VTableSize + 9])(base.PPV, Rotation);
		if (num != 0)
		{
			throw new COMException("SetRotation failed", num);
		}
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IDXGISwapChain1), new Guid("790a45f7-0d42-4876-983a-0a55cfe6f4aa"), (IntPtr p, bool owns) => new __MicroComIDXGISwapChain1Proxy(p, owns));
	}

	protected __MicroComIDXGISwapChain1Proxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
