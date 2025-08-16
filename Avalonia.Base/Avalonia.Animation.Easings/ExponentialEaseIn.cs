using System;

namespace Avalonia.Animation.Easings;

public class ExponentialEaseIn : Easing
{
	public override double Ease(double progress)
	{
		if (progress == 0.0)
		{
			return progress;
		}
		return Math.Pow(2.0, 10.0 * (progress - 1.0));
	}
}
