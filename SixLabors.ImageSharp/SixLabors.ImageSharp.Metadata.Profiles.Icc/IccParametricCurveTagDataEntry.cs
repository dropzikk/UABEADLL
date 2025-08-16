using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccParametricCurveTagDataEntry : IccTagDataEntry, IEquatable<IccParametricCurveTagDataEntry>
{
	public IccParametricCurve Curve { get; }

	public IccParametricCurveTagDataEntry(IccParametricCurve curve)
		: this(curve, IccProfileTag.Unknown)
	{
	}

	public IccParametricCurveTagDataEntry(IccParametricCurve curve, IccProfileTag tagSignature)
		: base(IccTypeSignature.ParametricCurve, tagSignature)
	{
		Curve = curve ?? throw new ArgumentNullException("curve");
	}

	public override bool Equals(IccTagDataEntry? other)
	{
		if (other is IccParametricCurveTagDataEntry other2)
		{
			return Equals(other2);
		}
		return false;
	}

	public bool Equals(IccParametricCurveTagDataEntry? other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (base.Equals(other))
		{
			return Curve.Equals(other.Curve);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is IccParametricCurveTagDataEntry other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(base.Signature, Curve);
	}
}
