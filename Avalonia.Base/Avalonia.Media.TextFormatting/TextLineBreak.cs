namespace Avalonia.Media.TextFormatting;

public class TextLineBreak
{
	public TextEndOfLine? TextEndOfLine { get; }

	public FlowDirection FlowDirection { get; }

	public bool IsSplit { get; }

	public TextLineBreak(TextEndOfLine? textEndOfLine = null, FlowDirection flowDirection = FlowDirection.LeftToRight, bool isSplit = false)
	{
		TextEndOfLine = textEndOfLine;
		FlowDirection = flowDirection;
		IsSplit = isSplit;
	}
}
