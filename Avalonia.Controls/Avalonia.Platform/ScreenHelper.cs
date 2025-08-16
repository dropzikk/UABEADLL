using System.Collections.Generic;
using Avalonia.Utilities;

namespace Avalonia.Platform;

internal static class ScreenHelper
{
	public static Screen? ScreenFromPoint(PixelPoint point, IReadOnlyList<Screen> screens)
	{
		foreach (Screen screen in screens)
		{
			if (screen.Bounds.ContainsExclusive(point))
			{
				return screen;
			}
		}
		return null;
	}

	public static Screen? ScreenFromRect(PixelRect bounds, IReadOnlyList<Screen> screens)
	{
		Screen result = null;
		double num = 0.0;
		foreach (Screen screen in screens)
		{
			double num2 = MathUtilities.Clamp(bounds.X, screen.Bounds.X, screen.Bounds.X + screen.Bounds.Width);
			double num3 = MathUtilities.Clamp(bounds.Y, screen.Bounds.Y, screen.Bounds.Y + screen.Bounds.Height);
			double num4 = MathUtilities.Clamp(bounds.X + bounds.Width, screen.Bounds.X, screen.Bounds.X + screen.Bounds.Width);
			double num5 = MathUtilities.Clamp(bounds.Y + bounds.Height, screen.Bounds.Y, screen.Bounds.Y + screen.Bounds.Height);
			double num6 = (num4 - num2) * (num5 - num3);
			if (num6 > num)
			{
				num = num6;
				result = screen;
			}
		}
		return result;
	}

	public static Screen? ScreenFromWindow(IWindowBaseImpl window, IReadOnlyList<Screen> screens)
	{
		return ScreenFromRect(new PixelRect(window.Position, PixelSize.FromSize(window.FrameSize ?? window.ClientSize, window.DesktopScaling)), screens);
	}
}
