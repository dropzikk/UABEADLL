using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccProfileSequenceDescTagDataEntry : IccTagDataEntry, IEquatable<IccProfileSequenceDescTagDataEntry>
{
	public IccProfileDescription[] Descriptions { get; }

	public IccProfileSequenceDescTagDataEntry(IccProfileDescription[] descriptions)
		: this(descriptions, IccProfileTag.Unknown)
	{
	}

	public IccProfileSequenceDescTagDataEntry(IccProfileDescription[] descriptions, IccProfileTag tagSignature)
		: base(IccTypeSignature.ProfileSequenceDesc, tagSignature)
	{
		Descriptions = descriptions ?? throw new ArgumentNullException("descriptions");
	}

	public override bool Equals(IccTagDataEntry? other)
	{
		if (other is IccProfileSequenceDescTagDataEntry other2)
		{
			return Equals(other2);
		}
		return false;
	}

	public bool Equals(IccProfileSequenceDescTagDataEntry? other)
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
			return Descriptions.AsSpan().SequenceEqual(other.Descriptions);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is IccProfileSequenceDescTagDataEntry other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(base.Signature, Descriptions);
	}
}
