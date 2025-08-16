namespace Avalonia.Media;

public readonly record struct FontMetrics
{
	public short DesignEmHeight { get; init; }

	public bool IsFixedPitch { get; init; }

	public int Ascent { get; init; }

	public int Descent { get; init; }

	public int LineGap { get; init; }

	public int LineSpacing => Descent - Ascent + LineGap;

	public int UnderlinePosition { get; init; }

	public int UnderlineThickness { get; init; }

	public int StrikethroughPosition { get; init; }

	public int StrikethroughThickness { get; init; }
}
