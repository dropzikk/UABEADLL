using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp.ColorSpaces;

public readonly struct CieLuv : IEquatable<CieLuv>
{
	public static readonly CieXyz DefaultWhitePoint = Illuminants.D65;

	public float L { get; }

	public float U { get; }

	public float V { get; }

	public CieXyz WhitePoint { get; }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CieLuv(float l, float u, float v)
		: this(l, u, v, DefaultWhitePoint)
	{
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CieLuv(float l, float u, float v, CieXyz whitePoint)
		: this(new Vector3(l, u, v), whitePoint)
	{
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CieLuv(Vector3 vector)
		: this(vector, DefaultWhitePoint)
	{
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CieLuv(Vector3 vector, CieXyz whitePoint)
	{
		L = vector.X;
		U = vector.Y;
		V = vector.Z;
		WhitePoint = whitePoint;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(CieLuv left, CieLuv right)
	{
		return left.Equals(right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(CieLuv left, CieLuv right)
	{
		return !left.Equals(right);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(L, U, V, WhitePoint);
	}

	public override string ToString()
	{
		return FormattableString.Invariant($"CieLuv({L:#0.##}, {U:#0.##}, {V:#0.##})");
	}

	public override bool Equals(object? obj)
	{
		if (obj is CieLuv other)
		{
			return Equals(other);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(CieLuv other)
	{
		if (L.Equals(other.L) && U.Equals(other.U) && V.Equals(other.V))
		{
			return WhitePoint.Equals(other.WhitePoint);
		}
		return false;
	}
}
