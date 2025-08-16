namespace Avalonia.Remote.Protocol.Viewport;

[AvaloniaRemoteMessageGuid("63481025-7016-43FE-BADC-F2FD0F88609E")]
public class ClientSupportedPixelFormatsMessage
{
	public PixelFormat[] Formats { get; set; }
}
