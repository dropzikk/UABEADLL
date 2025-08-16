using Avalonia.Animation.Utils;

namespace Avalonia.Animation.Easings;

public class BounceEaseOut : Easing
{
	public override double Ease(double progress)
	{
		return BounceEaseUtils.Bounce(progress);
	}
}
