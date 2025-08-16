using System;
using Avalonia.Media;

namespace Avalonia.Controls;

public class ColorChangedEventArgs : EventArgs
{
	public Color OldColor { get; private set; }

	public Color NewColor { get; private set; }

	public ColorChangedEventArgs(Color oldColor, Color newColor)
	{
		OldColor = oldColor;
		NewColor = newColor;
	}
}
