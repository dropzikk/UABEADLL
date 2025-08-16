namespace Avalonia.Rendering.Composition.Animations;

internal interface IInterpolator<T>
{
	T Interpolate(T from, T to, float progress);
}
