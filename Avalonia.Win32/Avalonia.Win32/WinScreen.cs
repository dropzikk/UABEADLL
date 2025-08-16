using System;
using Avalonia.Platform;

namespace Avalonia.Win32;

internal class WinScreen : Screen
{
	private readonly IntPtr _hMonitor;

	public IntPtr Handle => _hMonitor;

	public WinScreen(double scaling, PixelRect bounds, PixelRect workingArea, bool isPrimary, IntPtr hMonitor)
		: base(scaling, bounds, workingArea, isPrimary)
	{
		_hMonitor = hMonitor;
	}

	public override int GetHashCode()
	{
		return _hMonitor.GetHashCode();
	}

	public override bool Equals(object? obj)
	{
		if (obj is WinScreen winScreen)
		{
			return _hMonitor == winScreen._hMonitor;
		}
		return false;
	}
}
