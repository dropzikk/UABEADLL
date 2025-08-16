namespace SixLabors.ImageSharp.Formats.Webp.Lossy;

internal class Vp8FrameHeader
{
	public bool KeyFrame { get; set; }

	public sbyte Profile { get; set; }

	public uint PartitionLength { get; set; }
}
