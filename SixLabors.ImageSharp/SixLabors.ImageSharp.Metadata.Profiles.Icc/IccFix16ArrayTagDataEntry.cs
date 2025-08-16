using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccFix16ArrayTagDataEntry : IccTagDataEntry, IEquatable<IccFix16ArrayTagDataEntry>
{
	public float[] Data { get; }

	public IccFix16ArrayTagDataEntry(float[] data)
		: this(data, IccProfileTag.Unknown)
	{
	}

	public IccFix16ArrayTagDataEntry(float[] data, IccProfileTag tagSignature)
		: base(IccTypeSignature.S15Fixed16Array, tagSignature)
	{
		Data = data ?? throw new ArgumentNullException("data");
	}

	public override bool Equals(IccTagDataEntry? other)
	{
		if (other is IccFix16ArrayTagDataEntry other2)
		{
			return Equals(other2);
		}
		return false;
	}

	public bool Equals(IccFix16ArrayTagDataEntry? other)
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
		if (obj is IccFix16ArrayTagDataEntry other)
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
