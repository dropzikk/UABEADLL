using System;
using System.Runtime.InteropServices;
using Avalonia.Media;
using HarfBuzzSharp;
using SkiaSharp;

namespace Avalonia.Skia;

internal class GlyphTypefaceImpl : IGlyphTypeface, IDisposable
{
	private bool _isDisposed;

	private readonly SKTypeface _typeface;

	public Face Face { get; }

	public Font Font { get; }

	public SKFont SKFont { get; }

	public FontSimulations FontSimulations { get; }

	public int ReplacementCodepoint { get; }

	public FontMetrics Metrics { get; }

	public int GlyphCount { get; }

	public string FamilyName => _typeface.FamilyName;

	public FontWeight Weight { get; }

	public FontStyle Style { get; }

	public FontStretch Stretch { get; }

	public GlyphTypefaceImpl(SKTypeface typeface, FontSimulations fontSimulations)
	{
		_typeface = typeface ?? throw new ArgumentNullException("typeface");
		SKFont = new SKFont(typeface)
		{
			LinearMetrics = true,
			Embolden = ((fontSimulations & FontSimulations.Bold) != 0),
			SkewX = (((fontSimulations & FontSimulations.Oblique) != 0) ? (-0.2f) : 0f)
		};
		Face = new Face(GetTable)
		{
			UnitsPerEm = typeface.UnitsPerEm
		};
		Font = new Font(Face);
		Font.SetFunctionsOpenType();
		Font.OpenTypeMetrics.TryGetPosition(OpenTypeMetricsTag.HorizontalAscender, out var position);
		Font.OpenTypeMetrics.TryGetPosition(OpenTypeMetricsTag.HorizontalDescender, out var position2);
		Font.OpenTypeMetrics.TryGetPosition(OpenTypeMetricsTag.HorizontalLineGap, out var position3);
		Font.OpenTypeMetrics.TryGetPosition(OpenTypeMetricsTag.StrikeoutOffset, out var position4);
		Font.OpenTypeMetrics.TryGetPosition(OpenTypeMetricsTag.StrikeoutSize, out var position5);
		Font.OpenTypeMetrics.TryGetPosition(OpenTypeMetricsTag.UnderlineOffset, out var position6);
		Font.OpenTypeMetrics.TryGetPosition(OpenTypeMetricsTag.UnderlineSize, out var position7);
		Metrics = new FontMetrics
		{
			DesignEmHeight = (short)Face.UnitsPerEm,
			Ascent = -position,
			Descent = -position2,
			LineGap = position3,
			UnderlinePosition = -position6,
			UnderlineThickness = position7,
			StrikethroughPosition = -position4,
			StrikethroughThickness = position5,
			IsFixedPitch = typeface.IsFixedPitch
		};
		GlyphCount = typeface.GlyphCount;
		FontSimulations = fontSimulations;
		Weight = (FontWeight)typeface.FontWeight;
		Style = typeface.FontSlant.ToAvalonia();
		Stretch = (FontStretch)typeface.FontStyle.Width;
	}

	public bool TryGetGlyphMetrics(ushort glyph, out GlyphMetrics metrics)
	{
		metrics = default(GlyphMetrics);
		if (!Font.TryGetGlyphExtents(glyph, out var extents))
		{
			return false;
		}
		metrics = new GlyphMetrics
		{
			XBearing = extents.XBearing,
			YBearing = extents.YBearing,
			Width = extents.Width,
			Height = extents.Height
		};
		return true;
	}

	public ushort GetGlyph(uint codepoint)
	{
		if (Font.TryGetGlyph(codepoint, out var glyph))
		{
			return (ushort)glyph;
		}
		return 0;
	}

	public bool TryGetGlyph(uint codepoint, out ushort glyph)
	{
		glyph = GetGlyph(codepoint);
		return glyph != 0;
	}

	public ushort[] GetGlyphs(ReadOnlySpan<uint> codepoints)
	{
		ushort[] array = new ushort[codepoints.Length];
		for (int i = 0; i < codepoints.Length; i++)
		{
			if (Font.TryGetGlyph(codepoints[i], out var glyph))
			{
				array[i] = (ushort)glyph;
			}
		}
		return array;
	}

	public int GetGlyphAdvance(ushort glyph)
	{
		return Font.GetHorizontalGlyphAdvance(glyph);
	}

	public int[] GetGlyphAdvances(ReadOnlySpan<ushort> glyphs)
	{
		uint[] array = new uint[glyphs.Length];
		for (int i = 0; i < glyphs.Length; i++)
		{
			array[i] = glyphs[i];
		}
		return Font.GetHorizontalGlyphAdvances(array);
	}

	private Blob? GetTable(Face face, Tag tag)
	{
		int tableSize = _typeface.GetTableSize(tag);
		IntPtr data = Marshal.AllocCoTaskMem(tableSize);
		ReleaseDelegate releaseDelegate = delegate
		{
			Marshal.FreeCoTaskMem(data);
		};
		if (!_typeface.TryGetTableData(tag, 0, tableSize, data))
		{
			return null;
		}
		return new Blob(data, tableSize, MemoryMode.ReadOnly, releaseDelegate);
	}

	private void Dispose(bool disposing)
	{
		if (!_isDisposed)
		{
			_isDisposed = true;
			if (disposing)
			{
				Font.Dispose();
				Face.Dispose();
			}
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public bool TryGetTable(uint tag, out byte[] table)
	{
		return _typeface.TryGetTableData(tag, out table);
	}
}
