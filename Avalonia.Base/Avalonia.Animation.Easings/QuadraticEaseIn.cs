namespace Avalonia.Animation.Easings;

public class QuadraticEaseIn : Easing
{
	public override double Ease(double progress)
	{
		return progress * progress;
	}
}
