using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX.Impl;

internal class __MicroComIDXGISwapChain1VTable : __MicroComIDXGISwapChainVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetDesc1Delegate(void* @this, DXGI_SWAP_CHAIN_DESC1* pDesc);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetFullscreenDescDelegate(void* @this, DXGI_SWAP_CHAIN_FULLSCREEN_DESC* pDesc);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetHwndDelegate(void* @this, IntPtr* pHwnd);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetCoreWindowDelegate(void* @this, Guid* refiid, void** ppUnk);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int Present1Delegate(void* @this, ushort SyncInterval, ushort PresentFlags, DXGI_PRESENT_PARAMETERS* pPresentParameters);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int IsTemporaryMonoSupportedDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetRestrictToOutputDelegate(void* @this, void** ppRestrictToOutput);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetBackgroundColorDelegate(void* @this, DXGI_RGBA* pColor);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetBackgroundColorDelegate(void* @this, DXGI_RGBA* pColor);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetRotationDelegate(void* @this, DXGI_MODE_ROTATION Rotation);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetRotationDelegate(void* @this, DXGI_MODE_ROTATION* pRotation);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetDesc1(void* @this, DXGI_SWAP_CHAIN_DESC1* pDesc)
	{
		IDXGISwapChain1 iDXGISwapChain = null;
		try
		{
			iDXGISwapChain = (IDXGISwapChain1)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			DXGI_SWAP_CHAIN_DESC1 desc = iDXGISwapChain.Desc1;
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
	private unsafe static int GetFullscreenDesc(void* @this, DXGI_SWAP_CHAIN_FULLSCREEN_DESC* pDesc)
	{
		IDXGISwapChain1 iDXGISwapChain = null;
		try
		{
			iDXGISwapChain = (IDXGISwapChain1)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			DXGI_SWAP_CHAIN_FULLSCREEN_DESC fullscreenDesc = iDXGISwapChain.FullscreenDesc;
			*pDesc = fullscreenDesc;
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
	private unsafe static int GetHwnd(void* @this, IntPtr* pHwnd)
	{
		IDXGISwapChain1 iDXGISwapChain = null;
		try
		{
			iDXGISwapChain = (IDXGISwapChain1)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IntPtr hwnd = iDXGISwapChain.Hwnd;
			*pHwnd = hwnd;
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
	private unsafe static int GetCoreWindow(void* @this, Guid* refiid, void** ppUnk)
	{
		IDXGISwapChain1 iDXGISwapChain = null;
		try
		{
			iDXGISwapChain = (IDXGISwapChain1)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* coreWindow = iDXGISwapChain.GetCoreWindow(refiid);
			*ppUnk = coreWindow;
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
	private unsafe static int Present1(void* @this, ushort SyncInterval, ushort PresentFlags, DXGI_PRESENT_PARAMETERS* pPresentParameters)
	{
		IDXGISwapChain1 iDXGISwapChain = null;
		try
		{
			iDXGISwapChain = (IDXGISwapChain1)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			iDXGISwapChain.Present1(SyncInterval, PresentFlags, pPresentParameters);
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
	private unsafe static int IsTemporaryMonoSupported(void* @this)
	{
		IDXGISwapChain1 iDXGISwapChain = null;
		try
		{
			iDXGISwapChain = (IDXGISwapChain1)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return iDXGISwapChain.IsTemporaryMonoSupported();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGISwapChain, e);
			return 0;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetRestrictToOutput(void* @this, void** ppRestrictToOutput)
	{
		IDXGISwapChain1 iDXGISwapChain = null;
		try
		{
			iDXGISwapChain = (IDXGISwapChain1)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IDXGIOutput restrictToOutput = iDXGISwapChain.RestrictToOutput;
			*ppRestrictToOutput = MicroComRuntime.GetNativePointer(restrictToOutput, owned: true);
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
	private unsafe static int SetBackgroundColor(void* @this, DXGI_RGBA* pColor)
	{
		IDXGISwapChain1 iDXGISwapChain = null;
		try
		{
			iDXGISwapChain = (IDXGISwapChain1)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			iDXGISwapChain.SetBackgroundColor(pColor);
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
	private unsafe static int GetBackgroundColor(void* @this, DXGI_RGBA* pColor)
	{
		IDXGISwapChain1 iDXGISwapChain = null;
		try
		{
			iDXGISwapChain = (IDXGISwapChain1)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			DXGI_RGBA backgroundColor = iDXGISwapChain.BackgroundColor;
			*pColor = backgroundColor;
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
	private unsafe static int SetRotation(void* @this, DXGI_MODE_ROTATION Rotation)
	{
		IDXGISwapChain1 iDXGISwapChain = null;
		try
		{
			iDXGISwapChain = (IDXGISwapChain1)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			iDXGISwapChain.SetRotation(Rotation);
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
	private unsafe static int GetRotation(void* @this, DXGI_MODE_ROTATION* pRotation)
	{
		IDXGISwapChain1 iDXGISwapChain = null;
		try
		{
			iDXGISwapChain = (IDXGISwapChain1)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			DXGI_MODE_ROTATION rotation = iDXGISwapChain.Rotation;
			*pRotation = rotation;
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

	protected unsafe __MicroComIDXGISwapChain1VTable()
	{
		AddMethod((delegate*<void*, DXGI_SWAP_CHAIN_DESC1*, int>)(&GetDesc1));
		AddMethod((delegate*<void*, DXGI_SWAP_CHAIN_FULLSCREEN_DESC*, int>)(&GetFullscreenDesc));
		AddMethod((delegate*<void*, IntPtr*, int>)(&GetHwnd));
		AddMethod((delegate*<void*, Guid*, void**, int>)(&GetCoreWindow));
		AddMethod((delegate*<void*, ushort, ushort, DXGI_PRESENT_PARAMETERS*, int>)(&Present1));
		AddMethod((delegate*<void*, int>)(&IsTemporaryMonoSupported));
		AddMethod((delegate*<void*, void**, int>)(&GetRestrictToOutput));
		AddMethod((delegate*<void*, DXGI_RGBA*, int>)(&SetBackgroundColor));
		AddMethod((delegate*<void*, DXGI_RGBA*, int>)(&GetBackgroundColor));
		AddMethod((delegate*<void*, DXGI_MODE_ROTATION, int>)(&SetRotation));
		AddMethod((delegate*<void*, DXGI_MODE_ROTATION*, int>)(&GetRotation));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IDXGISwapChain1), new __MicroComIDXGISwapChain1VTable().CreateVTable());
	}
}
