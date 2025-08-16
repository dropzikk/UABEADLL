namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifSignedLongArray : ExifArrayValue<int>
{
	public override ExifDataType DataType => ExifDataType.SignedLong;

	public ExifSignedLongArray(ExifTagValue tag)
		: base(tag)
	{
	}

	private ExifSignedLongArray(ExifSignedLongArray value)
		: base((ExifArrayValue<int>)value)
	{
	}

	public override IExifValue DeepClone()
	{
		return new ExifSignedLongArray(this);
	}
}
