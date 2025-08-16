using Avalonia.Metadata;

namespace Avalonia.Media;

[NotClientImplementable]
public interface IConicGradientBrush : IGradientBrush, IBrush
{
	RelativePoint Center { get; }

	double Angle { get; }
}
