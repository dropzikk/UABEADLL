namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifUcs2String : ExifValue<string>
{
	public override ExifDataType DataType => ExifDataType.Byte;

	protected override string? StringValue => base.Value;

	public ExifUcs2String(ExifTag<string> tag)
		: base(tag)
	{
	}

	public ExifUcs2String(ExifTagValue tag)
		: base(tag)
	{
	}

	private ExifUcs2String(ExifUcs2String value)
		: base((ExifValue)value)
	{
	}

	public override object? GetValue()
	{
		return base.Value;
	}

	public override bool TrySetValue(object? value)
	{
		if (base.TrySetValue(value))
		{
			return true;
		}
		if (value is byte[] bytes)
		{
			base.Value = ExifUcs2StringHelpers.Ucs2Encoding.GetString(bytes);
			return true;
		}
		return false;
	}

	public override IExifValue DeepClone()
	{
		return new ExifUcs2String(this);
	}
}
