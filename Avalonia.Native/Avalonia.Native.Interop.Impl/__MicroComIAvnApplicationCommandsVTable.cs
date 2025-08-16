using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnApplicationCommandsVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int HideAppDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int ShowAllDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int HideOthersDelegate(void* @this);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int HideApp(void* @this)
	{
		IAvnApplicationCommands avnApplicationCommands = null;
		try
		{
			avnApplicationCommands = (IAvnApplicationCommands)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnApplicationCommands.HideApp();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnApplicationCommands, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int ShowAll(void* @this)
	{
		IAvnApplicationCommands avnApplicationCommands = null;
		try
		{
			avnApplicationCommands = (IAvnApplicationCommands)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnApplicationCommands.ShowAll();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnApplicationCommands, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int HideOthers(void* @this)
	{
		IAvnApplicationCommands avnApplicationCommands = null;
		try
		{
			avnApplicationCommands = (IAvnApplicationCommands)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnApplicationCommands.HideOthers();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnApplicationCommands, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIAvnApplicationCommandsVTable()
	{
		AddMethod((delegate*<void*, int>)(&HideApp));
		AddMethod((delegate*<void*, int>)(&ShowAll));
		AddMethod((delegate*<void*, int>)(&HideOthers));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnApplicationCommands), new __MicroComIAvnApplicationCommandsVTable().CreateVTable());
	}
}
