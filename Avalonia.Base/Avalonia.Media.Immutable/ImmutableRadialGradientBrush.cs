using System.Collections.Generic;

namespace Avalonia.Media.Immutable;

public class ImmutableRadialGradientBrush : ImmutableGradientBrush, IRadialGradientBrush, IGradientBrush, IBrush
{
	public RelativePoint Center { get; }

	public RelativePoint GradientOrigin { get; }

	public double Radius { get; }

	public ImmutableRadialGradientBrush(IReadOnlyList<ImmutableGradientStop> gradientStops, double opacity = 1.0, ImmutableTransform? transform = null, RelativePoint? transformOrigin = null, GradientSpreadMethod spreadMethod = GradientSpreadMethod.Pad, RelativePoint? center = null, RelativePoint? gradientOrigin = null, double radius = 0.5)
		: base(gradientStops, opacity, transform, transformOrigin, spreadMethod)
	{
		Center = center ?? RelativePoint.Center;
		GradientOrigin = gradientOrigin ?? RelativePoint.Center;
		Radius = radius;
	}

	public ImmutableRadialGradientBrush(RadialGradientBrush source)
		: base(source)
	{
		Center = source.Center;
		GradientOrigin = source.GradientOrigin;
		Radius = source.Radius;
	}
}
