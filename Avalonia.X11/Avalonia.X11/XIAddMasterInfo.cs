using System;

namespace Avalonia.X11;

internal struct XIAddMasterInfo
{
	public int Type;

	public IntPtr Name;

	public bool SendCore;

	public bool Enable;
}
