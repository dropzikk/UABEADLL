using System;

namespace SixLabors.ImageSharp;

internal readonly struct ValueSize : IEquatable<ValueSize>
{
	public enum ValueSizeType
	{
		Absolute,
		PercentageOfWidth,
		PercentageOfHeight
	}

	public float Value { get; }

	public ValueSizeType Type { get; }

	public ValueSize(float value, ValueSizeType type)
	{
		if (type != 0)
		{
			Guard.MustBeBetweenOrEqualTo(value, 0f, 1f, "value");
		}
		Value = value;
		Type = type;
	}

	public static implicit operator ValueSize(float f)
	{
		return Absolute(f);
	}

	public static ValueSize PercentageOfWidth(float percentage)
	{
		return new ValueSize(percentage, ValueSizeType.PercentageOfWidth);
	}

	public static ValueSize PercentageOfHeight(float percentage)
	{
		return new ValueSize(percentage, ValueSizeType.PercentageOfHeight);
	}

	public static ValueSize Absolute(float value)
	{
		return new ValueSize(value, ValueSizeType.Absolute);
	}

	public float Calculate(Size size)
	{
		return Type switch
		{
			ValueSizeType.PercentageOfWidth => Value * (float)size.Width, 
			ValueSizeType.PercentageOfHeight => Value * (float)size.Height, 
			_ => Value, 
		};
	}

	public override string ToString()
	{
		return $"{Value} - {Type}";
	}

	public override bool Equals(object? obj)
	{
		if (obj is ValueSize other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(ValueSize other)
	{
		if (Type == other.Type)
		{
			return Value.Equals(other.Value);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Value, Type);
	}
}
