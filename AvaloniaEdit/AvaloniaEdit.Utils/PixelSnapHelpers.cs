using System;
using Avalonia;

namespace AvaloniaEdit.Utils;

public static class PixelSnapHelpers
{
	public static Size GetPixelSize(Visual visual)
	{
		if (visual == null)
		{
			throw new ArgumentNullException("visual");
		}
		return new Size(1.0, 1.0);
	}

	public static double PixelAlign(double value, double pixelSize)
	{
		return pixelSize * (Math.Round(value / pixelSize + 0.5, MidpointRounding.AwayFromZero) - 0.5);
	}

	public static Rect PixelAlign(Rect rect, Size pixelSize)
	{
		double x = PixelAlign(rect.X, pixelSize.Width);
		double y = PixelAlign(rect.Y, pixelSize.Height);
		double width = Round(rect.Width, pixelSize.Width);
		double height = Round(rect.Height, pixelSize.Height);
		return new Rect(x, y, width, height);
	}

	public static Point Round(Point point, Size pixelSize)
	{
		return new Point(Round(point.X, pixelSize.Width), Round(point.Y, pixelSize.Height));
	}

	public static Rect Round(Rect rect, Size pixelSize)
	{
		return new Rect(Round(rect.X, pixelSize.Width), Round(rect.Y, pixelSize.Height), Round(rect.Width, pixelSize.Width), Round(rect.Height, pixelSize.Height));
	}

	public static double Round(double value, double pixelSize)
	{
		return pixelSize * Math.Round(value / pixelSize, MidpointRounding.AwayFromZero);
	}

	public static double RoundToOdd(double value, double pixelSize)
	{
		return Round(value - pixelSize, pixelSize * 2.0) + pixelSize;
	}
}
