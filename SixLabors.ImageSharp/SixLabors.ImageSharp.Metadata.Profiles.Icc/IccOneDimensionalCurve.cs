using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccOneDimensionalCurve : IEquatable<IccOneDimensionalCurve>
{
	public float[] BreakPoints { get; }

	public IccCurveSegment[] Segments { get; }

	public IccOneDimensionalCurve(float[] breakPoints, IccCurveSegment[] segments)
	{
		Guard.NotNull(breakPoints, "breakPoints");
		Guard.NotNull(segments, "segments");
		Guard.IsTrue(breakPoints.Length == segments.Length - 1, "breakPoints,segments", "Number of BreakPoints must be one less than number of Segments");
		BreakPoints = breakPoints;
		Segments = segments;
	}

	public bool Equals(IccOneDimensionalCurve? other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (BreakPoints.AsSpan().SequenceEqual(other.BreakPoints))
		{
			return Segments.AsSpan().SequenceEqual(other.Segments);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		return Equals(obj as IccOneDimensionalCurve);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(BreakPoints, Segments);
	}
}
