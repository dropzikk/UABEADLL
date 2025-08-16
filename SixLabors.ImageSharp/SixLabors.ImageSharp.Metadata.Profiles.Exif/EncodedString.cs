using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

public readonly struct EncodedString : IEquatable<EncodedString>
{
	public enum CharacterCode
	{
		ASCII,
		JIS,
		Unicode,
		Undefined
	}

	public CharacterCode Code { get; }

	public string Text { get; }

	public EncodedString(string text)
		: this(CharacterCode.Unicode, text)
	{
	}

	public EncodedString(CharacterCode code, string text)
	{
		Text = text;
		Code = code;
	}

	public static implicit operator EncodedString(string text)
	{
		return new EncodedString(text);
	}

	public static explicit operator string(EncodedString encodedString)
	{
		return encodedString.Text;
	}

	public static bool operator ==(EncodedString left, EncodedString right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(EncodedString left, EncodedString right)
	{
		return !(left == right);
	}

	public override bool Equals(object? obj)
	{
		if (obj is EncodedString other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(EncodedString other)
	{
		if (Text == other.Text)
		{
			return Code == other.Code;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Text, Code);
	}

	public override string ToString()
	{
		return Text;
	}
}
