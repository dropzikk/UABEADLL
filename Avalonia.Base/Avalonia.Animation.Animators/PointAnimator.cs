namespace Avalonia.Animation.Animators;

internal class PointAnimator : Animator<Point>
{
	public override Point Interpolate(double progress, Point oldValue, Point newValue)
	{
		return (newValue - oldValue) * progress + oldValue;
	}
}
