namespace SixLabors.ImageSharp.Formats.Webp.Lossy;

internal class Vp8SegmentHeader
{
	private const int NumMbSegments = 4;

	public bool UseSegment { get; set; }

	public bool UpdateMap { get; set; }

	public bool Delta { get; set; }

	public byte[] Quantizer { get; }

	public byte[] FilterStrength { get; }

	public Vp8SegmentHeader()
	{
		Quantizer = new byte[4];
		FilterStrength = new byte[4];
	}
}
