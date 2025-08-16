namespace Avalonia.Input;

public interface INavigableContainer
{
	IInputElement? GetControl(NavigationDirection direction, IInputElement? from, bool wrap);
}
