namespace Avalonia.Media.Immutable;

public class ImmutableGradientStop : IGradientStop
{
	public double Offset { get; }

	public Color Color { get; }

	public ImmutableGradientStop(double offset, Color color)
	{
		Offset = offset;
		Color = color;
	}
}
