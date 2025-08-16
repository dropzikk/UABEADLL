namespace Avalonia.Media.TextFormatting;

public abstract class DrawableTextRun : TextRun
{
	public abstract Size Size { get; }

	public abstract double Baseline { get; }

	public abstract void Draw(DrawingContext drawingContext, Point origin);
}
