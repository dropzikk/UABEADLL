using Avalonia.Media.Immutable;
using Avalonia.Metadata;

namespace Avalonia.Media;

public sealed class GeometryDrawing : Drawing
{
	private static readonly IPen s_boundsPen = new ImmutablePen(Colors.Black.ToUInt32(), 0.0);

	public static readonly StyledProperty<Geometry?> GeometryProperty = AvaloniaProperty.Register<GeometryDrawing, Geometry>("Geometry");

	public static readonly StyledProperty<IBrush?> BrushProperty = AvaloniaProperty.Register<GeometryDrawing, IBrush>("Brush", Brushes.Transparent);

	public static readonly StyledProperty<IPen?> PenProperty = AvaloniaProperty.Register<GeometryDrawing, IPen>("Pen");

	[Content]
	public Geometry? Geometry
	{
		get
		{
			return GetValue(GeometryProperty);
		}
		set
		{
			SetValue(GeometryProperty, value);
		}
	}

	public IBrush? Brush
	{
		get
		{
			return GetValue(BrushProperty);
		}
		set
		{
			SetValue(BrushProperty, value);
		}
	}

	public IPen? Pen
	{
		get
		{
			return GetValue(PenProperty);
		}
		set
		{
			SetValue(PenProperty, value);
		}
	}

	internal override void DrawCore(DrawingContext context)
	{
		if (Geometry != null)
		{
			context.DrawGeometry(Brush, Pen, Geometry);
		}
	}

	public override Rect GetBounds()
	{
		IPen pen = Pen ?? s_boundsPen;
		return Geometry?.GetRenderBounds(pen) ?? default(Rect);
	}
}
