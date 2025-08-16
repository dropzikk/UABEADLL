using Avalonia.Input;

namespace Avalonia.Controls;

public class Canvas : Panel, INavigableContainer
{
	public static readonly AttachedProperty<double> LeftProperty;

	public static readonly AttachedProperty<double> TopProperty;

	public static readonly AttachedProperty<double> RightProperty;

	public static readonly AttachedProperty<double> BottomProperty;

	static Canvas()
	{
		LeftProperty = AvaloniaProperty.RegisterAttached<Canvas, Control, double>("Left", double.NaN);
		TopProperty = AvaloniaProperty.RegisterAttached<Canvas, Control, double>("Top", double.NaN);
		RightProperty = AvaloniaProperty.RegisterAttached<Canvas, Control, double>("Right", double.NaN);
		BottomProperty = AvaloniaProperty.RegisterAttached<Canvas, Control, double>("Bottom", double.NaN);
		Visual.ClipToBoundsProperty.OverrideDefaultValue<Canvas>(defaultValue: false);
		Panel.AffectsParentArrange<Canvas>(new AvaloniaProperty[4] { LeftProperty, TopProperty, RightProperty, BottomProperty });
	}

	public static double GetLeft(AvaloniaObject element)
	{
		return element.GetValue(LeftProperty);
	}

	public static void SetLeft(AvaloniaObject element, double value)
	{
		element.SetValue(LeftProperty, value);
	}

	public static double GetTop(AvaloniaObject element)
	{
		return element.GetValue(TopProperty);
	}

	public static void SetTop(AvaloniaObject element, double value)
	{
		element.SetValue(TopProperty, value);
	}

	public static double GetRight(AvaloniaObject element)
	{
		return element.GetValue(RightProperty);
	}

	public static void SetRight(AvaloniaObject element, double value)
	{
		element.SetValue(RightProperty, value);
	}

	public static double GetBottom(AvaloniaObject element)
	{
		return element.GetValue(BottomProperty);
	}

	public static void SetBottom(AvaloniaObject element, double value)
	{
		element.SetValue(BottomProperty, value);
	}

	IInputElement? INavigableContainer.GetControl(NavigationDirection direction, IInputElement? from, bool wrap)
	{
		return null;
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		availableSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
		foreach (Control child in base.Children)
		{
			child.Measure(availableSize);
		}
		return default(Size);
	}

	protected virtual void ArrangeChild(Control child, Size finalSize)
	{
		double x = 0.0;
		double y = 0.0;
		double left = GetLeft(child);
		if (!double.IsNaN(left))
		{
			x = left;
		}
		else
		{
			double right = GetRight(child);
			if (!double.IsNaN(right))
			{
				x = finalSize.Width - child.DesiredSize.Width - right;
			}
		}
		double top = GetTop(child);
		if (!double.IsNaN(top))
		{
			y = top;
		}
		else
		{
			double bottom = GetBottom(child);
			if (!double.IsNaN(bottom))
			{
				y = finalSize.Height - child.DesiredSize.Height - bottom;
			}
		}
		child.Arrange(new Rect(new Point(x, y), child.DesiredSize));
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		foreach (Control child in base.Children)
		{
			ArrangeChild(child, finalSize);
		}
		return finalSize;
	}
}
