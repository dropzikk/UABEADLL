namespace Avalonia.Animation.Easings;

public class QuinticEaseIn : Easing
{
	public override double Ease(double progress)
	{
		double num = progress * progress;
		return num * num * progress;
	}
}
