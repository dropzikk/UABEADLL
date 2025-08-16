namespace Avalonia.Media.TextFormatting;

public class TextEndOfLine : TextRun
{
	public override int Length { get; }

	public TextEndOfLine(int textSourceLength = 1)
	{
		Length = textSourceLength;
	}
}
