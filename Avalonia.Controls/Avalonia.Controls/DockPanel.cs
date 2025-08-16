using System;

namespace Avalonia.Controls;

public class DockPanel : Panel
{
	public static readonly AttachedProperty<Dock> DockProperty;

	public static readonly StyledProperty<bool> LastChildFillProperty;

	public bool LastChildFill
	{
		get
		{
			return GetValue(LastChildFillProperty);
		}
		set
		{
			SetValue(LastChildFillProperty, value);
		}
	}

	static DockPanel()
	{
		DockProperty = AvaloniaProperty.RegisterAttached<DockPanel, Control, Dock>("Dock", Dock.Left);
		LastChildFillProperty = AvaloniaProperty.Register<DockPanel, bool>("LastChildFill", defaultValue: true);
		Panel.AffectsParentMeasure<DockPanel>(new AvaloniaProperty[1] { DockProperty });
	}

	public static Dock GetDock(Control control)
	{
		return control.GetValue(DockProperty);
	}

	public static void SetDock(Control control, Dock value)
	{
		control.SetValue(DockProperty, value);
	}

	protected override Size MeasureOverride(Size constraint)
	{
		Controls children = base.Children;
		double val = 0.0;
		double val2 = 0.0;
		double num = 0.0;
		double num2 = 0.0;
		int i = 0;
		for (int count = children.Count; i < count; i++)
		{
			Control control = children[i];
			Size availableSize = new Size(Math.Max(0.0, constraint.Width - num), Math.Max(0.0, constraint.Height - num2));
			control.Measure(availableSize);
			Size desiredSize = control.DesiredSize;
			switch (GetDock(control))
			{
			case Dock.Left:
			case Dock.Right:
				val2 = Math.Max(val2, num2 + desiredSize.Height);
				num += desiredSize.Width;
				break;
			case Dock.Bottom:
			case Dock.Top:
				val = Math.Max(val, num + desiredSize.Width);
				num2 += desiredSize.Height;
				break;
			}
		}
		val = Math.Max(val, num);
		val2 = Math.Max(val2, num2);
		return new Size(val, val2);
	}

	protected override Size ArrangeOverride(Size arrangeSize)
	{
		Controls children = base.Children;
		int count = children.Count;
		int num = count - (LastChildFill ? 1 : 0);
		double num2 = 0.0;
		double num3 = 0.0;
		double num4 = 0.0;
		double num5 = 0.0;
		for (int i = 0; i < count; i++)
		{
			Control control = children[i];
			Size desiredSize = control.DesiredSize;
			Rect rect = new Rect(num2, num3, Math.Max(0.0, arrangeSize.Width - (num2 + num4)), Math.Max(0.0, arrangeSize.Height - (num3 + num5)));
			if (i < num)
			{
				switch (GetDock(control))
				{
				case Dock.Left:
					num2 += desiredSize.Width;
					rect = rect.WithWidth(desiredSize.Width);
					break;
				case Dock.Right:
					num4 += desiredSize.Width;
					rect = rect.WithX(Math.Max(0.0, arrangeSize.Width - num4)).WithWidth(desiredSize.Width);
					break;
				case Dock.Top:
					num3 += desiredSize.Height;
					rect = rect.WithHeight(desiredSize.Height);
					break;
				case Dock.Bottom:
					num5 += desiredSize.Height;
					rect = rect.WithY(Math.Max(0.0, arrangeSize.Height - num5)).WithHeight(desiredSize.Height);
					break;
				}
			}
			control.Arrange(rect);
		}
		return arrangeSize;
	}
}
