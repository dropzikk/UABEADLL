using Avalonia.Metadata;

namespace Avalonia.Media;

[NotClientImplementable]
public interface IImageBrush : ITileBrush, IBrush
{
	IImageBrushSource? Source { get; }
}
