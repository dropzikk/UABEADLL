using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnGlContextVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int MakeCurrentDelegate(void* @this, void** ppv);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int LegacyMakeCurrentDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetSampleCountDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetStencilSizeDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate IntPtr GetNativeHandleDelegate(void* @this);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int MakeCurrent(void* @this, void** ppv)
	{
		IAvnGlContext avnGlContext = null;
		try
		{
			avnGlContext = (IAvnGlContext)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IUnknown obj = avnGlContext.MakeCurrent();
			*ppv = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnGlContext, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int LegacyMakeCurrent(void* @this)
	{
		IAvnGlContext avnGlContext = null;
		try
		{
			avnGlContext = (IAvnGlContext)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnGlContext.LegacyMakeCurrent();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnGlContext, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetSampleCount(void* @this)
	{
		IAvnGlContext avnGlContext = null;
		try
		{
			avnGlContext = (IAvnGlContext)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnGlContext.SampleCount;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnGlContext, e);
			return 0;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetStencilSize(void* @this)
	{
		IAvnGlContext avnGlContext = null;
		try
		{
			avnGlContext = (IAvnGlContext)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnGlContext.StencilSize;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnGlContext, e);
			return 0;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static IntPtr GetNativeHandle(void* @this)
	{
		IAvnGlContext avnGlContext = null;
		try
		{
			avnGlContext = (IAvnGlContext)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnGlContext.NativeHandle;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnGlContext, e);
			return (IntPtr)0;
		}
	}

	protected unsafe __MicroComIAvnGlContextVTable()
	{
		AddMethod((delegate*<void*, void**, int>)(&MakeCurrent));
		AddMethod((delegate*<void*, int>)(&LegacyMakeCurrent));
		AddMethod((delegate*<void*, int>)(&GetSampleCount));
		AddMethod((delegate*<void*, int>)(&GetStencilSize));
		AddMethod((delegate*<void*, IntPtr>)(&GetNativeHandle));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnGlContext), new __MicroComIAvnGlContextVTable().CreateVTable());
	}
}
