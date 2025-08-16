namespace Avalonia.Animation.Easings;

public class QuadraticEaseOut : Easing
{
	public override double Ease(double progress)
	{
		return 0.0 - progress * (progress - 2.0);
	}
}
