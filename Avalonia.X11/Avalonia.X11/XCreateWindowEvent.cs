using System;

namespace Avalonia.X11;

internal struct XCreateWindowEvent
{
	internal XEventName type;

	internal IntPtr serial;

	internal int send_event;

	internal IntPtr display;

	internal IntPtr parent;

	internal IntPtr window;

	internal int x;

	internal int y;

	internal int width;

	internal int height;

	internal int border_width;

	internal int override_redirect;
}
