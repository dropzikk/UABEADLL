using System;

namespace Avalonia.Animation.Easings;

public class CircularEaseIn : Easing
{
	public override double Ease(double p)
	{
		return 1.0 - Math.Sqrt(1.0 - p * p);
	}
}
