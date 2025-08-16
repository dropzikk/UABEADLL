using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccCurveTagDataEntry : IccTagDataEntry, IEquatable<IccCurveTagDataEntry>
{
	public float[] CurveData { get; }

	public float Gamma
	{
		get
		{
			if (!IsGamma)
			{
				return 0f;
			}
			return CurveData[0];
		}
	}

	public bool IsIdentityResponse => CurveData.Length == 0;

	public bool IsGamma => CurveData.Length == 1;

	public IccCurveTagDataEntry()
		: this(Array.Empty<float>(), IccProfileTag.Unknown)
	{
	}

	public IccCurveTagDataEntry(float gamma)
		: this(new float[1] { gamma }, IccProfileTag.Unknown)
	{
	}

	public IccCurveTagDataEntry(float[] curveData)
		: this(curveData, IccProfileTag.Unknown)
	{
	}

	public IccCurveTagDataEntry(IccProfileTag tagSignature)
		: this(Array.Empty<float>(), tagSignature)
	{
	}

	public IccCurveTagDataEntry(float gamma, IccProfileTag tagSignature)
		: this(new float[1] { gamma }, tagSignature)
	{
	}

	public IccCurveTagDataEntry(float[] curveData, IccProfileTag tagSignature)
		: base(IccTypeSignature.Curve, tagSignature)
	{
		CurveData = curveData ?? Array.Empty<float>();
	}

	public override bool Equals(IccTagDataEntry? other)
	{
		if (other is IccCurveTagDataEntry other2)
		{
			return Equals(other2);
		}
		return false;
	}

	public bool Equals(IccCurveTagDataEntry? other)
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
			return CurveData.AsSpan().SequenceEqual(other.CurveData);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is IccCurveTagDataEntry other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(base.Signature, CurveData);
	}
}
