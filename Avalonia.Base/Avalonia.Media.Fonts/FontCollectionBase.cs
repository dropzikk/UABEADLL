using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Avalonia.Platform;

namespace Avalonia.Media.Fonts;

public abstract class FontCollectionBase : IFontCollection, IReadOnlyList<FontFamily>, IEnumerable<FontFamily>, IEnumerable, IReadOnlyCollection<FontFamily>, IDisposable
{
	protected readonly ConcurrentDictionary<string, ConcurrentDictionary<FontCollectionKey, IGlyphTypeface?>> _glyphTypefaceCache = new ConcurrentDictionary<string, ConcurrentDictionary<FontCollectionKey, IGlyphTypeface>>();

	public abstract Uri Key { get; }

	public abstract int Count { get; }

	public abstract FontFamily this[int index] { get; }

	public abstract bool TryGetGlyphTypeface(string familyName, FontStyle style, FontWeight weight, FontStretch stretch, [NotNullWhen(true)] out IGlyphTypeface? glyphTypeface);

	public bool TryMatchCharacter(int codepoint, FontStyle style, FontWeight weight, FontStretch stretch, string? familyName, CultureInfo? culture, out Typeface match)
	{
		match = default(Typeface);
		ushort glyph;
		IGlyphTypeface glyphTypeface2;
		if (string.IsNullOrEmpty(familyName))
		{
			foreach (ConcurrentDictionary<FontCollectionKey, IGlyphTypeface> value in _glyphTypefaceCache.Values)
			{
				if (TryGetNearestMatch(value, new FontCollectionKey
				{
					Style = style,
					Weight = weight,
					Stretch = stretch
				}, out IGlyphTypeface glyphTypeface) && glyphTypeface.TryGetGlyph((uint)codepoint, out glyph))
				{
					match = new Typeface(glyphTypeface.FamilyName, style, weight, stretch);
					return true;
				}
			}
		}
		else if (TryGetGlyphTypeface(familyName, style, weight, stretch, out glyphTypeface2) && glyphTypeface2.TryGetGlyph((uint)codepoint, out glyph))
		{
			match = new Typeface(familyName, style, weight, stretch);
			return true;
		}
		return false;
	}

	public abstract void Initialize(IFontManagerImpl fontManager);

	public abstract IEnumerator<FontFamily> GetEnumerator();

	void IDisposable.Dispose()
	{
		foreach (ConcurrentDictionary<FontCollectionKey, IGlyphTypeface> value in _glyphTypefaceCache.Values)
		{
			foreach (KeyValuePair<FontCollectionKey, IGlyphTypeface> item in value)
			{
				item.Value?.Dispose();
			}
		}
		GC.SuppressFinalize(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	internal static bool TryGetNearestMatch(ConcurrentDictionary<FontCollectionKey, IGlyphTypeface?> glyphTypefaces, FontCollectionKey key, [NotNullWhen(true)] out IGlyphTypeface? glyphTypeface)
	{
		if (glyphTypefaces.TryGetValue(key, out glyphTypeface) && glyphTypeface != null)
		{
			return true;
		}
		if (key.Style != 0)
		{
			key = key with
			{
				Style = FontStyle.Normal
			};
		}
		if (key.Stretch != FontStretch.Normal)
		{
			if (TryFindStretchFallback(glyphTypefaces, key, out glyphTypeface))
			{
				return true;
			}
			if (key.Weight != FontWeight.Normal && TryFindStretchFallback(glyphTypefaces, key with
			{
				Weight = FontWeight.Normal
			}, out glyphTypeface))
			{
				return true;
			}
			key = key with
			{
				Stretch = FontStretch.Normal
			};
		}
		if (TryFindWeightFallback(glyphTypefaces, key, out glyphTypeface))
		{
			return true;
		}
		if (TryFindStretchFallback(glyphTypefaces, key, out glyphTypeface))
		{
			return true;
		}
		foreach (IGlyphTypeface value in glyphTypefaces.Values)
		{
			if (value != null)
			{
				glyphTypeface = value;
				return true;
			}
		}
		return false;
	}

	internal static bool TryFindStretchFallback(ConcurrentDictionary<FontCollectionKey, IGlyphTypeface?> glyphTypefaces, FontCollectionKey key, [NotNullWhen(true)] out IGlyphTypeface? glyphTypeface)
	{
		glyphTypeface = null;
		int stretch = (int)key.Stretch;
		if (stretch < 5)
		{
			for (int i = 0; stretch + i < 9; i++)
			{
				if (glyphTypefaces.TryGetValue(key with
				{
					Stretch = (FontStretch)(stretch + i)
				}, out glyphTypeface) && glyphTypeface != null)
				{
					return true;
				}
			}
		}
		else
		{
			for (int j = 0; stretch - j > 1; j++)
			{
				if (glyphTypefaces.TryGetValue(key with
				{
					Stretch = (FontStretch)(stretch - j)
				}, out glyphTypeface) && glyphTypeface != null)
				{
					return true;
				}
			}
		}
		return false;
	}

	internal static bool TryFindWeightFallback(ConcurrentDictionary<FontCollectionKey, IGlyphTypeface?> glyphTypefaces, FontCollectionKey key, [NotNullWhen(true)] out IGlyphTypeface? glyphTypeface)
	{
		glyphTypeface = null;
		int weight = (int)key.Weight;
		if (weight >= 400 && weight <= 500)
		{
			for (int i = 0; weight + i <= 500; i += 50)
			{
				if (glyphTypefaces.TryGetValue(key with
				{
					Weight = (FontWeight)(weight + i)
				}, out glyphTypeface) && glyphTypeface != null)
				{
					return true;
				}
			}
			for (int j = 0; weight - j >= 100; j += 50)
			{
				if (glyphTypefaces.TryGetValue(key with
				{
					Weight = (FontWeight)(weight - j)
				}, out glyphTypeface) && glyphTypeface != null)
				{
					return true;
				}
			}
			for (int k = 0; weight + k <= 900; k += 50)
			{
				if (glyphTypefaces.TryGetValue(key with
				{
					Weight = (FontWeight)(weight + k)
				}, out glyphTypeface) && glyphTypeface != null)
				{
					return true;
				}
			}
		}
		if (weight < 400)
		{
			for (int l = 0; weight - l >= 100; l += 50)
			{
				if (glyphTypefaces.TryGetValue(key with
				{
					Weight = (FontWeight)(weight - l)
				}, out glyphTypeface) && glyphTypeface != null)
				{
					return true;
				}
			}
			for (int m = 0; weight + m <= 900; m += 50)
			{
				if (glyphTypefaces.TryGetValue(key with
				{
					Weight = (FontWeight)(weight + m)
				}, out glyphTypeface) && glyphTypeface != null)
				{
					return true;
				}
			}
		}
		if (weight > 500)
		{
			for (int n = 0; weight + n <= 900; n += 50)
			{
				if (glyphTypefaces.TryGetValue(key with
				{
					Weight = (FontWeight)(weight + n)
				}, out glyphTypeface) && glyphTypeface != null)
				{
					return true;
				}
			}
			for (int num = 0; weight - num >= 100; num += 50)
			{
				if (glyphTypefaces.TryGetValue(key with
				{
					Weight = (FontWeight)(weight - num)
				}, out glyphTypeface) && glyphTypeface != null)
				{
					return true;
				}
			}
		}
		return false;
	}
}
