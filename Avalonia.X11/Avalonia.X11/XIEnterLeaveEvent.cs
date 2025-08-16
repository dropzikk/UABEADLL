using System;

namespace Avalonia.X11;

internal struct XIEnterLeaveEvent
{
	public XEventName type;

	public UIntPtr serial;

	public bool send_event;

	public IntPtr display;

	public int extension;

	public XiEventType evtype;

	public IntPtr time;

	public int deviceid;

	public int sourceid;

	public XiEnterLeaveDetail detail;

	public IntPtr RootWindow;

	public IntPtr EventWindow;

	public IntPtr ChildWindow;

	public double root_x;

	public double root_y;

	public double event_x;

	public double event_y;

	public int mode;

	public int focus;

	public int same_screen;

	public XIButtonState buttons;

	public XIModifierState mods;

	public XIModifierState group;
}
