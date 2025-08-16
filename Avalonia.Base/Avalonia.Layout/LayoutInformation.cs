namespace Avalonia.Layout;

public static class LayoutInformation
{
	public static Size? GetPreviousMeasureConstraint(Layoutable control)
	{
		return control.PreviousMeasure;
	}

	public static Rect? GetPreviousArrangeBounds(Layoutable control)
	{
		return control.PreviousArrange;
	}
}
