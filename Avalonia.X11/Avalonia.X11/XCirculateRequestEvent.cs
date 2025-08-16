using System;

namespace Avalonia.X11;

internal struct XCirculateRequestEvent
{
	internal XEventName type;

	internal IntPtr serial;

	internal int send_event;

	internal IntPtr display;

	internal IntPtr parent;

	internal IntPtr window;

	internal int place;
}
