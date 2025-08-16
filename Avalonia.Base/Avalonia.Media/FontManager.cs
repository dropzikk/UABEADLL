using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Avalonia.Media.Fonts;
using Avalonia.Platform;
using Avalonia.Utilities;

namespace Avalonia.Media;

public sealed class FontManager
{
	internal static Uri SystemFontsKey = new Uri("fonts:SystemFonts");

	public const string FontCollectionScheme = "fonts";

	private readonly ConcurrentDictionary<Uri, IFontCollection> _fontCollections = new ConcurrentDictionary<Uri, IFontCollection>();

	private readonly IReadOnlyList<FontFallback>? _fontFallbacks;

	public static FontManager Current
	{
		get
		{
			FontManager service = AvaloniaLocator.Current.GetService<FontManager>();
			if (service != null)
			{
				return service;
			}
			service = new FontManager(AvaloniaLocator.Current.GetRequiredService<IFontManagerImpl>());
			AvaloniaLocator.CurrentMutable.Bind<FontManager>().ToConstant(service);
			return service;
		}
	}

	public FontFamily DefaultFontFamily { get; }

	public IFontCollection SystemFonts => _fontCollections[SystemFontsKey];

	internal IFontManagerImpl PlatformImpl { get; }

	public FontManager(IFontManagerImpl platformImpl)
	{
		PlatformImpl = platformImpl;
		FontManagerOptions service = AvaloniaLocator.Current.GetService<FontManagerOptions>();
		_fontFallbacks = service?.FontFallbacks;
		string text = service?.DefaultFamilyName ?? PlatformImpl.GetDefaultFontFamilyName();
		if (string.IsNullOrEmpty(text))
		{
			throw new InvalidOperationException("Default font family name can't be null or empty.");
		}
		DefaultFontFamily = new FontFamily(text);
		AddFontCollection(new SystemFontCollection(this));
	}

	public bool TryGetGlyphTypeface(Typeface typeface, [NotNullWhen(true)] out IGlyphTypeface? glyphTypeface)
	{
		glyphTypeface = null;
		FontFamily fontFamily = typeface.FontFamily;
		if (typeface.FontFamily.Name == "$Default")
		{
			return TryGetGlyphTypeface(new Typeface(DefaultFontFamily, typeface.Style, typeface.Weight, typeface.Stretch), out glyphTypeface);
		}
		FontFamilyKey key = fontFamily.Key;
		if ((object)key != null)
		{
			Uri uri = key.Source;
			if (!uri.IsAbsoluteUri)
			{
				if (key.BaseUri == null)
				{
					throw new NotSupportedException("BaseUri can't be null.");
				}
				uri = new Uri(key.BaseUri, uri);
			}
			if (!_fontCollections.TryGetValue(uri, out IFontCollection value) && (uri.IsAbsoluteResm() || uri.IsAvares()))
			{
				EmbeddedFontCollection embeddedFontCollection = new EmbeddedFontCollection(uri, uri);
				embeddedFontCollection.Initialize(PlatformImpl);
				if (embeddedFontCollection.Count > 0 && _fontCollections.TryAdd(uri, embeddedFontCollection))
				{
					value = embeddedFontCollection;
				}
			}
			if (value != null && value.TryGetGlyphTypeface(fontFamily.FamilyNames.PrimaryFamilyName, typeface.Style, typeface.Weight, typeface.Stretch, out glyphTypeface))
			{
				return true;
			}
			if (!fontFamily.FamilyNames.HasFallbacks)
			{
				return false;
			}
		}
		for (int i = 0; i < fontFamily.FamilyNames.Count; i++)
		{
			string familyName = fontFamily.FamilyNames[i];
			if (SystemFonts.TryGetGlyphTypeface(familyName, typeface.Style, typeface.Weight, typeface.Stretch, out glyphTypeface) && (!fontFamily.FamilyNames.HasFallbacks || glyphTypeface.FamilyName != DefaultFontFamily.Name))
			{
				return true;
			}
		}
		if (typeface.FontFamily == DefaultFontFamily)
		{
			return false;
		}
		return TryGetGlyphTypeface(new Typeface("$Default", typeface.Style, typeface.Weight, typeface.Stretch), out glyphTypeface);
	}

	public void AddFontCollection(IFontCollection fontCollection)
	{
		Uri key = fontCollection.Key;
		if (!fontCollection.Key.IsFontCollection())
		{
			throw new ArgumentException("Font collection Key should follow the fonts: scheme.", "fontCollection");
		}
		_fontCollections.AddOrUpdate(key, fontCollection, delegate(Uri _, IFontCollection oldCollection)
		{
			oldCollection.Dispose();
			return fontCollection;
		});
		fontCollection.Initialize(PlatformImpl);
	}

	public void RemoveFontCollection(Uri key)
	{
		if (_fontCollections.TryRemove(key, out IFontCollection value))
		{
			value.Dispose();
		}
	}

	public bool TryMatchCharacter(int codepoint, FontStyle fontStyle, FontWeight fontWeight, FontStretch fontStretch, FontFamily? fontFamily, CultureInfo? culture, out Typeface typeface)
	{
		if (_fontFallbacks != null)
		{
			foreach (FontFallback fontFallback in _fontFallbacks)
			{
				if (fontFallback.UnicodeRange.IsInRange(codepoint))
				{
					typeface = new Typeface(fontFallback.FontFamily, fontStyle, fontWeight, fontStretch);
					if (TryGetGlyphTypeface(typeface, out IGlyphTypeface glyphTypeface) && glyphTypeface.TryGetGlyph((uint)codepoint, out var _))
					{
						return true;
					}
				}
			}
		}
		if (fontFamily != null && fontFamily.FamilyNames.HasFallbacks)
		{
			for (int i = 1; i < fontFamily.FamilyNames.Count; i++)
			{
				string familyName = fontFamily.FamilyNames[i];
				foreach (IFontCollection value in _fontCollections.Values)
				{
					if (value.TryMatchCharacter(codepoint, fontStyle, fontWeight, fontStretch, familyName, culture, out typeface))
					{
						return true;
					}
				}
			}
		}
		return PlatformImpl.TryMatchCharacter(codepoint, fontStyle, fontWeight, fontStretch, culture, out typeface);
	}
}
