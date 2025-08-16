namespace Avalonia.Animation.Animators;

internal class VectorAnimator : Animator<Vector>
{
	public override Vector Interpolate(double progress, Vector oldValue, Vector newValue)
	{
		return (newValue - oldValue) * progress + oldValue;
	}
}
