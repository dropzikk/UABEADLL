using Avalonia.Media;

namespace AvaloniaEdit.Rendering;

public interface IBackgroundRenderer
{
	KnownLayer Layer { get; }

	void Draw(TextView textView, DrawingContext drawingContext);
}
