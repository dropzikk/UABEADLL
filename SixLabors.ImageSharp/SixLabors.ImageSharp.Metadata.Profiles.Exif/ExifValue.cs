using System;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal abstract class ExifValue : IExifValue, IDeepCloneable<IExifValue>, IEquatable<ExifTag>
{
	public virtual ExifDataType DataType { get; }

	public virtual bool IsArray { get; }

	public ExifTag Tag { get; }

	protected ExifValue(ExifTag tag)
	{
		Tag = tag;
	}

	protected ExifValue(ExifTagValue tag)
	{
		Tag = new UnkownExifTag(tag);
	}

	internal ExifValue(ExifValue other)
	{
		Guard.NotNull(other, "other");
		DataType = other.DataType;
		IsArray = other.IsArray;
		Tag = other.Tag;
		if (!other.IsArray)
		{
			TrySetValue(other.GetValue());
		}
		else
		{
			TrySetValue(((Array)other.GetValue())?.Clone());
		}
	}

	public static bool operator ==(ExifValue left, ExifTag right)
	{
		return object.Equals(left, right);
	}

	public static bool operator !=(ExifValue left, ExifTag right)
	{
		return !object.Equals(left, right);
	}

	public override bool Equals(object? obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (this == obj)
		{
			return true;
		}
		if (obj is ExifTag other)
		{
			return Equals(other);
		}
		if (obj is ExifValue exifValue)
		{
			if (Tag.Equals(exifValue.Tag))
			{
				return object.Equals(GetValue(), exifValue.GetValue());
			}
			return false;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(ExifTag? other)
	{
		return Tag.Equals(other);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode()
	{
		return HashCode.Combine(Tag, GetValue());
	}

	public abstract object? GetValue();

	public abstract bool TrySetValue(object? value);

	public abstract IExifValue DeepClone();
}
internal abstract class ExifValue<TValueType> : ExifValue, IExifValue<TValueType>, IExifValue, IDeepCloneable<IExifValue>
{
	public TValueType? Value { get; set; }

	protected abstract string? StringValue { get; }

	protected ExifValue(ExifTag<TValueType> tag)
		: base(tag)
	{
	}

	protected ExifValue(ExifTagValue tag)
		: base(tag)
	{
	}

	internal ExifValue(ExifValue value)
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
			Value = default(TValueType);
			return true;
		}
		if (value.GetType() == typeof(TValueType))
		{
			Value = (TValueType)value;
			return true;
		}
		return false;
	}

	public override string? ToString()
	{
		if (!ExifTagDescriptionAttribute.TryGetDescription(base.Tag, Value, out string description))
		{
			return StringValue;
		}
		return description;
	}
}
