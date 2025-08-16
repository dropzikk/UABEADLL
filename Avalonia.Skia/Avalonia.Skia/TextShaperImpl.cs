using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Globalization;
using System.Runtime.InteropServices;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Avalonia.Media.TextFormatting.Unicode;
using Avalonia.Platform;
using HarfBuzzSharp;

namespace Avalonia.Skia;

internal class TextShaperImpl : ITextShaperImpl
{
	private static readonly ConcurrentDictionary<int, Language> s_cachedLanguage = new ConcurrentDictionary<int, Language>();

	public ShapedBuffer ShapeText(ReadOnlyMemory<char> text, TextShaperOptions options)
	{
		_ = text.Span;
		IGlyphTypeface typeface = options.Typeface;
		double fontRenderingEmSize = options.FontRenderingEmSize;
		sbyte bidiLevel = options.BidiLevel;
		CultureInfo culture = options.Culture;
		using HarfBuzzSharp.Buffer buffer = new HarfBuzzSharp.Buffer();
		int start;
		int length;
		ReadOnlySpan<char> span = GetContainingMemory(text, out start, out length).Span;
		buffer.AddUtf16(span, start, length);
		MergeBreakPair(buffer);
		buffer.GuessSegmentProperties();
		buffer.Direction = (((bidiLevel & 1) == 0) ? Direction.LeftToRight : Direction.RightToLeft);
		CultureInfo usedCulture = culture ?? CultureInfo.CurrentCulture;
		buffer.Language = s_cachedLanguage.GetOrAdd(usedCulture.LCID, (int _) => new Language(usedCulture));
		Font font = ((GlyphTypefaceImpl)typeface).Font;
		font.Shape(buffer);
		if (buffer.Direction == Direction.RightToLeft)
		{
			buffer.Reverse();
		}
		font.GetScale(out var xScale, out var _);
		double num = fontRenderingEmSize / (double)xScale;
		int length2 = buffer.Length;
		ShapedBuffer shapedBuffer = new ShapedBuffer(text, length2, typeface, fontRenderingEmSize, bidiLevel);
		ReadOnlySpan<HarfBuzzSharp.GlyphInfo> glyphInfoSpan = buffer.GetGlyphInfoSpan();
		ReadOnlySpan<GlyphPosition> glyphPositionSpan = buffer.GetGlyphPositionSpan();
		for (int i = 0; i < length2; i++)
		{
			HarfBuzzSharp.GlyphInfo glyphInfo = glyphInfoSpan[i];
			ushort num2 = (ushort)glyphInfo.Codepoint;
			int cluster = (int)glyphInfo.Cluster;
			double glyphAdvance = GetGlyphAdvance(glyphPositionSpan, i, num) + options.LetterSpacing;
			Vector glyphOffset = GetGlyphOffset(glyphPositionSpan, i, num);
			if (cluster < span.Length && span[cluster] == '\t')
			{
				num2 = typeface.GetGlyph(32u);
				glyphAdvance = ((options.IncrementalTabWidth > 0.0) ? options.IncrementalTabWidth : ((double)(4 * typeface.GetGlyphAdvance(num2)) * num));
			}
			shapedBuffer[i] = new Avalonia.Media.TextFormatting.GlyphInfo(num2, cluster, glyphAdvance, glyphOffset);
		}
		return shapedBuffer;
	}

	private unsafe static void MergeBreakPair(HarfBuzzSharp.Buffer buffer)
	{
		int length = buffer.Length;
		ReadOnlySpan<HarfBuzzSharp.GlyphInfo> glyphInfoSpan = buffer.GetGlyphInfoSpan();
		HarfBuzzSharp.GlyphInfo glyphInfo = glyphInfoSpan[length - 1];
		if (!new Codepoint(glyphInfo.Codepoint).IsBreakChar)
		{
			return;
		}
		if (length > 1 && glyphInfoSpan[length - 2].Codepoint == 13 && glyphInfo.Codepoint == 10)
		{
			HarfBuzzSharp.GlyphInfo glyphInfo2 = glyphInfoSpan[length - 2];
			glyphInfo2.Codepoint = 8204u;
			glyphInfo.Codepoint = 8204u;
			glyphInfo.Cluster = glyphInfo2.Cluster;
			fixed (HarfBuzzSharp.GlyphInfo* ptr = &glyphInfoSpan[length - 2])
			{
				*ptr = glyphInfo2;
			}
			fixed (HarfBuzzSharp.GlyphInfo* ptr = &glyphInfoSpan[length - 1])
			{
				*ptr = glyphInfo;
			}
		}
		else
		{
			glyphInfo.Codepoint = 8204u;
			fixed (HarfBuzzSharp.GlyphInfo* ptr = &glyphInfoSpan[length - 1])
			{
				*ptr = glyphInfo;
			}
		}
	}

	private static Vector GetGlyphOffset(ReadOnlySpan<GlyphPosition> glyphPositions, int index, double textScale)
	{
		GlyphPosition glyphPosition = glyphPositions[index];
		double x = (double)glyphPosition.XOffset * textScale;
		double y = (double)glyphPosition.YOffset * textScale;
		return new Vector(x, y);
	}

	private static double GetGlyphAdvance(ReadOnlySpan<GlyphPosition> glyphPositions, int index, double textScale)
	{
		return (double)glyphPositions[index].XAdvance * textScale;
	}

	private static ReadOnlyMemory<char> GetContainingMemory(ReadOnlyMemory<char> memory, out int start, out int length)
	{
		if (MemoryMarshal.TryGetString(memory, out string text, out start, out length))
		{
			return text.AsMemory();
		}
		if (MemoryMarshal.TryGetArray(memory, out var segment))
		{
			start = segment.Offset;
			length = segment.Count;
			return segment.Array.AsMemory();
		}
		if (MemoryMarshal.TryGetMemoryManager<char, MemoryManager<char>>(memory, out MemoryManager<char> manager, out start, out length))
		{
			return manager.Memory;
		}
		throw new InvalidOperationException("Memory not backed by string, array or manager");
	}
}
