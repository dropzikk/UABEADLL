namespace Avalonia.Animation.Animators;

internal class ThicknessAnimator : Animator<Thickness>
{
	public override Thickness Interpolate(double progress, Thickness oldValue, Thickness newValue)
	{
		return (newValue - oldValue) * progress + oldValue;
	}
}
