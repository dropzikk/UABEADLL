using System;

namespace Avalonia.Animation.Easings;

public class BackEaseInOut : Easing
{
	public override double Ease(double progress)
	{
		if (progress < 0.5)
		{
			double num = 2.0 * progress;
			return 0.5 * num * (num * num - Math.Sin(num * Math.PI));
		}
		double num2 = 1.0 - (2.0 * progress - 1.0);
		return 0.5 * (1.0 - num2 * (num2 * num2 - Math.Sin(num2 * Math.PI))) + 0.5;
	}
}
