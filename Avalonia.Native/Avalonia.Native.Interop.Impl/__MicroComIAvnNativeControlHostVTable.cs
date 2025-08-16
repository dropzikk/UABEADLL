using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnNativeControlHostVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateDefaultChildDelegate(void* @this, IntPtr parent, IntPtr* retOut);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void* CreateAttachmentDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void DestroyDefaultChildDelegate(void* @this, IntPtr child);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateDefaultChild(void* @this, IntPtr parent, IntPtr* retOut)
	{
		IAvnNativeControlHost avnNativeControlHost = null;
		try
		{
			avnNativeControlHost = (IAvnNativeControlHost)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IntPtr intPtr = avnNativeControlHost.CreateDefaultChild(parent);
			*retOut = intPtr;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnNativeControlHost, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void* CreateAttachment(void* @this)
	{
		IAvnNativeControlHost avnNativeControlHost = null;
		try
		{
			avnNativeControlHost = (IAvnNativeControlHost)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return MicroComRuntime.GetNativePointer(avnNativeControlHost.CreateAttachment(), owned: true);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnNativeControlHost, e);
			return null;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void DestroyDefaultChild(void* @this, IntPtr child)
	{
		IAvnNativeControlHost avnNativeControlHost = null;
		try
		{
			avnNativeControlHost = (IAvnNativeControlHost)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnNativeControlHost.DestroyDefaultChild(child);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnNativeControlHost, e);
		}
	}

	protected unsafe __MicroComIAvnNativeControlHostVTable()
	{
		AddMethod((delegate*<void*, IntPtr, IntPtr*, int>)(&CreateDefaultChild));
		AddMethod((delegate*<void*, void*>)(&CreateAttachment));
		AddMethod((delegate*<void*, IntPtr, void>)(&DestroyDefaultChild));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnNativeControlHost), new __MicroComIAvnNativeControlHostVTable().CreateVTable());
	}
}
