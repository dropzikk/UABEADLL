namespace Avalonia.Media;

public abstract class Drawing : AvaloniaObject
{
	internal Drawing()
	{
	}

	public void Draw(DrawingContext context)
	{
		DrawCore(context);
	}

	internal abstract void DrawCore(DrawingContext context);

	public abstract Rect GetBounds();
}
