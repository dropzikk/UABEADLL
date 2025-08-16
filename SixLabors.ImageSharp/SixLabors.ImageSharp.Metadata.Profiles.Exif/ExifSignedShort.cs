using System.Globalization;

namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifSignedShort : ExifValue<short>
{
	public override ExifDataType DataType => ExifDataType.SignedShort;

	protected override string StringValue => base.Value.ToString(CultureInfo.InvariantCulture);

	public ExifSignedShort(ExifTagValue tag)
		: base(tag)
	{
	}

	private ExifSignedShort(ExifSignedShort value)
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
			if (num >= -32768 && num <= 32767)
			{
				base.Value = (short)num;
				return true;
			}
			return false;
		}
		return false;
	}

	public override IExifValue DeepClone()
	{
		return new ExifSignedShort(this);
	}
}
