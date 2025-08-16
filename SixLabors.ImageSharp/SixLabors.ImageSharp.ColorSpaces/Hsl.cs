using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp.ColorSpaces;

public readonly struct Hsl : IEquatable<Hsl>
{
	private static readonly Vector3 Min = Vector3.Zero;

	private static readonly Vector3 Max = new Vector3(360f, 1f, 1f);

	public float H { get; }

	public float S { get; }

	public float L { get; }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Hsl(float h, float s, float l)
		: this(new Vector3(h, s, l))
	{
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Hsl(Vector3 vector)
	{
		vector = Vector3.Clamp(vector, Min, Max);
		H = vector.X;
		S = vector.Y;
		L = vector.Z;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Hsl left, Hsl right)
	{
		return left.Equals(right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Hsl left, Hsl right)
	{
		return !left.Equals(right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode()
	{
		return HashCode.Combine(H, S, L);
	}

	public override string ToString()
	{
		return FormattableString.Invariant($"Hsl({H:#0.##}, {S:#0.##}, {L:#0.##})");
	}

	public override bool Equals(object? obj)
	{
		if (obj is Hsl other)
		{
			return Equals(other);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Hsl other)
	{
		if (H.Equals(other.H) && S.Equals(other.S))
		{
			return L.Equals(other.L);
		}
		return false;
	}
}
