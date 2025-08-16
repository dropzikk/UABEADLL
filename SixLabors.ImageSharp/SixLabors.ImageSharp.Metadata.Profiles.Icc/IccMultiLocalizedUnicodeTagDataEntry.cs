using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccMultiLocalizedUnicodeTagDataEntry : IccTagDataEntry, IEquatable<IccMultiLocalizedUnicodeTagDataEntry>
{
	public IccLocalizedString[] Texts { get; }

	public IccMultiLocalizedUnicodeTagDataEntry(IccLocalizedString[] texts)
		: this(texts, IccProfileTag.Unknown)
	{
	}

	public IccMultiLocalizedUnicodeTagDataEntry(IccLocalizedString[] texts, IccProfileTag tagSignature)
		: base(IccTypeSignature.MultiLocalizedUnicode, tagSignature)
	{
		Texts = texts ?? throw new ArgumentNullException("texts");
	}

	public override bool Equals(IccTagDataEntry? other)
	{
		if (other is IccMultiLocalizedUnicodeTagDataEntry other2)
		{
			return Equals(other2);
		}
		return false;
	}

	public bool Equals(IccMultiLocalizedUnicodeTagDataEntry? other)
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
			return Texts.AsSpan().SequenceEqual(other.Texts);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is IccMultiLocalizedUnicodeTagDataEntry other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(base.Signature, Texts);
	}
}
