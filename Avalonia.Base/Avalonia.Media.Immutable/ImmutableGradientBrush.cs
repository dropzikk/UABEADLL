using System.Collections.Generic;

namespace Avalonia.Media.Immutable;

public abstract class ImmutableGradientBrush : IGradientBrush, IBrush, IImmutableBrush
{
	public IReadOnlyList<IGradientStop> GradientStops { get; }

	public double Opacity { get; }

	public ITransform? Transform { get; }

	public RelativePoint TransformOrigin { get; }

	public GradientSpreadMethod SpreadMethod { get; }

	protected ImmutableGradientBrush(IReadOnlyList<ImmutableGradientStop> gradientStops, double opacity, ImmutableTransform? transform, RelativePoint? transformOrigin, GradientSpreadMethod spreadMethod)
	{
		GradientStops = gradientStops;
		Opacity = opacity;
		Transform = transform;
		TransformOrigin = (transformOrigin.HasValue ? transformOrigin.Value : RelativePoint.TopLeft);
		SpreadMethod = spreadMethod;
	}

	protected ImmutableGradientBrush(GradientBrush source)
		: this(source.GradientStops.ToImmutable(), source.Opacity, source.Transform?.ToImmutable(), source.TransformOrigin, source.SpreadMethod)
	{
	}
}
