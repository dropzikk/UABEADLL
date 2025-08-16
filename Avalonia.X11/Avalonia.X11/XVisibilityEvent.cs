using System;

namespace Avalonia.X11;

internal struct XVisibilityEvent
{
	internal XEventName type;

	internal IntPtr serial;

	internal int send_event;

	internal IntPtr display;

	internal IntPtr window;

	internal int state;
}
