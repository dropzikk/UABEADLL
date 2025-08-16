using System;

namespace Avalonia.X11;

internal struct XIValuatorClassInfo
{
	public int Type;

	public int Sourceid;

	public int Number;

	public IntPtr Label;

	public double Min;

	public double Max;

	public double Value;

	public int Resolution;

	public int Mode;
}
