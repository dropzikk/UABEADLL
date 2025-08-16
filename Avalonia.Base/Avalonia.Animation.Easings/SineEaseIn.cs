using System;

namespace Avalonia.Animation.Easings;

public class SineEaseIn : Easing
{
	public override double Ease(double progress)
	{
		return Math.Sin((progress - 1.0) * (Math.PI / 2.0)) + 1.0;
	}
}
