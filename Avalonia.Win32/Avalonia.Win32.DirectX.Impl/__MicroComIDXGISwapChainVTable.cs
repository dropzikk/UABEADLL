using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX.Impl;

internal class __MicroComIDXGISwapChainVTable : __MicroComIDXGIDeviceSubObjectVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int PresentDelegate(void* @this, ushort SyncInterval, ushort Flags);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetBufferDelegate(void* @this, ushort Buffer, Guid* riid, void** ppSurface);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetFullscreenStateDelegate(void* @this, int Fullscreen, void* pTarget);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetFullscreenStateDelegate(void* @this, int* pFullscreen, void** ppTarget);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetDescDelegate(void* @this, DXGI_SWAP_CHAIN_DESC* pDesc);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int ResizeBuffersDelegate(void* @this, ushort BufferCount, ushort Width, ushort Height, DXGI_FORMAT NewFormat, ushort SwapChainFlags);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int ResizeTargetDelegate(void* @this, DXGI_MODE_DESC* pNewTargetParameters);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetContainingOutputDelegate(void* @this, void** ppOutput);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetFrameStatisticsDelegate(void* @this, DXGI_FRAME_STATISTICS* pStats);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetLastPresentCountDelegate(void* @this, ushort* pLastPresentCount);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int Present(void* @this, ushort SyncInterval, ushort Flags)
	{
		IDXGISwapChain iDXGISwapChain = null;
		try
		{
			iDXGISwapChain = (IDXGISwapChain)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			iDXGISwapChain.Present(SyncInterval, Flags);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGISwapChain, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetBuffer(void* @this, ushort Buffer, Guid* riid, void** ppSurface)
	{
		IDXGISwapChain iDXGISwapChain = null;
		try
		{
			iDXGISwapChain = (IDXGISwapChain)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* buffer = iDXGISwapChain.GetBuffer(Buffer, riid);
			*ppSurface = buffer;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGISwapChain, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetFullscreenState(void* @this, int Fullscreen, void* pTarget)
	{
		IDXGISwapChain iDXGISwapChain = null;
		try
		{
			iDXGISwapChain = (IDXGISwapChain)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			iDXGISwapChain.SetFullscreenState(Fullscreen, MicroComRuntime.CreateProxyOrNullFor<IDXGIOutput>(pTarget, ownsHandle: false));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGISwapChain, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetFullscreenState(void* @this, int* pFullscreen, void** ppTarget)
	{
		IDXGISwapChain iDXGISwapChain = null;
		try
		{
			iDXGISwapChain = (IDXGISwapChain)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IDXGIOutput fullscreenState = iDXGISwapChain.GetFullscreenState(pFullscreen);
			*ppTarget = MicroComRuntime.GetNativePointer(fullscreenState, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGISwapChain, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetDesc(void* @this, DXGI_SWAP_CHAIN_DESC* pDesc)
	{
		IDXGISwapChain iDXGISwapChain = null;
		try
		{
			iDXGISwapChain = (IDXGISwapChain)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			DXGI_SWAP_CHAIN_DESC desc = iDXGISwapChain.Desc;
			*pDesc = desc;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGISwapChain, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int ResizeBuffers(void* @this, ushort BufferCount, ushort Width, ushort Height, DXGI_FORMAT NewFormat, ushort SwapChainFlags)
	{
		IDXGISwapChain iDXGISwapChain = null;
		try
		{
			iDXGISwapChain = (IDXGISwapChain)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			iDXGISwapChain.ResizeBuffers(BufferCount, Width, Height, NewFormat, SwapChainFlags);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGISwapChain, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int ResizeTarget(void* @this, DXGI_MODE_DESC* pNewTargetParameters)
	{
		IDXGISwapChain iDXGISwapChain = null;
		try
		{
			iDXGISwapChain = (IDXGISwapChain)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			iDXGISwapChain.ResizeTarget(pNewTargetParameters);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGISwapChain, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetContainingOutput(void* @this, void** ppOutput)
	{
		IDXGISwapChain iDXGISwapChain = null;
		try
		{
			iDXGISwapChain = (IDXGISwapChain)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IDXGIOutput containingOutput = iDXGISwapChain.ContainingOutput;
			*ppOutput = MicroComRuntime.GetNativePointer(containingOutput, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGISwapChain, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetFrameStatistics(void* @this, DXGI_FRAME_STATISTICS* pStats)
	{
		IDXGISwapChain iDXGISwapChain = null;
		try
		{
			iDXGISwapChain = (IDXGISwapChain)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			DXGI_FRAME_STATISTICS frameStatistics = iDXGISwapChain.FrameStatistics;
			*pStats = frameStatistics;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGISwapChain, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetLastPresentCount(void* @this, ushort* pLastPresentCount)
	{
		IDXGISwapChain iDXGISwapChain = null;
		try
		{
			iDXGISwapChain = (IDXGISwapChain)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			ushort lastPresentCount = iDXGISwapChain.LastPresentCount;
			*pLastPresentCount = lastPresentCount;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGISwapChain, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIDXGISwapChainVTable()
	{
		AddMethod((delegate*<void*, ushort, ushort, int>)(&Present));
		AddMethod((delegate*<void*, ushort, Guid*, void**, int>)(&GetBuffer));
		AddMethod((delegate*<void*, int, void*, int>)(&SetFullscreenState));
		AddMethod((delegate*<void*, int*, void**, int>)(&GetFullscreenState));
		AddMethod((delegate*<void*, DXGI_SWAP_CHAIN_DESC*, int>)(&GetDesc));
		AddMethod((delegate*<void*, ushort, ushort, ushort, DXGI_FORMAT, ushort, int>)(&ResizeBuffers));
		AddMethod((delegate*<void*, DXGI_MODE_DESC*, int>)(&ResizeTarget));
		AddMethod((delegate*<void*, void**, int>)(&GetContainingOutput));
		AddMethod((delegate*<void*, DXGI_FRAME_STATISTICS*, int>)(&GetFrameStatistics));
		AddMethod((delegate*<void*, ushort*, int>)(&GetLastPresentCount));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IDXGISwapChain), new __MicroComIDXGISwapChainVTable().CreateVTable());
	}
}
