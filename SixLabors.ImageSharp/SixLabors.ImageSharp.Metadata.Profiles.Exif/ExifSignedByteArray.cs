namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifSignedByteArray : ExifArrayValue<sbyte>
{
	public override ExifDataType DataType => ExifDataType.SignedByte;

	public ExifSignedByteArray(ExifTagValue tag)
		: base(tag)
	{
	}

	private ExifSignedByteArray(ExifSignedByteArray value)
		: base((ExifArrayValue<sbyte>)value)
	{
	}

	public override IExifValue DeepClone()
	{
		return new ExifSignedByteArray(this);
	}
}
