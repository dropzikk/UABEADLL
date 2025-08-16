using System.Globalization;

namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifNumber : ExifValue<Number>
{
	public override ExifDataType DataType
	{
		get
		{
			if (base.Value > ushort.MaxValue)
			{
				return ExifDataType.Long;
			}
			return ExifDataType.Short;
		}
	}

	protected override string StringValue => base.Value.ToString(CultureInfo.InvariantCulture);

	public ExifNumber(ExifTag<Number> tag)
		: base(tag)
	{
	}

	private ExifNumber(ExifNumber value)
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
			if (!(value is uint num2))
			{
				if (!(value is short num3))
				{
					if (value is ushort num4)
					{
						base.Value = num4;
						return true;
					}
					return false;
				}
				if ((long)num3 >= 0L)
				{
					base.Value = (uint)num3;
					return true;
				}
				return false;
			}
			base.Value = num2;
			return true;
		}
		if ((long)num >= 0L)
		{
			base.Value = (uint)num;
			return true;
		}
		return false;
	}

	public override IExifValue DeepClone()
	{
		return new ExifNumber(this);
	}
}
