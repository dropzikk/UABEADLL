using System;

namespace Avalonia.X11;

internal struct XMapEvent
{
	internal XEventName type;

	internal IntPtr serial;

	internal int send_event;

	internal IntPtr display;

	internal IntPtr xevent;

	internal IntPtr window;

	internal int override_redirect;
}
