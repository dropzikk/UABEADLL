using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccSampledCurveElement : IccCurveSegment, IEquatable<IccSampledCurveElement>
{
	public float[] CurveEntries { get; }

	public IccSampledCurveElement(float[] curveEntries)
		: base(IccCurveSegmentSignature.SampledCurve)
	{
		Guard.NotNull(curveEntries, "curveEntries");
		Guard.IsTrue(curveEntries.Length != 0, "curveEntries", "There must be at least one value");
		CurveEntries = curveEntries;
	}

	public override bool Equals(IccCurveSegment? other)
	{
		if (base.Equals(other) && other is IccSampledCurveElement iccSampledCurveElement)
		{
			return CurveEntries.AsSpan().SequenceEqual(iccSampledCurveElement.CurveEntries);
		}
		return false;
	}

	public bool Equals(IccSampledCurveElement? other)
	{
		return Equals((IccCurveSegment?)other);
	}

	public override bool Equals(object? obj)
	{
		return Equals(obj as IccSampledCurveElement);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(base.GetHashCode(), CurveEntries);
	}
}
