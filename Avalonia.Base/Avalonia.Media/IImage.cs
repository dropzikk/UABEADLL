namespace Avalonia.Media;

public interface IImage
{
	Size Size { get; }

	void Draw(DrawingContext context, Rect sourceRect, Rect destRect);
}
