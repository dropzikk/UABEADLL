using System.Globalization;

namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifLong : ExifValue<uint>
{
	public override ExifDataType DataType => ExifDataType.Long;

	protected override string StringValue => base.Value.ToString(CultureInfo.InvariantCulture);

	public ExifLong(ExifTag<uint> tag)
		: base(tag)
	{
	}

	public ExifLong(ExifTagValue tag)
		: base(tag)
	{
	}

	private ExifLong(ExifLong value)
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
			if ((long)num >= 0L)
			{
				base.Value = (uint)num;
				return true;
			}
			return false;
		}
		return false;
	}

	public override IExifValue DeepClone()
	{
		return new ExifLong(this);
	}
}
