namespace Avalonia.Animation.Easings;

public class QuadraticEaseInOut : Easing
{
	public override double Ease(double progress)
	{
		if (progress < 0.5)
		{
			return 2.0 * progress * progress;
		}
		return progress * (-2.0 * progress + 4.0) - 1.0;
	}
}
