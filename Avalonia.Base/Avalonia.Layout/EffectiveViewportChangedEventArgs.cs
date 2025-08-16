using System;

namespace Avalonia.Layout;

public class EffectiveViewportChangedEventArgs : EventArgs
{
	public Rect EffectiveViewport { get; }

	public EffectiveViewportChangedEventArgs(Rect effectiveViewport)
	{
		EffectiveViewport = effectiveViewport;
	}
}
