using System;

namespace Avalonia.X11;

internal struct XMappingEvent
{
	internal XEventName type;

	internal IntPtr serial;

	internal int send_event;

	internal IntPtr display;

	internal IntPtr window;

	internal int request;

	internal int first_keycode;

	internal int count;
}
