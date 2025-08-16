using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal abstract class ExifArrayValue<TValueType> : ExifValue, IExifValue<TValueType[]>, IExifValue, IDeepCloneable<IExifValue>
{
	public override bool IsArray => true;

	public TValueType[]? Value { get; set; }

	protected ExifArrayValue(ExifTag<TValueType[]> tag)
		: base(tag)
	{
	}

	protected ExifArrayValue(ExifTagValue tag)
		: base(tag)
	{
	}

	internal ExifArrayValue(ExifArrayValue<TValueType> value)
		: base(value)
	{
	}

	public override object? GetValue()
	{
		return Value;
	}

	public override bool TrySetValue(object? value)
	{
		if (value == null)
		{
			Value = null;
			return true;
		}
		Type type = value.GetType();
		if (value.GetType() == typeof(TValueType[]))
		{
			Value = (TValueType[])value;
			return true;
		}
		if (type == typeof(TValueType))
		{
			Value = new TValueType[1] { (TValueType)value };
			return true;
		}
		return false;
	}
}
