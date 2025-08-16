using System;

namespace Avalonia.X11;

internal struct XTextProperty
{
	internal string value;

	internal IntPtr encoding;

	internal int format;

	internal IntPtr nitems;
}
