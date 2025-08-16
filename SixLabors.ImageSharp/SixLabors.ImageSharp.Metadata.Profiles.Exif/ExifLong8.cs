using System.Globalization;

namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifLong8 : ExifValue<ulong>
{
	public override ExifDataType DataType => ExifDataType.Long8;

	protected override string StringValue => base.Value.ToString(CultureInfo.InvariantCulture);

	public ExifLong8(ExifTag<ulong> tag)
		: base(tag)
	{
	}

	public ExifLong8(ExifTagValue tag)
		: base(tag)
	{
	}

	private ExifLong8(ExifLong8 value)
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
				if (value is long num3)
				{
					if (num3 >= 0)
					{
						base.Value = (ulong)num3;
						return true;
					}
					return false;
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
		return new ExifLong8(this);
	}
}
