using System;
using System.Globalization;
using System.Text;

namespace SixLabors.ImageSharp;

internal readonly struct LongRational : IEquatable<LongRational>
{
	public long Numerator { get; }

	public long Denominator { get; }

	public bool IsIndeterminate
	{
		get
		{
			if (Denominator == 0L)
			{
				return Numerator == 0;
			}
			return false;
		}
	}

	public bool IsInteger => Denominator == 1;

	public bool IsNegativeInfinity
	{
		get
		{
			if (Denominator == 0L)
			{
				return Numerator == -1;
			}
			return false;
		}
	}

	public bool IsPositiveInfinity
	{
		get
		{
			if (Denominator == 0L)
			{
				return Numerator == 1;
			}
			return false;
		}
	}

	public bool IsZero
	{
		get
		{
			if (Denominator == 1)
			{
				return Numerator == 0;
			}
			return false;
		}
	}

	public LongRational(long numerator, long denominator)
	{
		Numerator = numerator;
		Denominator = denominator;
	}

	public override bool Equals(object? obj)
	{
		if (obj is LongRational other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(LongRational other)
	{
		if (Numerator == other.Numerator)
		{
			return Denominator == other.Denominator;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Numerator, Denominator);
	}

	public override string ToString()
	{
		return ToString(CultureInfo.InvariantCulture);
	}

	public string ToString(IFormatProvider provider)
	{
		if (IsIndeterminate)
		{
			return "[ Indeterminate ]";
		}
		if (IsPositiveInfinity)
		{
			return "[ PositiveInfinity ]";
		}
		if (IsNegativeInfinity)
		{
			return "[ NegativeInfinity ]";
		}
		if (IsZero)
		{
			return "0";
		}
		if (IsInteger)
		{
			return Numerator.ToString(provider);
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(Numerator.ToString(provider)).Append('/').Append(Denominator.ToString(provider));
		return stringBuilder.ToString();
	}

	public static LongRational FromDouble(double value, bool bestPrecision)
	{
		if (value == 0.0)
		{
			return new LongRational(0L, 1L);
		}
		if (double.IsNaN(value))
		{
			return new LongRational(0L, 0L);
		}
		if (double.IsPositiveInfinity(value))
		{
			return new LongRational(1L, 0L);
		}
		if (double.IsNegativeInfinity(value))
		{
			return new LongRational(-1L, 0L);
		}
		long num = 1L;
		long num2 = 1L;
		double num3 = Math.Abs(value);
		double num4 = (double)num / (double)num2;
		double num5 = (bestPrecision ? double.Epsilon : 1E-06);
		while (Math.Abs(num4 - num3) > num5)
		{
			if (num4 < num3)
			{
				num++;
			}
			else
			{
				num2++;
				num = (int)(num3 * (double)num2);
			}
			num4 = (double)num / (double)num2;
		}
		if (value < 0.0)
		{
			num *= -1;
		}
		return new LongRational(num, num2).Simplify();
	}

	private static long GreatestCommonDivisor(long left, long right)
	{
		if (right != 0L)
		{
			return GreatestCommonDivisor(right, left % right);
		}
		return left;
	}

	public LongRational Simplify()
	{
		if (IsIndeterminate || IsNegativeInfinity || IsPositiveInfinity || IsInteger || IsZero)
		{
			return this;
		}
		if (Numerator == Denominator)
		{
			return new LongRational(1L, 1L);
		}
		long num = GreatestCommonDivisor(Math.Abs(Numerator), Math.Abs(Denominator));
		if (num > 1)
		{
			return new LongRational(Numerator / num, Denominator / num);
		}
		return this;
	}
}
