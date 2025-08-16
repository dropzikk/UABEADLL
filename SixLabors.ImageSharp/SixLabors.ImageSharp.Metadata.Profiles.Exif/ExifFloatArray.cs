namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifFloatArray : ExifArrayValue<float>
{
	public override ExifDataType DataType => ExifDataType.SingleFloat;

	public ExifFloatArray(ExifTagValue tag)
		: base(tag)
	{
	}

	private ExifFloatArray(ExifFloatArray value)
		: base((ExifArrayValue<float>)value)
	{
	}

	public override IExifValue DeepClone()
	{
		return new ExifFloatArray(this);
	}
}
