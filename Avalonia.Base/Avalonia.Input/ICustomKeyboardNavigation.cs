namespace Avalonia.Input;

public interface ICustomKeyboardNavigation
{
	(bool handled, IInputElement? next) GetNext(IInputElement element, NavigationDirection direction);
}
