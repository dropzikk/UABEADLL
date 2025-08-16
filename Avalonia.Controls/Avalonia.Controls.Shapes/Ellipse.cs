using Avalonia.Media;

namespace Avalonia.Controls.Shapes;

public class Ellipse : Shape
{
	static Ellipse()
	{
		Shape.AffectsGeometry<Ellipse>(new AvaloniaProperty[2]
		{
			Visual.BoundsProperty,
			Shape.StrokeThicknessProperty
		});
	}

	protected override Geometry CreateDefiningGeometry()
	{
		return new EllipseGeometry(new Rect(base.Bounds.Size).Deflate(base.StrokeThickness / 2.0));
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		return new Size(base.StrokeThickness, base.StrokeThickness);
	}
}
