namespace SixLabors.ImageSharp.Formats.Webp.Lossy;

internal class Vp8ProbaArray
{
	public byte[] Probabilities { get; }

	public Vp8ProbaArray()
	{
		Probabilities = new byte[11];
	}
}
