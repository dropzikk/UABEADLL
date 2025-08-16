using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnTrayIconVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetIconDelegate(void* @this, void* data, IntPtr length);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetMenuDelegate(void* @this, void* menu);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetIsVisibleDelegate(void* @this, int isVisible);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetIcon(void* @this, void* data, IntPtr length)
	{
		IAvnTrayIcon avnTrayIcon = null;
		try
		{
			avnTrayIcon = (IAvnTrayIcon)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnTrayIcon.SetIcon(data, length);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnTrayIcon, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetMenu(void* @this, void* menu)
	{
		IAvnTrayIcon avnTrayIcon = null;
		try
		{
			avnTrayIcon = (IAvnTrayIcon)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnTrayIcon.SetMenu(MicroComRuntime.CreateProxyOrNullFor<IAvnMenu>(menu, ownsHandle: false));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnTrayIcon, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetIsVisible(void* @this, int isVisible)
	{
		IAvnTrayIcon avnTrayIcon = null;
		try
		{
			avnTrayIcon = (IAvnTrayIcon)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnTrayIcon.SetIsVisible(isVisible);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnTrayIcon, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIAvnTrayIconVTable()
	{
		AddMethod((delegate*<void*, void*, IntPtr, int>)(&SetIcon));
		AddMethod((delegate*<void*, void*, int>)(&SetMenu));
		AddMethod((delegate*<void*, int, int>)(&SetIsVisible));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnTrayIcon), new __MicroComIAvnTrayIconVTable().CreateVTable());
	}
}
