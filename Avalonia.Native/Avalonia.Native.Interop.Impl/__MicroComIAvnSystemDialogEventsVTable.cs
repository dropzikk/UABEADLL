using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnSystemDialogEventsVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void OnCompletedDelegate(void* @this, int numResults, void* ptrFirstResult);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void OnCompleted(void* @this, int numResults, void* ptrFirstResult)
	{
		IAvnSystemDialogEvents avnSystemDialogEvents = null;
		try
		{
			avnSystemDialogEvents = (IAvnSystemDialogEvents)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnSystemDialogEvents.OnCompleted(numResults, ptrFirstResult);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnSystemDialogEvents, e);
		}
	}

	protected unsafe __MicroComIAvnSystemDialogEventsVTable()
	{
		AddMethod((delegate*<void*, int, void*, void>)(&OnCompleted));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnSystemDialogEvents), new __MicroComIAvnSystemDialogEventsVTable().CreateVTable());
	}
}
