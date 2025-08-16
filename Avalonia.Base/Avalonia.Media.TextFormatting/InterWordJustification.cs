using System;
using System.Collections.Generic;
using Avalonia.Media.TextFormatting.Unicode;

namespace Avalonia.Media.TextFormatting;

internal class InterWordJustification : JustificationProperties
{
	public override double Width { get; }

	public InterWordJustification(double width)
	{
		Width = width;
	}

	public override void Justify(TextLine textLine)
	{
		if (!(textLine is TextLineImpl textLineImpl))
		{
			return;
		}
		double width = Width;
		if (double.IsInfinity(width))
		{
			return;
		}
		Queue<int> queue = new Queue<int>();
		int num = textLine.FirstTextSourceIndex;
		for (int i = 0; i < textLineImpl.TextRuns.Count; i++)
		{
			TextRun textRun = textLineImpl.TextRuns[i];
			ReadOnlyMemory<char> text = textRun.Text;
			if (text.IsEmpty)
			{
				continue;
			}
			LineBreakEnumerator lineBreakEnumerator = new LineBreakEnumerator(text.Span);
			LineBreak lineBreak;
			while (lineBreakEnumerator.MoveNext(out lineBreak))
			{
				if (!lineBreak.Required && lineBreak.PositionWrap != textRun.Length)
				{
					queue.Enqueue(num + lineBreak.PositionMeasure);
				}
			}
			num += textRun.Length;
		}
		if (queue.Count == 0)
		{
			return;
		}
		double num2 = Math.Max(0.0, width - textLineImpl.WidthIncludingTrailingWhitespace) / (double)queue.Count;
		num = textLine.FirstTextSourceIndex;
		foreach (TextRun textRun2 in textLineImpl.TextRuns)
		{
			if (textRun2.Text.IsEmpty)
			{
				continue;
			}
			if (textRun2 is ShapedTextRun shapedTextRun)
			{
				GlyphRun glyphRun = shapedTextRun.GlyphRun;
				ShapedBuffer shapedBuffer = shapedTextRun.ShapedBuffer;
				while (queue.Count > 0)
				{
					int num3 = queue.Dequeue();
					if (num3 >= num)
					{
						int num4 = Math.Max(0, num - glyphRun.Metrics.FirstCluster);
						int index = glyphRun.FindGlyphIndex(num3 - num4);
						GlyphInfo glyphInfo = shapedBuffer[index];
						shapedBuffer[index] = new GlyphInfo(glyphInfo.GlyphIndex, glyphInfo.GlyphCluster, glyphInfo.GlyphAdvance + num2);
					}
				}
				glyphRun.GlyphInfos = shapedBuffer;
			}
			num += textRun2.Length;
		}
	}
}
