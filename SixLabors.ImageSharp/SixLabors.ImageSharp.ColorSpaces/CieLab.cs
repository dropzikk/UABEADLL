using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp.ColorSpaces;

public readonly struct CieLab : IEquatable<CieLab>
{
	public static readonly CieXyz DefaultWhitePoint = Illuminants.D50;

	public float L { get; }

	public float A { get; }

	public float B { get; }

	public CieXyz WhitePoint { get; }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CieLab(float l, float a, float b)
		: this(l, a, b, DefaultWhitePoint)
	{
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CieLab(float l, float a, float b, CieXyz whitePoint)
		: this(new Vector3(l, a, b), whitePoint)
	{
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CieLab(Vector3 vector)
		: this(vector, DefaultWhitePoint)
	{
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CieLab(Vector3 vector, CieXyz whitePoint)
	{
		this = default(CieLab);
		L = vector.X;
		A = vector.Y;
		B = vector.Z;
		WhitePoint = whitePoint;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(CieLab left, CieLab right)
	{
		return left.Equals(right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(CieLab left, CieLab right)
	{
		return !left.Equals(right);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(L, A, B, WhitePoint);
	}

	public override string ToString()
	{
		return FormattableString.Invariant($"CieLab({L:#0.##}, {A:#0.##}, {B:#0.##})");
	}

	public override bool Equals(object? obj)
	{
		if (obj is CieLab other)
		{
			return Equals(other);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(CieLab other)
	{
		if (L.Equals(other.L) && A.Equals(other.A) && B.Equals(other.B))
		{
			return WhitePoint.Equals(other.WhitePoint);
		}
		return false;
	}
}
