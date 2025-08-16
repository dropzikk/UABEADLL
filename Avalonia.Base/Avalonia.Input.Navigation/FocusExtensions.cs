namespace Avalonia.Input.Navigation;

internal static class FocusExtensions
{
	public static bool CanFocus(this IInputElement e)
	{
		bool flag = (e as Visual)?.IsVisible ?? true;
		return e.Focusable && e.IsEffectivelyEnabled && flag;
	}

	public static bool CanFocusDescendants(this IInputElement e)
	{
		bool flag = (e as Visual)?.IsVisible ?? true;
		return e.IsEffectivelyEnabled && flag;
	}
}
