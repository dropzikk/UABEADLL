using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccUInt32ArrayTagDataEntry : IccTagDataEntry, IEquatable<IccUInt32ArrayTagDataEntry>
{
	public uint[] Data { get; }

	public IccUInt32ArrayTagDataEntry(uint[] data)
		: this(data, IccProfileTag.Unknown)
	{
	}

	public IccUInt32ArrayTagDataEntry(uint[] data, IccProfileTag tagSignature)
		: base(IccTypeSignature.UInt32Array, tagSignature)
	{
		Data = data ?? throw new ArgumentNullException("data");
	}

	public override bool Equals(IccTagDataEntry? other)
	{
		if (other is IccUInt32ArrayTagDataEntry other2)
		{
			return Equals(other2);
		}
		return false;
	}

	public bool Equals(IccUInt32ArrayTagDataEntry? other)
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
		if (obj is IccUInt32ArrayTagDataEntry other)
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
