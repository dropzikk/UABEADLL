using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using Avalonia.Platform;

namespace Avalonia.Media.Fonts;

public class EmbeddedFontCollection : FontCollectionBase
{
	private readonly List<FontFamily> _fontFamilies = new List<FontFamily>(1);

	private readonly Uri _key;

	private readonly Uri _source;

	public override Uri Key => _key;

	public override FontFamily this[int index] => _fontFamilies[index];

	public override int Count => _fontFamilies.Count;

	public EmbeddedFontCollection(Uri key, Uri source)
	{
		_key = key;
		_source = source;
	}

	public override void Initialize(IFontManagerImpl fontManager)
	{
		IAssetLoader requiredService = AvaloniaLocator.Current.GetRequiredService<IAssetLoader>();
		foreach (Uri item in FontFamilyLoader.LoadFontAssets(_source))
		{
			Stream stream = requiredService.Open(item);
			if (!fontManager.TryCreateGlyphTypeface(stream, out IGlyphTypeface glyphTypeface))
			{
				continue;
			}
			if (!_glyphTypefaceCache.TryGetValue(glyphTypeface.FamilyName, out ConcurrentDictionary<FontCollectionKey, IGlyphTypeface> value))
			{
				value = new ConcurrentDictionary<FontCollectionKey, IGlyphTypeface>();
				if (_glyphTypefaceCache.TryAdd(glyphTypeface.FamilyName, value))
				{
					_fontFamilies.Add(new FontFamily(_key, glyphTypeface.FamilyName));
				}
			}
			FontCollectionKey key = new FontCollectionKey(glyphTypeface.Style, glyphTypeface.Weight, glyphTypeface.Stretch);
			value.TryAdd(key, glyphTypeface);
		}
	}

	public override bool TryGetGlyphTypeface(string familyName, FontStyle style, FontWeight weight, FontStretch stretch, [NotNullWhen(true)] out IGlyphTypeface? glyphTypeface)
	{
		FontCollectionKey key = new FontCollectionKey(style, weight, stretch);
		if (_glyphTypefaceCache.TryGetValue(familyName, out ConcurrentDictionary<FontCollectionKey, IGlyphTypeface> value) && FontCollectionBase.TryGetNearestMatch(value, key, out glyphTypeface))
		{
			return true;
		}
		for (int i = 0; i < Count; i++)
		{
			FontFamily fontFamily = _fontFamilies[i];
			if (fontFamily.Name.ToLower(CultureInfo.InvariantCulture).StartsWith(familyName.ToLower(CultureInfo.InvariantCulture)) && _glyphTypefaceCache.TryGetValue(fontFamily.Name, out value) && FontCollectionBase.TryGetNearestMatch(value, key, out glyphTypeface))
			{
				return true;
			}
		}
		glyphTypeface = null;
		return false;
	}

	public override IEnumerator<FontFamily> GetEnumerator()
	{
		return _fontFamilies.GetEnumerator();
	}
}
