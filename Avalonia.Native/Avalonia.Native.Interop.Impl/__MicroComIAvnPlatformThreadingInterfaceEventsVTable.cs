using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnPlatformThreadingInterfaceEventsVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void SignaledDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void TimerDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void ReadyForBackgroundProcessingDelegate(void* @this);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void Signaled(void* @this)
	{
		IAvnPlatformThreadingInterfaceEvents avnPlatformThreadingInterfaceEvents = null;
		try
		{
			avnPlatformThreadingInterfaceEvents = (IAvnPlatformThreadingInterfaceEvents)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnPlatformThreadingInterfaceEvents.Signaled();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnPlatformThreadingInterfaceEvents, e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void Timer(void* @this)
	{
		IAvnPlatformThreadingInterfaceEvents avnPlatformThreadingInterfaceEvents = null;
		try
		{
			avnPlatformThreadingInterfaceEvents = (IAvnPlatformThreadingInterfaceEvents)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnPlatformThreadingInterfaceEvents.Timer();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnPlatformThreadingInterfaceEvents, e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void ReadyForBackgroundProcessing(void* @this)
	{
		IAvnPlatformThreadingInterfaceEvents avnPlatformThreadingInterfaceEvents = null;
		try
		{
			avnPlatformThreadingInterfaceEvents = (IAvnPlatformThreadingInterfaceEvents)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnPlatformThreadingInterfaceEvents.ReadyForBackgroundProcessing();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnPlatformThreadingInterfaceEvents, e);
		}
	}

	protected unsafe __MicroComIAvnPlatformThreadingInterfaceEventsVTable()
	{
		AddMethod((delegate*<void*, void>)(&Signaled));
		AddMethod((delegate*<void*, void>)(&Timer));
		AddMethod((delegate*<void*, void>)(&ReadyForBackgroundProcessing));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnPlatformThreadingInterfaceEvents), new __MicroComIAvnPlatformThreadingInterfaceEventsVTable().CreateVTable());
	}
}
