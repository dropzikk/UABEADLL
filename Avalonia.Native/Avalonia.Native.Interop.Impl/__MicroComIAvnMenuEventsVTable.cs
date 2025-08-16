using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnMenuEventsVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void NeedsUpdateDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void OpeningDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void ClosedDelegate(void* @this);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void NeedsUpdate(void* @this)
	{
		IAvnMenuEvents avnMenuEvents = null;
		try
		{
			avnMenuEvents = (IAvnMenuEvents)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnMenuEvents.NeedsUpdate();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnMenuEvents, e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void Opening(void* @this)
	{
		IAvnMenuEvents avnMenuEvents = null;
		try
		{
			avnMenuEvents = (IAvnMenuEvents)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnMenuEvents.Opening();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnMenuEvents, e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void Closed(void* @this)
	{
		IAvnMenuEvents avnMenuEvents = null;
		try
		{
			avnMenuEvents = (IAvnMenuEvents)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnMenuEvents.Closed();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnMenuEvents, e);
		}
	}

	protected unsafe __MicroComIAvnMenuEventsVTable()
	{
		AddMethod((delegate*<void*, void>)(&NeedsUpdate));
		AddMethod((delegate*<void*, void>)(&Opening));
		AddMethod((delegate*<void*, void>)(&Closed));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnMenuEvents), new __MicroComIAvnMenuEventsVTable().CreateVTable());
	}
}
