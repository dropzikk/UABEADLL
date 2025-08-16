using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp.ColorSpaces;

public readonly struct CieLchuv : IEquatable<CieLchuv>
{
	private static readonly Vector3 Min = new Vector3(0f, -200f, 0f);

	private static readonly Vector3 Max = new Vector3(100f, 200f, 360f);

	public static readonly CieXyz DefaultWhitePoint = Illuminants.D65;

	public float L { get; }

	public float C { get; }

	public float H { get; }

	public CieXyz WhitePoint { get; }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CieLchuv(float l, float c, float h)
		: this(l, c, h, DefaultWhitePoint)
	{
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CieLchuv(float l, float c, float h, CieXyz whitePoint)
		: this(new Vector3(l, c, h), whitePoint)
	{
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CieLchuv(Vector3 vector)
		: this(vector, DefaultWhitePoint)
	{
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CieLchuv(Vector3 vector, CieXyz whitePoint)
	{
		this = default(CieLchuv);
		vector = Vector3.Clamp(vector, Min, Max);
		L = vector.X;
		C = vector.Y;
		H = vector.Z;
		WhitePoint = whitePoint;
	}

	public static bool operator ==(CieLchuv left, CieLchuv right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(CieLchuv left, CieLchuv right)
	{
		return !left.Equals(right);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(L, C, H, WhitePoint);
	}

	public override string ToString()
	{
		return FormattableString.Invariant($"CieLchuv({L:#0.##}, {C:#0.##}, {H:#0.##})");
	}

	public override bool Equals(object? obj)
	{
		if (obj is CieLchuv other)
		{
			return Equals(other);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(CieLchuv other)
	{
		if (L.Equals(other.L) && C.Equals(other.C) && H.Equals(other.H))
		{
			return WhitePoint.Equals(other.WhitePoint);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float Saturation()
	{
		float num = 100f * (C / L);
		if (float.IsNaN(num))
		{
			return 0f;
		}
		return num;
	}
}
