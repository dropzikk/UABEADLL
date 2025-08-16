using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp.ColorSpaces;

public readonly struct CieXyy : IEquatable<CieXyy>
{
	public float X { get; }

	public float Y { get; }

	public float Yl { get; }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CieXyy(float x, float y, float yl)
	{
		X = x;
		Y = y;
		Yl = yl;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CieXyy(Vector3 vector)
	{
		this = default(CieXyy);
		X = vector.X;
		Y = vector.Y;
		Yl = vector.Z;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(CieXyy left, CieXyy right)
	{
		return left.Equals(right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(CieXyy left, CieXyy right)
	{
		return !left.Equals(right);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(X, Y, Yl);
	}

	public override string ToString()
	{
		return FormattableString.Invariant($"CieXyy({X:#0.##}, {Y:#0.##}, {Yl:#0.##})");
	}

	public override bool Equals(object? obj)
	{
		if (obj is CieXyy other)
		{
			return Equals(other);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(CieXyy other)
	{
		if (X.Equals(other.X) && Y.Equals(other.Y))
		{
			return Yl.Equals(other.Yl);
		}
		return false;
	}
}
