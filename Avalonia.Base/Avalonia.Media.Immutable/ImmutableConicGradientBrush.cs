using System.Collections.Generic;

namespace Avalonia.Media.Immutable;

public class ImmutableConicGradientBrush : ImmutableGradientBrush, IConicGradientBrush, IGradientBrush, IBrush
{
	public RelativePoint Center { get; }

	public double Angle { get; }

	public ImmutableConicGradientBrush(IReadOnlyList<ImmutableGradientStop> gradientStops, double opacity = 1.0, ImmutableTransform? transform = null, RelativePoint? transformOrigin = null, GradientSpreadMethod spreadMethod = GradientSpreadMethod.Pad, RelativePoint? center = null, double angle = 0.0)
		: base(gradientStops, opacity, transform, transformOrigin, spreadMethod)
	{
		Center = center ?? RelativePoint.Center;
		Angle = angle;
	}

	public ImmutableConicGradientBrush(ConicGradientBrush source)
		: base(source)
	{
		Center = source.Center;
		Angle = source.Angle;
	}
}
