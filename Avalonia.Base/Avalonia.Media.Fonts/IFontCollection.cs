using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Avalonia.Platform;

namespace Avalonia.Media.Fonts;

public interface IFontCollection : IReadOnlyList<FontFamily>, IEnumerable<FontFamily>, IEnumerable, IReadOnlyCollection<FontFamily>, IDisposable
{
	Uri Key { get; }

	void Initialize(IFontManagerImpl fontManager);

	bool TryGetGlyphTypeface(string familyName, FontStyle style, FontWeight weight, FontStretch stretch, [NotNullWhen(true)] out IGlyphTypeface? glyphTypeface);

	bool TryMatchCharacter(int codepoint, FontStyle fontStyle, FontWeight fontWeight, FontStretch fontStretch, string? familyName, CultureInfo? culture, out Typeface typeface);
}
