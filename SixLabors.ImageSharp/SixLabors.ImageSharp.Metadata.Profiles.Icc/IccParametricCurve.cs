using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccParametricCurve : IEquatable<IccParametricCurve>
{
	public IccParametricCurveType Type { get; }

	public float G { get; }

	public float A { get; }

	public float B { get; }

	public float C { get; }

	public float D { get; }

	public float E { get; }

	public float F { get; }

	public IccParametricCurve(float g)
		: this(IccParametricCurveType.Type1, g, 0f, 0f, 0f, 0f, 0f, 0f)
	{
	}

	public IccParametricCurve(float g, float a, float b)
		: this(IccParametricCurveType.Cie122_1996, g, a, b, 0f, 0f, 0f, 0f)
	{
	}

	public IccParametricCurve(float g, float a, float b, float c)
		: this(IccParametricCurveType.Iec61966_3, g, a, b, c, 0f, 0f, 0f)
	{
	}

	public IccParametricCurve(float g, float a, float b, float c, float d)
		: this(IccParametricCurveType.SRgb, g, a, b, c, d, 0f, 0f)
	{
	}

	public IccParametricCurve(float g, float a, float b, float c, float d, float e, float f)
		: this(IccParametricCurveType.Type5, g, a, b, c, d, e, f)
	{
	}

	private IccParametricCurve(IccParametricCurveType type, float g, float a, float b, float c, float d, float e, float f)
	{
		Type = type;
		G = g;
		A = a;
		B = b;
		C = c;
		D = d;
		E = e;
		F = f;
	}

	public bool Equals(IccParametricCurve? other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (Type == other.Type && G.Equals(other.G) && A.Equals(other.A) && B.Equals(other.B) && C.Equals(other.C) && D.Equals(other.D) && E.Equals(other.E))
		{
			return F.Equals(other.F);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is IccParametricCurve other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Type, G.GetHashCode(), A.GetHashCode(), B.GetHashCode(), C.GetHashCode(), D.GetHashCode(), E.GetHashCode(), F.GetHashCode());
	}
}
