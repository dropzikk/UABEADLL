using System.Globalization;

namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifSignedLong : ExifValue<int>
{
	public override ExifDataType DataType => ExifDataType.SignedLong;

	protected override string StringValue => base.Value.ToString(CultureInfo.InvariantCulture);

	public ExifSignedLong(ExifTagValue tag)
		: base(tag)
	{
	}

	private ExifSignedLong(ExifSignedLong value)
		: base((ExifValue)value)
	{
	}

	public override IExifValue DeepClone()
	{
		return new ExifSignedLong(this);
	}
}
