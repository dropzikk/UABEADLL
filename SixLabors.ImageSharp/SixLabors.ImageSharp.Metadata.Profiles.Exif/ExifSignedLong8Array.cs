namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifSignedLong8Array : ExifArrayValue<long>
{
	public override ExifDataType DataType => ExifDataType.SignedLong8;

	public ExifSignedLong8Array(ExifTagValue tag)
		: base(tag)
	{
	}

	private ExifSignedLong8Array(ExifSignedLong8Array value)
		: base((ExifArrayValue<long>)value)
	{
	}

	public override IExifValue DeepClone()
	{
		return new ExifSignedLong8Array(this);
	}
}
