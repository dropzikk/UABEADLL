namespace Avalonia.Animation.Animators;

internal class RectAnimator : Animator<Rect>
{
	public override Rect Interpolate(double progress, Rect oldValue, Rect newValue)
	{
		Point point = newValue.Position - oldValue.Position;
		Size size = newValue.Size - oldValue.Size;
		Point position = point * progress + oldValue.Position;
		Size size2 = size * progress + oldValue.Size;
		return new Rect(position, size2);
	}
}
