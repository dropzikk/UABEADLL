using Avalonia.Metadata;

namespace Avalonia.Media;

[NotClientImplementable]
public interface IExperimentalAcrylicMaterial
{
	AcrylicBackgroundSource BackgroundSource { get; }

	Color TintColor { get; }

	double TintOpacity { get; }

	Color MaterialColor { get; }

	Color FallbackColor { get; }
}
