using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccUInt64ArrayTagDataEntry : IccTagDataEntry, IEquatable<IccUInt64ArrayTagDataEntry>
{
	public ulong[] Data { get; }

	public IccUInt64ArrayTagDataEntry(ulong[] data)
		: this(data, IccProfileTag.Unknown)
	{
	}

	public IccUInt64ArrayTagDataEntry(ulong[] data, IccProfileTag tagSignature)
		: base(IccTypeSignature.UInt64Array, tagSignature)
	{
		Data = data ?? throw new ArgumentNullException("data");
	}

	public override bool Equals(IccTagDataEntry? other)
	{
		if (other is IccUInt64ArrayTagDataEntry other2)
		{
			return Equals(other2);
		}
		return false;
	}

	public bool Equals(IccUInt64ArrayTagDataEntry? other)
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
		if (obj is IccUInt64ArrayTagDataEntry other)
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
