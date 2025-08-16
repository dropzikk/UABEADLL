using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp.ColorSpaces;

public readonly struct Hsv : IEquatable<Hsv>
{
	private static readonly Vector3 Min = Vector3.Zero;

	private static readonly Vector3 Max = new Vector3(360f, 1f, 1f);

	public float H { get; }

	public float S { get; }

	public float V { get; }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Hsv(float h, float s, float v)
		: this(new Vector3(h, s, v))
	{
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Hsv(Vector3 vector)
	{
		vector = Vector3.Clamp(vector, Min, Max);
		H = vector.X;
		S = vector.Y;
		V = vector.Z;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Hsv left, Hsv right)
	{
		return left.Equals(right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Hsv left, Hsv right)
	{
		return !left.Equals(right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode()
	{
		return HashCode.Combine(H, S, V);
	}

	public override string ToString()
	{
		return FormattableString.Invariant($"Hsv({H:#0.##}, {S:#0.##}, {V:#0.##})");
	}

	public override bool Equals(object? obj)
	{
		if (obj is Hsv other)
		{
			return Equals(other);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Hsv other)
	{
		if (H.Equals(other.H) && S.Equals(other.S))
		{
			return V.Equals(other.V);
		}
		return false;
	}
}
