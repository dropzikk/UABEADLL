using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX.Impl;

internal class __MicroComIDXGIDeviceVTable : __MicroComIDXGIObjectVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetAdapterDelegate(void* @this, void** pAdapter);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateSurfaceDelegate(void* @this, DXGI_SURFACE_DESC* pDesc, ushort NumSurfaces, uint Usage, void** pSharedResource, void** ppSurface);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int QueryResourceResidencyDelegate(void* @this, void* ppResources, DXGI_RESIDENCY* pResidencyStatus, ushort NumResources);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetGPUThreadPriorityDelegate(void* @this, int Priority);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetGPUThreadPriorityDelegate(void* @this, int* pPriority);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetAdapter(void* @this, void** pAdapter)
	{
		IDXGIDevice iDXGIDevice = null;
		try
		{
			iDXGIDevice = (IDXGIDevice)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IDXGIAdapter adapter = iDXGIDevice.Adapter;
			*pAdapter = MicroComRuntime.GetNativePointer(adapter, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIDevice, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateSurface(void* @this, DXGI_SURFACE_DESC* pDesc, ushort NumSurfaces, uint Usage, void** pSharedResource, void** ppSurface)
	{
		IDXGIDevice iDXGIDevice = null;
		try
		{
			iDXGIDevice = (IDXGIDevice)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IDXGISurface obj = iDXGIDevice.CreateSurface(pDesc, NumSurfaces, Usage, pSharedResource);
			*ppSurface = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIDevice, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int QueryResourceResidency(void* @this, void* ppResources, DXGI_RESIDENCY* pResidencyStatus, ushort NumResources)
	{
		IDXGIDevice iDXGIDevice = null;
		try
		{
			iDXGIDevice = (IDXGIDevice)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			iDXGIDevice.QueryResourceResidency(MicroComRuntime.CreateProxyOrNullFor<IUnknown>(ppResources, ownsHandle: false), pResidencyStatus, NumResources);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIDevice, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetGPUThreadPriority(void* @this, int Priority)
	{
		IDXGIDevice iDXGIDevice = null;
		try
		{
			iDXGIDevice = (IDXGIDevice)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			iDXGIDevice.SetGPUThreadPriority(Priority);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIDevice, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetGPUThreadPriority(void* @this, int* pPriority)
	{
		IDXGIDevice iDXGIDevice = null;
		try
		{
			iDXGIDevice = (IDXGIDevice)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			int gPUThreadPriority = iDXGIDevice.GPUThreadPriority;
			*pPriority = gPUThreadPriority;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIDevice, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIDXGIDeviceVTable()
	{
		AddMethod((delegate*<void*, void**, int>)(&GetAdapter));
		AddMethod((delegate*<void*, DXGI_SURFACE_DESC*, ushort, uint, void**, void**, int>)(&CreateSurface));
		AddMethod((delegate*<void*, void*, DXGI_RESIDENCY*, ushort, int>)(&QueryResourceResidency));
		AddMethod((delegate*<void*, int, int>)(&SetGPUThreadPriority));
		AddMethod((delegate*<void*, int*, int>)(&GetGPUThreadPriority));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IDXGIDevice), new __MicroComIDXGIDeviceVTable().CreateVTable());
	}
}
