using Avalonia.Media;
using Avalonia.Utilities;

namespace Avalonia.Controls.Primitives;

internal struct Rgb
{
	public double R;

	public double G;

	public double B;

	public Rgb(double r, double g, double b)
	{
		R = r;
		G = g;
		B = b;
	}

	public Rgb(Color color)
	{
		R = (double)(int)color.R / 255.0;
		G = (double)(int)color.G / 255.0;
		B = (double)(int)color.B / 255.0;
	}

	public Color ToColor(double alpha = 1.0)
	{
		return Color.FromArgb((byte)MathUtilities.Clamp(alpha * 255.0, 0.0, 255.0), (byte)MathUtilities.Clamp(R * 255.0, 0.0, 255.0), (byte)MathUtilities.Clamp(G * 255.0, 0.0, 255.0), (byte)MathUtilities.Clamp(B * 255.0, 0.0, 255.0));
	}

	public Hsv ToHsv()
	{
		return new Hsv(Color.ToHsv(MathUtilities.Clamp(R, 0.0, 1.0), MathUtilities.Clamp(G, 0.0, 1.0), MathUtilities.Clamp(B, 0.0, 1.0)));
	}
}
