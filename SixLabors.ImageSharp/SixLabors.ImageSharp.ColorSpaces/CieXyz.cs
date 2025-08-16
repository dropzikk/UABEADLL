using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp.ColorSpaces;

public readonly struct CieXyz : IEquatable<CieXyz>
{
	public float X { get; }

	public float Y { get; }

	public float Z { get; }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CieXyz(float x, float y, float z)
		: this(new Vector3(x, y, z))
	{
	}

	public CieXyz(Vector3 vector)
	{
		this = default(CieXyz);
		X = vector.X;
		Y = vector.Y;
		Z = vector.Z;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(CieXyz left, CieXyz right)
	{
		return left.Equals(right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(CieXyz left, CieXyz right)
	{
		return !left.Equals(right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector3 ToVector3()
	{
		return new Vector3(X, Y, Z);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(X, Y, Z);
	}

	public override string ToString()
	{
		return FormattableString.Invariant($"CieXyz({X:#0.##}, {Y:#0.##}, {Z:#0.##})");
	}

	public override bool Equals(object? obj)
	{
		if (obj is CieXyz other)
		{
			return Equals(other);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(CieXyz other)
	{
		if (X.Equals(other.X) && Y.Equals(other.Y))
		{
			return Z.Equals(other.Z);
		}
		return false;
	}
}
