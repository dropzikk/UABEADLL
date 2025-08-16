using System;

namespace SixLabors.ImageSharp.Formats.Png.Chunks;

public readonly struct PngTextData : IEquatable<PngTextData>
{
	public string Keyword { get; }

	public string Value { get; }

	public string LanguageTag { get; }

	public string TranslatedKeyword { get; }

	public PngTextData(string keyword, string value, string languageTag, string translatedKeyword)
	{
		Guard.NotNullOrWhiteSpace(keyword, "keyword");
		Keyword = keyword.Trim();
		Value = value;
		LanguageTag = languageTag;
		TranslatedKeyword = translatedKeyword;
	}

	public static bool operator ==(PngTextData left, PngTextData right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(PngTextData left, PngTextData right)
	{
		return !(left == right);
	}

	public override bool Equals(object? obj)
	{
		if (obj is PngTextData other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Keyword, Value, LanguageTag, TranslatedKeyword);
	}

	public override string ToString()
	{
		return $"PngTextData [ Name={Keyword}, Value={Value} ]";
	}

	public bool Equals(PngTextData other)
	{
		if (Keyword.Equals(other.Keyword, StringComparison.OrdinalIgnoreCase) && Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase) && LanguageTag.Equals(other.LanguageTag, StringComparison.OrdinalIgnoreCase))
		{
			return TranslatedKeyword.Equals(other.TranslatedKeyword, StringComparison.OrdinalIgnoreCase);
		}
		return false;
	}
}
