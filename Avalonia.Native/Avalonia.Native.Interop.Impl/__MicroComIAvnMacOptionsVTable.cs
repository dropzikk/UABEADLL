using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnMacOptionsVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetShowInDockDelegate(void* @this, int show);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetApplicationTitleDelegate(void* @this, byte* utf8string);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetDisableSetProcessNameDelegate(void* @this, int disable);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetDisableAppDelegateDelegate(void* @this, int disable);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetShowInDock(void* @this, int show)
	{
		IAvnMacOptions avnMacOptions = null;
		try
		{
			avnMacOptions = (IAvnMacOptions)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnMacOptions.SetShowInDock(show);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnMacOptions, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetApplicationTitle(void* @this, byte* utf8string)
	{
		IAvnMacOptions avnMacOptions = null;
		try
		{
			avnMacOptions = (IAvnMacOptions)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnMacOptions.SetApplicationTitle((utf8string == null) ? null : Marshal.PtrToStringAnsi(new IntPtr(utf8string)));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnMacOptions, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetDisableSetProcessName(void* @this, int disable)
	{
		IAvnMacOptions avnMacOptions = null;
		try
		{
			avnMacOptions = (IAvnMacOptions)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnMacOptions.SetDisableSetProcessName(disable);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnMacOptions, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetDisableAppDelegate(void* @this, int disable)
	{
		IAvnMacOptions avnMacOptions = null;
		try
		{
			avnMacOptions = (IAvnMacOptions)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnMacOptions.SetDisableAppDelegate(disable);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnMacOptions, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIAvnMacOptionsVTable()
	{
		AddMethod((delegate*<void*, int, int>)(&SetShowInDock));
		AddMethod((delegate*<void*, byte*, int>)(&SetApplicationTitle));
		AddMethod((delegate*<void*, int, int>)(&SetDisableSetProcessName));
		AddMethod((delegate*<void*, int, int>)(&SetDisableAppDelegate));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnMacOptions), new __MicroComIAvnMacOptionsVTable().CreateVTable());
	}
}
