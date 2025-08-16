namespace SixLabors.ImageSharp.Formats.Webp.Lossy;

internal class Vp8EncSegmentHeader
{
	public int NumSegments { get; }

	public bool UpdateMap { get; set; }

	public int Size { get; set; }

	public Vp8EncSegmentHeader(int numSegments)
	{
		NumSegments = numSegments;
		UpdateMap = NumSegments > 1;
		Size = 0;
	}
}
