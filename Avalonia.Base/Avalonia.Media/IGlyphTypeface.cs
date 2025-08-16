using System;
using Avalonia.Metadata;

namespace Avalonia.Media;

[Unstable]
public interface IGlyphTypeface : IDisposable
{
	string FamilyName { get; }

	FontWeight Weight { get; }

	FontStyle Style { get; }

	FontStretch Stretch { get; }

	int GlyphCount { get; }

	FontMetrics Metrics { get; }

	FontSimulations FontSimulations { get; }

	bool TryGetGlyphMetrics(ushort glyph, out GlyphMetrics metrics);

	ushort GetGlyph(uint codepoint);

	bool TryGetGlyph(uint codepoint, out ushort glyph);

	ushort[] GetGlyphs(ReadOnlySpan<uint> codepoints);

	int GetGlyphAdvance(ushort glyph);

	int[] GetGlyphAdvances(ReadOnlySpan<ushort> glyphs);

	bool TryGetTable(uint tag, out byte[] table);
}
