namespace Avalonia.Remote.Protocol.Viewport;

[AvaloniaRemoteMessageGuid("BD7A8DE6-3DB8-4A13-8583-D6D4AB189A31")]
public class ClientViewportAllocatedMessage
{
	public double Width { get; set; }

	public double Height { get; set; }

	public double DpiX { get; set; }

	public double DpiY { get; set; }
}
