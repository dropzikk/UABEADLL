using System;
using System.Numerics;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccResponseCurve : IEquatable<IccResponseCurve>
{
	public IccCurveMeasurementEncodings CurveType { get; }

	public Vector3[] XyzValues { get; }

	public IccResponseNumber[][] ResponseArrays { get; }

	public IccResponseCurve(IccCurveMeasurementEncodings curveType, Vector3[] xyzValues, IccResponseNumber[][] responseArrays)
	{
		Guard.NotNull(xyzValues, "xyzValues");
		Guard.NotNull(responseArrays, "responseArrays");
		Guard.IsTrue(xyzValues.Length == responseArrays.Length, "xyzValues,responseArrays", "Arrays must have same length");
		Guard.MustBeBetweenOrEqualTo(xyzValues.Length, 1, 15, "xyzValues");
		CurveType = curveType;
		XyzValues = xyzValues;
		ResponseArrays = responseArrays;
	}

	public bool Equals(IccResponseCurve? other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (CurveType == other.CurveType && XyzValues.AsSpan().SequenceEqual(other.XyzValues))
		{
			return EqualsResponseArray(other);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is IccResponseCurve other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(CurveType, XyzValues, ResponseArrays);
	}

	private bool EqualsResponseArray(IccResponseCurve other)
	{
		if (ResponseArrays.Length != other.ResponseArrays.Length)
		{
			return false;
		}
		for (int i = 0; i < ResponseArrays.Length; i++)
		{
			if (!ResponseArrays[i].AsSpan().SequenceEqual(other.ResponseArrays[i]))
			{
				return false;
			}
		}
		return true;
	}
}
