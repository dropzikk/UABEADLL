namespace Avalonia.Media.TextFormatting;

public abstract class JustificationProperties
{
	public abstract double Width { get; }

	public abstract void Justify(TextLine textLine);
}
