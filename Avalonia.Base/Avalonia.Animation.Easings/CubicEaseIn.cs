namespace Avalonia.Animation.Easings;

public class CubicEaseIn : Easing
{
	public override double Ease(double progress)
	{
		return progress * progress * progress;
	}
}
