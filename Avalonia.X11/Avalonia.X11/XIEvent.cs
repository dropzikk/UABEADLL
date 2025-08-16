using System;

namespace Avalonia.X11;

internal struct XIEvent
{
	public int type;

	public UIntPtr serial;

	public bool send_event;

	public IntPtr display;

	public int extension;

	public XiEventType evtype;

	public IntPtr time;
}
