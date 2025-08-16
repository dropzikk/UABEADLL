using Avalonia.Collections;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Utilities;

namespace Avalonia.Controls;

public class TickBar : Control
{
	public static readonly StyledProperty<IBrush?> FillProperty;

	public static readonly StyledProperty<double> MinimumProperty;

	public static readonly StyledProperty<double> MaximumProperty;

	public static readonly StyledProperty<double> TickFrequencyProperty;

	public static readonly StyledProperty<Orientation> OrientationProperty;

	public static readonly StyledProperty<AvaloniaList<double>?> TicksProperty;

	public static readonly StyledProperty<bool> IsDirectionReversedProperty;

	public static readonly StyledProperty<TickBarPlacement> PlacementProperty;

	public static readonly StyledProperty<Rect> ReservedSpaceProperty;

	public IBrush? Fill
	{
		get
		{
			return GetValue(FillProperty);
		}
		set
		{
			SetValue(FillProperty, value);
		}
	}

	public double Minimum
	{
		get
		{
			return GetValue(MinimumProperty);
		}
		set
		{
			SetValue(MinimumProperty, value);
		}
	}

	public double Maximum
	{
		get
		{
			return GetValue(MaximumProperty);
		}
		set
		{
			SetValue(MaximumProperty, value);
		}
	}

	public double TickFrequency
	{
		get
		{
			return GetValue(TickFrequencyProperty);
		}
		set
		{
			SetValue(TickFrequencyProperty, value);
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

	public AvaloniaList<double>? Ticks
	{
		get
		{
			return GetValue(TicksProperty);
		}
		set
		{
			SetValue(TicksProperty, value);
		}
	}

	public bool IsDirectionReversed
	{
		get
		{
			return GetValue(IsDirectionReversedProperty);
		}
		set
		{
			SetValue(IsDirectionReversedProperty, value);
		}
	}

	public TickBarPlacement Placement
	{
		get
		{
			return GetValue(PlacementProperty);
		}
		set
		{
			SetValue(PlacementProperty, value);
		}
	}

	public Rect ReservedSpace
	{
		get
		{
			return GetValue(ReservedSpaceProperty);
		}
		set
		{
			SetValue(ReservedSpaceProperty, value);
		}
	}

	static TickBar()
	{
		FillProperty = AvaloniaProperty.Register<TickBar, IBrush>("Fill");
		MinimumProperty = AvaloniaProperty.Register<TickBar, double>("Minimum", 0.0);
		MaximumProperty = AvaloniaProperty.Register<TickBar, double>("Maximum", 0.0);
		TickFrequencyProperty = AvaloniaProperty.Register<TickBar, double>("TickFrequency", 0.0);
		OrientationProperty = AvaloniaProperty.Register<TickBar, Orientation>("Orientation", Orientation.Horizontal);
		TicksProperty = AvaloniaProperty.Register<TickBar, AvaloniaList<double>>("Ticks");
		IsDirectionReversedProperty = AvaloniaProperty.Register<TickBar, bool>("IsDirectionReversed", defaultValue: false);
		PlacementProperty = AvaloniaProperty.Register<TickBar, TickBarPlacement>("Placement", TickBarPlacement.Left);
		ReservedSpaceProperty = AvaloniaProperty.Register<TickBar, Rect>("ReservedSpace");
		Visual.AffectsRender<TickBar>(new AvaloniaProperty[9] { FillProperty, IsDirectionReversedProperty, ReservedSpaceProperty, MaximumProperty, MinimumProperty, OrientationProperty, PlacementProperty, TickFrequencyProperty, TicksProperty });
	}

	public sealed override void Render(DrawingContext dc)
	{
		Size size = new Size(base.Bounds.Width, base.Bounds.Height);
		double num = Maximum - Minimum;
		double num2 = 0.0;
		double num3 = 0.0;
		double num4 = 1.0;
		Point point = default(Point);
		Point point2 = default(Point);
		double num5 = ((Orientation == Orientation.Horizontal) ? ReservedSpace.Width : ReservedSpace.Height);
		double num6 = num5 * 0.5;
		switch (Placement)
		{
		case TickBarPlacement.Top:
			if (MathUtilities.GreaterThanOrClose(num5, size.Width))
			{
				return;
			}
			size = new Size(size.Width - num5, size.Height);
			num2 = 0.0 - size.Height;
			point = new Point(num6, size.Height);
			point2 = new Point(num6 + size.Width, size.Height);
			num4 = size.Width / num;
			break;
		case TickBarPlacement.Bottom:
			if (MathUtilities.GreaterThanOrClose(num5, size.Width))
			{
				return;
			}
			size = new Size(size.Width - num5, size.Height);
			num2 = size.Height;
			point = new Point(num6, 0.0);
			point2 = new Point(num6 + size.Width, 0.0);
			num4 = size.Width / num;
			break;
		case TickBarPlacement.Left:
			if (MathUtilities.GreaterThanOrClose(num5, size.Height))
			{
				return;
			}
			size = new Size(size.Width, size.Height - num5);
			num2 = 0.0 - size.Width;
			point = new Point(size.Width, size.Height + num6);
			point2 = new Point(size.Width, num6);
			num4 = size.Height / num * -1.0;
			break;
		case TickBarPlacement.Right:
			if (MathUtilities.GreaterThanOrClose(num5, size.Height))
			{
				return;
			}
			size = new Size(size.Width, size.Height - num5);
			num2 = size.Width;
			point = new Point(0.0, size.Height + num6);
			point2 = new Point(0.0, num6);
			num4 = size.Height / num * -1.0;
			break;
		}
		num3 = num2 * 0.75;
		if (IsDirectionReversed)
		{
			num4 *= -1.0;
			Point point3 = point;
			point = point2;
			point2 = point3;
		}
		ImmutablePen pen = new ImmutablePen(Fill?.ToImmutable());
		if (Placement == TickBarPlacement.Left || Placement == TickBarPlacement.Right)
		{
			double num7 = TickFrequency;
			if (num7 > 0.0)
			{
				double num8 = (Maximum - Minimum) / size.Height;
				if (num7 < num8)
				{
					num7 = num8;
				}
			}
			dc.DrawLine(pen, point, new Point(point.X + num2, point.Y));
			dc.DrawLine(pen, new Point(point.X, point2.Y), new Point(point.X + num2, point2.Y));
			AvaloniaList<double> avaloniaList = Ticks ?? null;
			if (avaloniaList != null && avaloniaList.Count > 0)
			{
				for (int i = 0; i < avaloniaList.Count; i++)
				{
					if (!MathUtilities.LessThanOrClose(avaloniaList[i], Minimum) && !MathUtilities.GreaterThanOrClose(avaloniaList[i], Maximum))
					{
						double y = (avaloniaList[i] - Minimum) * num4 + point.Y;
						dc.DrawLine(pen, new Point(point.X, y), new Point(point.X + num3, y));
					}
				}
			}
			else if (num7 > 0.0)
			{
				for (double num9 = num7; num9 < num; num9 += num7)
				{
					double y2 = num9 * num4 + point.Y;
					dc.DrawLine(pen, new Point(point.X, y2), new Point(point.X + num3, y2));
				}
			}
			return;
		}
		double num10 = TickFrequency;
		if (num10 > 0.0)
		{
			double num11 = (Maximum - Minimum) / size.Width;
			if (num10 < num11)
			{
				num10 = num11;
			}
		}
		dc.DrawLine(pen, point, new Point(point.X, point.Y + num2));
		dc.DrawLine(pen, new Point(point2.X, point.Y), new Point(point2.X, point.Y + num2));
		AvaloniaList<double> avaloniaList2 = Ticks ?? null;
		if (avaloniaList2 != null && avaloniaList2.Count > 0)
		{
			for (int j = 0; j < avaloniaList2.Count; j++)
			{
				if (!MathUtilities.LessThanOrClose(avaloniaList2[j], Minimum) && !MathUtilities.GreaterThanOrClose(avaloniaList2[j], Maximum))
				{
					double x = (avaloniaList2[j] - Minimum) * num4 + point.X;
					dc.DrawLine(pen, new Point(x, point.Y), new Point(x, point.Y + num3));
				}
			}
		}
		else if (num10 > 0.0)
		{
			for (double num12 = num10; num12 < num; num12 += num10)
			{
				double x2 = num12 * num4 + point.X;
				dc.DrawLine(pen, new Point(x2, point.Y), new Point(x2, point.Y + num3));
			}
		}
	}
}
