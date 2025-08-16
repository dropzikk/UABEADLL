using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX.Impl;

internal class __MicroComIDXGIAdapter1VTable : __MicroComIDXGIAdapterVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetDesc1Delegate(void* @this, DXGI_ADAPTER_DESC1* pDesc);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetDesc1(void* @this, DXGI_ADAPTER_DESC1* pDesc)
	{
		IDXGIAdapter1 iDXGIAdapter = null;
		try
		{
			iDXGIAdapter = (IDXGIAdapter1)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			DXGI_ADAPTER_DESC1 desc = iDXGIAdapter.Desc1;
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

	protected unsafe __MicroComIDXGIAdapter1VTable()
	{
		AddMethod((delegate*<void*, DXGI_ADAPTER_DESC1*, int>)(&GetDesc1));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IDXGIAdapter1), new __MicroComIDXGIAdapter1VTable().CreateVTable());
	}
}
