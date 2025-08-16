using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccUInt8ArrayTagDataEntry : IccTagDataEntry, IEquatable<IccUInt8ArrayTagDataEntry>
{
	public byte[] Data { get; }

	public IccUInt8ArrayTagDataEntry(byte[] data)
		: this(data, IccProfileTag.Unknown)
	{
	}

	public IccUInt8ArrayTagDataEntry(byte[] data, IccProfileTag tagSignature)
		: base(IccTypeSignature.UInt8Array, tagSignature)
	{
		Data = data ?? throw new ArgumentNullException("data");
	}

	public override bool Equals(IccTagDataEntry? other)
	{
		if (other is IccUInt8ArrayTagDataEntry other2)
		{
			return Equals(other2);
		}
		return false;
	}

	public bool Equals(IccUInt8ArrayTagDataEntry? other)
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
		if (obj is IccUInt8ArrayTagDataEntry other)
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
