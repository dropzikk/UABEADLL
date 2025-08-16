using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnNativeControlHostTopLevelAttachmentVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate IntPtr GetParentHandleDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int InitializeWithChildHandleDelegate(void* @this, IntPtr child);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int AttachToDelegate(void* @this, void* host);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void ShowInBoundsDelegate(void* @this, float x, float y, float width, float height);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void HideWithSizeDelegate(void* @this, float width, float height);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void ReleaseChildDelegate(void* @this);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static IntPtr GetParentHandle(void* @this)
	{
		IAvnNativeControlHostTopLevelAttachment avnNativeControlHostTopLevelAttachment = null;
		try
		{
			avnNativeControlHostTopLevelAttachment = (IAvnNativeControlHostTopLevelAttachment)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnNativeControlHostTopLevelAttachment.ParentHandle;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnNativeControlHostTopLevelAttachment, e);
			return (IntPtr)0;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int InitializeWithChildHandle(void* @this, IntPtr child)
	{
		IAvnNativeControlHostTopLevelAttachment avnNativeControlHostTopLevelAttachment = null;
		try
		{
			avnNativeControlHostTopLevelAttachment = (IAvnNativeControlHostTopLevelAttachment)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnNativeControlHostTopLevelAttachment.InitializeWithChildHandle(child);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnNativeControlHostTopLevelAttachment, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int AttachTo(void* @this, void* host)
	{
		IAvnNativeControlHostTopLevelAttachment avnNativeControlHostTopLevelAttachment = null;
		try
		{
			avnNativeControlHostTopLevelAttachment = (IAvnNativeControlHostTopLevelAttachment)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnNativeControlHostTopLevelAttachment.AttachTo(MicroComRuntime.CreateProxyOrNullFor<IAvnNativeControlHost>(host, ownsHandle: false));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnNativeControlHostTopLevelAttachment, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void ShowInBounds(void* @this, float x, float y, float width, float height)
	{
		IAvnNativeControlHostTopLevelAttachment avnNativeControlHostTopLevelAttachment = null;
		try
		{
			avnNativeControlHostTopLevelAttachment = (IAvnNativeControlHostTopLevelAttachment)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnNativeControlHostTopLevelAttachment.ShowInBounds(x, y, width, height);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnNativeControlHostTopLevelAttachment, e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void HideWithSize(void* @this, float width, float height)
	{
		IAvnNativeControlHostTopLevelAttachment avnNativeControlHostTopLevelAttachment = null;
		try
		{
			avnNativeControlHostTopLevelAttachment = (IAvnNativeControlHostTopLevelAttachment)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnNativeControlHostTopLevelAttachment.HideWithSize(width, height);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnNativeControlHostTopLevelAttachment, e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void ReleaseChild(void* @this)
	{
		IAvnNativeControlHostTopLevelAttachment avnNativeControlHostTopLevelAttachment = null;
		try
		{
			avnNativeControlHostTopLevelAttachment = (IAvnNativeControlHostTopLevelAttachment)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnNativeControlHostTopLevelAttachment.ReleaseChild();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnNativeControlHostTopLevelAttachment, e);
		}
	}

	protected unsafe __MicroComIAvnNativeControlHostTopLevelAttachmentVTable()
	{
		AddMethod((delegate*<void*, IntPtr>)(&GetParentHandle));
		AddMethod((delegate*<void*, IntPtr, int>)(&InitializeWithChildHandle));
		AddMethod((delegate*<void*, void*, int>)(&AttachTo));
		AddMethod((delegate*<void*, float, float, float, float, void>)(&ShowInBounds));
		AddMethod((delegate*<void*, float, float, void>)(&HideWithSize));
		AddMethod((delegate*<void*, void>)(&ReleaseChild));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnNativeControlHostTopLevelAttachment), new __MicroComIAvnNativeControlHostTopLevelAttachmentVTable().CreateVTable());
	}
}
