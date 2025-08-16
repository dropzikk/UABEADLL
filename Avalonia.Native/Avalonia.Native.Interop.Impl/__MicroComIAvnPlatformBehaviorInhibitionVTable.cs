using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnPlatformBehaviorInhibitionVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void SetInhibitAppSleepDelegate(void* @this, int inhibitAppSleep, byte* reason);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void SetInhibitAppSleep(void* @this, int inhibitAppSleep, byte* reason)
	{
		IAvnPlatformBehaviorInhibition avnPlatformBehaviorInhibition = null;
		try
		{
			avnPlatformBehaviorInhibition = (IAvnPlatformBehaviorInhibition)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnPlatformBehaviorInhibition.SetInhibitAppSleep(inhibitAppSleep, (reason == null) ? null : Marshal.PtrToStringAnsi(new IntPtr(reason)));
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnPlatformBehaviorInhibition, e);
		}
	}

	protected unsafe __MicroComIAvnPlatformBehaviorInhibitionVTable()
	{
		AddMethod((delegate*<void*, int, byte*, void>)(&SetInhibitAppSleep));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnPlatformBehaviorInhibition), new __MicroComIAvnPlatformBehaviorInhibitionVTable().CreateVTable());
	}
}
