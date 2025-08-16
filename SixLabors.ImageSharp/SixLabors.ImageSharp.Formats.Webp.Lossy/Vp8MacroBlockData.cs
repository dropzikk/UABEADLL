namespace SixLabors.ImageSharp.Formats.Webp.Lossy;

internal class Vp8MacroBlockData
{
	public short[] Coeffs { get; set; }

	public bool IsI4x4 { get; set; }

	public byte[] Modes { get; }

	public byte UvMode { get; set; }

	public uint NonZeroY { get; set; }

	public uint NonZeroUv { get; set; }

	public bool Skip { get; set; }

	public byte Segment { get; set; }

	public Vp8MacroBlockData()
	{
		Modes = new byte[16];
		Coeffs = new short[384];
	}
}
