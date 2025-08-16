using Avalonia.Animation.Utils;

namespace Avalonia.Animation.Easings;

public class BounceEaseInOut : Easing
{
	public override double Ease(double progress)
	{
		if (progress < 0.5)
		{
			return 0.5 * (1.0 - BounceEaseUtils.Bounce(1.0 - progress * 2.0));
		}
		return 0.5 * BounceEaseUtils.Bounce(progress * 2.0 - 1.0) + 0.5;
	}
}
