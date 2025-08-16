using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccColorantTableTagDataEntry : IccTagDataEntry, IEquatable<IccColorantTableTagDataEntry>
{
	public IccColorantTableEntry[] ColorantData { get; }

	public IccColorantTableTagDataEntry(IccColorantTableEntry[] colorantData)
		: this(colorantData, IccProfileTag.Unknown)
	{
	}

	public IccColorantTableTagDataEntry(IccColorantTableEntry[] colorantData, IccProfileTag tagSignature)
		: base(IccTypeSignature.ColorantTable, tagSignature)
	{
		Guard.NotNull(colorantData, "colorantData");
		Guard.MustBeBetweenOrEqualTo(colorantData.Length, 1, 15, "colorantData");
		ColorantData = colorantData;
	}

	public override bool Equals(IccTagDataEntry? other)
	{
		if (other is IccColorantTableTagDataEntry other2)
		{
			return Equals(other2);
		}
		return false;
	}

	public bool Equals(IccColorantTableTagDataEntry? other)
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
			return ColorantData.AsSpan().SequenceEqual(other.ColorantData);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is IccColorantTableTagDataEntry other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(base.Signature, ColorantData);
	}
}
