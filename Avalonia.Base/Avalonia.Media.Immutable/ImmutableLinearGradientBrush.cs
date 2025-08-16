using System.Collections.Generic;

namespace Avalonia.Media.Immutable;

public class ImmutableLinearGradientBrush : ImmutableGradientBrush, ILinearGradientBrush, IGradientBrush, IBrush
{
	public RelativePoint StartPoint { get; }

	public RelativePoint EndPoint { get; }

	public ImmutableLinearGradientBrush(IReadOnlyList<ImmutableGradientStop> gradientStops, double opacity = 1.0, ImmutableTransform? transform = null, RelativePoint? transformOrigin = null, GradientSpreadMethod spreadMethod = GradientSpreadMethod.Pad, RelativePoint? startPoint = null, RelativePoint? endPoint = null)
		: base(gradientStops, opacity, transform, transformOrigin, spreadMethod)
	{
		StartPoint = startPoint ?? RelativePoint.TopLeft;
		EndPoint = endPoint ?? RelativePoint.BottomRight;
	}

	public ImmutableLinearGradientBrush(LinearGradientBrush source)
		: base(source)
	{
		StartPoint = source.StartPoint;
		EndPoint = source.EndPoint;
	}
}
