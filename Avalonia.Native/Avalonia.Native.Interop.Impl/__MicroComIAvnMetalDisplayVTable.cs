using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnMetalDisplayVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateDeviceDelegate(void* @this, void** ret);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateDevice(void* @this, void** ret)
	{
		IAvnMetalDisplay avnMetalDisplay = null;
		try
		{
			avnMetalDisplay = (IAvnMetalDisplay)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IAvnMetalDevice obj = avnMetalDisplay.CreateDevice();
			*ret = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnMetalDisplay, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIAvnMetalDisplayVTable()
	{
		AddMethod((delegate*<void*, void**, int>)(&CreateDevice));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnMetalDisplay), new __MicroComIAvnMetalDisplayVTable().CreateVTable());
	}
}
