namespace Avalonia.Media;

public readonly record struct GlyphMetrics
{
	public int XBearing { get; init; }

	public int YBearing { get; init; }

	public int Width { get; init; }

	public int Height { get; init; }
}
