using System;

namespace Avalonia.X11;

internal struct XSelectionClearEvent
{
	internal XEventName type;

	internal IntPtr serial;

	internal int send_event;

	internal IntPtr display;

	internal IntPtr window;

	internal IntPtr selection;

	internal IntPtr time;
}
