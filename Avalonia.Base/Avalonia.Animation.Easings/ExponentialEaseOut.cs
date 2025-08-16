using System;

namespace Avalonia.Animation.Easings;

public class ExponentialEaseOut : Easing
{
	public override double Ease(double progress)
	{
		if (progress == 1.0)
		{
			return progress;
		}
		return 1.0 - Math.Pow(2.0, -10.0 * progress);
	}
}
