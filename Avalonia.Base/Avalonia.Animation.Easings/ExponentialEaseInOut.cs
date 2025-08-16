using System;

namespace Avalonia.Animation.Easings;

public class ExponentialEaseInOut : Easing
{
	public override double Ease(double progress)
	{
		if (progress < 0.5)
		{
			return 0.5 * Math.Pow(2.0, 20.0 * progress - 10.0);
		}
		return -0.5 * Math.Pow(2.0, -20.0 * progress + 10.0) + 1.0;
	}
}
