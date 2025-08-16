namespace Avalonia.Animation.Animators;

internal class DecimalAnimator : Animator<decimal>
{
	public override decimal Interpolate(double progress, decimal oldValue, decimal newValue)
	{
		return (newValue - oldValue) * (decimal)progress + oldValue;
	}
}
