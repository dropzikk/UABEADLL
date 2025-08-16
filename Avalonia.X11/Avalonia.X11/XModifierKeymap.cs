using System;

namespace Avalonia.X11;

internal struct XModifierKeymap
{
	public int max_keypermod;

	public IntPtr modifiermap;
}
