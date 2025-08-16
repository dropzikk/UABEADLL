using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccDateTimeTagDataEntry : IccTagDataEntry, IEquatable<IccDateTimeTagDataEntry>
{
	public DateTime Value { get; }

	public IccDateTimeTagDataEntry(DateTime value)
		: this(value, IccProfileTag.Unknown)
	{
	}

	public IccDateTimeTagDataEntry(DateTime value, IccProfileTag tagSignature)
		: base(IccTypeSignature.DateTime, tagSignature)
	{
		Value = value;
	}

	public override bool Equals(IccTagDataEntry? other)
	{
		if (other is IccDateTimeTagDataEntry other2)
		{
			return Equals(other2);
		}
		return false;
	}

	public bool Equals(IccDateTimeTagDataEntry? other)
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
			return Value.Equals(other.Value);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is IccDateTimeTagDataEntry other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(base.Signature, Value);
	}
}
