using System;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Utilities;

namespace Avalonia.Controls;

public class WrapPanel : Panel, INavigableContainer
{
	private struct UVSize
	{
		internal double U;

		internal double V;

		private Orientation _orientation;

		internal double Width
		{
			get
			{
				if (_orientation != 0)
				{
					return V;
				}
				return U;
			}
			set
			{
				if (_orientation == Orientation.Horizontal)
				{
					U = value;
				}
				else
				{
					V = value;
				}
			}
		}

		internal double Height
		{
			get
			{
				if (_orientation != 0)
				{
					return U;
				}
				return V;
			}
			set
			{
				if (_orientation == Orientation.Horizontal)
				{
					V = value;
				}
				else
				{
					U = value;
				}
			}
		}

		internal UVSize(Orientation orientation, double width, double height)
		{
			U = (V = 0.0);
			_orientation = orientation;
			Width = width;
			Height = height;
		}

		internal UVSize(Orientation orientation)
		{
			U = (V = 0.0);
			_orientation = orientation;
		}
	}

	public static readonly StyledProperty<Orientation> OrientationProperty;

	public static readonly StyledProperty<double> ItemWidthProperty;

	public static readonly StyledProperty<double> ItemHeightProperty;

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

	public double ItemWidth
	{
		get
		{
			return GetValue(ItemWidthProperty);
		}
		set
		{
			SetValue(ItemWidthProperty, value);
		}
	}

	public double ItemHeight
	{
		get
		{
			return GetValue(ItemHeightProperty);
		}
		set
		{
			SetValue(ItemHeightProperty, value);
		}
	}

	static WrapPanel()
	{
		OrientationProperty = AvaloniaProperty.Register<WrapPanel, Orientation>("Orientation", Orientation.Horizontal);
		ItemWidthProperty = AvaloniaProperty.Register<WrapPanel, double>("ItemWidth", double.NaN);
		ItemHeightProperty = AvaloniaProperty.Register<WrapPanel, double>("ItemHeight", double.NaN);
		Layoutable.AffectsMeasure<WrapPanel>(new AvaloniaProperty[3] { OrientationProperty, ItemWidthProperty, ItemHeightProperty });
	}

	IInputElement? INavigableContainer.GetControl(NavigationDirection direction, IInputElement? from, bool wrap)
	{
		Orientation orientation = Orientation;
		Controls children = base.Children;
		bool flag = orientation == Orientation.Horizontal;
		int num = ((from != null) ? base.Children.IndexOf((Control)from) : (-1));
		switch (direction)
		{
		case NavigationDirection.First:
			num = 0;
			break;
		case NavigationDirection.Last:
			num = children.Count - 1;
			break;
		case NavigationDirection.Next:
			num++;
			break;
		case NavigationDirection.Previous:
			num--;
			break;
		case NavigationDirection.Left:
			num = (flag ? (num - 1) : (-1));
			break;
		case NavigationDirection.Right:
			num = (flag ? (num + 1) : (-1));
			break;
		case NavigationDirection.Up:
			num = (flag ? (-1) : (num - 1));
			break;
		case NavigationDirection.Down:
			num = (flag ? (-1) : (num + 1));
			break;
		}
		if (num >= 0 && num < children.Count)
		{
			return children[num];
		}
		return null;
	}

	protected override Size MeasureOverride(Size constraint)
	{
		double itemWidth = ItemWidth;
		double itemHeight = ItemHeight;
		Orientation orientation = Orientation;
		Controls children = base.Children;
		UVSize uVSize = new UVSize(orientation);
		UVSize uVSize2 = new UVSize(orientation);
		UVSize uVSize3 = new UVSize(orientation, constraint.Width, constraint.Height);
		bool flag = !double.IsNaN(itemWidth);
		bool flag2 = !double.IsNaN(itemHeight);
		Size availableSize = new Size(flag ? itemWidth : constraint.Width, flag2 ? itemHeight : constraint.Height);
		int i = 0;
		for (int count = children.Count; i < count; i++)
		{
			Control control = children[i];
			control.Measure(availableSize);
			UVSize uVSize4 = new UVSize(orientation, flag ? itemWidth : control.DesiredSize.Width, flag2 ? itemHeight : control.DesiredSize.Height);
			if (MathUtilities.GreaterThan(uVSize.U + uVSize4.U, uVSize3.U))
			{
				uVSize2.U = Math.Max(uVSize.U, uVSize2.U);
				uVSize2.V += uVSize.V;
				uVSize = uVSize4;
				if (MathUtilities.GreaterThan(uVSize4.U, uVSize3.U))
				{
					uVSize2.U = Math.Max(uVSize4.U, uVSize2.U);
					uVSize2.V += uVSize4.V;
					uVSize = new UVSize(orientation);
				}
			}
			else
			{
				uVSize.U += uVSize4.U;
				uVSize.V = Math.Max(uVSize4.V, uVSize.V);
			}
		}
		uVSize2.U = Math.Max(uVSize.U, uVSize2.U);
		uVSize2.V += uVSize.V;
		return new Size(uVSize2.Width, uVSize2.Height);
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		double itemWidth = ItemWidth;
		double itemHeight = ItemHeight;
		Orientation orientation = Orientation;
		Controls children = base.Children;
		int num = 0;
		double num2 = 0.0;
		double itemU = ((orientation == Orientation.Horizontal) ? itemWidth : itemHeight);
		UVSize uVSize = new UVSize(orientation);
		UVSize uVSize2 = new UVSize(orientation, finalSize.Width, finalSize.Height);
		bool flag = !double.IsNaN(itemWidth);
		bool flag2 = !double.IsNaN(itemHeight);
		bool useItemU = ((orientation == Orientation.Horizontal) ? flag : flag2);
		for (int i = 0; i < children.Count; i++)
		{
			Control control = children[i];
			UVSize uVSize3 = new UVSize(orientation, flag ? itemWidth : control.DesiredSize.Width, flag2 ? itemHeight : control.DesiredSize.Height);
			if (MathUtilities.GreaterThan(uVSize.U + uVSize3.U, uVSize2.U))
			{
				ArrangeLine(num2, uVSize.V, num, i, useItemU, itemU);
				num2 += uVSize.V;
				uVSize = uVSize3;
				if (MathUtilities.GreaterThan(uVSize3.U, uVSize2.U))
				{
					ArrangeLine(num2, uVSize3.V, i, ++i, useItemU, itemU);
					num2 += uVSize3.V;
					uVSize = new UVSize(orientation);
				}
				num = i;
			}
			else
			{
				uVSize.U += uVSize3.U;
				uVSize.V = Math.Max(uVSize3.V, uVSize.V);
			}
		}
		if (num < children.Count)
		{
			ArrangeLine(num2, uVSize.V, num, children.Count, useItemU, itemU);
		}
		return finalSize;
	}

	private void ArrangeLine(double v, double lineV, int start, int end, bool useItemU, double itemU)
	{
		Orientation orientation = Orientation;
		Controls children = base.Children;
		double num = 0.0;
		bool flag = orientation == Orientation.Horizontal;
		for (int i = start; i < end; i++)
		{
			Control control = children[i];
			UVSize uVSize = new UVSize(orientation, control.DesiredSize.Width, control.DesiredSize.Height);
			double num2 = (useItemU ? itemU : uVSize.U);
			control.Arrange(new Rect(flag ? num : v, flag ? v : num, flag ? num2 : lineV, flag ? lineV : num2));
			num += num2;
		}
	}
}
