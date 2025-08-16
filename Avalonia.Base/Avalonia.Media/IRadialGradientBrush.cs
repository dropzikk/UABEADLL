using Avalonia.Metadata;

namespace Avalonia.Media;

[NotClientImplementable]
public interface IRadialGradientBrush : IGradientBrush, IBrush
{
	RelativePoint Center { get; }

	RelativePoint GradientOrigin { get; }

	double Radius { get; }
}
