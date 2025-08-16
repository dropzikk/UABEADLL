using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccUnknownTagDataEntry : IccTagDataEntry, IEquatable<IccUnknownTagDataEntry>
{
	public byte[] Data { get; }

	public IccUnknownTagDataEntry(byte[] data)
		: this(data, IccProfileTag.Unknown)
	{
	}

	public IccUnknownTagDataEntry(byte[] data, IccProfileTag tagSignature)
		: base(IccTypeSignature.Unknown, tagSignature)
	{
		Data = data ?? throw new ArgumentNullException("data");
	}

	public override bool Equals(IccTagDataEntry? other)
	{
		if (other is IccUnknownTagDataEntry other2)
		{
			return Equals(other2);
		}
		return false;
	}

	public bool Equals(IccUnknownTagDataEntry? other)
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
		if (obj is IccUnknownTagDataEntry other)
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
