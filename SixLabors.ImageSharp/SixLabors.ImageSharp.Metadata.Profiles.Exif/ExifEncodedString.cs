namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifEncodedString : ExifValue<EncodedString>
{
	public override ExifDataType DataType => ExifDataType.Undefined;

	protected override string StringValue => base.Value.Text;

	public ExifEncodedString(ExifTag<EncodedString> tag)
		: base(tag)
	{
	}

	public ExifEncodedString(ExifTagValue tag)
		: base(tag)
	{
	}

	private ExifEncodedString(ExifEncodedString value)
		: base((ExifValue)value)
	{
	}

	public override bool TrySetValue(object? value)
	{
		if (base.TrySetValue(value))
		{
			return true;
		}
		if (value is string text)
		{
			base.Value = new EncodedString(text);
			return true;
		}
		if (value is byte[] array && ExifEncodedStringHelpers.TryParse(array, out var encodedString))
		{
			base.Value = encodedString;
			return true;
		}
		return false;
	}

	public override IExifValue DeepClone()
	{
		return new ExifEncodedString(this);
	}
}
