using Avalonia.Metadata;

namespace Avalonia.Input;

[Unstable]
public interface IKeyboardNavigationHandler
{
	[PrivateApi]
	void SetOwner(IInputRoot owner);

	void Move(IInputElement element, NavigationDirection direction, KeyModifiers keyModifiers = KeyModifiers.None);
}
