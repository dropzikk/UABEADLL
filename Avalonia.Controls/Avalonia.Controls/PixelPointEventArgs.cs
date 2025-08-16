using System;

namespace Avalonia.Controls;

public class PixelPointEventArgs : EventArgs
{
	public PixelPoint Point { get; }

	public PixelPointEventArgs(PixelPoint point)
	{
		Point = point;
	}
}
