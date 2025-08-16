using System;
using System.Collections.Generic;
using Avalonia.Utilities;

namespace Avalonia.Media.TextFormatting;

public class TextLayout : IDisposable
{
	private class CachedMetrics
	{
		public double Height;

		public double Baseline;

		public double Width;

		public double WidthIncludingTrailingWhitespace;

		public double Extent;

		public double OverhangAfter;

		public double OverhangLeading;

		public double OverhangTrailing;
	}

	private readonly ITextSource _textSource;

	private readonly TextParagraphProperties _paragraphProperties;

	private readonly TextTrimming _textTrimming;

	private readonly TextLine[] _textLines;

	private readonly CachedMetrics _metrics = new CachedMetrics();

	private int _textSourceLength;

	public double LineHeight => _paragraphProperties.LineHeight;

	public double MaxWidth { get; }

	public double MaxHeight { get; }

	public int MaxLines { get; }

	public double LetterSpacing => _paragraphProperties.LetterSpacing;

	public IReadOnlyList<TextLine> TextLines => _textLines;

	public double Height => _metrics.Height;

	public double Extent => _metrics.Extent;

	public double Baseline => _metrics.Baseline;

	public double OverhangAfter => _metrics.OverhangAfter;

	public double OverhangLeading => _metrics.OverhangLeading;

	public double OverhangTrailing => _metrics.OverhangTrailing;

	public double Width => _metrics.Width;

	public double WidthIncludingTrailingWhitespace => _metrics.WidthIncludingTrailingWhitespace;

	public TextLayout(string? text, Typeface typeface, double fontSize, IBrush? foreground, TextAlignment textAlignment = TextAlignment.Left, TextWrapping textWrapping = TextWrapping.NoWrap, TextTrimming? textTrimming = null, TextDecorationCollection? textDecorations = null, FlowDirection flowDirection = FlowDirection.LeftToRight, double maxWidth = double.PositiveInfinity, double maxHeight = double.PositiveInfinity, double lineHeight = double.NaN, double letterSpacing = 0.0, int maxLines = 0, IReadOnlyList<ValueSpan<TextRunProperties>>? textStyleOverrides = null)
	{
		_paragraphProperties = CreateTextParagraphProperties(typeface, fontSize, foreground, textAlignment, textWrapping, textDecorations, flowDirection, lineHeight, letterSpacing);
		_textSource = new FormattedTextSource(text ?? "", _paragraphProperties.DefaultTextRunProperties, textStyleOverrides);
		_textTrimming = textTrimming ?? TextTrimming.None;
		MaxWidth = maxWidth;
		MaxHeight = maxHeight;
		MaxLines = maxLines;
		_textLines = CreateTextLines();
	}

	public TextLayout(ITextSource textSource, TextParagraphProperties paragraphProperties, TextTrimming? textTrimming = null, double maxWidth = double.PositiveInfinity, double maxHeight = double.PositiveInfinity, int maxLines = 0)
	{
		_textSource = textSource;
		_paragraphProperties = paragraphProperties;
		_textTrimming = textTrimming ?? TextTrimming.None;
		MaxWidth = maxWidth;
		MaxHeight = maxHeight;
		MaxLines = maxLines;
		_textLines = CreateTextLines();
	}

	public void Draw(DrawingContext context, Point origin)
	{
		if (_textLines.Length != 0)
		{
			Point point = origin;
			point.Deconstruct(out var x, out var y);
			double x2 = x;
			double num = y;
			TextLine[] textLines = _textLines;
			foreach (TextLine textLine in textLines)
			{
				textLine.Draw(context, new Point(x2, num));
				num += textLine.Height;
			}
		}
	}

	public Rect HitTestTextPosition(int textPosition)
	{
		if (_textLines.Length == 0)
		{
			return default(Rect);
		}
		if (textPosition < 0)
		{
			textPosition = _textSourceLength;
		}
		double num = 0.0;
		for (int i = 0; i < _textLines.Length; i++)
		{
			TextLine textLine = _textLines[i];
			if (textLine.FirstTextSourceIndex + textLine.Length <= textPosition && i + 1 < _textLines.Length)
			{
				num += textLine.Height;
				continue;
			}
			CharacterHit characterHit = new CharacterHit(textPosition);
			double distanceFromCharacterHit = textLine.GetDistanceFromCharacterHit(characterHit);
			CharacterHit nextCaretCharacterHit = textLine.GetNextCaretCharacterHit(characterHit);
			double distanceFromCharacterHit2 = textLine.GetDistanceFromCharacterHit(nextCaretCharacterHit);
			return new Rect(distanceFromCharacterHit, num, distanceFromCharacterHit2 - distanceFromCharacterHit, textLine.Height);
		}
		return default(Rect);
	}

	public IEnumerable<Rect> HitTestTextRange(int start, int length)
	{
		if (start + length <= 0)
		{
			return Array.Empty<Rect>();
		}
		List<Rect> list = new List<Rect>(_textLines.Length);
		double num = 0.0;
		TextLine[] textLines = _textLines;
		foreach (TextLine textLine in textLines)
		{
			if (textLine.FirstTextSourceIndex + textLine.Length <= start)
			{
				num += textLine.Height;
				continue;
			}
			IReadOnlyList<TextBounds> textBounds = textLine.GetTextBounds(start, length);
			if (textBounds.Count > 0)
			{
				foreach (TextBounds item in textBounds)
				{
					Rect? rect = ((list.Count > 0) ? new Rect?(list[list.Count - 1]) : ((Rect?)null));
					if (rect.HasValue && MathUtilities.AreClose(rect.Value.Right, item.Rectangle.Left) && MathUtilities.AreClose(rect.Value.Top, num))
					{
						list[list.Count - 1] = rect.Value.WithWidth(rect.Value.Width + item.Rectangle.Width);
					}
					else
					{
						list.Add(item.Rectangle.WithY(num));
					}
					foreach (TextRunBounds textRunBound in item.TextRunBounds)
					{
						start += textRunBound.Length;
						length -= textRunBound.Length;
					}
				}
			}
			if (textLine.FirstTextSourceIndex + textLine.Length >= start + length)
			{
				break;
			}
			num += textLine.Height;
		}
		return list;
	}

	public TextHitTestResult HitTestPoint(in Point point)
	{
		double num = 0.0;
		TextLine textLine = null;
		CharacterHit characterHitFromDistance;
		for (int i = 0; i < _textLines.Length; i++)
		{
			textLine = _textLines[i];
			if (num + textLine.Height > point.Y)
			{
				characterHitFromDistance = textLine.GetCharacterHitFromDistance(point.X);
				return GetHitTestResult(textLine, characterHitFromDistance, point);
			}
			num += textLine.Height;
		}
		if (textLine == null)
		{
			return default(TextHitTestResult);
		}
		characterHitFromDistance = textLine.GetCharacterHitFromDistance(point.X);
		return GetHitTestResult(textLine, characterHitFromDistance, point);
	}

	public int GetLineIndexFromCharacterIndex(int charIndex, bool trailingEdge)
	{
		if (charIndex < 0)
		{
			return 0;
		}
		if (charIndex > _textSourceLength)
		{
			return _textLines.Length - 1;
		}
		for (int i = 0; i < _textLines.Length; i++)
		{
			TextLine textLine = _textLines[i];
			if (textLine.FirstTextSourceIndex + textLine.Length >= charIndex && charIndex >= textLine.FirstTextSourceIndex && charIndex <= textLine.FirstTextSourceIndex + textLine.Length - ((!trailingEdge) ? 1 : 0))
			{
				return i;
			}
		}
		return _textLines.Length - 1;
	}

	private TextHitTestResult GetHitTestResult(TextLine textLine, CharacterHit characterHit, Point point)
	{
		Point point2 = point;
		point2.Deconstruct(out var x, out var y);
		double num = x;
		double num2 = y;
		bool isInside = num >= 0.0 && num <= textLine.Width && num2 >= 0.0 && num2 <= textLine.Height;
		int num3 = 0;
		if (_paragraphProperties.FlowDirection == FlowDirection.LeftToRight)
		{
			num3 = textLine.FirstTextSourceIndex + textLine.Length;
			if (num >= textLine.Width && textLine.Length > 0 && textLine.NewLineLength > 0)
			{
				num3 -= textLine.NewLineLength;
			}
			TextEndOfLine textEndOfLine = textLine.TextLineBreak?.TextEndOfLine;
			if (textEndOfLine != null)
			{
				num3 -= textEndOfLine.Length;
			}
		}
		else
		{
			if (num <= textLine.WidthIncludingTrailingWhitespace - textLine.Width && textLine.Length > 0 && textLine.NewLineLength > 0)
			{
				num3 += textLine.NewLineLength;
			}
			TextEndOfLine textEndOfLine2 = textLine.TextLineBreak?.TextEndOfLine;
			if (textEndOfLine2 != null)
			{
				num3 += textEndOfLine2.Length;
			}
		}
		int num4 = characterHit.FirstCharacterIndex + characterHit.TrailingLength;
		bool isTrailing = (num3 == num4 && characterHit.TrailingLength > 0) || num2 > Height;
		if (num4 == textLine.FirstTextSourceIndex + textLine.Length)
		{
			num4 -= textLine.NewLineLength;
		}
		if (textLine.NewLineLength > 0 && num4 + textLine.NewLineLength == characterHit.FirstCharacterIndex + characterHit.TrailingLength)
		{
			characterHit = new CharacterHit(characterHit.FirstCharacterIndex);
		}
		return new TextHitTestResult(characterHit, num4, isInside, isTrailing);
	}

	internal static TextParagraphProperties CreateTextParagraphProperties(Typeface typeface, double fontSize, IBrush? foreground, TextAlignment textAlignment, TextWrapping textWrapping, TextDecorationCollection? textDecorations, FlowDirection flowDirection, double lineHeight, double letterSpacing)
	{
		GenericTextRunProperties defaultTextRunProperties = new GenericTextRunProperties(typeface, fontSize, textDecorations, foreground);
		return new GenericTextParagraphProperties(flowDirection, textAlignment, firstLineInParagraph: true, alwaysCollapsible: false, defaultTextRunProperties, textWrapping, lineHeight, 0.0, letterSpacing);
	}

	private TextLine[] CreateTextLines()
	{
		FormattingObjectPool instance = FormattingObjectPool.Instance;
		double lineStartOfLongestLine = double.MaxValue;
		Point origin = default(Point);
		bool first = true;
		double accBlackBoxTop;
		double accBlackBoxLeft = (accBlackBoxTop = double.MaxValue);
		double accBlackBoxBottom;
		double accBlackBoxRight = (accBlackBoxBottom = double.MinValue);
		if (MathUtilities.IsZero(MaxWidth) || MathUtilities.IsZero(MaxHeight))
		{
			TextLineImpl textLineImpl = TextFormatterImpl.CreateEmptyTextLine(0, double.PositiveInfinity, _paragraphProperties);
			UpdateMetrics(textLineImpl, ref lineStartOfLongestLine, ref origin, ref first, ref accBlackBoxLeft, ref accBlackBoxTop, ref accBlackBoxRight, ref accBlackBoxBottom);
			return new TextLine[1] { textLineImpl };
		}
		FormattingObjectPool.RentedList<TextLine> rentedList = instance.TextLines.Rent();
		try
		{
			_textSourceLength = 0;
			TextLine textLine = null;
			TextFormatter current = TextFormatter.Current;
			TextLine textLine2;
			do
			{
				textLine2 = current.FormatLine(_textSource, _textSourceLength, MaxWidth, _paragraphProperties, textLine?.TextLineBreak);
				if (textLine2 == null)
				{
					if (textLine != null && textLine.NewLineLength > 0)
					{
						TextLineImpl textLineImpl2 = TextFormatterImpl.CreateEmptyTextLine(_textSourceLength, MaxWidth, _paragraphProperties);
						rentedList.Add(textLineImpl2);
						UpdateMetrics(textLineImpl2, ref lineStartOfLongestLine, ref origin, ref first, ref accBlackBoxLeft, ref accBlackBoxTop, ref accBlackBoxRight, ref accBlackBoxBottom);
					}
					break;
				}
				_textSourceLength += textLine2.Length;
				if (rentedList.Count > 0 && !double.IsPositiveInfinity(MaxHeight) && Height + textLine2.Height > MaxHeight)
				{
					if (textLine?.TextLineBreak != null && _textTrimming != TextTrimming.None)
					{
						TextLine value = textLine.Collapse(GetCollapsingProperties(MaxWidth));
						rentedList[rentedList.Count - 1] = value;
					}
					break;
				}
				if (textLine2.HasOverflowed && _textTrimming != TextTrimming.None)
				{
					textLine2 = textLine2.Collapse(GetCollapsingProperties(MaxWidth));
				}
				rentedList.Add(textLine2);
				UpdateMetrics(textLine2, ref lineStartOfLongestLine, ref origin, ref first, ref accBlackBoxLeft, ref accBlackBoxTop, ref accBlackBoxRight, ref accBlackBoxBottom);
				textLine = textLine2;
				if (MaxLines > 0 && rentedList.Count >= MaxLines)
				{
					TextLineBreak textLineBreak = textLine2.TextLineBreak;
					if (textLineBreak != null && textLineBreak.IsSplit)
					{
						rentedList[rentedList.Count - 1] = textLine2.Collapse(GetCollapsingProperties(WidthIncludingTrailingWhitespace));
					}
					break;
				}
			}
			while (!(textLine2.TextLineBreak?.TextEndOfLine is TextEndOfParagraph));
			if (rentedList.Count == 0)
			{
				TextLineImpl textLineImpl3 = TextFormatterImpl.CreateEmptyTextLine(0, MaxWidth, _paragraphProperties);
				rentedList.Add(textLineImpl3);
				UpdateMetrics(textLineImpl3, ref lineStartOfLongestLine, ref origin, ref first, ref accBlackBoxLeft, ref accBlackBoxTop, ref accBlackBoxRight, ref accBlackBoxBottom);
			}
			if (_paragraphProperties.TextAlignment == TextAlignment.Justify)
			{
				double num = MaxWidth;
				if (_paragraphProperties.TextWrapping != 0)
				{
					num = WidthIncludingTrailingWhitespace;
				}
				if (num > 0.0)
				{
					InterWordJustification justificationProperties = new InterWordJustification(num);
					for (int i = 0; i < rentedList.Count; i++)
					{
						rentedList[i].Justify(justificationProperties);
					}
				}
			}
			return rentedList.ToArray();
		}
		finally
		{
			instance.TextLines.Return(ref rentedList);
		}
	}

	private void UpdateMetrics(TextLine currentLine, ref double lineStartOfLongestLine, ref Point origin, ref bool first, ref double accBlackBoxLeft, ref double accBlackBoxTop, ref double accBlackBoxRight, ref double accBlackBoxBottom)
	{
		double val = origin.X + currentLine.Start + currentLine.OverhangLeading;
		double val2 = origin.X + currentLine.Start + currentLine.Width - currentLine.OverhangTrailing;
		double num = origin.Y + currentLine.Height + currentLine.OverhangAfter;
		double val3 = num - currentLine.Extent;
		accBlackBoxLeft = Math.Min(accBlackBoxLeft, val);
		accBlackBoxRight = Math.Max(accBlackBoxRight, val2);
		accBlackBoxBottom = Math.Max(accBlackBoxBottom, num);
		accBlackBoxTop = Math.Min(accBlackBoxTop, val3);
		_metrics.OverhangAfter = currentLine.OverhangAfter;
		_metrics.Height += currentLine.Height;
		_metrics.Width = Math.Max(_metrics.Width, currentLine.Width);
		_metrics.WidthIncludingTrailingWhitespace = Math.Max(_metrics.WidthIncludingTrailingWhitespace, currentLine.WidthIncludingTrailingWhitespace);
		lineStartOfLongestLine = Math.Min(lineStartOfLongestLine, currentLine.Start);
		_metrics.Extent = accBlackBoxBottom - accBlackBoxTop;
		_metrics.OverhangLeading = accBlackBoxLeft - lineStartOfLongestLine;
		_metrics.OverhangTrailing = _metrics.Width - (accBlackBoxRight - lineStartOfLongestLine);
		if (first)
		{
			_metrics.Baseline = currentLine.Baseline;
			first = false;
		}
		origin = origin.WithY(origin.Y + currentLine.Height);
	}

	private TextCollapsingProperties? GetCollapsingProperties(double width)
	{
		if (_textTrimming == TextTrimming.None)
		{
			return null;
		}
		return _textTrimming.CreateCollapsingProperties(new TextCollapsingCreateInfo(width, _paragraphProperties.DefaultTextRunProperties, _paragraphProperties.FlowDirection));
	}

	public void Dispose()
	{
		TextLine[] textLines = _textLines;
		for (int i = 0; i < textLines.Length; i++)
		{
			textLines[i].Dispose();
		}
	}
}
