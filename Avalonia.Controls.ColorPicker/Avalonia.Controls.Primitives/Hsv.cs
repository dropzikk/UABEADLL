using Avalonia.Media;

namespace Avalonia.Controls.Primitives;

internal struct Hsv
{
	public double H;

	public double S;

	public double V;

	public Hsv(double h, double s, double v)
	{
		H = h;
		S = s;
		V = v;
	}

	public Hsv(HsvColor hsvColor)
	{
		H = hsvColor.H;
		S = hsvColor.S;
		V = hsvColor.V;
	}

	public HsvColor ToHsvColor(double alpha = 1.0)
	{
		return HsvColor.FromAhsv(alpha, H, S, V);
	}

	public Rgb ToRgb()
	{
		return new Rgb(HsvColor.ToRgb(H, S, V));
	}
}
