namespace SixLabors.ImageSharp.Formats.Pbm;

public class PbmMetadata : IDeepCloneable
{
	public PbmEncoding Encoding { get; set; }

	public PbmColorType ColorType { get; set; } = PbmColorType.Grayscale;

	public PbmComponentType ComponentType { get; set; }

	public PbmMetadata()
	{
		ComponentType = ((ColorType != 0) ? PbmComponentType.Byte : PbmComponentType.Bit);
	}

	private PbmMetadata(PbmMetadata other)
	{
		Encoding = other.Encoding;
		ColorType = other.ColorType;
		ComponentType = other.ComponentType;
	}

	public IDeepCloneable DeepClone()
	{
		return new PbmMetadata(this);
	}
}
