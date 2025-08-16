using System;

namespace Avalonia.Controls.Platform;

public class SafeAreaChangedArgs : EventArgs
{
	public Thickness SafeAreaPadding { get; }

	public SafeAreaChangedArgs(Thickness safeArePadding)
	{
		SafeAreaPadding = safeArePadding;
	}
}
