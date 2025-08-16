using System.Globalization;

namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifSignedRational : ExifValue<SignedRational>
{
	public override ExifDataType DataType => ExifDataType.SignedRational;

	protected override string StringValue => base.Value.ToString(CultureInfo.InvariantCulture);

	internal ExifSignedRational(ExifTag<SignedRational> tag)
		: base(tag)
	{
	}

	internal ExifSignedRational(ExifTagValue tag)
		: base(tag)
	{
	}

	private ExifSignedRational(ExifSignedRational value)
		: base((ExifValue)value)
	{
	}

	public override IExifValue DeepClone()
	{
		return new ExifSignedRational(this);
	}
}
