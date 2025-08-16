namespace Avalonia.Media.TextFormatting;

public abstract class TextFormatter
{
	public static TextFormatter Current
	{
		get
		{
			TextFormatter service = AvaloniaLocator.Current.GetService<TextFormatter>();
			if (service != null)
			{
				return service;
			}
			service = new TextFormatterImpl();
			AvaloniaLocator.CurrentMutable.Bind<TextFormatter>().ToConstant(service);
			return service;
		}
	}

	public abstract TextLine? FormatLine(ITextSource textSource, int firstTextSourceIndex, double paragraphWidth, TextParagraphProperties paragraphProperties, TextLineBreak? previousLineBreak = null);
}
