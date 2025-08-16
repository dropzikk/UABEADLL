using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Avalonia.Platform;
using SkiaSharp;

namespace Avalonia.Skia;

internal class GlyphRunImpl : IGlyphRunImpl, IDisposable
{
	private readonly GlyphTypefaceImpl _glyphTypefaceImpl;

	private readonly ushort[] _glyphIndices;

	private readonly SKPoint[] _glyphPositions;

	private readonly ConcurrentDictionary<SKFontEdging, SKTextBlob> _textBlobCache = new ConcurrentDictionary<SKFontEdging, SKTextBlob>();

	public IGlyphTypeface GlyphTypeface => _glyphTypefaceImpl;

	public double FontRenderingEmSize { get; }

	public Point BaselineOrigin { get; }

	public Rect Bounds { get; }

	public GlyphRunImpl(IGlyphTypeface glyphTypeface, double fontRenderingEmSize, IReadOnlyList<GlyphInfo> glyphInfos, Point baselineOrigin)
	{
		if (glyphTypeface == null)
		{
			throw new ArgumentNullException("glyphTypeface");
		}
		_glyphTypefaceImpl = (GlyphTypefaceImpl)glyphTypeface;
		if (glyphInfos == null)
		{
			throw new ArgumentNullException("glyphInfos");
		}
		int count = glyphInfos.Count;
		_glyphIndices = new ushort[count];
		_glyphPositions = new SKPoint[count];
		double num = 0.0;
		for (int i = 0; i < count; i++)
		{
			GlyphInfo glyphInfo = glyphInfos[i];
			Vector glyphOffset = glyphInfo.GlyphOffset;
			_glyphIndices[i] = glyphInfo.GlyphIndex;
			_glyphPositions[i] = new SKPoint((float)(num + glyphOffset.X), (float)glyphOffset.Y);
			num += glyphInfos[i].GlyphAdvance;
		}
		_glyphTypefaceImpl.SKFont.Size = (float)fontRenderingEmSize;
		Rect rect = default(Rect);
		SKRect[] array = ArrayPool<SKRect>.Shared.Rent(glyphInfos.Count);
		_glyphTypefaceImpl.SKFont.GetGlyphWidths(_glyphIndices, null, array);
		num = 0.0;
		for (int j = 0; j < glyphInfos.Count; j++)
		{
			SKRect sKRect = array[j];
			double glyphAdvance = glyphInfos[j].GlyphAdvance;
			rect = rect.Union(new Rect(num + (double)sKRect.Left, baselineOrigin.Y + (double)sKRect.Top, sKRect.Width, sKRect.Height));
			num += glyphAdvance;
		}
		ArrayPool<SKRect>.Shared.Return(array);
		FontRenderingEmSize = fontRenderingEmSize;
		BaselineOrigin = baselineOrigin;
		Bounds = rect;
	}

	public SKTextBlob GetTextBlob(RenderOptions renderOptions)
	{
		SKFontEdging edging = SKFontEdging.SubpixelAntialias;
		switch (renderOptions.TextRenderingMode)
		{
		case TextRenderingMode.Alias:
			edging = SKFontEdging.Alias;
			break;
		case TextRenderingMode.Antialias:
			edging = SKFontEdging.Antialias;
			break;
		case TextRenderingMode.Unspecified:
			edging = ((renderOptions.EdgeMode != EdgeMode.Aliased) ? SKFontEdging.SubpixelAntialias : SKFontEdging.Alias);
			break;
		}
		return _textBlobCache.GetOrAdd(edging, delegate
		{
			SKFont sKFont = _glyphTypefaceImpl.SKFont;
			sKFont.Hinting = SKFontHinting.Full;
			sKFont.Subpixel = edging == SKFontEdging.SubpixelAntialias;
			sKFont.Edging = edging;
			sKFont.Size = (float)FontRenderingEmSize;
			SKTextBlobBuilder sKTextBlobBuilder = SKCacheBase<SKTextBlobBuilder, SKTextBlobBuilderCache>.Shared.Get();
			SKPositionedRunBuffer sKPositionedRunBuffer = sKTextBlobBuilder.AllocatePositionedRun(sKFont, _glyphIndices.Length, null);
			sKPositionedRunBuffer.SetPositions(_glyphPositions);
			sKPositionedRunBuffer.SetGlyphs(_glyphIndices);
			SKTextBlob result = sKTextBlobBuilder.Build();
			SKCacheBase<SKTextBlobBuilder, SKTextBlobBuilderCache>.Shared.Return(sKTextBlobBuilder);
			return result;
		});
	}

	public void Dispose()
	{
		foreach (KeyValuePair<SKFontEdging, SKTextBlob> item in _textBlobCache)
		{
			item.Value.Dispose();
		}
	}

	public IReadOnlyList<float> GetIntersections(float lowerLimit, float upperLimit)
	{
		return GetTextBlob(default(RenderOptions)).GetIntercepts(lowerLimit, upperLimit);
	}
}
