using System;

namespace Avalonia.Animation.Easings;

public class SineEaseOut : Easing
{
	public override double Ease(double progress)
	{
		return Math.Sin(progress * (Math.PI / 2.0));
	}
}
