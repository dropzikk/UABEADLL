using System;
using System.Globalization;

namespace SixLabors.ImageSharp;

public readonly struct Rational : IEquatable<Rational>
{
	public uint Numerator { get; }

	public uint Denominator { get; }

	public Rational(uint value)
		: this(value, 1u)
	{
	}

	public Rational(uint numerator, uint denominator)
		: this(numerator, denominator, simplify: true)
	{
	}

	public Rational(uint numerator, uint denominator, bool simplify)
	{
		if (simplify)
		{
			LongRational longRational = new LongRational(numerator, denominator).Simplify();
			Numerator = (uint)longRational.Numerator;
			Denominator = (uint)longRational.Denominator;
		}
		else
		{
			Numerator = numerator;
			Denominator = denominator;
		}
	}

	public Rational(double value)
		: this(value, bestPrecision: false)
	{
	}

	public Rational(double value, bool bestPrecision)
	{
		LongRational longRational = LongRational.FromDouble(Math.Abs(value), bestPrecision);
		Numerator = (uint)longRational.Numerator;
		Denominator = (uint)longRational.Denominator;
	}

	public static bool operator ==(Rational left, Rational right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(Rational left, Rational right)
	{
		return !left.Equals(right);
	}

	public static Rational FromDouble(double value)
	{
		return new Rational(value, bestPrecision: false);
	}

	public static Rational FromDouble(double value, bool bestPrecision)
	{
		return new Rational(value, bestPrecision);
	}

	public override bool Equals(object? obj)
	{
		if (obj is Rational other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(Rational other)
	{
		if (Numerator == other.Numerator)
		{
			return Denominator == other.Denominator;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return new LongRational(Numerator, Denominator).GetHashCode();
	}

	public double ToDouble()
	{
		return (double)Numerator / (double)Denominator;
	}

	public float ToSingle()
	{
		return (float)Numerator / (float)Denominator;
	}

	public override string ToString()
	{
		return ToString(CultureInfo.InvariantCulture);
	}

	public string ToString(IFormatProvider provider)
	{
		return new LongRational(Numerator, Denominator).ToString(provider);
	}
}
