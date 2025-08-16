using System;

namespace Avalonia.Animation.Easings;

public class ElasticEaseInOut : Easing
{
	public override double Ease(double progress)
	{
		if (progress < 0.5)
		{
			double num = 2.0 * progress;
			return 0.5 * Math.Sin(20.420352248333657 * num) * Math.Pow(2.0, 10.0 * (num - 1.0));
		}
		return 0.5 * (Math.Sin(-20.420352248333657 * (2.0 * progress - 1.0 + 1.0)) * Math.Pow(2.0, -10.0 * (2.0 * progress - 1.0)) + 2.0);
	}
}
