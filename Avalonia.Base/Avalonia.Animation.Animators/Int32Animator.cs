using System;

namespace Avalonia.Animation.Animators;

internal class Int32Animator : Animator<int>
{
	private const double maxVal = 2147483647.0;

	public override int Interpolate(double progress, int oldValue, int newValue)
	{
		double num = (double)oldValue / 2147483647.0;
		double num2 = (double)newValue / 2147483647.0 - num;
		return (int)Math.Round(2147483647.0 * (num2 * progress + num));
	}
}
