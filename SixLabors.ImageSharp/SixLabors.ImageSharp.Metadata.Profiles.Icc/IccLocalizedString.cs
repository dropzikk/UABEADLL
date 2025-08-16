using System;
using System.Globalization;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal readonly struct IccLocalizedString : IEquatable<IccLocalizedString>
{
	public string Text { get; }

	public CultureInfo Culture { get; }

	public IccLocalizedString(string text)
		: this(CultureInfo.CurrentCulture, text)
	{
	}

	public IccLocalizedString(CultureInfo culture, string text)
	{
		Culture = culture ?? throw new ArgumentNullException("culture");
		Text = text ?? throw new ArgumentNullException("text");
	}

	public bool Equals(IccLocalizedString other)
	{
		if (Culture.Equals(other.Culture))
		{
			return Text == other.Text;
		}
		return false;
	}

	public override string ToString()
	{
		return Culture.Name + ": " + Text;
	}

	public override bool Equals(object? obj)
	{
		if (obj is IccLocalizedString other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Culture, Text);
	}
}
