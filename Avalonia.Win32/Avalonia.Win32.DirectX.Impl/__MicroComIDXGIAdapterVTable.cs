using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX.Impl;

internal class __MicroComIDXGIAdapterVTable : __MicroComIDXGIObjectVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int EnumOutputsDelegate(void* @this, ushort Output, void* ppOutput);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetDescDelegate(void* @this, DXGI_ADAPTER_DESC* pDesc);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CheckInterfaceSupportDelegate(void* @this, Guid* InterfaceName, ulong* pUMDVersion);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int EnumOutputs(void* @this, ushort Output, void* ppOutput)
	{
		IDXGIAdapter iDXGIAdapter = null;
		try
		{
			iDXGIAdapter = (IDXGIAdapter)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return iDXGIAdapter.EnumOutputs(Output, ppOutput);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIAdapter, e);
			return 0;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetDesc(void* @this, DXGI_ADAPTER_DESC* pDesc)
	{
		IDXGIAdapter iDXGIAdapter = null;
		try
		{
			iDXGIAdapter = (IDXGIAdapter)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			DXGI_ADAPTER_DESC desc = iDXGIAdapter.Desc;
			*pDesc = desc;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIAdapter, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CheckInterfaceSupport(void* @this, Guid* InterfaceName, ulong* pUMDVersion)
	{
		IDXGIAdapter iDXGIAdapter = null;
		try
		{
			iDXGIAdapter = (IDXGIAdapter)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			ulong num = iDXGIAdapter.CheckInterfaceSupport(InterfaceName);
			*pUMDVersion = num;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIAdapter, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIDXGIAdapterVTable()
	{
		AddMethod((delegate*<void*, ushort, void*, int>)(&EnumOutputs));
		AddMethod((delegate*<void*, DXGI_ADAPTER_DESC*, int>)(&GetDesc));
		AddMethod((delegate*<void*, Guid*, ulong*, int>)(&CheckInterfaceSupport));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IDXGIAdapter), new __MicroComIDXGIAdapterVTable().CreateVTable());
	}
}
