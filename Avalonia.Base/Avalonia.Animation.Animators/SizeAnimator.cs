namespace Avalonia.Animation.Animators;

internal class SizeAnimator : Animator<Size>
{
	public override Size Interpolate(double progress, Size oldValue, Size newValue)
	{
		return (newValue - oldValue) * progress + oldValue;
	}
}
