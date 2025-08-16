using System;

namespace Avalonia.Layout;

public static class LayoutExtensions
{
	public static Rect Align(this Rect rect, Rect constraint, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
	{
		switch (horizontalAlignment)
		{
		case HorizontalAlignment.Center:
			rect = rect.WithX((constraint.Width - rect.Width) / 2.0);
			break;
		case HorizontalAlignment.Right:
			rect = rect.WithX(constraint.Width - rect.Width);
			break;
		case HorizontalAlignment.Stretch:
			rect = new Rect(0.0, rect.Y, Math.Max(constraint.Width, rect.Width), rect.Height);
			break;
		}
		switch (verticalAlignment)
		{
		case VerticalAlignment.Center:
			rect = rect.WithY((constraint.Height - rect.Height) / 2.0);
			break;
		case VerticalAlignment.Bottom:
			rect = rect.WithY(constraint.Height - rect.Height);
			break;
		case VerticalAlignment.Stretch:
			rect = new Rect(rect.X, 0.0, rect.Width, Math.Max(constraint.Height, rect.Height));
			break;
		}
		return rect;
	}
}
