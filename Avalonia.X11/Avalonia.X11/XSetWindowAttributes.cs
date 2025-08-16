using System;

namespace Avalonia.X11;

internal struct XSetWindowAttributes
{
	internal IntPtr background_pixmap;

	internal IntPtr background_pixel;

	internal IntPtr border_pixmap;

	internal IntPtr border_pixel;

	internal Gravity bit_gravity;

	internal Gravity win_gravity;

	internal int backing_store;

	internal IntPtr backing_planes;

	internal IntPtr backing_pixel;

	internal int save_under;

	internal IntPtr event_mask;

	internal IntPtr do_not_propagate_mask;

	internal int override_redirect;

	internal IntPtr colormap;

	internal IntPtr cursor;
}
