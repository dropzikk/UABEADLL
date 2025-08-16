using System;
using System.ComponentModel;

namespace Avalonia.Platform;

public class Screen
{
	public double Scaling { get; }

	[Obsolete("Use the Scaling property instead.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public double PixelDensity => Scaling;

	public PixelRect Bounds { get; }

	public PixelRect WorkingArea { get; }

	public bool IsPrimary { get; }

	[Obsolete("Use the IsPrimary property instead.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool Primary => IsPrimary;

	public Screen(double scaling, PixelRect bounds, PixelRect workingArea, bool isPrimary)
	{
		Scaling = scaling;
		Bounds = bounds;
		WorkingArea = workingArea;
		IsPrimary = isPrimary;
	}
}
