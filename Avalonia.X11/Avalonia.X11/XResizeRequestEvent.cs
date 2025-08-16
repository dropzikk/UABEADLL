using System;

namespace Avalonia.X11;

internal struct XResizeRequestEvent
{
	internal XEventName type;

	internal IntPtr serial;

	internal int send_event;

	internal IntPtr display;

	internal IntPtr window;

	internal int width;

	internal int height;
}
