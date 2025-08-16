namespace Avalonia.Media.TextFormatting;

public sealed class TextTrailingWordEllipsis : TextCollapsingProperties
{
	public override double Width { get; }

	public override TextRun Symbol { get; }

	public override FlowDirection FlowDirection { get; }

	public TextTrailingWordEllipsis(string ellipsis, double width, TextRunProperties textRunProperties, FlowDirection flowDirection)
	{
		Width = width;
		Symbol = new TextCharacters(ellipsis, textRunProperties);
		FlowDirection = flowDirection;
	}

	public override TextRun[]? Collapse(TextLine textLine)
	{
		return TextEllipsisHelper.Collapse(textLine, this, isWordEllipsis: true);
	}
}
