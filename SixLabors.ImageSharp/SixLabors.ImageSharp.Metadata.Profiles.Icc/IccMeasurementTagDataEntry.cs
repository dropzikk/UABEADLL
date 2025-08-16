using System;
using System.Numerics;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccMeasurementTagDataEntry : IccTagDataEntry, IEquatable<IccMeasurementTagDataEntry>
{
	public IccStandardObserver Observer { get; }

	public Vector3 XyzBacking { get; }

	public IccMeasurementGeometry Geometry { get; }

	public float Flare { get; }

	public IccStandardIlluminant Illuminant { get; }

	public IccMeasurementTagDataEntry(IccStandardObserver observer, Vector3 xyzBacking, IccMeasurementGeometry geometry, float flare, IccStandardIlluminant illuminant)
		: this(observer, xyzBacking, geometry, flare, illuminant, IccProfileTag.Unknown)
	{
	}

	public IccMeasurementTagDataEntry(IccStandardObserver observer, Vector3 xyzBacking, IccMeasurementGeometry geometry, float flare, IccStandardIlluminant illuminant, IccProfileTag tagSignature)
		: base(IccTypeSignature.Measurement, tagSignature)
	{
		Observer = observer;
		XyzBacking = xyzBacking;
		Geometry = geometry;
		Flare = flare;
		Illuminant = illuminant;
	}

	public override bool Equals(IccTagDataEntry? other)
	{
		if (other is IccMeasurementTagDataEntry other2)
		{
			return Equals(other2);
		}
		return false;
	}

	public bool Equals(IccMeasurementTagDataEntry? other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (base.Equals(other) && Observer == other.Observer && XyzBacking.Equals(other.XyzBacking) && Geometry == other.Geometry && Flare.Equals(other.Flare))
		{
			return Illuminant == other.Illuminant;
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is IccMeasurementTagDataEntry other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(base.Signature, Observer, XyzBacking, Geometry, Flare, Illuminant);
	}
}
