namespace Avalonia.Media.TextFormatting;

public abstract class TextCollapsingProperties
{
	public abstract double Width { get; }

	public abstract TextRun Symbol { get; }

	public abstract FlowDirection FlowDirection { get; }

	public abstract TextRun[]? Collapse(TextLine textLine);
}
