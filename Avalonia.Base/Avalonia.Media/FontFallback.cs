namespace Avalonia.Media;

public class FontFallback
{
	public FontFamily FontFamily { get; set; } = Avalonia.Media.FontFamily.Default;

	public UnicodeRange UnicodeRange { get; set; } = UnicodeRange.Default;
}
