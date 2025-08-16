namespace Avalonia.Media.TextFormatting;

public readonly record struct TextMetrics
{
	public double FontRenderingEmSize { get; }

	public double Ascent { get; }

	public double Descent { get; }

	public double LineGap { get; }

	public double LineHeight { get; }

	public double UnderlineThickness { get; }

	public double UnderlinePosition { get; }

	public double StrikethroughThickness { get; }

	public double StrikethroughPosition { get; }

	public TextMetrics(IGlyphTypeface glyphTypeface, double fontRenderingEmSize)
	{
		FontMetrics metrics = glyphTypeface.Metrics;
		double num = fontRenderingEmSize / (double)metrics.DesignEmHeight;
		FontRenderingEmSize = fontRenderingEmSize;
		Ascent = (double)metrics.Ascent * num;
		Descent = (double)metrics.Descent * num;
		LineGap = (double)metrics.LineGap * num;
		LineHeight = Descent - Ascent + LineGap;
		UnderlineThickness = (double)metrics.UnderlineThickness * num;
		UnderlinePosition = (double)metrics.UnderlinePosition * num;
		StrikethroughThickness = (double)metrics.StrikethroughThickness * num;
		StrikethroughPosition = (double)metrics.StrikethroughPosition * num;
	}
}
