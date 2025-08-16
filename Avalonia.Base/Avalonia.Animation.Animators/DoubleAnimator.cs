namespace Avalonia.Animation.Animators;

internal class DoubleAnimator : Animator<double>
{
	public override double Interpolate(double progress, double oldValue, double newValue)
	{
		return (newValue - oldValue) * progress + oldValue;
	}
}
