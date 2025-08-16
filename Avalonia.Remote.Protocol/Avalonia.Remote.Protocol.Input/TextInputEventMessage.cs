namespace Avalonia.Remote.Protocol.Input;

[AvaloniaRemoteMessageGuid("C174102E-7405-4594-916F-B10B8248A17D")]
public class TextInputEventMessage : InputEventMessageBase
{
	public string Text { get; set; }
}
