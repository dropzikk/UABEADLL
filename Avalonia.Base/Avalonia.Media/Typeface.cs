using System;
using System.Diagnostics;

namespace Avalonia.Media;

[DebuggerDisplay("Name = {FontFamily.Name}, Weight = {Weight}, Style = {Style}")]
public readonly struct Typeface : IEquatable<Typeface>
{
	public static Typeface Default { get; } = new Typeface(Avalonia.Media.FontFamily.Default);

	public FontFamily FontFamily { get; }

	public FontStyle Style { get; }

	public FontWeight Weight { get; }

	public FontStretch Stretch { get; }

	public IGlyphTypeface GlyphTypeface
	{
		get
		{
			if (FontManager.Current.TryGetGlyphTypeface(this, out IGlyphTypeface glyphTypeface))
			{
				return glyphTypeface;
			}
			throw new InvalidOperationException("Could not create glyphTypeface.");
		}
	}

	public Typeface(FontFamily fontFamily, FontStyle style = FontStyle.Normal, FontWeight weight = FontWeight.Normal, FontStretch stretch = FontStretch.Normal)
	{
		if (weight <= (FontWeight)0)
		{
			throw new ArgumentException("Font weight must be > 0.");
		}
		if (stretch < FontStretch.UltraCondensed)
		{
			throw new ArgumentException("Font stretch must be > 1.");
		}
		FontFamily = fontFamily;
		Style = style;
		Weight = weight;
		Stretch = stretch;
	}

	public Typeface(string fontFamilyName, FontStyle style = FontStyle.Normal, FontWeight weight = FontWeight.Normal, FontStretch stretch = FontStretch.Normal)
		: this(new FontFamily(fontFamilyName), style, weight, stretch)
	{
	}

	public static bool operator !=(Typeface a, Typeface b)
	{
		return !(a == b);
	}

	public static bool operator ==(Typeface a, Typeface b)
	{
		return a.Equals(b);
	}

	public override bool Equals(object? obj)
	{
		if (obj is Typeface other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(Typeface other)
	{
		if (FontFamily == other.FontFamily && Style == other.Style && Weight == other.Weight)
		{
			return Stretch == other.Stretch;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (int)(((((uint)(((FontFamily != null) ? FontFamily.GetHashCode() : 0) * 397) ^ (uint)Style) * 397) ^ (uint)Weight) * 397) ^ (int)Stretch;
	}
}
