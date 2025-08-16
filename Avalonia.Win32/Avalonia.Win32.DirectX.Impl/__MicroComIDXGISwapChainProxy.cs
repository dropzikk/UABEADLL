using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX.Impl;

internal class __MicroComIDXGISwapChainProxy : __MicroComIDXGIDeviceSubObjectProxy, IDXGISwapChain, IDXGIDeviceSubObject, IDXGIObject, IUnknown, IDisposable
{
	public unsafe DXGI_SWAP_CHAIN_DESC Desc
	{
		get
		{
			DXGI_SWAP_CHAIN_DESC result = default(DXGI_SWAP_CHAIN_DESC);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 4])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetDesc failed", num);
			}
			return result;
		}
	}

	public unsafe IDXGIOutput ContainingOutput
	{
		get
		{
			void* pObject = null;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 7])(base.PPV, &pObject);
			if (num != 0)
			{
				throw new COMException("GetContainingOutput failed", num);
			}
			return MicroComRuntime.CreateProxyOrNullFor<IDXGIOutput>(pObject, ownsHandle: true);
		}
	}

	public unsafe DXGI_FRAME_STATISTICS FrameStatistics
	{
		get
		{
			DXGI_FRAME_STATISTICS result = default(DXGI_FRAME_STATISTICS);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 8])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetFrameStatistics failed", num);
			}
			return result;
		}
	}

	public unsafe ushort LastPresentCount
	{
		get
		{
			ushort result = 0;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 9])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetLastPresentCount failed", num);
			}
			return result;
		}
	}

	protected override int VTableSize => base.VTableSize + 10;

	public unsafe void Present(ushort SyncInterval, ushort Flags)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, ushort, ushort, int>)(*base.PPV)[base.VTableSize])(base.PPV, SyncInterval, Flags);
		if (num != 0)
		{
			throw new COMException("Present failed", num);
		}
	}

	public unsafe void* GetBuffer(ushort Buffer, Guid* riid)
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, ushort, void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, Buffer, riid, &result);
		if (num != 0)
		{
			throw new COMException("GetBuffer failed", num);
		}
		return result;
	}

	public unsafe void SetFullscreenState(int Fullscreen, IDXGIOutput pTarget)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int, void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, Fullscreen, MicroComRuntime.GetNativePointer(pTarget));
		if (num != 0)
		{
			throw new COMException("SetFullscreenState failed", num);
		}
	}

	public unsafe IDXGIOutput GetFullscreenState(int* pFullscreen)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 3])(base.PPV, pFullscreen, &pObject);
		if (num != 0)
		{
			throw new COMException("GetFullscreenState failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IDXGIOutput>(pObject, ownsHandle: true);
	}

	public unsafe void ResizeBuffers(ushort BufferCount, ushort Width, ushort Height, DXGI_FORMAT NewFormat, ushort SwapChainFlags)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, ushort, ushort, ushort, DXGI_FORMAT, ushort, int>)(*base.PPV)[base.VTableSize + 5])(base.PPV, BufferCount, Width, Height, NewFormat, SwapChainFlags);
		if (num != 0)
		{
			throw new COMException("ResizeBuffers failed", num);
		}
	}

	public unsafe void ResizeTarget(DXGI_MODE_DESC* pNewTargetParameters)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 6])(base.PPV, pNewTargetParameters);
		if (num != 0)
		{
			throw new COMException("ResizeTarget failed", num);
		}
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IDXGISwapChain), new Guid("310d36a0-d2e7-4c0a-aa04-6a9d23b8886a"), (IntPtr p, bool owns) => new __MicroComIDXGISwapChainProxy(p, owns));
	}

	protected __MicroComIDXGISwapChainProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
