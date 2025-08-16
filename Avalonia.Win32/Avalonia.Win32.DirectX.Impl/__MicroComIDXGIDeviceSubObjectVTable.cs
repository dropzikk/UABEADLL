using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX.Impl;

internal class __MicroComIDXGIDeviceSubObjectVTable : __MicroComIDXGIObjectVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetDeviceDelegate(void* @this, Guid* riid, void** ppDevice);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetDevice(void* @this, Guid* riid, void** ppDevice)
	{
		IDXGIDeviceSubObject iDXGIDeviceSubObject = null;
		try
		{
			iDXGIDeviceSubObject = (IDXGIDeviceSubObject)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* device = iDXGIDeviceSubObject.GetDevice(riid);
			*ppDevice = device;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIDeviceSubObject, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIDXGIDeviceSubObjectVTable()
	{
		AddMethod((delegate*<void*, Guid*, void**, int>)(&GetDevice));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IDXGIDeviceSubObject), new __MicroComIDXGIDeviceSubObjectVTable().CreateVTable());
	}
}
