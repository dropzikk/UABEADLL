using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnApplicationEventsVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void FilesOpenedDelegate(void* @this, void* urls);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int TryShutdownDelegate(void* @this);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void FilesOpened(void* @this, void* urls)
	{
		IAvnApplicationEvents avnApplicationEvents = null;
		try
		{
			avnApplicationEvents = (IAvnApplicationEvents)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnApplicationEvents.FilesOpened(MicroComRuntime.CreateProxyOrNullFor<IAvnStringArray>(urls, ownsHandle: false));
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnApplicationEvents, e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int TryShutdown(void* @this)
	{
		IAvnApplicationEvents avnApplicationEvents = null;
		try
		{
			avnApplicationEvents = (IAvnApplicationEvents)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnApplicationEvents.TryShutdown();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnApplicationEvents, e);
			return 0;
		}
	}

	protected unsafe __MicroComIAvnApplicationEventsVTable()
	{
		AddMethod((delegate*<void*, void*, void>)(&FilesOpened));
		AddMethod((delegate*<void*, int>)(&TryShutdown));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnApplicationEvents), new __MicroComIAvnApplicationEventsVTable().CreateVTable());
	}
}
