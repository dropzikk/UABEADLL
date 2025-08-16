namespace Avalonia.Remote.Protocol.Input;

[AvaloniaRemoteMessageGuid("7E9E2818-F93F-411A-800E-6B1AEB11DA46")]
public class PointerPressedEventMessage : PointerEventMessageBase
{
	public MouseButton Button { get; set; }
}
