using Avalonia.Interactivity;

namespace Avalonia.Input;

public class GotFocusEventArgs : RoutedEventArgs
{
	public NavigationMethod NavigationMethod { get; init; }

	public KeyModifiers KeyModifiers { get; init; }

	public GotFocusEventArgs()
		: base(InputElement.GotFocusEvent)
	{
	}
}
