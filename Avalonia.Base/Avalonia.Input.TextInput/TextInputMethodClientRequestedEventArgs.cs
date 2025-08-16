using Avalonia.Interactivity;

namespace Avalonia.Input.TextInput;

public class TextInputMethodClientRequestedEventArgs : RoutedEventArgs
{
	public TextInputMethodClient? Client { get; set; }
}
