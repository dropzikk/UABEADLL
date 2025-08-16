using System;

namespace Avalonia.X11;

internal struct XRRMonitorInfo
{
	public IntPtr Name;

	public int Primary;

	public int Automatic;

	public int NOutput;

	public int X;

	public int Y;

	public int Width;

	public int Height;

	public int MWidth;

	public int MHeight;

	public unsafe IntPtr* Outputs;
}
