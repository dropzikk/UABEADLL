using Avalonia.Metadata;

namespace Avalonia.Media;

[NotClientImplementable]
public interface IPen
{
	IBrush? Brush { get; }

	IDashStyle? DashStyle { get; }

	PenLineCap LineCap { get; }

	PenLineJoin LineJoin { get; }

	double MiterLimit { get; }

	double Thickness { get; }
}
