using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnMetalDeviceVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate IntPtr GetDeviceDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate IntPtr GetQueueDelegate(void* @this);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static IntPtr GetDevice(void* @this)
	{
		IAvnMetalDevice avnMetalDevice = null;
		try
		{
			avnMetalDevice = (IAvnMetalDevice)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnMetalDevice.Device;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnMetalDevice, e);
			return (IntPtr)0;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static IntPtr GetQueue(void* @this)
	{
		IAvnMetalDevice avnMetalDevice = null;
		try
		{
			avnMetalDevice = (IAvnMetalDevice)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnMetalDevice.Queue;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnMetalDevice, e);
			return (IntPtr)0;
		}
	}

	protected unsafe __MicroComIAvnMetalDeviceVTable()
	{
		AddMethod((delegate*<void*, IntPtr>)(&GetDevice));
		AddMethod((delegate*<void*, IntPtr>)(&GetQueue));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnMetalDevice), new __MicroComIAvnMetalDeviceVTable().CreateVTable());
	}
}
