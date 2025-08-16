namespace SixLabors.ImageSharp.Formats.Tga;

public class TgaMetadata : IDeepCloneable
{
	public TgaBitsPerPixel BitsPerPixel { get; set; } = TgaBitsPerPixel.Pixel24;

	public byte AlphaChannelBits { get; set; }

	public TgaMetadata()
	{
	}

	private TgaMetadata(TgaMetadata other)
	{
		BitsPerPixel = other.BitsPerPixel;
	}

	public IDeepCloneable DeepClone()
	{
		return new TgaMetadata(this);
	}
}
