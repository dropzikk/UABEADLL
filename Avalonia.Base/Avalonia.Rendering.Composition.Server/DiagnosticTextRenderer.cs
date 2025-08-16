using System;
using Avalonia.Media;
using Avalonia.Platform;

namespace Avalonia.Rendering.Composition.Server;

internal sealed class DiagnosticTextRenderer
{
	private const char FirstChar = ' ';

	private const char LastChar = '~';

	private readonly GlyphRun[] _runs = new GlyphRun[95];

	public double GetMaxHeight()
	{
		double num = 0.0;
		for (char c = ' '; c <= '~'; c = (char)(c + 1))
		{
			double height = _runs[c - 32].Bounds.Height;
			if (height > num)
			{
				num = height;
			}
		}
		return num;
	}

	public DiagnosticTextRenderer(IGlyphTypeface typeface, double fontRenderingEmSize)
	{
		char[] array = new char[95];
		for (char c = ' '; c <= '~'; c = (char)(c + 1))
		{
			int num = c - 32;
			array[num] = c;
			ushort glyph = typeface.GetGlyph(c);
			_runs[num] = new GlyphRun(typeface, fontRenderingEmSize, array.AsMemory(num, 1), new ushort[1] { glyph }, null);
		}
	}

	public Size MeasureAsciiText(ReadOnlySpan<char> text)
	{
		double num = 0.0;
		double num2 = 0.0;
		ReadOnlySpan<char> readOnlySpan = text;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			char c = readOnlySpan[i];
			char c2 = ((c >= ' ' && c <= '~') ? c : ' ');
			GlyphRun glyphRun = _runs[c2 - 32];
			num += glyphRun.Bounds.Width;
			num2 = Math.Max(num2, glyphRun.Bounds.Height);
		}
		return new Size(num, num2);
	}

	public void DrawAsciiText(IDrawingContextImpl context, ReadOnlySpan<char> text, IBrush foreground)
	{
		double num = 0.0;
		Matrix transform = context.Transform;
		ReadOnlySpan<char> readOnlySpan = text;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			char c = readOnlySpan[i];
			char c2 = ((c >= ' ' && c <= '~') ? c : ' ');
			GlyphRun glyphRun = _runs[c2 - 32];
			context.Transform = transform * Matrix.CreateTranslation(num, 0.0);
			context.DrawGlyphRun(foreground, glyphRun.PlatformImpl.Item);
			num += glyphRun.Bounds.Width;
		}
		context.Transform = transform;
	}
}
