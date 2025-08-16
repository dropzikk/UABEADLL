namespace Avalonia.Media.TextFormatting;

public interface ITextSource
{
	TextRun? GetTextRun(int textSourceIndex);
}
