using System;
using Avalonia.Media;

namespace Avalonia.Controls.Shapes;

public class Path : Shape
{
	public static readonly StyledProperty<Geometry> DataProperty;

	private EventHandler? _geometryChangedHandler;

	public Geometry Data
	{
		get
		{
			return GetValue(DataProperty);
		}
		set
		{
			SetValue(DataProperty, value);
		}
	}

	private EventHandler? GeometryChangedHandler => GeometryChanged;

	static Path()
	{
		DataProperty = AvaloniaProperty.Register<Path, Geometry>("Data");
		Shape.AffectsGeometry<Path>(new AvaloniaProperty[1] { DataProperty });
		DataProperty.Changed.AddClassHandler(delegate(Path o, AvaloniaPropertyChangedEventArgs e)
		{
			o.DataChanged(e);
		});
	}

	protected override Geometry CreateDefiningGeometry()
	{
		return Data;
	}

	protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnAttachedToVisualTree(e);
		if (Data != null)
		{
			Data.Changed += GeometryChangedHandler;
		}
	}

	protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnDetachedFromVisualTree(e);
		if (Data != null)
		{
			Data.Changed -= GeometryChangedHandler;
		}
	}

	private void DataChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (base.VisualRoot != null)
		{
			Geometry geometry = (Geometry)e.OldValue;
			Geometry geometry2 = (Geometry)e.NewValue;
			if (geometry != null)
			{
				geometry.Changed -= GeometryChangedHandler;
			}
			if (geometry2 != null)
			{
				geometry2.Changed += GeometryChangedHandler;
			}
		}
	}

	private void GeometryChanged(object? sender, EventArgs e)
	{
		InvalidateGeometry();
	}
}
