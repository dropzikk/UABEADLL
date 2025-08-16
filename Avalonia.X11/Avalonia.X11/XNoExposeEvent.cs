using System;

namespace Avalonia.X11;

internal struct XNoExposeEvent
{
	internal XEventName type;

	internal IntPtr serial;

	internal int send_event;

	internal IntPtr display;

	internal IntPtr drawable;

	internal int major_code;

	internal int minor_code;
}
