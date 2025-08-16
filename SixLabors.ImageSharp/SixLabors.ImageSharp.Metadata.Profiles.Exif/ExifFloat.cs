using System.Globalization;

namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifFloat : ExifValue<float>
{
	public override ExifDataType DataType => ExifDataType.SingleFloat;

	protected override string StringValue => base.Value.ToString(CultureInfo.InvariantCulture);

	public ExifFloat(ExifTagValue tag)
		: base(tag)
	{
	}

	private ExifFloat(ExifFloat value)
		: base((ExifValue)value)
	{
	}

	public override bool TrySetValue(object? value)
	{
		if (base.TrySetValue(value))
		{
			return true;
		}
		if (value is int num)
		{
			base.Value = num;
			return true;
		}
		return false;
	}

	public override IExifValue DeepClone()
	{
		return new ExifFloat(this);
	}
}
