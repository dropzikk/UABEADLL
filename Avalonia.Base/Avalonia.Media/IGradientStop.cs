using Avalonia.Metadata;

namespace Avalonia.Media;

[NotClientImplementable]
public interface IGradientStop
{
	Color Color { get; }

	double Offset { get; }
}
