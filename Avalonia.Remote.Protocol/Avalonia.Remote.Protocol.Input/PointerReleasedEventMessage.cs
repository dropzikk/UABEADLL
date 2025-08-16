namespace Avalonia.Remote.Protocol.Input;

[AvaloniaRemoteMessageGuid("4ADC84EE-E7C8-4BCF-986C-DE3A2F78EDE4")]
public class PointerReleasedEventMessage : PointerEventMessageBase
{
	public MouseButton Button { get; set; }
}
