using System;

namespace Avalonia.X11;

internal struct XSelectionEvent
{
	internal XEventName type;

	internal IntPtr serial;

	internal int send_event;

	internal IntPtr display;

	internal IntPtr requestor;

	internal IntPtr selection;

	internal IntPtr target;

	internal IntPtr property;

	internal IntPtr time;
}
