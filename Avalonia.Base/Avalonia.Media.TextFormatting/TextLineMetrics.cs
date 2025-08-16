namespace Avalonia.Media.TextFormatting;

public readonly record struct TextLineMetrics
{
	public bool HasOverflowed { get; init; }

	public double Height { get; init; }

	public int NewlineLength { get; init; }

	public double Start { get; init; }

	public double TextBaseline { get; init; }

	public int TrailingWhitespaceLength { get; init; }

	public double Width { get; init; }

	public double WidthIncludingTrailingWhitespace { get; init; }

	public double Extent { get; init; }

	public double OverhangAfter { get; init; }

	public double OverhangLeading { get; init; }

	public double OverhangTrailing { get; init; }
}
