using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using Avalonia.Media;
using Avalonia.Platform;
using SkiaSharp;

namespace Avalonia.Skia;

internal class FontManagerImpl : IFontManagerImpl
{
	private SKFontManager _skFontManager = SKFontManager.Default;

	[ThreadStatic]
	private static string[]? t_languageTagBuffer;

	public string GetDefaultFontFamilyName()
	{
		return SKTypeface.Default.FamilyName;
	}

	public string[] GetInstalledFontFamilyNames(bool checkForUpdates = false)
	{
		if (checkForUpdates)
		{
			_skFontManager = SKFontManager.CreateDefault();
		}
		return _skFontManager.GetFontFamilies();
	}

	public bool TryMatchCharacter(int codepoint, FontStyle fontStyle, FontWeight fontWeight, FontStretch fontStretch, CultureInfo? culture, out Typeface fontKey)
	{
		SKFontStyle style;
		if (fontWeight != FontWeight.Normal)
		{
			if (fontWeight != FontWeight.Bold)
			{
				goto IL_0056;
			}
			if (fontStyle == FontStyle.Normal && fontStretch == FontStretch.Normal)
			{
				style = SKFontStyle.Bold;
			}
			else
			{
				if (fontStyle != FontStyle.Italic || fontStretch != FontStretch.Normal)
				{
					goto IL_0056;
				}
				style = SKFontStyle.BoldItalic;
			}
		}
		else if (fontStyle == FontStyle.Normal && fontStretch == FontStretch.Normal)
		{
			style = SKFontStyle.Normal;
		}
		else
		{
			if (fontStyle != FontStyle.Italic || fontStretch != FontStretch.Normal)
			{
				goto IL_0056;
			}
			style = SKFontStyle.Italic;
		}
		goto IL_0060;
		IL_0056:
		style = new SKFontStyle((SKFontStyleWeight)fontWeight, (SKFontStyleWidth)fontStretch, (SKFontStyleSlant)fontStyle);
		goto IL_0060;
		IL_0060:
		if (culture == null)
		{
			culture = CultureInfo.CurrentUICulture;
		}
		if (t_languageTagBuffer == null)
		{
			t_languageTagBuffer = new string[2];
		}
		t_languageTagBuffer[0] = culture.TwoLetterISOLanguageName;
		t_languageTagBuffer[1] = culture.ThreeLetterISOLanguageName;
		SKTypeface sKTypeface = _skFontManager.MatchCharacter(null, style, t_languageTagBuffer, codepoint);
		if (sKTypeface != null)
		{
			fontKey = new Typeface(sKTypeface.FamilyName, fontStyle, fontWeight, fontStretch);
			return true;
		}
		fontKey = default(Typeface);
		return false;
	}

	public bool TryCreateGlyphTypeface(string familyName, FontStyle style, FontWeight weight, FontStretch stretch, [NotNullWhen(true)] out IGlyphTypeface? glyphTypeface)
	{
		glyphTypeface = null;
		SKFontStyle style2 = new SKFontStyle((SKFontStyleWeight)weight, (SKFontStyleWidth)stretch, (SKFontStyleSlant)style);
		SKTypeface sKTypeface = _skFontManager.MatchFamily(familyName, style2);
		if (sKTypeface == null)
		{
			return false;
		}
		FontSimulations fontSimulations = FontSimulations.None;
		if (weight >= FontWeight.DemiBold && !sKTypeface.IsBold)
		{
			fontSimulations |= FontSimulations.Bold;
		}
		if (style == FontStyle.Italic && !sKTypeface.IsItalic)
		{
			fontSimulations |= FontSimulations.Oblique;
		}
		glyphTypeface = new GlyphTypefaceImpl(sKTypeface, fontSimulations);
		return true;
	}

	public bool TryCreateGlyphTypeface(Stream stream, [NotNullWhen(true)] out IGlyphTypeface? glyphTypeface)
	{
		SKTypeface sKTypeface = SKTypeface.FromStream(stream);
		if (sKTypeface != null)
		{
			glyphTypeface = new GlyphTypefaceImpl(sKTypeface, FontSimulations.None);
			return true;
		}
		glyphTypeface = null;
		return false;
	}
}
