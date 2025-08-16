using System;

namespace Avalonia.Animation.Easings;

public class BackEaseOut : Easing
{
	public override double Ease(double progress)
	{
		double num = 1.0 - progress;
		return 1.0 - num * (num * num - Math.Sin(num * Math.PI));
	}
}
