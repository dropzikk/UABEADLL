using Avalonia.Media;

namespace Avalonia.Animation.Animators;

internal class BoxShadowAnimator : Animator<BoxShadow>
{
	private static ColorAnimator s_colorAnimator = new ColorAnimator();

	private static DoubleAnimator s_doubleAnimator = new DoubleAnimator();

	private static BoolAnimator s_boolAnimator = new BoolAnimator();

	public override BoxShadow Interpolate(double progress, BoxShadow oldValue, BoxShadow newValue)
	{
		BoxShadow result = default(BoxShadow);
		result.OffsetX = s_doubleAnimator.Interpolate(progress, oldValue.OffsetX, newValue.OffsetX);
		result.OffsetY = s_doubleAnimator.Interpolate(progress, oldValue.OffsetY, newValue.OffsetY);
		result.Blur = s_doubleAnimator.Interpolate(progress, oldValue.Blur, newValue.Blur);
		result.Spread = s_doubleAnimator.Interpolate(progress, oldValue.Spread, newValue.Spread);
		result.Color = s_colorAnimator.Interpolate(progress, oldValue.Color, newValue.Color);
		result.IsInset = s_boolAnimator.Interpolate(progress, oldValue.IsInset, newValue.IsInset);
		return result;
	}
}
