using Avalonia.Animation.Utils;

namespace Avalonia.Animation.Easings;

public class BounceEaseIn : Easing
{
	public override double Ease(double progress)
	{
		return 1.0 - BounceEaseUtils.Bounce(1.0 - progress);
	}
}
