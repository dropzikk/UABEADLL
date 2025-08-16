using Avalonia.Metadata;

namespace Avalonia.Media;

[NotClientImplementable]
public interface ILinearGradientBrush : IGradientBrush, IBrush
{
	RelativePoint StartPoint { get; }

	RelativePoint EndPoint { get; }
}
