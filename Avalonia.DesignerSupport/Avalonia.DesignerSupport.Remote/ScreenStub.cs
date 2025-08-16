using System.Collections.Generic;
using Avalonia.Platform;

namespace Avalonia.DesignerSupport.Remote;

internal class ScreenStub : IScreenImpl
{
	public int ScreenCount => 1;

	public IReadOnlyList<Screen> AllScreens { get; } = new Screen[1]
	{
		new Screen(1.0, new PixelRect(0, 0, 4000, 4000), new PixelRect(0, 0, 4000, 4000), isPrimary: true)
	};

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
