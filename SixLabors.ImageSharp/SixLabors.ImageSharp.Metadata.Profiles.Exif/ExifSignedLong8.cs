using System.Globalization;

namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifSignedLong8 : ExifValue<long>
{
	public override ExifDataType DataType => ExifDataType.SignedLong8;

	protected override string StringValue => base.Value.ToString(CultureInfo.InvariantCulture);

	public ExifSignedLong8(ExifTagValue tag)
		: base(tag)
	{
	}

	private ExifSignedLong8(ExifSignedLong8 value)
		: base((ExifValue)value)
	{
	}

	public override IExifValue DeepClone()
	{
		return new ExifSignedLong8(this);
	}
}
