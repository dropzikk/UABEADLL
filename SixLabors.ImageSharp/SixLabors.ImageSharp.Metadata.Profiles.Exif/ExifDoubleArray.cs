namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifDoubleArray : ExifArrayValue<double>
{
	public override ExifDataType DataType => ExifDataType.DoubleFloat;

	public ExifDoubleArray(ExifTag<double[]> tag)
		: base(tag)
	{
	}

	public ExifDoubleArray(ExifTagValue tag)
		: base(tag)
	{
	}

	private ExifDoubleArray(ExifDoubleArray value)
		: base((ExifArrayValue<double>)value)
	{
	}

	public override IExifValue DeepClone()
	{
		return new ExifDoubleArray(this);
	}
}
