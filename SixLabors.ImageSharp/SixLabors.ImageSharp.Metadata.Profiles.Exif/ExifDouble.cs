using System.Globalization;

namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifDouble : ExifValue<double>
{
	public override ExifDataType DataType => ExifDataType.DoubleFloat;

	protected override string StringValue => base.Value.ToString(CultureInfo.InvariantCulture);

	public ExifDouble(ExifTag<double> tag)
		: base(tag)
	{
	}

	public ExifDouble(ExifTagValue tag)
		: base(tag)
	{
	}

	private ExifDouble(ExifDouble value)
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
		return new ExifDouble(this);
	}
}
