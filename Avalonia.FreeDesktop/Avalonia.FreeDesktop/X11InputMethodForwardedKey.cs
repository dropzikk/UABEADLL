using Avalonia.Input;
using Avalonia.Input.Raw;

namespace Avalonia.FreeDesktop;

internal struct X11InputMethodForwardedKey
{
	public int KeyVal { get; set; }

	public KeyModifiers Modifiers { get; set; }

	public RawKeyEventType Type { get; set; }
}
