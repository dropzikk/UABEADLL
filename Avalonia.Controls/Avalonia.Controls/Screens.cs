using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Avalonia.Platform;

namespace Avalonia.Controls;

public class Screens
{
	private readonly IScreenImpl _iScreenImpl;

	public int ScreenCount => _iScreenImpl?.ScreenCount ?? 0;

	public IReadOnlyList<Screen> All => _iScreenImpl?.AllScreens ?? Array.Empty<Screen>();

	public Screen? Primary => All.FirstOrDefault((Screen x) => x.IsPrimary);

	public Screens(IScreenImpl iScreenImpl)
	{
		_iScreenImpl = iScreenImpl;
	}

	public Screen? ScreenFromBounds(PixelRect bounds)
	{
		return _iScreenImpl.ScreenFromRect(bounds);
	}

	public Screen? ScreenFromWindow(WindowBase window)
	{
		if (window.PlatformImpl == null)
		{
			throw new ObjectDisposedException("Window platform implementation was already disposed.");
		}
		return _iScreenImpl.ScreenFromWindow(window.PlatformImpl);
	}

	[Obsolete("Use ScreenFromWindow(WindowBase) overload.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public Screen? ScreenFromWindow(IWindowBaseImpl window)
	{
		return _iScreenImpl.ScreenFromWindow(window);
	}

	public Screen? ScreenFromPoint(PixelPoint point)
	{
		return _iScreenImpl.ScreenFromPoint(point);
	}

	public Screen? ScreenFromVisual(Visual visual)
	{
		PixelPoint topLeft = visual.PointToScreen(visual.Bounds.TopLeft);
		PixelPoint bottomRight = visual.PointToScreen(visual.Bounds.BottomRight);
		return ScreenFromBounds(new PixelRect(topLeft, bottomRight));
	}
}
