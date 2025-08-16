namespace Avalonia.Input;

public static class KeyboardNavigation
{
	public static readonly AttachedProperty<int> TabIndexProperty = AvaloniaProperty.RegisterAttached<StyledElement, int>("TabIndex", typeof(KeyboardNavigation), int.MaxValue);

	public static readonly AttachedProperty<KeyboardNavigationMode> TabNavigationProperty = AvaloniaProperty.RegisterAttached<InputElement, KeyboardNavigationMode>("TabNavigation", typeof(KeyboardNavigation), KeyboardNavigationMode.Continue);

	public static readonly AttachedProperty<IInputElement?> TabOnceActiveElementProperty = AvaloniaProperty.RegisterAttached<InputElement, IInputElement>("TabOnceActiveElement", typeof(KeyboardNavigation));

	public static readonly AttachedProperty<bool> IsTabStopProperty = AvaloniaProperty.RegisterAttached<InputElement, bool>("IsTabStop", typeof(KeyboardNavigation), defaultValue: true);

	public static int GetTabIndex(IInputElement element)
	{
		return ((AvaloniaObject)element).GetValue(TabIndexProperty);
	}

	public static void SetTabIndex(IInputElement element, int value)
	{
		((AvaloniaObject)element).SetValue(TabIndexProperty, value);
	}

	public static KeyboardNavigationMode GetTabNavigation(InputElement element)
	{
		return element.GetValue(TabNavigationProperty);
	}

	public static void SetTabNavigation(InputElement element, KeyboardNavigationMode value)
	{
		element.SetValue(TabNavigationProperty, value);
	}

	public static IInputElement? GetTabOnceActiveElement(InputElement element)
	{
		return element.GetValue(TabOnceActiveElementProperty);
	}

	public static void SetTabOnceActiveElement(InputElement element, IInputElement? value)
	{
		element.SetValue(TabOnceActiveElementProperty, value);
	}

	public static void SetIsTabStop(InputElement element, bool value)
	{
		element.SetValue(IsTabStopProperty, value);
	}

	public static bool GetIsTabStop(InputElement element)
	{
		return element.GetValue(IsTabStopProperty);
	}
}
