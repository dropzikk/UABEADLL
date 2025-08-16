namespace Avalonia.Animation.Easings;

public class QuinticEaseOut : Easing
{
	public override double Ease(double progress)
	{
		double num = progress - 1.0;
		double num2 = num * num;
		return num2 * num2 * num + 1.0;
	}
}
