using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using Avalonia.Media;
using Avalonia.Metadata;

namespace Avalonia.Platform;

[Unstable]
public interface IFontManagerImpl
{
	string GetDefaultFontFamilyName();

	string[] GetInstalledFontFamilyNames(bool checkForUpdates = false);

	bool TryMatchCharacter(int codepoint, FontStyle fontStyle, FontWeight fontWeight, FontStretch fontStretch, CultureInfo? culture, out Typeface typeface);

	bool TryCreateGlyphTypeface(string familyName, FontStyle style, FontWeight weight, FontStretch stretch, [NotNullWhen(true)] out IGlyphTypeface? glyphTypeface);

	bool TryCreateGlyphTypeface(Stream stream, [NotNullWhen(true)] out IGlyphTypeface? glyphTypeface);
}
