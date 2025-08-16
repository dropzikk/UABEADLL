using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnActionCallbackVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void RunDelegate(void* @this);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void Run(void* @this)
	{
		IAvnActionCallback avnActionCallback = null;
		try
		{
			avnActionCallback = (IAvnActionCallback)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnActionCallback.Run();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnActionCallback, e);
		}
	}

	protected unsafe __MicroComIAvnActionCallbackVTable()
	{
		AddMethod((delegate*<void*, void>)(&Run));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnActionCallback), new __MicroComIAvnActionCallbackVTable().CreateVTable());
	}
}
