namespace Avalonia.Animation.Easings;

public class QuinticEaseInOut : Easing
{
	public override double Ease(double progress)
	{
		if (progress < 0.5)
		{
			double num = progress * progress;
			return 16.0 * num * num * progress;
		}
		double num2 = 2.0 * progress - 2.0;
		double num3 = num2 * num2;
		return 0.5 * num3 * num3 * num2 + 1.0;
	}
}
