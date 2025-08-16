using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccProfileSequenceIdentifierTagDataEntry : IccTagDataEntry, IEquatable<IccProfileSequenceIdentifierTagDataEntry>
{
	public IccProfileSequenceIdentifier[] Data { get; }

	public IccProfileSequenceIdentifierTagDataEntry(IccProfileSequenceIdentifier[] data)
		: this(data, IccProfileTag.Unknown)
	{
	}

	public IccProfileSequenceIdentifierTagDataEntry(IccProfileSequenceIdentifier[] data, IccProfileTag tagSignature)
		: base(IccTypeSignature.ProfileSequenceIdentifier, tagSignature)
	{
		Data = data ?? throw new ArgumentNullException("data");
	}

	public override bool Equals(IccTagDataEntry? other)
	{
		if (other is IccProfileSequenceIdentifierTagDataEntry other2)
		{
			return Equals(other2);
		}
		return false;
	}

	public bool Equals(IccProfileSequenceIdentifierTagDataEntry? other)
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
			return Data.AsSpan().SequenceEqual(other.Data);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is IccProfileSequenceIdentifierTagDataEntry other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(base.Signature, Data);
	}
}
