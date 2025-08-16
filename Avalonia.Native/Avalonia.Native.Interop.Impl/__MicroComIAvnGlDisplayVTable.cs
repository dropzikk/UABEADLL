using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnGlDisplayVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateContextDelegate(void* @this, void* share, void** ppv);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void LegacyClearCurrentContextDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int WrapContextDelegate(void* @this, IntPtr native, void** ppv);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate IntPtr GetProcAddressDelegate(void* @this, byte* proc);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateContext(void* @this, void* share, void** ppv)
	{
		IAvnGlDisplay avnGlDisplay = null;
		try
		{
			avnGlDisplay = (IAvnGlDisplay)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IAvnGlContext obj = avnGlDisplay.CreateContext(MicroComRuntime.CreateProxyOrNullFor<IAvnGlContext>(share, ownsHandle: false));
			*ppv = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnGlDisplay, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void LegacyClearCurrentContext(void* @this)
	{
		IAvnGlDisplay avnGlDisplay = null;
		try
		{
			avnGlDisplay = (IAvnGlDisplay)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnGlDisplay.LegacyClearCurrentContext();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnGlDisplay, e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int WrapContext(void* @this, IntPtr native, void** ppv)
	{
		IAvnGlDisplay avnGlDisplay = null;
		try
		{
			avnGlDisplay = (IAvnGlDisplay)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IAvnGlContext obj = avnGlDisplay.WrapContext(native);
			*ppv = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnGlDisplay, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static IntPtr GetProcAddress(void* @this, byte* proc)
	{
		IAvnGlDisplay avnGlDisplay = null;
		try
		{
			avnGlDisplay = (IAvnGlDisplay)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnGlDisplay.GetProcAddress((proc == null) ? null : Marshal.PtrToStringAnsi(new IntPtr(proc)));
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnGlDisplay, e);
			return (IntPtr)0;
		}
	}

	protected unsafe __MicroComIAvnGlDisplayVTable()
	{
		AddMethod((delegate*<void*, void*, void**, int>)(&CreateContext));
		AddMethod((delegate*<void*, void>)(&LegacyClearCurrentContext));
		AddMethod((delegate*<void*, IntPtr, void**, int>)(&WrapContext));
		AddMethod((delegate*<void*, byte*, IntPtr>)(&GetProcAddress));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnGlDisplay), new __MicroComIAvnGlDisplayVTable().CreateVTable());
	}
}
