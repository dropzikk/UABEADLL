using System;
using System.Globalization;

namespace SixLabors.ImageSharp;

public readonly struct SignedRational : IEquatable<SignedRational>
{
	public int Numerator { get; }

	public int Denominator { get; }

	public SignedRational(int value)
		: this(value, 1)
	{
	}

	public SignedRational(int numerator, int denominator)
		: this(numerator, denominator, simplify: true)
	{
	}

	public SignedRational(int numerator, int denominator, bool simplify)
	{
		if (simplify)
		{
			LongRational longRational = new LongRational(numerator, denominator).Simplify();
			Numerator = (int)longRational.Numerator;
			Denominator = (int)longRational.Denominator;
		}
		else
		{
			Numerator = numerator;
			Denominator = denominator;
		}
	}

	public SignedRational(double value)
		: this(value, bestPrecision: false)
	{
	}

	public SignedRational(double value, bool bestPrecision)
	{
		LongRational longRational = LongRational.FromDouble(value, bestPrecision);
		Numerator = (int)longRational.Numerator;
		Denominator = (int)longRational.Denominator;
	}

	public static bool operator ==(SignedRational left, SignedRational right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(SignedRational left, SignedRational right)
	{
		return !left.Equals(right);
	}

	public static SignedRational FromDouble(double value)
	{
		return new SignedRational(value, bestPrecision: false);
	}

	public static SignedRational FromDouble(double value, bool bestPrecision)
	{
		return new SignedRational(value, bestPrecision);
	}

	public override bool Equals(object? obj)
	{
		if (obj is SignedRational other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(SignedRational other)
	{
		LongRational longRational = new LongRational(Numerator, Denominator);
		LongRational other2 = new LongRational(other.Numerator, other.Denominator);
		return longRational.Equals(other2);
	}

	public override int GetHashCode()
	{
		return new LongRational(Numerator, Denominator).GetHashCode();
	}

	public double ToDouble()
	{
		return (double)Numerator / (double)Denominator;
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
