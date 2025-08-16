using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX.Impl;

internal class __MicroComIDXGIFactoryVTable : __MicroComIDXGIObjectVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int EnumAdaptersDelegate(void* @this, ushort Adapter, void* ppAdapter);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int MakeWindowAssociationDelegate(void* @this, IntPtr WindowHandle, ushort Flags);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetWindowAssociationDelegate(void* @this, IntPtr* pWindowHandle);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateSwapChainDelegate(void* @this, void* pDevice, DXGI_SWAP_CHAIN_DESC* pDesc, void** ppSwapChain);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateSoftwareAdapterDelegate(void* @this, void* Module, void** ppAdapter);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int EnumAdapters(void* @this, ushort Adapter, void* ppAdapter)
	{
		IDXGIFactory iDXGIFactory = null;
		try
		{
			iDXGIFactory = (IDXGIFactory)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return iDXGIFactory.EnumAdapters(Adapter, ppAdapter);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIFactory, e);
			return 0;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int MakeWindowAssociation(void* @this, IntPtr WindowHandle, ushort Flags)
	{
		IDXGIFactory iDXGIFactory = null;
		try
		{
			iDXGIFactory = (IDXGIFactory)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			iDXGIFactory.MakeWindowAssociation(WindowHandle, Flags);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIFactory, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetWindowAssociation(void* @this, IntPtr* pWindowHandle)
	{
		IDXGIFactory iDXGIFactory = null;
		try
		{
			iDXGIFactory = (IDXGIFactory)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IntPtr windowAssociation = iDXGIFactory.WindowAssociation;
			*pWindowHandle = windowAssociation;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIFactory, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateSwapChain(void* @this, void* pDevice, DXGI_SWAP_CHAIN_DESC* pDesc, void** ppSwapChain)
	{
		IDXGIFactory iDXGIFactory = null;
		try
		{
			iDXGIFactory = (IDXGIFactory)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IDXGISwapChain obj = iDXGIFactory.CreateSwapChain(MicroComRuntime.CreateProxyOrNullFor<IUnknown>(pDevice, ownsHandle: false), pDesc);
			*ppSwapChain = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIFactory, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateSoftwareAdapter(void* @this, void* Module, void** ppAdapter)
	{
		IDXGIFactory iDXGIFactory = null;
		try
		{
			iDXGIFactory = (IDXGIFactory)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IDXGIAdapter obj = iDXGIFactory.CreateSoftwareAdapter(Module);
			*ppAdapter = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIFactory, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIDXGIFactoryVTable()
	{
		AddMethod((delegate*<void*, ushort, void*, int>)(&EnumAdapters));
		AddMethod((delegate*<void*, IntPtr, ushort, int>)(&MakeWindowAssociation));
		AddMethod((delegate*<void*, IntPtr*, int>)(&GetWindowAssociation));
		AddMethod((delegate*<void*, void*, DXGI_SWAP_CHAIN_DESC*, void**, int>)(&CreateSwapChain));
		AddMethod((delegate*<void*, void*, void**, int>)(&CreateSoftwareAdapter));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IDXGIFactory), new __MicroComIDXGIFactoryVTable().CreateVTable());
	}
}
