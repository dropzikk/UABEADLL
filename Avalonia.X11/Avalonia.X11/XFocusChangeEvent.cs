using System;

namespace Avalonia.X11;

internal struct XFocusChangeEvent
{
	internal XEventName type;

	internal IntPtr serial;

	internal int send_event;

	internal IntPtr display;

	internal IntPtr window;

	internal int mode;

	internal NotifyDetail detail;
}
