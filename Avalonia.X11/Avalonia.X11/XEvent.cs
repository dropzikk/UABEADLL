using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Avalonia.X11;

[StructLayout(LayoutKind.Explicit)]
internal struct XEvent
{
	[FieldOffset(0)]
	internal XEventName type;

	[FieldOffset(0)]
	internal XAnyEvent AnyEvent;

	[FieldOffset(0)]
	internal XKeyEvent KeyEvent;

	[FieldOffset(0)]
	internal XButtonEvent ButtonEvent;

	[FieldOffset(0)]
	internal XMotionEvent MotionEvent;

	[FieldOffset(0)]
	internal XCrossingEvent CrossingEvent;

	[FieldOffset(0)]
	internal XFocusChangeEvent FocusChangeEvent;

	[FieldOffset(0)]
	internal XExposeEvent ExposeEvent;

	[FieldOffset(0)]
	internal XGraphicsExposeEvent GraphicsExposeEvent;

	[FieldOffset(0)]
	internal XNoExposeEvent NoExposeEvent;

	[FieldOffset(0)]
	internal XVisibilityEvent VisibilityEvent;

	[FieldOffset(0)]
	internal XCreateWindowEvent CreateWindowEvent;

	[FieldOffset(0)]
	internal XDestroyWindowEvent DestroyWindowEvent;

	[FieldOffset(0)]
	internal XUnmapEvent UnmapEvent;

	[FieldOffset(0)]
	internal XMapEvent MapEvent;

	[FieldOffset(0)]
	internal XMapRequestEvent MapRequestEvent;

	[FieldOffset(0)]
	internal XReparentEvent ReparentEvent;

	[FieldOffset(0)]
	internal XConfigureEvent ConfigureEvent;

	[FieldOffset(0)]
	internal XGravityEvent GravityEvent;

	[FieldOffset(0)]
	internal XResizeRequestEvent ResizeRequestEvent;

	[FieldOffset(0)]
	internal XConfigureRequestEvent ConfigureRequestEvent;

	[FieldOffset(0)]
	internal XCirculateEvent CirculateEvent;

	[FieldOffset(0)]
	internal XCirculateRequestEvent CirculateRequestEvent;

	[FieldOffset(0)]
	internal XPropertyEvent PropertyEvent;

	[FieldOffset(0)]
	internal XSelectionClearEvent SelectionClearEvent;

	[FieldOffset(0)]
	internal XSelectionRequestEvent SelectionRequestEvent;

	[FieldOffset(0)]
	internal XSelectionEvent SelectionEvent;

	[FieldOffset(0)]
	internal XColormapEvent ColormapEvent;

	[FieldOffset(0)]
	internal XClientMessageEvent ClientMessageEvent;

	[FieldOffset(0)]
	internal XMappingEvent MappingEvent;

	[FieldOffset(0)]
	internal XErrorEvent ErrorEvent;

	[FieldOffset(0)]
	internal XKeymapEvent KeymapEvent;

	[FieldOffset(0)]
	internal XGenericEventCookie GenericEventCookie;

	[FieldOffset(0)]
	internal XEventPad Pad;

	public override string ToString()
	{
		switch (type)
		{
		case XEventName.ButtonPress:
		case XEventName.ButtonRelease:
			return ToString(ButtonEvent);
		case XEventName.CirculateNotify:
		case XEventName.CirculateRequest:
			return ToString(CirculateEvent);
		case XEventName.ClientMessage:
			return ToString(ClientMessageEvent);
		case XEventName.ColormapNotify:
			return ToString(ColormapEvent);
		case XEventName.ConfigureNotify:
			return ToString(ConfigureEvent);
		case XEventName.ConfigureRequest:
			return ToString(ConfigureRequestEvent);
		case XEventName.CreateNotify:
			return ToString(CreateWindowEvent);
		case XEventName.DestroyNotify:
			return ToString(DestroyWindowEvent);
		case XEventName.Expose:
			return ToString(ExposeEvent);
		case XEventName.FocusIn:
		case XEventName.FocusOut:
			return ToString(FocusChangeEvent);
		case XEventName.GraphicsExpose:
			return ToString(GraphicsExposeEvent);
		case XEventName.GravityNotify:
			return ToString(GravityEvent);
		case XEventName.KeymapNotify:
			return ToString(KeymapEvent);
		case XEventName.MapNotify:
			return ToString(MapEvent);
		case XEventName.MappingNotify:
			return ToString(MappingEvent);
		case XEventName.MapRequest:
			return ToString(MapRequestEvent);
		case XEventName.MotionNotify:
			return ToString(MotionEvent);
		case XEventName.NoExpose:
			return ToString(NoExposeEvent);
		case XEventName.PropertyNotify:
			return ToString(PropertyEvent);
		case XEventName.ReparentNotify:
			return ToString(ReparentEvent);
		case XEventName.ResizeRequest:
			return ToString(ResizeRequestEvent);
		case XEventName.SelectionClear:
			return ToString(SelectionClearEvent);
		case XEventName.SelectionNotify:
			return ToString(SelectionEvent);
		case XEventName.SelectionRequest:
			return ToString(SelectionRequestEvent);
		case XEventName.UnmapNotify:
			return ToString(UnmapEvent);
		case XEventName.VisibilityNotify:
			return ToString(VisibilityEvent);
		case XEventName.EnterNotify:
		case XEventName.LeaveNotify:
			return ToString(CrossingEvent);
		default:
			return type.ToString();
		}
	}

	[UnconditionalSuppressMessage("Trimming", "IL2075", Justification = "This method is not supported by NativeAOT.")]
	public static string ToString(object ev)
	{
		string text = string.Empty;
		Type type = ev.GetType();
		FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		for (int i = 0; i < fields.Length; i++)
		{
			if (!string.IsNullOrEmpty(text))
			{
				text += ", ";
			}
			object value = fields[i].GetValue(ev);
			text = text + fields[i].Name + "=" + ((value == null) ? "<null>" : value.ToString());
		}
		return type.Name + " (" + text + ")";
	}
}
