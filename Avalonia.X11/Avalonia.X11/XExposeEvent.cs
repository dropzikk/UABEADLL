using System;

namespace Avalonia.X11;

internal struct XExposeEvent
{
	internal XEventName type;

	internal IntPtr serial;

	internal int send_event;

	internal IntPtr display;

	internal IntPtr window;

	internal int x;

	internal int y;

	internal int width;

	internal int height;

	internal int count;
}
