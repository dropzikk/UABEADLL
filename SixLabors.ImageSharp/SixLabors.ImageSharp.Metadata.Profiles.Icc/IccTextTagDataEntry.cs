using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccTextTagDataEntry : IccTagDataEntry, IEquatable<IccTextTagDataEntry>
{
	public string Text { get; }

	public IccTextTagDataEntry(string text)
		: this(text, IccProfileTag.Unknown)
	{
	}

	public IccTextTagDataEntry(string text, IccProfileTag tagSignature)
		: base(IccTypeSignature.Text, tagSignature)
	{
		Text = text ?? throw new ArgumentNullException("text");
	}

	public override bool Equals(IccTagDataEntry? other)
	{
		if (other is IccTextTagDataEntry other2)
		{
			return Equals(other2);
		}
		return false;
	}

	public bool Equals(IccTextTagDataEntry? other)
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
			return string.Equals(Text, other.Text, StringComparison.OrdinalIgnoreCase);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is IccTextTagDataEntry other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(base.Signature, Text);
	}
}
