using System;
using System.Globalization;

namespace Avalonia.Media.TextFormatting;

public abstract class TextRunProperties : IEquatable<TextRunProperties>
{
	private IGlyphTypeface? _cachedGlyphTypeFace;

	public abstract Typeface Typeface { get; }

	public abstract double FontRenderingEmSize { get; }

	public abstract TextDecorationCollection? TextDecorations { get; }

	public abstract IBrush? ForegroundBrush { get; }

	public abstract IBrush? BackgroundBrush { get; }

	public abstract CultureInfo? CultureInfo { get; }

	public virtual BaselineAlignment BaselineAlignment => BaselineAlignment.Baseline;

	internal IGlyphTypeface CachedGlyphTypeface => _cachedGlyphTypeFace ?? (_cachedGlyphTypeFace = Typeface.GlyphTypeface);

	public bool Equals(TextRunProperties? other)
	{
		if ((object)other == null)
		{
			return false;
		}
		if ((object)this == other)
		{
			return true;
		}
		if (Typeface.Equals(other.Typeface) && FontRenderingEmSize.Equals(other.FontRenderingEmSize) && object.Equals(TextDecorations, other.TextDecorations) && object.Equals(ForegroundBrush, other.ForegroundBrush) && object.Equals(BackgroundBrush, other.BackgroundBrush))
		{
			return object.Equals(CultureInfo, other.CultureInfo);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (this != obj)
		{
			if (obj is TextRunProperties other)
			{
				return Equals(other);
			}
			return false;
		}
		return true;
	}

	public override int GetHashCode()
	{
		return (((((((((Typeface.GetHashCode() * 397) ^ FontRenderingEmSize.GetHashCode()) * 397) ^ ((TextDecorations != null) ? TextDecorations.GetHashCode() : 0)) * 397) ^ ((ForegroundBrush != null) ? ForegroundBrush.GetHashCode() : 0)) * 397) ^ ((BackgroundBrush != null) ? BackgroundBrush.GetHashCode() : 0)) * 397) ^ ((CultureInfo != null) ? CultureInfo.GetHashCode() : 0);
	}

	public static bool operator ==(TextRunProperties left, TextRunProperties right)
	{
		return object.Equals(left, right);
	}

	public static bool operator !=(TextRunProperties left, TextRunProperties right)
	{
		return !object.Equals(left, right);
	}

	internal TextRunProperties WithTypeface(Typeface typeface)
	{
		if (this is GenericTextRunProperties genericTextRunProperties && genericTextRunProperties.Typeface == typeface)
		{
			return this;
		}
		return new GenericTextRunProperties(typeface, FontRenderingEmSize, TextDecorations, ForegroundBrush, BackgroundBrush, BaselineAlignment);
	}
}
