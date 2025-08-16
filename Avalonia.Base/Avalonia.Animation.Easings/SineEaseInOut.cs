using System;

namespace Avalonia.Animation.Easings;

public class SineEaseInOut : Easing
{
	public override double Ease(double progress)
	{
		return 0.5 * (1.0 - Math.Cos(progress * Math.PI));
	}
}
