using System.Collections.Generic;
using System.Linq;
using Avalonia.Layout;

namespace Avalonia.Controls;

public class ReversibleStackPanel : StackPanel
{
	public static readonly StyledProperty<bool> ReverseOrderProperty = AvaloniaProperty.Register<ReversibleStackPanel, bool>("ReverseOrder", defaultValue: false);

	public bool ReverseOrder
	{
		get
		{
			return GetValue(ReverseOrderProperty);
		}
		set
		{
			SetValue(ReverseOrderProperty, value);
		}
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		Orientation orientation = base.Orientation;
		double spacing = base.Spacing;
		Rect constraint = new Rect(finalSize);
		double num = 0.0;
		IEnumerable<Control> enumerable;
		if (!ReverseOrder)
		{
			IEnumerable<Control> children = base.Children;
			enumerable = children;
		}
		else
		{
			enumerable = base.Children.Reverse();
		}
		foreach (Control item in enumerable)
		{
			if (item.IsVisible)
			{
				double width = item.DesiredSize.Width;
				double height = item.DesiredSize.Height;
				if (orientation == Orientation.Vertical)
				{
					Rect rect = new Rect(0.0, num, width, height).Align(constraint, item.HorizontalAlignment, VerticalAlignment.Top);
					ArrangeChild(item, rect, finalSize, orientation);
					num += height + spacing;
				}
				else
				{
					Rect rect2 = new Rect(num, 0.0, width, height).Align(constraint, HorizontalAlignment.Left, item.VerticalAlignment);
					ArrangeChild(item, rect2, finalSize, orientation);
					num += width + spacing;
				}
			}
		}
		return finalSize;
	}
}
