namespace Avalonia.Remote.Protocol.Viewport;

[AvaloniaRemoteMessageGuid("9B47B3D8-61DF-4C38-ACD4-8C1BB72554AC")]
public class RequestViewportResizeMessage
{
	public double Width { get; set; }

	public double Height { get; set; }
}
