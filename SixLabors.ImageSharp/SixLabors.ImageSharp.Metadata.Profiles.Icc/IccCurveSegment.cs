using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal abstract class IccCurveSegment : IEquatable<IccCurveSegment>
{
	public IccCurveSegmentSignature Signature { get; }

	protected IccCurveSegment(IccCurveSegmentSignature signature)
	{
		Signature = signature;
	}

	public virtual bool Equals(IccCurveSegment? other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		return Signature == other.Signature;
	}

	public override bool Equals(object? obj)
	{
		return Equals(obj as IccCurveSegment);
	}

	public override int GetHashCode()
	{
		return Signature.GetHashCode();
	}
}
