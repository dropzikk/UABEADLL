using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Platform;

namespace Avalonia.Media.Fonts;

internal class SystemFontCollection : FontCollectionBase
{
	private readonly FontManager _fontManager;

	private readonly string[] _familyNames;

	public override Uri Key => FontManager.SystemFontsKey;

	public override FontFamily this[int index] => new FontFamily(_familyNames[index]);

	public override int Count => _familyNames.Length;

	public SystemFontCollection(FontManager fontManager)
	{
		_fontManager = fontManager;
		_familyNames = fontManager.PlatformImpl.GetInstalledFontFamilyNames();
	}

	public override bool TryGetGlyphTypeface(string familyName, FontStyle style, FontWeight weight, FontStretch stretch, [NotNullWhen(true)] out IGlyphTypeface? glyphTypeface)
	{
		glyphTypeface = null;
		FontCollectionKey key2 = new FontCollectionKey(style, weight, stretch);
		ConcurrentDictionary<FontCollectionKey, IGlyphTypeface> orAdd = _glyphTypefaceCache.GetOrAdd(familyName, (string key) => new ConcurrentDictionary<FontCollectionKey, IGlyphTypeface>());
		if (!orAdd.TryGetValue(key2, out glyphTypeface))
		{
			_fontManager.PlatformImpl.TryCreateGlyphTypeface(familyName, style, weight, stretch, out glyphTypeface);
			if (!orAdd.TryAdd(key2, glyphTypeface))
			{
				return false;
			}
		}
		return glyphTypeface != null;
	}

	public override void Initialize(IFontManagerImpl fontManager)
	{
	}

	public override IEnumerator<FontFamily> GetEnumerator()
	{
		string[] familyNames = _familyNames;
		foreach (string name in familyNames)
		{
			yield return new FontFamily(name);
		}
	}
}
