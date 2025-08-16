using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Metadata;

namespace Avalonia.Controls;

public class Decorator : Control
{
	public static readonly StyledProperty<Control?> ChildProperty;

	public static readonly StyledProperty<Thickness> PaddingProperty;

	[Content]
	public Control? Child
	{
		get
		{
			return GetValue(ChildProperty);
		}
		set
		{
			SetValue(ChildProperty, value);
		}
	}

	public Thickness Padding
	{
		get
		{
			return GetValue(PaddingProperty);
		}
		set
		{
			SetValue(PaddingProperty, value);
		}
	}

	static Decorator()
	{
		ChildProperty = AvaloniaProperty.Register<Decorator, Control>("Child");
		PaddingProperty = AvaloniaProperty.Register<Decorator, Thickness>("Padding");
		Layoutable.AffectsMeasure<Decorator>(new AvaloniaProperty[2] { ChildProperty, PaddingProperty });
		ChildProperty.Changed.AddClassHandler(delegate(Decorator x, AvaloniaPropertyChangedEventArgs e)
		{
			x.ChildChanged(e);
		});
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		return LayoutHelper.MeasureChild(Child, availableSize, Padding);
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		return LayoutHelper.ArrangeChild(Child, finalSize, Padding);
	}

	private void ChildChanged(AvaloniaPropertyChangedEventArgs e)
	{
		Control control = (Control)e.OldValue;
		Control control2 = (Control)e.NewValue;
		if (control != null)
		{
			((ISetLogicalParent)control).SetParent((ILogical?)null);
			base.LogicalChildren.Clear();
			base.VisualChildren.Remove(control);
		}
		if (control2 != null)
		{
			((ISetLogicalParent)control2).SetParent((ILogical?)this);
			base.VisualChildren.Add(control2);
			base.LogicalChildren.Add(control2);
		}
	}
}
