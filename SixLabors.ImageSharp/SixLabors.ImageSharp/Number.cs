using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace SixLabors.ImageSharp;

[StructLayout(LayoutKind.Explicit)]
public struct Number : IEquatable<Number>, IComparable<Number>
{
	[FieldOffset(0)]
	private readonly int signedValue;

	[FieldOffset(0)]
	private readonly uint unsignedValue;

	[FieldOffset(4)]
	private readonly bool isSigned;

	public Number(int value)
	{
		this = default(Number);
		signedValue = value;
		isSigned = true;
	}

	public Number(uint value)
	{
		this = default(Number);
		unsignedValue = value;
		isSigned = false;
	}

	public static implicit operator Number(int value)
	{
		return new Number(value);
	}

	public static implicit operator Number(uint value)
	{
		return new Number(value);
	}

	public static implicit operator Number(ushort value)
	{
		return new Number((uint)value);
	}

	public static explicit operator int(Number number)
	{
		if (!number.isSigned)
		{
			return (int)Numerics.Clamp(number.unsignedValue, 0u, 2147483647u);
		}
		return number.signedValue;
	}

	public static explicit operator uint(Number number)
	{
		if (!number.isSigned)
		{
			return number.unsignedValue;
		}
		return (uint)Numerics.Clamp(number.signedValue, 0, int.MaxValue);
	}

	public static explicit operator ushort(Number number)
	{
		if (!number.isSigned)
		{
			return (ushort)Numerics.Clamp(number.unsignedValue, 0u, 65535u);
		}
		return (ushort)Numerics.Clamp(number.signedValue, 0, 65535);
	}

	public static bool operator ==(Number left, Number right)
	{
		return object.Equals(left, right);
	}

	public static bool operator !=(Number left, Number right)
	{
		return !object.Equals(left, right);
	}

	public static bool operator >(Number left, Number right)
	{
		return left.CompareTo(right) == 1;
	}

	public static bool operator <(Number left, Number right)
	{
		return left.CompareTo(right) == -1;
	}

	public static bool operator >=(Number left, Number right)
	{
		return left.CompareTo(right) >= 0;
	}

	public static bool operator <=(Number left, Number right)
	{
		return left.CompareTo(right) <= 0;
	}

	public int CompareTo(Number other)
	{
		if (!isSigned)
		{
			return unsignedValue.CompareTo(other.unsignedValue);
		}
		return signedValue.CompareTo(other.signedValue);
	}

	public override bool Equals(object? obj)
	{
		if (obj is Number other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(Number other)
	{
		if (isSigned != other.isSigned)
		{
			return false;
		}
		if (!isSigned)
		{
			return unsignedValue.Equals(other.unsignedValue);
		}
		return signedValue.Equals(other.signedValue);
	}

	public override int GetHashCode()
	{
		if (!isSigned)
		{
			return unsignedValue.GetHashCode();
		}
		return signedValue.GetHashCode();
	}

	public override string ToString()
	{
		return ToString(CultureInfo.InvariantCulture);
	}

	public string ToString(IFormatProvider provider)
	{
		if (!isSigned)
		{
			return unsignedValue.ToString(provider);
		}
		return signedValue.ToString(provider);
	}
}
