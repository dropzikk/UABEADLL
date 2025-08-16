namespace Avalonia.Animation.Easings;

public class CubicEaseOut : Easing
{
	public override double Ease(double progress)
	{
		double num = progress - 1.0;
		return num * num * num + 1.0;
	}
}
