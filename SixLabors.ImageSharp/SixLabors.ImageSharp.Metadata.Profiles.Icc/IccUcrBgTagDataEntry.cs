using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccUcrBgTagDataEntry : IccTagDataEntry, IEquatable<IccUcrBgTagDataEntry>
{
	public ushort[] UcrCurve { get; }

	public ushort[] BgCurve { get; }

	public string Description { get; }

	public IccUcrBgTagDataEntry(ushort[] ucrCurve, ushort[] bgCurve, string description)
		: this(ucrCurve, bgCurve, description, IccProfileTag.Unknown)
	{
	}

	public IccUcrBgTagDataEntry(ushort[] ucrCurve, ushort[] bgCurve, string description, IccProfileTag tagSignature)
		: base(IccTypeSignature.UcrBg, tagSignature)
	{
		UcrCurve = ucrCurve ?? throw new ArgumentNullException("ucrCurve");
		BgCurve = bgCurve ?? throw new ArgumentNullException("bgCurve");
		Description = description ?? throw new ArgumentNullException("description");
	}

	public override bool Equals(IccTagDataEntry? other)
	{
		if (other is IccUcrBgTagDataEntry other2)
		{
			return Equals(other2);
		}
		return false;
	}

	public bool Equals(IccUcrBgTagDataEntry? other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (base.Equals(other) && UcrCurve.AsSpan().SequenceEqual(other.UcrCurve) && BgCurve.AsSpan().SequenceEqual(other.BgCurve))
		{
			return string.Equals(Description, other.Description, StringComparison.OrdinalIgnoreCase);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is IccUcrBgTagDataEntry other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(base.Signature, UcrCurve, BgCurve, Description);
	}
}
