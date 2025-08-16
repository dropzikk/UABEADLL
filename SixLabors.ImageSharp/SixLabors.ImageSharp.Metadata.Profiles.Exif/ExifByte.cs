using System.Globalization;

namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifByte : ExifValue<byte>
{
	public override ExifDataType DataType { get; }

	protected override string StringValue => base.Value.ToString("X2", CultureInfo.InvariantCulture);

	public ExifByte(ExifTag<byte> tag, ExifDataType dataType)
		: base(tag)
	{
		DataType = dataType;
	}

	public ExifByte(ExifTagValue tag, ExifDataType dataType)
		: base(tag)
	{
		DataType = dataType;
	}

	private ExifByte(ExifByte value)
		: base((ExifValue)value)
	{
		DataType = value.DataType;
	}

	public override bool TrySetValue(object? value)
	{
		if (base.TrySetValue(value))
		{
			return true;
		}
		if (value is int num)
		{
			if (num >= 0 && num <= 255)
			{
				base.Value = (byte)num;
				return true;
			}
			return false;
		}
		return base.TrySetValue(value);
	}

	public override IExifValue DeepClone()
	{
		return new ExifByte(this);
	}
}
