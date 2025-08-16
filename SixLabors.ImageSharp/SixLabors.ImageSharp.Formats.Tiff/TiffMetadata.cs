namespace SixLabors.ImageSharp.Formats.Tiff;

public class TiffMetadata : IDeepCloneable
{
	public ByteOrder ByteOrder { get; set; }

	public TiffFormatType FormatType { get; set; }

	public TiffMetadata()
	{
	}

	private TiffMetadata(TiffMetadata other)
	{
		ByteOrder = other.ByteOrder;
		FormatType = other.FormatType;
	}

	public IDeepCloneable DeepClone()
	{
		return new TiffMetadata(this);
	}
}
