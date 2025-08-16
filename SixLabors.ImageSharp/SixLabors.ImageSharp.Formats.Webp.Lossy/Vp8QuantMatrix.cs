namespace SixLabors.ImageSharp.Formats.Webp.Lossy;

internal class Vp8QuantMatrix
{
	private int dither;

	public int[] Y1Mat { get; } = new int[2];

	public int[] Y2Mat { get; } = new int[2];

	public int[] UvMat { get; } = new int[2];

	public int UvQuant { get; set; }

	public int Dither
	{
		get
		{
			return dither;
		}
		set
		{
			Guard.MustBeBetweenOrEqualTo(value, 0, 255, "Dither");
			dither = value;
		}
	}
}
