using Avalonia.Metadata;
using Avalonia.Platform;

namespace Avalonia.Input;

[NotClientImplementable]
public interface IInputRoot : IInputElement
{
	IKeyboardNavigationHandler KeyboardNavigationHandler { get; }

	IFocusManager? FocusManager { get; }

	IPlatformSettings? PlatformSettings { get; }

	IInputElement? PointerOverElement { get; set; }

	bool ShowAccessKeys { get; set; }
}
