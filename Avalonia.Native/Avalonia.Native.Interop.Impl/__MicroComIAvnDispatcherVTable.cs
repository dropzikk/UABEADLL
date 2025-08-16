using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnDispatcherVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void PostDelegate(void* @this, void* cb);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void Post(void* @this, void* cb)
	{
		IAvnDispatcher avnDispatcher = null;
		try
		{
			avnDispatcher = (IAvnDispatcher)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnDispatcher.Post(MicroComRuntime.CreateProxyOrNullFor<IAvnActionCallback>(cb, ownsHandle: false));
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnDispatcher, e);
		}
	}

	protected unsafe __MicroComIAvnDispatcherVTable()
	{
		AddMethod((delegate*<void*, void*, void>)(&Post));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnDispatcher), new __MicroComIAvnDispatcherVTable().CreateVTable());
	}
}
