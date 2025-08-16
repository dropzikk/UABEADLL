using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccScreeningTagDataEntry : IccTagDataEntry, IEquatable<IccScreeningTagDataEntry>
{
	public IccScreeningFlag Flags { get; }

	public IccScreeningChannel[] Channels { get; }

	public IccScreeningTagDataEntry(IccScreeningFlag flags, IccScreeningChannel[] channels)
		: this(flags, channels, IccProfileTag.Unknown)
	{
	}

	public IccScreeningTagDataEntry(IccScreeningFlag flags, IccScreeningChannel[] channels, IccProfileTag tagSignature)
		: base(IccTypeSignature.Screening, tagSignature)
	{
		Flags = flags;
		Channels = channels ?? throw new ArgumentNullException("channels");
	}

	public override bool Equals(IccTagDataEntry? other)
	{
		if (other is IccScreeningTagDataEntry other2)
		{
			return Equals(other2);
		}
		return false;
	}

	public bool Equals(IccScreeningTagDataEntry? other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (base.Equals(other) && Flags == other.Flags)
		{
			return Channels.AsSpan().SequenceEqual(other.Channels);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is IccScreeningTagDataEntry other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(base.Signature, Flags, Channels);
	}
}
