using Avalonia.Metadata;

namespace Avalonia.Input.Raw;

[PrivateApi]
public class RawKeyEventArgs : RawInputEventArgs
{
	public Key Key { get; set; }

	public RawInputModifiers Modifiers { get; set; }

	public RawKeyEventType Type { get; set; }

	public RawKeyEventArgs(IKeyboardDevice device, ulong timestamp, IInputRoot root, RawKeyEventType type, Key key, RawInputModifiers modifiers)
		: base(device, timestamp, root)
	{
		Key = key;
		Type = type;
		Modifiers = modifiers;
	}
}
