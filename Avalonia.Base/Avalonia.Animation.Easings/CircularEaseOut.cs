using System;

namespace Avalonia.Animation.Easings;

public class CircularEaseOut : Easing
{
	public override double Ease(double progress)
	{
		return Math.Sqrt((2.0 - progress) * progress);
	}
}
