namespace Avalonia.Remote.Protocol.Input;

[AvaloniaRemoteMessageGuid("1C3B691E-3D54-4237-BFB0-9FEA83BC1DB8")]
public class KeyEventMessage : InputEventMessageBase
{
	public bool IsDown { get; set; }

	public Key Key { get; set; }
}
