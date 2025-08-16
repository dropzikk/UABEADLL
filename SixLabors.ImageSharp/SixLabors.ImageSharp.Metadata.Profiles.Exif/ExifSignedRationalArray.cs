namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifSignedRationalArray : ExifArrayValue<SignedRational>
{
	public override ExifDataType DataType => ExifDataType.SignedRational;

	public ExifSignedRationalArray(ExifTag<SignedRational[]> tag)
		: base(tag)
	{
	}

	public ExifSignedRationalArray(ExifTagValue tag)
		: base(tag)
	{
	}

	private ExifSignedRationalArray(ExifSignedRationalArray value)
		: base((ExifArrayValue<SignedRational>)value)
	{
	}

	public override IExifValue DeepClone()
	{
		return new ExifSignedRationalArray(this);
	}
}
