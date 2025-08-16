namespace Avalonia.Animation.Animators;

internal class BoolAnimator : Animator<bool>
{
	public override bool Interpolate(double progress, bool oldValue, bool newValue)
	{
		if (progress >= 1.0)
		{
			return newValue;
		}
		_ = 0.0;
		return oldValue;
	}
}
