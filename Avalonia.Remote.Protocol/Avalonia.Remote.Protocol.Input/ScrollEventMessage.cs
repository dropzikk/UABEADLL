namespace Avalonia.Remote.Protocol.Input;

[AvaloniaRemoteMessageGuid("79301A05-F02D-4B90-BB39-472563B504AE")]
public class ScrollEventMessage : PointerEventMessageBase
{
	public double DeltaX { get; set; }

	public double DeltaY { get; set; }
}
