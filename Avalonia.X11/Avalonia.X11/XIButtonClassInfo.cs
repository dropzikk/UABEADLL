using System;

namespace Avalonia.X11;

internal struct XIButtonClassInfo
{
	public int Type;

	public int Sourceid;

	public int NumButtons;

	public unsafe IntPtr* Labels;

	public XIButtonState State;
}
