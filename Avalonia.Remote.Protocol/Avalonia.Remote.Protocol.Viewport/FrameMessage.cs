namespace Avalonia.Remote.Protocol.Viewport;

[AvaloniaRemoteMessageGuid("F58313EE-FE69-4536-819D-F52EDF201A0E")]
public class FrameMessage
{
	public long SequenceId { get; set; }

	public PixelFormat Format { get; set; }

	public byte[] Data { get; set; }

	public int Width { get; set; }

	public int Height { get; set; }

	public int Stride { get; set; }

	public double DpiX { get; set; }

	public double DpiY { get; set; }
}
