using System;
using Avalonia.Media.Fonts;

namespace Avalonia.Media;

public sealed class FontFamily
{
	private struct FontFamilyIdentifier
	{
		public string Name { get; }

		public Uri? Source { get; }

		public FontFamilyIdentifier(string name, Uri? source)
		{
			Name = name;
			Source = source;
		}
	}

	public const string DefaultFontFamilyName = "$Default";

	public static FontFamily Default { get; }

	public string Name => FamilyNames.PrimaryFamilyName;

	public FamilyNameCollection FamilyNames { get; }

	public FontFamilyKey? Key { get; }

	static FontFamily()
	{
		Default = new FontFamily("$Default");
	}

	public FontFamily(string name)
		: this(null, name)
	{
	}

	public FontFamily(Uri? baseUri, string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentNullException("name");
		}
		FontFamilyIdentifier fontFamilyIdentifier = GetFontFamilyIdentifier(name);
		if (fontFamilyIdentifier.Source != null)
		{
			if (baseUri != null && !baseUri.IsAbsoluteUri)
			{
				throw new ArgumentException("Base uri must be an absolute uri.", "baseUri");
			}
			Key = new FontFamilyKey(fontFamilyIdentifier.Source, baseUri);
		}
		FamilyNames = new FamilyNameCollection(fontFamilyIdentifier.Name);
	}

	public static implicit operator FontFamily(string s)
	{
		return new FontFamily(s);
	}

	private static FontFamilyIdentifier GetFontFamilyIdentifier(string name)
	{
		string[] array = name.Split('#');
		switch (array.Length)
		{
		case 1:
			return new FontFamilyIdentifier(array[0], null);
		case 2:
		{
			Uri source = (array[0].StartsWith("/", StringComparison.Ordinal) ? new Uri(array[0], UriKind.Relative) : new Uri(array[0], UriKind.RelativeOrAbsolute));
			return new FontFamilyIdentifier(array[1], source);
		}
		default:
			return new FontFamilyIdentifier(name, null);
		}
	}

	public static FontFamily Parse(string s)
	{
		return Parse(s, null);
	}

	public static FontFamily Parse(string s, Uri? baseUri)
	{
		if (string.IsNullOrEmpty(s))
		{
			throw new ArgumentException("Specified family is not supported.", "s");
		}
		return new FontFamily(baseUri, s);
	}

	public override string ToString()
	{
		if (Key != null)
		{
			return Key?.ToString() + "#" + FamilyNames;
		}
		return FamilyNames.ToString();
	}

	public override int GetHashCode()
	{
		return (FamilyNames.GetHashCode() * 397) ^ (((object)Key != null) ? Key.GetHashCode() : 0);
	}

	public static bool operator !=(FontFamily? a, FontFamily? b)
	{
		return !(a == b);
	}

	public static bool operator ==(FontFamily? a, FontFamily? b)
	{
		if ((object)a == b)
		{
			return true;
		}
		return a?.Equals(b) ?? false;
	}

	public override bool Equals(object? obj)
	{
		if (this == obj)
		{
			return true;
		}
		if (!(obj is FontFamily fontFamily))
		{
			return false;
		}
		if (!object.Equals(Key, fontFamily.Key))
		{
			return false;
		}
		return fontFamily.FamilyNames.Equals(FamilyNames);
	}
}
