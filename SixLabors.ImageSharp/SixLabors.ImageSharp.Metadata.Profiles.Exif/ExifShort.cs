using System.Globalization;

namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifShort : ExifValue<ushort>
{
	public override ExifDataType DataType => ExifDataType.Short;

	protected override string StringValue => base.Value.ToString(CultureInfo.InvariantCulture);

	public ExifShort(ExifTag<ushort> tag)
		: base(tag)
	{
	}

	public ExifShort(ExifTagValue tag)
		: base(tag)
	{
	}

	private ExifShort(ExifShort value)
		: base((ExifValue)value)
	{
	}

	public override bool TrySetValue(object? value)
	{
		if (base.TrySetValue(value))
		{
			return true;
		}
		if (!(value is int num))
		{
			if (value is short num2)
			{
				if (num2 >= 0)
				{
					base.Value = (ushort)num2;
					return true;
				}
				return false;
			}
			return false;
		}
		if (num >= 0 && num <= 65535)
		{
			base.Value = (ushort)num;
			return true;
		}
		return false;
	}

	public override IExifValue DeepClone()
	{
		return new ExifShort(this);
	}
}
