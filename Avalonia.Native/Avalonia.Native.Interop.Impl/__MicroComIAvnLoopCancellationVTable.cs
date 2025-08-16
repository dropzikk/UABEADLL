using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnLoopCancellationVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void CancelDelegate(void* @this);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void Cancel(void* @this)
	{
		IAvnLoopCancellation avnLoopCancellation = null;
		try
		{
			avnLoopCancellation = (IAvnLoopCancellation)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnLoopCancellation.Cancel();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnLoopCancellation, e);
		}
	}

	protected unsafe __MicroComIAvnLoopCancellationVTable()
	{
		AddMethod((delegate*<void*, void>)(&Cancel));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnLoopCancellation), new __MicroComIAvnLoopCancellationVTable().CreateVTable());
	}
}
