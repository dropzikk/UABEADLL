using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnPredicateCallbackVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int EvaluateDelegate(void* @this);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int Evaluate(void* @this)
	{
		IAvnPredicateCallback avnPredicateCallback = null;
		try
		{
			avnPredicateCallback = (IAvnPredicateCallback)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnPredicateCallback.Evaluate();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnPredicateCallback, e);
			return 0;
		}
	}

	protected unsafe __MicroComIAvnPredicateCallbackVTable()
	{
		AddMethod((delegate*<void*, int>)(&Evaluate));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnPredicateCallback), new __MicroComIAvnPredicateCallbackVTable().CreateVTable());
	}
}
