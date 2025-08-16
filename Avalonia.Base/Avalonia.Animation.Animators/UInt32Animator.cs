using System;

namespace Avalonia.Animation.Animators;

internal class UInt32Animator : Animator<uint>
{
	private const double maxVal = 4294967295.0;

	public override uint Interpolate(double progress, uint oldValue, uint newValue)
	{
		double num = (double)oldValue / 4294967295.0;
		double num2 = (double)newValue / 4294967295.0 - num;
		return (uint)Math.Round(4294967295.0 * (num2 * progress + num));
	}
}
