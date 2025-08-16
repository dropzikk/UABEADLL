using System;

namespace Avalonia.X11;

internal struct XIDeviceEvent
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

	public int detail;

	public IntPtr RootWindow;

	public IntPtr EventWindow;

	public IntPtr ChildWindow;

	public double root_x;

	public double root_y;

	public double event_x;

	public double event_y;

	public XiDeviceEventFlags flags;

	public XIButtonState buttons;

	public XIValuatorState valuators;

	public XIModifierState mods;

	public XIModifierState group;
}
