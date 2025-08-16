using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccColorantOrderTagDataEntry : IccTagDataEntry, IEquatable<IccColorantOrderTagDataEntry>
{
	public byte[] ColorantNumber { get; }

	public IccColorantOrderTagDataEntry(byte[] colorantNumber)
		: this(colorantNumber, IccProfileTag.Unknown)
	{
	}

	public IccColorantOrderTagDataEntry(byte[] colorantNumber, IccProfileTag tagSignature)
		: base(IccTypeSignature.ColorantOrder, tagSignature)
	{
		Guard.NotNull(colorantNumber, "colorantNumber");
		Guard.MustBeBetweenOrEqualTo(colorantNumber.Length, 1, 15, "colorantNumber");
		ColorantNumber = colorantNumber;
	}

	public override bool Equals(IccTagDataEntry? other)
	{
		if (other is IccColorantOrderTagDataEntry other2)
		{
			return Equals(other2);
		}
		return false;
	}

	public bool Equals(IccColorantOrderTagDataEntry? other)
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
			return ColorantNumber.AsSpan().SequenceEqual(other.ColorantNumber);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is IccColorantOrderTagDataEntry other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(base.Signature, ColorantNumber);
	}
}
