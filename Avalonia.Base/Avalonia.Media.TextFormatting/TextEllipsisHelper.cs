using System;
using System.Collections.Generic;
using Avalonia.Media.TextFormatting.Unicode;

namespace Avalonia.Media.TextFormatting;

internal static class TextEllipsisHelper
{
	public static TextRun[]? Collapse(TextLine textLine, TextCollapsingProperties properties, bool isWordEllipsis)
	{
		IReadOnlyList<TextRun> textRuns = textLine.TextRuns;
		if (textRuns.Count == 0)
		{
			return null;
		}
		int i = 0;
		double num = 0.0;
		int num2 = 0;
		ShapedTextRun shapedTextRun = TextFormatterImpl.CreateSymbol(properties.Symbol, FlowDirection.LeftToRight);
		if (properties.Width < shapedTextRun.GlyphRun.Bounds.Width)
		{
			return Array.Empty<TextRun>();
		}
		double num3 = properties.Width - shapedTextRun.Size.Width;
		ReadOnlyMemory<char> text;
		if (properties.FlowDirection == FlowDirection.LeftToRight)
		{
			for (; i < textRuns.Count; i++)
			{
				TextRun textRun = textRuns[i];
				if (!(textRun is ShapedTextRun shapedTextRun2))
				{
					if (textRun is DrawableTextRun drawableTextRun)
					{
						if (num + drawableTextRun.Size.Width > num3)
						{
							return CreateCollapsedRuns(textLine, num2, FlowDirection.LeftToRight, shapedTextRun);
						}
						num3 -= drawableTextRun.Size.Width;
					}
				}
				else
				{
					num += shapedTextRun2.Size.Width;
					if (num > num3)
					{
						if (shapedTextRun2.TryMeasureCharacters(num3, out var length) && isWordEllipsis && length < textLine.Length)
						{
							int num4 = 0;
							text = textRun.Text;
							LineBreakEnumerator lineBreakEnumerator = new LineBreakEnumerator(text.Span);
							LineBreak lineBreak;
							while (num4 < length && lineBreakEnumerator.MoveNext(out lineBreak))
							{
								int positionMeasure = lineBreak.PositionMeasure;
								if (positionMeasure == 0 || positionMeasure >= length)
								{
									break;
								}
								num4 = positionMeasure;
							}
							length = num4;
						}
						num2 += length;
						return CreateCollapsedRuns(textLine, num2, FlowDirection.LeftToRight, shapedTextRun);
					}
					num3 -= shapedTextRun2.Size.Width;
				}
				num2 += textRun.Length;
			}
		}
		else
		{
			for (i = textRuns.Count - 1; i >= 0; i--)
			{
				TextRun textRun2 = textRuns[i];
				if (!(textRun2 is ShapedTextRun shapedTextRun3))
				{
					if (textRun2 is DrawableTextRun drawableTextRun2)
					{
						if (num + drawableTextRun2.Size.Width > num3)
						{
							return CreateCollapsedRuns(textLine, num2, FlowDirection.RightToLeft, shapedTextRun);
						}
						num3 -= drawableTextRun2.Size.Width;
					}
				}
				else
				{
					num += shapedTextRun3.Size.Width;
					if (num > num3)
					{
						if (shapedTextRun3.TryMeasureCharacters(num3, out var length2) && isWordEllipsis && length2 < textLine.Length)
						{
							int num5 = 0;
							text = textRun2.Text;
							LineBreakEnumerator lineBreakEnumerator2 = new LineBreakEnumerator(text.Span);
							LineBreak lineBreak2;
							while (num5 < length2 && lineBreakEnumerator2.MoveNext(out lineBreak2))
							{
								int positionMeasure2 = lineBreak2.PositionMeasure;
								if (positionMeasure2 == 0 || positionMeasure2 >= length2)
								{
									break;
								}
								num5 = positionMeasure2;
							}
							length2 = num5;
						}
						num2 += length2;
						return CreateCollapsedRuns(textLine, num2, FlowDirection.RightToLeft, shapedTextRun);
					}
					num3 -= shapedTextRun3.Size.Width;
				}
				num2 += textRun2.Length;
			}
		}
		return null;
	}

	private static TextRun[] CreateCollapsedRuns(TextLine textLine, int collapsedLength, FlowDirection flowDirection, TextRun shapedSymbol)
	{
		IReadOnlyList<TextRun> textRuns = textLine.TextRuns;
		if (collapsedLength <= 0)
		{
			return new TextRun[1] { shapedSymbol };
		}
		if (flowDirection == FlowDirection.RightToLeft)
		{
			collapsedLength = textLine.Length - collapsedLength;
		}
		FormattingObjectPool instance = FormattingObjectPool.Instance;
		var (rentedList3, rentedList4) = (SplitResult<FormattingObjectPool.RentedList<TextRun>>)(ref TextFormatterImpl.SplitTextRuns(textRuns, collapsedLength, instance));
		try
		{
			if (flowDirection == FlowDirection.RightToLeft)
			{
				TextRun[] array = new TextRun[rentedList4.Count + 1];
				rentedList4.CopyTo(array, 1);
				array[0] = shapedSymbol;
				return array;
			}
			TextRun[] array2 = new TextRun[rentedList3.Count + 1];
			rentedList3.CopyTo(array2);
			array2[^1] = shapedSymbol;
			return array2;
		}
		finally
		{
			instance.TextRunLists.Return(ref rentedList3);
			instance.TextRunLists.Return(ref rentedList4);
		}
	}
}
