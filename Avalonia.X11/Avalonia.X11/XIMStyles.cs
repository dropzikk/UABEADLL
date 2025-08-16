using System;

namespace Avalonia.X11;

internal struct XIMStyles
{
	public ushort count_styles;

	public unsafe IntPtr* supported_styles;
}
