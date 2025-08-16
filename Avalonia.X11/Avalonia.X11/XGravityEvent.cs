using System;

namespace Avalonia.X11;

internal struct XGravityEvent
{
	internal XEventName type;

	internal IntPtr serial;

	internal int send_event;

	internal IntPtr display;

	internal IntPtr xevent;

	internal IntPtr window;

	internal int x;

	internal int y;
}
