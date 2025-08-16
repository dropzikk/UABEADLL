using System;
using System.Numerics;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccViewingConditionsTagDataEntry : IccTagDataEntry, IEquatable<IccViewingConditionsTagDataEntry>
{
	public Vector3 IlluminantXyz { get; }

	public Vector3 SurroundXyz { get; }

	public IccStandardIlluminant Illuminant { get; }

	public IccViewingConditionsTagDataEntry(Vector3 illuminantXyz, Vector3 surroundXyz, IccStandardIlluminant illuminant)
		: this(illuminantXyz, surroundXyz, illuminant, IccProfileTag.Unknown)
	{
	}

	public IccViewingConditionsTagDataEntry(Vector3 illuminantXyz, Vector3 surroundXyz, IccStandardIlluminant illuminant, IccProfileTag tagSignature)
		: base(IccTypeSignature.ViewingConditions, tagSignature)
	{
		IlluminantXyz = illuminantXyz;
		SurroundXyz = surroundXyz;
		Illuminant = illuminant;
	}

	public override bool Equals(IccTagDataEntry? other)
	{
		if (other is IccViewingConditionsTagDataEntry other2)
		{
			return Equals(other2);
		}
		return false;
	}

	public bool Equals(IccViewingConditionsTagDataEntry? other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (base.Equals(other) && IlluminantXyz.Equals(other.IlluminantXyz) && SurroundXyz.Equals(other.SurroundXyz))
		{
			return Illuminant == other.Illuminant;
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is IccViewingConditionsTagDataEntry other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(base.Signature, IlluminantXyz, SurroundXyz, Illuminant);
	}
}
