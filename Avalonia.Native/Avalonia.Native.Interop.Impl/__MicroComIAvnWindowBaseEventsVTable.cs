using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnWindowBaseEventsVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int PaintDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void ClosedDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void ActivatedDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void DeactivatedDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void ResizedDelegate(void* @this, AvnSize* size, AvnPlatformResizeReason reason);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void PositionChangedDelegate(void* @this, AvnPoint position);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void RawMouseEventDelegate(void* @this, AvnRawMouseEventType type, ulong timeStamp, AvnInputModifiers modifiers, AvnPoint point, AvnVector delta);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int RawKeyEventDelegate(void* @this, AvnRawKeyEventType type, ulong timeStamp, AvnInputModifiers modifiers, uint key);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int RawTextInputEventDelegate(void* @this, ulong timeStamp, byte* text);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void ScalingChangedDelegate(void* @this, double scaling);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void RunRenderPriorityJobsDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void LostFocusDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate AvnDragDropEffects DragEventDelegate(void* @this, AvnDragEventType type, AvnPoint position, AvnInputModifiers modifiers, AvnDragDropEffects effects, void* clipboard, IntPtr dataObjectHandle);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void* GetAutomationPeerDelegate(void* @this);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int Paint(void* @this)
	{
		IAvnWindowBaseEvents avnWindowBaseEvents = null;
		try
		{
			avnWindowBaseEvents = (IAvnWindowBaseEvents)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnWindowBaseEvents.Paint();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBaseEvents, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void Closed(void* @this)
	{
		IAvnWindowBaseEvents avnWindowBaseEvents = null;
		try
		{
			avnWindowBaseEvents = (IAvnWindowBaseEvents)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnWindowBaseEvents.Closed();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBaseEvents, e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void Activated(void* @this)
	{
		IAvnWindowBaseEvents avnWindowBaseEvents = null;
		try
		{
			avnWindowBaseEvents = (IAvnWindowBaseEvents)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnWindowBaseEvents.Activated();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBaseEvents, e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void Deactivated(void* @this)
	{
		IAvnWindowBaseEvents avnWindowBaseEvents = null;
		try
		{
			avnWindowBaseEvents = (IAvnWindowBaseEvents)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnWindowBaseEvents.Deactivated();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBaseEvents, e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void Resized(void* @this, AvnSize* size, AvnPlatformResizeReason reason)
	{
		IAvnWindowBaseEvents avnWindowBaseEvents = null;
		try
		{
			avnWindowBaseEvents = (IAvnWindowBaseEvents)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnWindowBaseEvents.Resized(size, reason);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBaseEvents, e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void PositionChanged(void* @this, AvnPoint position)
	{
		IAvnWindowBaseEvents avnWindowBaseEvents = null;
		try
		{
			avnWindowBaseEvents = (IAvnWindowBaseEvents)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnWindowBaseEvents.PositionChanged(position);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBaseEvents, e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void RawMouseEvent(void* @this, AvnRawMouseEventType type, ulong timeStamp, AvnInputModifiers modifiers, AvnPoint point, AvnVector delta)
	{
		IAvnWindowBaseEvents avnWindowBaseEvents = null;
		try
		{
			avnWindowBaseEvents = (IAvnWindowBaseEvents)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnWindowBaseEvents.RawMouseEvent(type, timeStamp, modifiers, point, delta);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBaseEvents, e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int RawKeyEvent(void* @this, AvnRawKeyEventType type, ulong timeStamp, AvnInputModifiers modifiers, uint key)
	{
		IAvnWindowBaseEvents avnWindowBaseEvents = null;
		try
		{
			avnWindowBaseEvents = (IAvnWindowBaseEvents)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnWindowBaseEvents.RawKeyEvent(type, timeStamp, modifiers, key);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBaseEvents, e);
			return 0;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int RawTextInputEvent(void* @this, ulong timeStamp, byte* text)
	{
		IAvnWindowBaseEvents avnWindowBaseEvents = null;
		try
		{
			avnWindowBaseEvents = (IAvnWindowBaseEvents)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnWindowBaseEvents.RawTextInputEvent(timeStamp, (text == null) ? null : Marshal.PtrToStringAnsi(new IntPtr(text)));
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBaseEvents, e);
			return 0;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void ScalingChanged(void* @this, double scaling)
	{
		IAvnWindowBaseEvents avnWindowBaseEvents = null;
		try
		{
			avnWindowBaseEvents = (IAvnWindowBaseEvents)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnWindowBaseEvents.ScalingChanged(scaling);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBaseEvents, e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void RunRenderPriorityJobs(void* @this)
	{
		IAvnWindowBaseEvents avnWindowBaseEvents = null;
		try
		{
			avnWindowBaseEvents = (IAvnWindowBaseEvents)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnWindowBaseEvents.RunRenderPriorityJobs();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBaseEvents, e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void LostFocus(void* @this)
	{
		IAvnWindowBaseEvents avnWindowBaseEvents = null;
		try
		{
			avnWindowBaseEvents = (IAvnWindowBaseEvents)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnWindowBaseEvents.LostFocus();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBaseEvents, e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static AvnDragDropEffects DragEvent(void* @this, AvnDragEventType type, AvnPoint position, AvnInputModifiers modifiers, AvnDragDropEffects effects, void* clipboard, IntPtr dataObjectHandle)
	{
		IAvnWindowBaseEvents avnWindowBaseEvents = null;
		try
		{
			avnWindowBaseEvents = (IAvnWindowBaseEvents)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnWindowBaseEvents.DragEvent(type, position, modifiers, effects, MicroComRuntime.CreateProxyOrNullFor<IAvnClipboard>(clipboard, ownsHandle: false), dataObjectHandle);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBaseEvents, e);
			return AvnDragDropEffects.None;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void* GetAutomationPeer(void* @this)
	{
		IAvnWindowBaseEvents avnWindowBaseEvents = null;
		try
		{
			avnWindowBaseEvents = (IAvnWindowBaseEvents)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return MicroComRuntime.GetNativePointer(avnWindowBaseEvents.AutomationPeer, owned: true);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBaseEvents, e);
			return null;
		}
	}

	protected unsafe __MicroComIAvnWindowBaseEventsVTable()
	{
		AddMethod((delegate*<void*, int>)(&Paint));
		AddMethod((delegate*<void*, void>)(&Closed));
		AddMethod((delegate*<void*, void>)(&Activated));
		AddMethod((delegate*<void*, void>)(&Deactivated));
		AddMethod((delegate*<void*, AvnSize*, AvnPlatformResizeReason, void>)(&Resized));
		AddMethod((delegate*<void*, AvnPoint, void>)(&PositionChanged));
		AddMethod((delegate*<void*, AvnRawMouseEventType, ulong, AvnInputModifiers, AvnPoint, AvnVector, void>)(&RawMouseEvent));
		AddMethod((delegate*<void*, AvnRawKeyEventType, ulong, AvnInputModifiers, uint, int>)(&RawKeyEvent));
		AddMethod((delegate*<void*, ulong, byte*, int>)(&RawTextInputEvent));
		AddMethod((delegate*<void*, double, void>)(&ScalingChanged));
		AddMethod((delegate*<void*, void>)(&RunRenderPriorityJobs));
		AddMethod((delegate*<void*, void>)(&LostFocus));
		AddMethod((delegate*<void*, AvnDragEventType, AvnPoint, AvnInputModifiers, AvnDragDropEffects, void*, IntPtr, AvnDragDropEffects>)(&DragEvent));
		AddMethod((delegate*<void*, void*>)(&GetAutomationPeer));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnWindowBaseEvents), new __MicroComIAvnWindowBaseEventsVTable().CreateVTable());
	}
}
