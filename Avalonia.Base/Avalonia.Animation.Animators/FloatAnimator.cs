namespace Avalonia.Animation.Animators;

internal class FloatAnimator : Animator<float>
{
	public override float Interpolate(double progress, float oldValue, float newValue)
	{
		return (float)((double)(newValue - oldValue) * progress + (double)oldValue);
	}
}
