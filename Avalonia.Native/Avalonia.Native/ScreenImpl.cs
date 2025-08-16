using System;
using System.Collections.Generic;
using Avalonia.Native.Interop;
using Avalonia.Platform;

namespace Avalonia.Native;

internal class ScreenImpl : IScreenImpl, IDisposable
{
	private IAvnScreens _native;

	public int ScreenCount => _native.ScreenCount;

	public IReadOnlyList<Screen> AllScreens
	{
		get
		{
			if (_native != null)
			{
				int screenCount = ScreenCount;
				Screen[] array = new Screen[screenCount];
				for (int i = 0; i < screenCount; i++)
				{
					AvnScreen screen = _native.GetScreen(i);
					array[i] = new Screen(screen.Scaling, screen.Bounds.ToAvaloniaPixelRect(), screen.WorkingArea.ToAvaloniaPixelRect(), screen.IsPrimary.FromComBool());
				}
				return array;
			}
			return Array.Empty<Screen>();
		}
	}

	public ScreenImpl(IAvnScreens native)
	{
		_native = native;
	}

	public void Dispose()
	{
		_native?.Dispose();
		_native = null;
	}

	public Screen ScreenFromPoint(PixelPoint point)
	{
		return ScreenHelper.ScreenFromPoint(point, AllScreens);
	}

	public Screen ScreenFromRect(PixelRect rect)
	{
		return ScreenHelper.ScreenFromRect(rect, AllScreens);
	}

	public Screen ScreenFromWindow(IWindowBaseImpl window)
	{
		return ScreenHelper.ScreenFromWindow(window, AllScreens);
	}
}
