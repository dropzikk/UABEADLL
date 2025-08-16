using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccUInt16ArrayTagDataEntry : IccTagDataEntry, IEquatable<IccUInt16ArrayTagDataEntry>
{
	public ushort[] Data { get; }

	public IccUInt16ArrayTagDataEntry(ushort[] data)
		: this(data, IccProfileTag.Unknown)
	{
	}

	public IccUInt16ArrayTagDataEntry(ushort[] data, IccProfileTag tagSignature)
		: base(IccTypeSignature.UInt16Array, tagSignature)
	{
		Data = data ?? throw new ArgumentNullException("data");
	}

	public override bool Equals(IccTagDataEntry? other)
	{
		if (other is IccUInt16ArrayTagDataEntry other2)
		{
			return Equals(other2);
		}
		return false;
	}

	public bool Equals(IccUInt16ArrayTagDataEntry? other)
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
		if (obj is IccUInt16ArrayTagDataEntry other)
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
