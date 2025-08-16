namespace Avalonia.Input;

public class InputMethod
{
	public static readonly AvaloniaProperty<bool> IsInputMethodEnabledProperty = AvaloniaProperty.RegisterAttached<InputMethod, InputElement, bool>("IsInputMethodEnabled", defaultValue: true);

	public static void SetIsInputMethodEnabled(InputElement target, bool value)
	{
		target.SetValue(IsInputMethodEnabledProperty, value);
	}

	public static bool GetIsInputMethodEnabled(InputElement target)
	{
		return AvaloniaObjectExtensions.GetValue(target, IsInputMethodEnabledProperty);
	}

	private InputMethod()
	{
	}
}
