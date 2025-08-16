using System;

namespace Avalonia.X11;

internal struct XColormapEvent
{
	internal XEventName type;

	internal IntPtr serial;

	internal int send_event;

	internal IntPtr display;

	internal IntPtr window;

	internal IntPtr colormap;

	internal int c_new;

	internal int state;
}
