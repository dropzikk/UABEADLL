using System.Globalization;

namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifSignedByte : ExifValue<sbyte>
{
	public override ExifDataType DataType => ExifDataType.SignedByte;

	protected override string StringValue => base.Value.ToString("X2", CultureInfo.InvariantCulture);

	public ExifSignedByte(ExifTagValue tag)
		: base(tag)
	{
	}

	private ExifSignedByte(ExifSignedByte value)
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
			if (num >= -128 && num <= 127)
			{
				base.Value = (sbyte)num;
				return true;
			}
			return false;
		}
		return false;
	}

	public override IExifValue DeepClone()
	{
		return new ExifSignedByte(this);
	}
}
