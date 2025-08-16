namespace Avalonia.Animation.Easings;

public class LinearEasing : Easing
{
	public override double Ease(double progress)
	{
		return progress;
	}
}
