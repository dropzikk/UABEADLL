using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;

namespace Avalonia.Controls;

public class StackPanel : Panel, INavigableContainer, IScrollSnapPointsInfo
{
	public static readonly StyledProperty<double> SpacingProperty;

	public static readonly StyledProperty<Orientation> OrientationProperty;

	public static readonly StyledProperty<bool> AreHorizontalSnapPointsRegularProperty;

	public static readonly StyledProperty<bool> AreVerticalSnapPointsRegularProperty;

	public static readonly RoutedEvent<RoutedEventArgs> HorizontalSnapPointsChangedEvent;

	public static readonly RoutedEvent<RoutedEventArgs> VerticalSnapPointsChangedEvent;

	public double Spacing
	{
		get
		{
			return GetValue(SpacingProperty);
		}
		set
		{
			SetValue(SpacingProperty, value);
		}
	}

	public Orientation Orientation
	{
		get
		{
			return GetValue(OrientationProperty);
		}
		set
		{
			SetValue(OrientationProperty, value);
		}
	}

	public bool AreHorizontalSnapPointsRegular
	{
		get
		{
			return GetValue(AreHorizontalSnapPointsRegularProperty);
		}
		set
		{
			SetValue(AreHorizontalSnapPointsRegularProperty, value);
		}
	}

	public bool AreVerticalSnapPointsRegular
	{
		get
		{
			return GetValue(AreVerticalSnapPointsRegularProperty);
		}
		set
		{
			SetValue(AreVerticalSnapPointsRegularProperty, value);
		}
	}

	public event EventHandler<RoutedEventArgs>? HorizontalSnapPointsChanged
	{
		add
		{
			AddHandler(HorizontalSnapPointsChangedEvent, value);
		}
		remove
		{
			RemoveHandler(HorizontalSnapPointsChangedEvent, value);
		}
	}

	public event EventHandler<RoutedEventArgs>? VerticalSnapPointsChanged
	{
		add
		{
			AddHandler(VerticalSnapPointsChangedEvent, value);
		}
		remove
		{
			RemoveHandler(VerticalSnapPointsChangedEvent, value);
		}
	}

	static StackPanel()
	{
		SpacingProperty = AvaloniaProperty.Register<StackPanel, double>("Spacing", 0.0);
		OrientationProperty = AvaloniaProperty.Register<StackPanel, Orientation>("Orientation", Orientation.Vertical);
		AreHorizontalSnapPointsRegularProperty = AvaloniaProperty.Register<StackPanel, bool>("AreHorizontalSnapPointsRegular", defaultValue: false);
		AreVerticalSnapPointsRegularProperty = AvaloniaProperty.Register<StackPanel, bool>("AreVerticalSnapPointsRegular", defaultValue: false);
		HorizontalSnapPointsChangedEvent = RoutedEvent.Register<StackPanel, RoutedEventArgs>("HorizontalSnapPointsChanged", RoutingStrategies.Bubble);
		VerticalSnapPointsChangedEvent = RoutedEvent.Register<StackPanel, RoutedEventArgs>("VerticalSnapPointsChanged", RoutingStrategies.Bubble);
		Layoutable.AffectsMeasure<StackPanel>(new AvaloniaProperty[1] { SpacingProperty });
		Layoutable.AffectsMeasure<StackPanel>(new AvaloniaProperty[1] { OrientationProperty });
	}

	IInputElement? INavigableContainer.GetControl(NavigationDirection direction, IInputElement? from, bool wrap)
	{
		IInputElement controlInDirection = GetControlInDirection(direction, from as Control);
		if (controlInDirection == null && wrap)
		{
			if (Orientation == Orientation.Vertical)
			{
				switch (direction)
				{
				case NavigationDirection.Previous:
				case NavigationDirection.Up:
				case NavigationDirection.PageUp:
					controlInDirection = GetControlInDirection(NavigationDirection.Last, null);
					break;
				case NavigationDirection.Next:
				case NavigationDirection.Down:
				case NavigationDirection.PageDown:
					controlInDirection = GetControlInDirection(NavigationDirection.First, null);
					break;
				}
			}
			else
			{
				switch (direction)
				{
				case NavigationDirection.Previous:
				case NavigationDirection.Left:
				case NavigationDirection.PageUp:
					controlInDirection = GetControlInDirection(NavigationDirection.Last, null);
					break;
				case NavigationDirection.Next:
				case NavigationDirection.Right:
				case NavigationDirection.PageDown:
					controlInDirection = GetControlInDirection(NavigationDirection.First, null);
					break;
				}
			}
		}
		return controlInDirection;
	}

	protected virtual IInputElement? GetControlInDirection(NavigationDirection direction, Control? from)
	{
		bool flag = Orientation == Orientation.Horizontal;
		int num = ((from != null) ? base.Children.IndexOf(from) : (-1));
		switch (direction)
		{
		case NavigationDirection.First:
			num = 0;
			break;
		case NavigationDirection.Last:
			num = base.Children.Count - 1;
			break;
		case NavigationDirection.Next:
			num++;
			break;
		case NavigationDirection.Previous:
			if (num != -1)
			{
				num--;
			}
			break;
		case NavigationDirection.Left:
			if (num != -1)
			{
				num = (flag ? (num - 1) : (-1));
			}
			break;
		case NavigationDirection.Right:
			if (num != -1)
			{
				num = (flag ? (num + 1) : (-1));
			}
			break;
		case NavigationDirection.Up:
			if (num != -1)
			{
				num = (flag ? (-1) : (num - 1));
			}
			break;
		case NavigationDirection.Down:
			if (num != -1)
			{
				num = (flag ? (-1) : (num + 1));
			}
			break;
		default:
			num = -1;
			break;
		}
		if (num >= 0 && num < base.Children.Count)
		{
			return base.Children[num];
		}
		return null;
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		Size size = default(Size);
		Controls children = base.Children;
		Size size2 = availableSize;
		bool flag = Orientation == Orientation.Horizontal;
		double spacing = Spacing;
		bool flag2 = false;
		size2 = ((!flag) ? size2.WithHeight(double.PositiveInfinity) : size2.WithWidth(double.PositiveInfinity));
		int i = 0;
		for (int count = children.Count; i < count; i++)
		{
			Control control = children[i];
			bool isVisible = control.IsVisible;
			if (isVisible && !flag2)
			{
				flag2 = true;
			}
			control.Measure(size2);
			Size desiredSize = control.DesiredSize;
			if (flag)
			{
				size = size.WithWidth(size.Width + (isVisible ? spacing : 0.0) + desiredSize.Width);
				size = size.WithHeight(Math.Max(size.Height, desiredSize.Height));
			}
			else
			{
				size = size.WithWidth(Math.Max(size.Width, desiredSize.Width));
				size = size.WithHeight(size.Height + (isVisible ? spacing : 0.0) + desiredSize.Height);
			}
		}
		if (flag)
		{
			return size.WithWidth(size.Width - (flag2 ? spacing : 0.0));
		}
		return size.WithHeight(size.Height - (flag2 ? spacing : 0.0));
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		Controls children = base.Children;
		bool flag = Orientation == Orientation.Horizontal;
		Rect rect = new Rect(finalSize);
		double num = 0.0;
		double spacing = Spacing;
		int i = 0;
		for (int count = children.Count; i < count; i++)
		{
			Control control = children[i];
			if (control.IsVisible)
			{
				if (flag)
				{
					rect = rect.WithX(rect.X + num);
					num = control.DesiredSize.Width;
					rect = rect.WithWidth(num).WithHeight(Math.Max(finalSize.Height, control.DesiredSize.Height));
					num += spacing;
				}
				else
				{
					rect = rect.WithY(rect.Y + num);
					num = control.DesiredSize.Height;
					rect = rect.WithHeight(num).WithWidth(Math.Max(finalSize.Width, control.DesiredSize.Width));
					num += spacing;
				}
				ArrangeChild(control, rect, finalSize, Orientation);
			}
		}
		RaiseEvent(new RoutedEventArgs((Orientation == Orientation.Horizontal) ? HorizontalSnapPointsChangedEvent : VerticalSnapPointsChangedEvent));
		return finalSize;
	}

	internal virtual void ArrangeChild(Control child, Rect rect, Size panelSize, Orientation orientation)
	{
		child.Arrange(rect);
	}

	public IReadOnlyList<double> GetIrregularSnapPoints(Orientation orientation, SnapPointsAlignment snapPointsAlignment)
	{
		List<double> list = new List<double>();
		switch (orientation)
		{
		case Orientation.Horizontal:
			if (AreHorizontalSnapPointsRegular)
			{
				throw new InvalidOperationException();
			}
			if (Orientation != 0)
			{
				break;
			}
			foreach (Visual visualChild in base.VisualChildren)
			{
				double item2 = 0.0;
				switch (snapPointsAlignment)
				{
				case SnapPointsAlignment.Near:
					item2 = visualChild.Bounds.Left;
					break;
				case SnapPointsAlignment.Center:
					item2 = visualChild.Bounds.Center.X;
					break;
				case SnapPointsAlignment.Far:
					item2 = visualChild.Bounds.Right;
					break;
				}
				list.Add(item2);
			}
			break;
		case Orientation.Vertical:
			if (AreVerticalSnapPointsRegular)
			{
				throw new InvalidOperationException();
			}
			if (Orientation != Orientation.Vertical)
			{
				break;
			}
			foreach (Visual visualChild2 in base.VisualChildren)
			{
				double item = 0.0;
				switch (snapPointsAlignment)
				{
				case SnapPointsAlignment.Near:
					item = visualChild2.Bounds.Top;
					break;
				case SnapPointsAlignment.Center:
					item = visualChild2.Bounds.Center.Y;
					break;
				case SnapPointsAlignment.Far:
					item = visualChild2.Bounds.Bottom;
					break;
				}
				list.Add(item);
			}
			break;
		}
		return list;
	}

	public double GetRegularSnapPoints(Orientation orientation, SnapPointsAlignment snapPointsAlignment, out double offset)
	{
		offset = 0.0;
		Visual visual = base.VisualChildren.FirstOrDefault();
		if (visual == null)
		{
			return 0.0;
		}
		double num = 0.0;
		switch (Orientation)
		{
		case Orientation.Horizontal:
			if (!AreHorizontalSnapPointsRegular)
			{
				throw new InvalidOperationException();
			}
			num = visual.Bounds.Width;
			switch (snapPointsAlignment)
			{
			case SnapPointsAlignment.Near:
				offset = visual.Bounds.Left;
				break;
			case SnapPointsAlignment.Center:
				offset = visual.Bounds.Center.X;
				break;
			case SnapPointsAlignment.Far:
				offset = visual.Bounds.Right;
				break;
			}
			break;
		case Orientation.Vertical:
			if (!AreVerticalSnapPointsRegular)
			{
				throw new InvalidOperationException();
			}
			num = visual.Bounds.Height;
			switch (snapPointsAlignment)
			{
			case SnapPointsAlignment.Near:
				offset = visual.Bounds.Top;
				break;
			case SnapPointsAlignment.Center:
				offset = visual.Bounds.Center.Y;
				break;
			case SnapPointsAlignment.Far:
				offset = visual.Bounds.Bottom;
				break;
			}
			break;
		}
		return num + Spacing;
	}
}
