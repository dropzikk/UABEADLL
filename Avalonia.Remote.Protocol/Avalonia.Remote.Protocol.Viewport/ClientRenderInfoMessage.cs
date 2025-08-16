namespace Avalonia.Remote.Protocol.Viewport;

[AvaloniaRemoteMessageGuid("7A3c25d3-3652-438D-8EF1-86E942CC96C0")]
public class ClientRenderInfoMessage
{
	public double DpiX { get; set; }

	public double DpiY { get; set; }
}
