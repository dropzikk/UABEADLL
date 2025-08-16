using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccUFix16ArrayTagDataEntry : IccTagDataEntry, IEquatable<IccUFix16ArrayTagDataEntry>
{
	public float[] Data { get; }

	public IccUFix16ArrayTagDataEntry(float[] data)
		: this(data, IccProfileTag.Unknown)
	{
	}

	public IccUFix16ArrayTagDataEntry(float[] data, IccProfileTag tagSignature)
		: base(IccTypeSignature.U16Fixed16Array, tagSignature)
	{
		Data = data ?? throw new ArgumentNullException("data");
	}

	public override bool Equals(IccTagDataEntry? other)
	{
		if (other is IccUFix16ArrayTagDataEntry other2)
		{
			return Equals(other2);
		}
		return false;
	}

	public bool Equals(IccUFix16ArrayTagDataEntry? other)
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
		if (obj is IccUFix16ArrayTagDataEntry other)
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
