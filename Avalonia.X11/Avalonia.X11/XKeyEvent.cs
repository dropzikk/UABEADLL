using System;

namespace Avalonia.X11;

internal struct XKeyEvent
{
	internal XEventName type;

	internal IntPtr serial;

	internal int send_event;

	internal IntPtr display;

	internal IntPtr window;

	internal IntPtr root;

	internal IntPtr subwindow;

	internal IntPtr time;

	internal int x;

	internal int y;

	internal int x_root;

	internal int y_root;

	internal XModifierMask state;

	internal int keycode;

	internal int same_screen;
}
