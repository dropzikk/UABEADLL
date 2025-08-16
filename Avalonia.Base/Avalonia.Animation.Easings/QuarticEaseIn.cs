namespace Avalonia.Animation.Easings;

public class QuarticEaseIn : Easing
{
	public override double Ease(double progress)
	{
		double num = progress * progress;
		return num * num;
	}
}
