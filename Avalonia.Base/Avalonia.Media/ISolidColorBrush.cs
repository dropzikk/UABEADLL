using Avalonia.Metadata;

namespace Avalonia.Media;

[NotClientImplementable]
public interface ISolidColorBrush : IBrush
{
	Color Color { get; }
}
