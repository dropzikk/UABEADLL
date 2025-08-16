using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnWindowEventsVTable : __MicroComIAvnWindowBaseEventsVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int ClosingDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void WindowStateChangedDelegate(void* @this, AvnWindowState state);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void GotInputWhenDisabledDelegate(void* @this);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int Closing(void* @this)
	{
		IAvnWindowEvents avnWindowEvents = null;
		try
		{
			avnWindowEvents = (IAvnWindowEvents)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnWindowEvents.Closing();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowEvents, e);
			return 0;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void WindowStateChanged(void* @this, AvnWindowState state)
	{
		IAvnWindowEvents avnWindowEvents = null;
		try
		{
			avnWindowEvents = (IAvnWindowEvents)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnWindowEvents.WindowStateChanged(state);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowEvents, e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void GotInputWhenDisabled(void* @this)
	{
		IAvnWindowEvents avnWindowEvents = null;
		try
		{
			avnWindowEvents = (IAvnWindowEvents)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnWindowEvents.GotInputWhenDisabled();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowEvents, e);
		}
	}

	protected unsafe __MicroComIAvnWindowEventsVTable()
	{
		AddMethod((delegate*<void*, int>)(&Closing));
		AddMethod((delegate*<void*, AvnWindowState, void>)(&WindowStateChanged));
		AddMethod((delegate*<void*, void>)(&GotInputWhenDisabled));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnWindowEvents), new __MicroComIAvnWindowEventsVTable().CreateVTable());
	}
}
