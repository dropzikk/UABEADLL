using System.Globalization;

namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifString : ExifValue<string>
{
	public override ExifDataType DataType => ExifDataType.Ascii;

	protected override string? StringValue => base.Value;

	public ExifString(ExifTag<string> tag)
		: base(tag)
	{
	}

	public ExifString(ExifTagValue tag)
		: base(tag)
	{
	}

	private ExifString(ExifString value)
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
			base.Value = num.ToString(CultureInfo.InvariantCulture);
			return true;
		}
		return false;
	}

	public override IExifValue DeepClone()
	{
		return new ExifString(this);
	}
}
