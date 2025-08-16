using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnPlatformThreadingInterfaceVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetCurrentThreadIsLoopThreadDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void SetEventsDelegate(void* @this, void* cb);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void* CreateLoopCancellationDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void RunLoopDelegate(void* @this, void* cancel);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void SignalDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void UpdateTimerDelegate(void* @this, int ms);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void RequestBackgroundProcessingDelegate(void* @this);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetCurrentThreadIsLoopThread(void* @this)
	{
		IAvnPlatformThreadingInterface avnPlatformThreadingInterface = null;
		try
		{
			avnPlatformThreadingInterface = (IAvnPlatformThreadingInterface)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnPlatformThreadingInterface.CurrentThreadIsLoopThread;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnPlatformThreadingInterface, e);
			return 0;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void SetEvents(void* @this, void* cb)
	{
		IAvnPlatformThreadingInterface avnPlatformThreadingInterface = null;
		try
		{
			avnPlatformThreadingInterface = (IAvnPlatformThreadingInterface)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnPlatformThreadingInterface.SetEvents(MicroComRuntime.CreateProxyOrNullFor<IAvnPlatformThreadingInterfaceEvents>(cb, ownsHandle: false));
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnPlatformThreadingInterface, e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void* CreateLoopCancellation(void* @this)
	{
		IAvnPlatformThreadingInterface avnPlatformThreadingInterface = null;
		try
		{
			avnPlatformThreadingInterface = (IAvnPlatformThreadingInterface)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return MicroComRuntime.GetNativePointer(avnPlatformThreadingInterface.CreateLoopCancellation(), owned: true);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnPlatformThreadingInterface, e);
			return null;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void RunLoop(void* @this, void* cancel)
	{
		IAvnPlatformThreadingInterface avnPlatformThreadingInterface = null;
		try
		{
			avnPlatformThreadingInterface = (IAvnPlatformThreadingInterface)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnPlatformThreadingInterface.RunLoop(MicroComRuntime.CreateProxyOrNullFor<IAvnLoopCancellation>(cancel, ownsHandle: false));
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnPlatformThreadingInterface, e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void Signal(void* @this)
	{
		IAvnPlatformThreadingInterface avnPlatformThreadingInterface = null;
		try
		{
			avnPlatformThreadingInterface = (IAvnPlatformThreadingInterface)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnPlatformThreadingInterface.Signal();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnPlatformThreadingInterface, e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void UpdateTimer(void* @this, int ms)
	{
		IAvnPlatformThreadingInterface avnPlatformThreadingInterface = null;
		try
		{
			avnPlatformThreadingInterface = (IAvnPlatformThreadingInterface)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnPlatformThreadingInterface.UpdateTimer(ms);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnPlatformThreadingInterface, e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void RequestBackgroundProcessing(void* @this)
	{
		IAvnPlatformThreadingInterface avnPlatformThreadingInterface = null;
		try
		{
			avnPlatformThreadingInterface = (IAvnPlatformThreadingInterface)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnPlatformThreadingInterface.RequestBackgroundProcessing();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnPlatformThreadingInterface, e);
		}
	}

	protected unsafe __MicroComIAvnPlatformThreadingInterfaceVTable()
	{
		AddMethod((delegate*<void*, int>)(&GetCurrentThreadIsLoopThread));
		AddMethod((delegate*<void*, void*, void>)(&SetEvents));
		AddMethod((delegate*<void*, void*>)(&CreateLoopCancellation));
		AddMethod((delegate*<void*, void*, void>)(&RunLoop));
		AddMethod((delegate*<void*, void>)(&Signal));
		AddMethod((delegate*<void*, int, void>)(&UpdateTimer));
		AddMethod((delegate*<void*, void>)(&RequestBackgroundProcessing));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnPlatformThreadingInterface), new __MicroComIAvnPlatformThreadingInterfaceVTable().CreateVTable());
	}
}
