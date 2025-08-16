namespace Avalonia.Remote.Protocol.Viewport;

[AvaloniaRemoteMessageGuid("6E3C5310-E2B1-4C3D-8688-01183AA48C5B")]
public class MeasureViewportMessage
{
	public double Width { get; set; }

	public double Height { get; set; }
}
