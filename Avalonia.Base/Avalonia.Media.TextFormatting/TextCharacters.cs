using System;
using Avalonia.Media.TextFormatting.Unicode;

namespace Avalonia.Media.TextFormatting;

public class TextCharacters : TextRun
{
	public override int Length => Text.Length;

	public override ReadOnlyMemory<char> Text { get; }

	public override TextRunProperties Properties { get; }

	public TextCharacters(string text, TextRunProperties textRunProperties)
		: this(text.AsMemory(), textRunProperties)
	{
	}

	public TextCharacters(ReadOnlyMemory<char> text, TextRunProperties textRunProperties)
	{
		if (textRunProperties.FontRenderingEmSize <= 0.0)
		{
			throw new ArgumentOutOfRangeException("textRunProperties", textRunProperties.FontRenderingEmSize, "Invalid FontRenderingEmSize");
		}
		Text = text;
		Properties = textRunProperties;
	}

	internal void GetShapeableCharacters(ReadOnlyMemory<char> text, sbyte biDiLevel, FontManager fontManager, ref TextRunProperties? previousProperties, FormattingObjectPool.RentedList<TextRun> results)
	{
		TextRunProperties properties = Properties;
		while (!text.IsEmpty)
		{
			UnshapedTextRun unshapedTextRun = CreateShapeableRun(text, properties, biDiLevel, fontManager, ref previousProperties);
			results.Add(unshapedTextRun);
			text = text.Slice(unshapedTextRun.Length);
			previousProperties = unshapedTextRun.Properties;
		}
	}

	private static UnshapedTextRun CreateShapeableRun(ReadOnlyMemory<char> text, TextRunProperties defaultProperties, sbyte biDiLevel, FontManager fontManager, ref TextRunProperties? previousProperties)
	{
		Typeface typeface = defaultProperties.Typeface;
		IGlyphTypeface cachedGlyphTypeface = defaultProperties.CachedGlyphTypeface;
		Typeface? typeface2 = previousProperties?.Typeface;
		IGlyphTypeface glyphTypeface = previousProperties?.CachedGlyphTypeface;
		ReadOnlySpan<char> span = text.Span;
		if (TryGetShapeableLength(span, cachedGlyphTypeface, null, out var length))
		{
			return new UnshapedTextRun(text.Slice(0, length), defaultProperties.WithTypeface(typeface), biDiLevel);
		}
		if (glyphTypeface != null && TryGetShapeableLength(span, glyphTypeface, cachedGlyphTypeface, out length))
		{
			return new UnshapedTextRun(text.Slice(0, length), defaultProperties.WithTypeface(typeface2.Value), biDiLevel);
		}
		Codepoint codepoint = Codepoint.ReplacementCodepoint;
		CodepointEnumerator codepointEnumerator = new CodepointEnumerator(text.Slice(length).Span);
		Codepoint codepoint2;
		while (codepointEnumerator.MoveNext(out codepoint2))
		{
			if (!codepoint2.IsWhiteSpace)
			{
				codepoint = codepoint2;
				break;
			}
		}
		if (fontManager.TryMatchCharacter(codepoint, typeface.Style, typeface.Weight, typeface.Stretch, typeface.FontFamily, defaultProperties.CultureInfo, out var typeface3) && fontManager.TryGetGlyphTypeface(typeface3, out IGlyphTypeface glyphTypeface2) && TryGetShapeableLength(span, glyphTypeface2, cachedGlyphTypeface, out length))
		{
			return new UnshapedTextRun(text.Slice(0, length), defaultProperties.WithTypeface(typeface3), biDiLevel);
		}
		GraphemeEnumerator graphemeEnumerator = new GraphemeEnumerator(span);
		Grapheme grapheme;
		ushort glyph;
		while (graphemeEnumerator.MoveNext(out grapheme) && (grapheme.FirstCodepoint.IsWhiteSpace || !cachedGlyphTypeface.TryGetGlyph(grapheme.FirstCodepoint, out glyph)))
		{
			length += grapheme.Length;
		}
		return new UnshapedTextRun(text.Slice(0, length), defaultProperties, biDiLevel);
	}

	internal static bool TryGetShapeableLength(ReadOnlySpan<char> text, IGlyphTypeface glyphTypeface, IGlyphTypeface? defaultGlyphTypeface, out int length)
	{
		length = 0;
		Script script = Script.Unknown;
		if (text.IsEmpty)
		{
			return false;
		}
		GraphemeEnumerator graphemeEnumerator = new GraphemeEnumerator(text);
		Grapheme grapheme;
		while (graphemeEnumerator.MoveNext(out grapheme))
		{
			Codepoint firstCodepoint = grapheme.FirstCodepoint;
			Script script2 = firstCodepoint.Script;
			if ((!firstCodepoint.IsWhiteSpace && defaultGlyphTypeface != null && defaultGlyphTypeface.TryGetGlyph(firstCodepoint, out var glyph)) || (!firstCodepoint.IsBreakChar && firstCodepoint.GeneralCategory != GeneralCategory.Control && !glyphTypeface.TryGetGlyph(firstCodepoint, out glyph)))
			{
				break;
			}
			if (script2 != script)
			{
				if (script == Script.Unknown || (script2 != Script.Common && (script == Script.Common || script == Script.Inherited)))
				{
					script = script2;
				}
				else if (script2 != Script.Inherited && script2 != Script.Common)
				{
					break;
				}
			}
			length += grapheme.Length;
		}
		return length > 0;
	}
}
