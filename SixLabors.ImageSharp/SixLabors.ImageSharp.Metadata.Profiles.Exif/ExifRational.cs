using System.Globalization;

namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifRational : ExifValue<Rational>
{
	public override ExifDataType DataType => ExifDataType.Rational;

	protected override string StringValue => base.Value.ToString(CultureInfo.InvariantCulture);

	public ExifRational(ExifTag<Rational> tag)
		: base(tag)
	{
	}

	public ExifRational(ExifTagValue tag)
		: base(tag)
	{
	}

	private ExifRational(ExifRational value)
		: base((ExifValue)value)
	{
	}

	public override bool TrySetValue(object? value)
	{
		if (base.TrySetValue(value))
		{
			return true;
		}
		if (value is SignedRational signedRational)
		{
			if ((long)signedRational.Numerator >= 0L && (long)signedRational.Denominator >= 0L)
			{
				base.Value = new Rational((uint)signedRational.Numerator, (uint)signedRational.Denominator);
			}
			return true;
		}
		return false;
	}

	public override IExifValue DeepClone()
	{
		return new ExifRational(this);
	}
}
