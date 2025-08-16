using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

public abstract class IccTagDataEntry : IEquatable<IccTagDataEntry>
{
	public IccTypeSignature Signature { get; }

	public IccProfileTag TagSignature { get; set; }

	protected IccTagDataEntry(IccTypeSignature signature)
		: this(signature, IccProfileTag.Unknown)
	{
	}

	protected IccTagDataEntry(IccTypeSignature signature, IccProfileTag tagSignature)
	{
		Signature = signature;
		TagSignature = tagSignature;
	}

	public override bool Equals(object? obj)
	{
		if (obj is IccTagDataEntry other)
		{
			return Equals(other);
		}
		return false;
	}

	public virtual bool Equals(IccTagDataEntry? other)
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

	public override int GetHashCode()
	{
		return Signature.GetHashCode();
	}
}
