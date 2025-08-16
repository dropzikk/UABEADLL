using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp;

internal readonly struct Complex64 : IEquatable<Complex64>
{
	public readonly float Real;

	public readonly float Imaginary;

	public Complex64(float real, float imaginary)
	{
		Real = real;
		Imaginary = imaginary;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Complex64 operator *(Complex64 value, float scalar)
	{
		return new Complex64(value.Real * scalar, value.Imaginary * scalar);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ComplexVector4 operator *(Complex64 value, Vector4 vector)
	{
		ComplexVector4 result = default(ComplexVector4);
		result.Real = vector * value.Real;
		result.Imaginary = vector * value.Imaginary;
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ComplexVector4 operator *(Complex64 value, ComplexVector4 vector)
	{
		Vector4 real = value.Real * vector.Real - value.Imaginary * vector.Imaginary;
		Vector4 imaginary = value.Real * vector.Imaginary + value.Imaginary * vector.Real;
		ComplexVector4 result = default(ComplexVector4);
		result.Real = real;
		result.Imaginary = imaginary;
		return result;
	}

	public bool Equals(Complex64 other)
	{
		if (Real.Equals(other.Real))
		{
			return Imaginary.Equals(other.Imaginary);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is Complex64 other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (Real.GetHashCode() * 397) ^ Imaginary.GetHashCode();
	}

	public override string ToString()
	{
		return $"{Real}{((Imaginary >= 0f) ? "+" : string.Empty)}{Imaginary}j";
	}
}
