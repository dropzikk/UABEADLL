using System;

namespace Avalonia.Animation.Easings;

public class ElasticEaseIn : Easing
{
	public override double Ease(double progress)
	{
		return Math.Sin(20.420352248333657 * progress) * Math.Pow(2.0, 10.0 * (progress - 1.0));
	}
}
