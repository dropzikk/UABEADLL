using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp;

internal struct ComplexVector4 : IEquatable<ComplexVector4>
{
	public Vector4 Real;

	public Vector4 Imaginary;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Sum(ComplexVector4 value)
	{
		Real += value.Real;
		Imaginary += value.Imaginary;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector4 WeightedSum(float a, float b)
	{
		return Real * a + Imaginary * b;
	}

	public bool Equals(ComplexVector4 other)
	{
		if (Real.Equals(other.Real))
		{
			return Imaginary.Equals(other.Imaginary);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is ComplexVector4 other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (Real.GetHashCode() * 397) ^ Imaginary.GetHashCode();
	}
}
