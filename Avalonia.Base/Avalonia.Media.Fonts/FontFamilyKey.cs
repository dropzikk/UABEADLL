using System;

namespace Avalonia.Media.Fonts;

public class FontFamilyKey
{
	public Uri Source { get; }

	public Uri? BaseUri { get; }

	public FontFamilyKey(Uri source, Uri? baseUri = null)
	{
		Source = source ?? throw new ArgumentNullException("source");
		BaseUri = baseUri;
	}

	public override int GetHashCode()
	{
		int num = -2128831035;
		num = (num * 16777619) ^ Source.GetHashCode();
		if (BaseUri != null)
		{
			num = (num * 16777619) ^ BaseUri.GetHashCode();
		}
		return num;
	}

	public static bool operator !=(FontFamilyKey? a, FontFamilyKey? b)
	{
		return !(a == b);
	}

	public static bool operator ==(FontFamilyKey? a, FontFamilyKey? b)
	{
		if ((object)a == b)
		{
			return true;
		}
		return a?.Equals(b) ?? false;
	}

	public override bool Equals(object? obj)
	{
		if (!(obj is FontFamilyKey fontFamilyKey))
		{
			return false;
		}
		if (Source != fontFamilyKey.Source)
		{
			return false;
		}
		if (BaseUri != fontFamilyKey.BaseUri)
		{
			return false;
		}
		return true;
	}

	public override string ToString()
	{
		if (!Source.IsAbsoluteUri && BaseUri != null)
		{
			return BaseUri.AbsoluteUri + Source.OriginalString;
		}
		return Source.ToString();
	}
}
