namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifLongArray : ExifArrayValue<uint>
{
	public override ExifDataType DataType => ExifDataType.Long;

	public ExifLongArray(ExifTag<uint[]> tag)
		: base(tag)
	{
	}

	public ExifLongArray(ExifTagValue tag)
		: base(tag)
	{
	}

	private ExifLongArray(ExifLongArray value)
		: base((ExifArrayValue<uint>)value)
	{
	}

	public override IExifValue DeepClone()
	{
		return new ExifLongArray(this);
	}
}
