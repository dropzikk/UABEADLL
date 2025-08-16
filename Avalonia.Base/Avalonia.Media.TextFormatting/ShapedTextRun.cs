using System;
using Avalonia.Media.TextFormatting.Unicode;

namespace Avalonia.Media.TextFormatting;

public sealed class ShapedTextRun : DrawableTextRun, IDisposable
{
	private GlyphRun? _glyphRun;

	public bool IsReversed { get; private set; }

	public sbyte BidiLevel => ShapedBuffer.BidiLevel;

	public ShapedBuffer ShapedBuffer { get; }

	public override ReadOnlyMemory<char> Text => ShapedBuffer.Text;

	public override TextRunProperties Properties { get; }

	public override int Length => ShapedBuffer.Text.Length;

	public TextMetrics TextMetrics { get; }

	public override double Baseline => 0.0 - TextMetrics.Ascent;

	public override Size Size => GlyphRun.Bounds.Size;

	public GlyphRun GlyphRun => _glyphRun ?? (_glyphRun = CreateGlyphRun());

	public ShapedTextRun(ShapedBuffer shapedBuffer, TextRunProperties properties)
	{
		ShapedBuffer = shapedBuffer;
		Properties = properties;
		TextMetrics = new TextMetrics(properties.CachedGlyphTypeface, properties.FontRenderingEmSize);
	}

	public override void Draw(DrawingContext drawingContext, Point origin)
	{
		using (drawingContext.PushTransform(Matrix.CreateTranslation(origin)))
		{
			if (GlyphRun.GlyphInfos.Count == 0 || Properties.Typeface == default(Typeface) || Properties.ForegroundBrush == null)
			{
				return;
			}
			if (Properties.BackgroundBrush != null)
			{
				drawingContext.DrawRectangle(Properties.BackgroundBrush, null, GlyphRun.Bounds);
			}
			drawingContext.DrawGlyphRun(Properties.ForegroundBrush, GlyphRun);
			if (Properties.TextDecorations == null)
			{
				return;
			}
			foreach (TextDecoration textDecoration in Properties.TextDecorations)
			{
				textDecoration.Draw(drawingContext, GlyphRun, TextMetrics, Properties.ForegroundBrush);
			}
		}
	}

	internal void Reverse()
	{
		_glyphRun = null;
		ShapedBuffer.Reverse();
		IsReversed = !IsReversed;
	}

	internal bool TryMeasureCharacters(double availableWidth, out int length)
	{
		length = 0;
		double num = 0.0;
		ReadOnlySpan<char> span = GlyphRun.Characters.Span;
		for (int i = 0; i < ShapedBuffer.Length; i++)
		{
			double glyphAdvance = ShapedBuffer[i].GlyphAdvance;
			if (num + glyphAdvance > availableWidth)
			{
				break;
			}
			Codepoint.ReadAt(span, length, out var count);
			length += count;
			num += glyphAdvance;
		}
		return length > 0;
	}

	internal bool TryMeasureCharactersBackwards(double availableWidth, out int length, out double width)
	{
		length = 0;
		width = 0.0;
		ReadOnlySpan<char> span = GlyphRun.Characters.Span;
		for (int num = ShapedBuffer.Length - 1; num >= 0; num--)
		{
			double glyphAdvance = ShapedBuffer[num].GlyphAdvance;
			if (width + glyphAdvance > availableWidth)
			{
				break;
			}
			Codepoint.ReadAt(span, length, out var count);
			length += count;
			width += glyphAdvance;
		}
		return length > 0;
	}

	internal SplitResult<ShapedTextRun> Split(int length)
	{
		bool isReversed = IsReversed;
		if (isReversed)
		{
			Reverse();
			length = Length - length;
		}
		SplitResult<ShapedBuffer> splitResult = ShapedBuffer.Split(length);
		ShapedTextRun shapedTextRun = new ShapedTextRun(splitResult.First, Properties);
		ShapedTextRun shapedTextRun2 = new ShapedTextRun(splitResult.Second, Properties);
		if (isReversed)
		{
			return new SplitResult<ShapedTextRun>(shapedTextRun2, shapedTextRun);
		}
		return new SplitResult<ShapedTextRun>(shapedTextRun, shapedTextRun2);
	}

	internal GlyphRun CreateGlyphRun()
	{
		IGlyphTypeface glyphTypeface = ShapedBuffer.GlyphTypeface;
		double fontRenderingEmSize = ShapedBuffer.FontRenderingEmSize;
		ReadOnlyMemory<char> text = Text;
		ShapedBuffer shapedBuffer = ShapedBuffer;
		int bidiLevel = BidiLevel;
		return new GlyphRun(glyphTypeface, fontRenderingEmSize, text, shapedBuffer, null, bidiLevel);
	}

	public void Dispose()
	{
		_glyphRun?.Dispose();
		ShapedBuffer.Dispose();
	}
}
