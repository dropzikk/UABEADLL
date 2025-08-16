using System.Globalization;

namespace Avalonia.Media.TextFormatting;

public class GenericTextRunProperties : TextRunProperties
{
	private const double DefaultFontRenderingEmSize = 12.0;

	public override Typeface Typeface { get; }

	public override double FontRenderingEmSize { get; }

	public override TextDecorationCollection? TextDecorations { get; }

	public override IBrush? ForegroundBrush { get; }

	public override IBrush? BackgroundBrush { get; }

	public override BaselineAlignment BaselineAlignment { get; }

	public override CultureInfo? CultureInfo { get; }

	public GenericTextRunProperties(Typeface typeface, double fontRenderingEmSize = 12.0, TextDecorationCollection? textDecorations = null, IBrush? foregroundBrush = null, IBrush? backgroundBrush = null, BaselineAlignment baselineAlignment = BaselineAlignment.Baseline, CultureInfo? cultureInfo = null)
	{
		Typeface = typeface;
		FontRenderingEmSize = fontRenderingEmSize;
		TextDecorations = textDecorations;
		ForegroundBrush = foregroundBrush;
		BackgroundBrush = backgroundBrush;
		BaselineAlignment = baselineAlignment;
		CultureInfo = cultureInfo;
	}
}
