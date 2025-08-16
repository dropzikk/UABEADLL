using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionGraphicsDeviceInteropVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetRenderingDeviceDelegate(void* @this, void** value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetRenderingDeviceDelegate(void* @this, void* value);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetRenderingDevice(void* @this, void** value)
	{
		ICompositionGraphicsDeviceInterop compositionGraphicsDeviceInterop = null;
		try
		{
			compositionGraphicsDeviceInterop = (ICompositionGraphicsDeviceInterop)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IUnknown renderingDevice = compositionGraphicsDeviceInterop.RenderingDevice;
			*value = MicroComRuntime.GetNativePointer(renderingDevice, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionGraphicsDeviceInterop, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetRenderingDevice(void* @this, void* value)
	{
		ICompositionGraphicsDeviceInterop compositionGraphicsDeviceInterop = null;
		try
		{
			compositionGraphicsDeviceInterop = (ICompositionGraphicsDeviceInterop)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionGraphicsDeviceInterop.SetRenderingDevice(MicroComRuntime.CreateProxyOrNullFor<IUnknown>(value, ownsHandle: false));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionGraphicsDeviceInterop, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComICompositionGraphicsDeviceInteropVTable()
	{
		AddMethod((delegate*<void*, void**, int>)(&GetRenderingDevice));
		AddMethod((delegate*<void*, void*, int>)(&SetRenderingDevice));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(ICompositionGraphicsDeviceInterop), new __MicroComICompositionGraphicsDeviceInteropVTable().CreateVTable());
	}
}
