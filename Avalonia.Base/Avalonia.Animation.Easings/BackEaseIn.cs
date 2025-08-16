using System;

namespace Avalonia.Animation.Easings;

public class BackEaseIn : Easing
{
	public override double Ease(double p)
	{
		return p * (p * p - Math.Sin(p * Math.PI));
	}
}
