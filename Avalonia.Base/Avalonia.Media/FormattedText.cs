using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Avalonia.Media.TextFormatting;
using Avalonia.Utilities;

namespace Avalonia.Media;

public class FormattedText
{
	private struct LineEnumerator : IEnumerator, IDisposable
	{
		private int _lineCount;

		private double _totalHeight;

		private TextLine? _nextLine;

		private readonly TextFormatter _formatter;

		private readonly FormattedText _that;

		private readonly ITextSource _textSource;

		private double _previousHeight;

		private TextLineBreak? _previousLineBreak;

		private int _position;

		private int _length;

		public int Position
		{
			get
			{
				return _position;
			}
			private set
			{
				_position = value;
			}
		}

		public int Length
		{
			get
			{
				return _length;
			}
			private set
			{
				_length = value;
			}
		}

		public TextLine? Current { get; private set; }

		object? IEnumerator.Current => Current;

		internal double CurrentParagraphWidth => MaxLineLength(_lineCount);

		internal LineEnumerator(FormattedText text)
		{
			_previousHeight = 0.0;
			_length = 0;
			_previousLineBreak = null;
			_position = 0;
			_lineCount = 0;
			_totalHeight = 0.0;
			Current = null;
			_nextLine = null;
			_formatter = TextFormatter.Current;
			_that = text;
			FormattedText that = _that;
			_textSource = that._textSourceImpl ?? (that._textSourceImpl = new TextSourceImplementation(_that));
		}

		public void Dispose()
		{
			Current = null;
			_nextLine = null;
		}

		private double MaxLineLength(int line)
		{
			if (_that._maxTextWidths == null)
			{
				return _that._maxTextWidth;
			}
			return _that._maxTextWidths[Math.Min(line, _that._maxTextWidths.Length - 1)];
		}

		public bool MoveNext()
		{
			if (Current == null)
			{
				if (_that._text.Length == 0)
				{
					return false;
				}
				Current = FormatLine(_textSource, Position, MaxLineLength(_lineCount), _that._defaultParaProps, null);
				if (Current == null)
				{
					return false;
				}
				if (_totalHeight + Current.Height > _that._maxTextHeight)
				{
					Current = null;
					return false;
				}
			}
			else
			{
				if (_nextLine == null)
				{
					return false;
				}
				_totalHeight += _previousHeight;
				Position += Length;
				_lineCount++;
				Current = _nextLine;
				_nextLine = null;
			}
			TextLineBreak textLineBreak = Current.TextLineBreak;
			if (Position + Current.Length < _that._text.Length)
			{
				bool flag = false;
				if (_lineCount + 1 >= _that._maxLineCount)
				{
					flag = false;
				}
				else
				{
					_nextLine = FormatLine(_textSource, Position + Current.Length, MaxLineLength(_lineCount + 1), _that._defaultParaProps, textLineBreak);
					if (_nextLine != null)
					{
						flag = _totalHeight + Current.Height + _nextLine.Height <= _that._maxTextHeight;
					}
				}
				if (!flag)
				{
					_nextLine = null;
					if (_that._trimming != TextTrimming.None && !Current.HasCollapsed)
					{
						TextWrapping textWrapping = _that._defaultParaProps.TextWrapping;
						_that._defaultParaProps.SetTextWrapping(TextWrapping.NoWrap);
						Current = FormatLine(_that._textSourceImpl, Position, MaxLineLength(_lineCount), _that._defaultParaProps, _previousLineBreak);
						if (Current != null)
						{
							textLineBreak = Current.TextLineBreak;
						}
						_that._defaultParaProps.SetTextWrapping(textWrapping);
					}
				}
			}
			if (Current != null)
			{
				_previousHeight = Current.Height;
				Length = Current.Length;
			}
			_previousLineBreak = textLineBreak;
			return true;
		}

		private TextLine? FormatLine(ITextSource textSource, int textSourcePosition, double maxLineLength, TextParagraphProperties paraProps, TextLineBreak? lineBreak)
		{
			TextLine textLine = _formatter.FormatLine(textSource, textSourcePosition, maxLineLength, paraProps, lineBreak);
			if (textLine != null && _that._trimming != TextTrimming.None && textLine.HasOverflowed && textLine.Length > 0)
			{
				GenericTextRunProperties textRunProperties = (GenericTextRunProperties)new SpanRider(_that._formatRuns, _that._latestPosition, Math.Min(textSourcePosition + textLine.Length - 1, _that._text.Length - 1)).CurrentElement;
				TextCollapsingProperties textCollapsingProperties = _that._trimming.CreateCollapsingProperties(new TextCollapsingCreateInfo(maxLineLength, textRunProperties, paraProps.FlowDirection));
				textLine = textLine.Collapse(textCollapsingProperties);
			}
			return textLine;
		}

		public void Reset()
		{
			Position = 0;
			_lineCount = 0;
			_totalHeight = 0.0;
			Current = null;
			_nextLine = null;
		}
	}

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

	private class TextSourceImplementation : ITextSource
	{
		private readonly FormattedText _that;

		public TextSourceImplementation(FormattedText text)
		{
			_that = text;
		}

		public TextRun GetTextRun(int textSourceCharacterIndex)
		{
			if (textSourceCharacterIndex >= _that._text.Length)
			{
				return new TextEndOfParagraph();
			}
			SpanRider spanRider = new SpanRider(_that._formatRuns, _that._latestPosition, textSourceCharacterIndex);
			ReadOnlyMemory<char> text = _that._text.AsMemory(textSourceCharacterIndex, spanRider.Length);
			TextRunProperties textRunProperties = (GenericTextRunProperties)spanRider.CurrentElement;
			return new TextCharacters(text, textRunProperties);
		}
	}

	public const double DefaultRealToIdeal = 300.0;

	public const double DefaultIdealToReal = 1.0 / 300.0;

	public const int IdealInfiniteWidth = 1073741822;

	public const double RealInfiniteWidth = 3579139.4066666667;

	public const double GreatestMultiplierOfEm = 100.0;

	private const double MaxFontEmSize = 35791.39406666667;

	private string _text;

	private readonly SpanVector _formatRuns = new SpanVector(null);

	private SpanPosition _latestPosition;

	private GenericTextParagraphProperties _defaultParaProps;

	private double _maxTextWidth = double.PositiveInfinity;

	private double[]? _maxTextWidths;

	private double _maxTextHeight = double.PositiveInfinity;

	private int _maxLineCount = int.MaxValue;

	private TextTrimming _trimming = TextTrimming.WordEllipsis;

	private TextSourceImplementation? _textSourceImpl;

	private CachedMetrics? _metrics;

	public FlowDirection FlowDirection
	{
		get
		{
			return _defaultParaProps.FlowDirection;
		}
		set
		{
			ValidateFlowDirection(value, "value");
			_defaultParaProps.SetFlowDirection(value);
			InvalidateMetrics();
		}
	}

	public TextAlignment TextAlignment
	{
		get
		{
			return _defaultParaProps.TextAlignment;
		}
		set
		{
			_defaultParaProps.SetTextAlignment(value);
			InvalidateMetrics();
		}
	}

	public double LineHeight
	{
		get
		{
			return _defaultParaProps.LineHeight;
		}
		set
		{
			if (value < 0.0)
			{
				throw new ArgumentOutOfRangeException("value", "Parameter must be greater than or equal to zero.");
			}
			_defaultParaProps.SetLineHeight(value);
			InvalidateMetrics();
		}
	}

	public double MaxTextWidth
	{
		get
		{
			return _maxTextWidth;
		}
		set
		{
			if (value < 0.0)
			{
				throw new ArgumentOutOfRangeException("value", "Parameter must be greater than or equal to zero.");
			}
			_maxTextWidth = value;
			InvalidateMetrics();
		}
	}

	public double MaxTextHeight
	{
		get
		{
			return _maxTextHeight;
		}
		set
		{
			if (value <= 0.0)
			{
				throw new ArgumentOutOfRangeException("value", "'MaxTextHeight' property value must be greater than zero.");
			}
			if (double.IsNaN(value))
			{
				throw new ArgumentOutOfRangeException("value", "'MaxTextHeight' property value cannot be NaN.");
			}
			_maxTextHeight = value;
			InvalidateMetrics();
		}
	}

	public int MaxLineCount
	{
		get
		{
			return _maxLineCount;
		}
		set
		{
			if (value <= 0)
			{
				throw new ArgumentOutOfRangeException("value", "The parameter value must be greater than zero.");
			}
			_maxLineCount = value;
			InvalidateMetrics();
		}
	}

	public TextTrimming Trimming
	{
		get
		{
			return _trimming;
		}
		set
		{
			_trimming = value;
			_defaultParaProps.SetTextWrapping((_trimming == TextTrimming.None) ? TextWrapping.Wrap : TextWrapping.WrapWithOverflow);
			InvalidateMetrics();
		}
	}

	private CachedMetrics Metrics => _metrics ?? (_metrics = DrawAndCalculateMetrics(null, default(Point), getBlackBoxMetrics: false));

	private CachedMetrics BlackBoxMetrics
	{
		get
		{
			if (_metrics == null || double.IsNaN(_metrics.Extent))
			{
				_metrics = DrawAndCalculateMetrics(null, default(Point), getBlackBoxMetrics: true);
			}
			return _metrics;
		}
	}

	public double Height => Metrics.Height;

	public double Extent => BlackBoxMetrics.Extent;

	public double Baseline => Metrics.Baseline;

	public double OverhangAfter => BlackBoxMetrics.OverhangAfter;

	public double OverhangLeading => BlackBoxMetrics.OverhangLeading;

	public double OverhangTrailing => BlackBoxMetrics.OverhangTrailing;

	public double Width => Metrics.Width;

	public double WidthIncludingTrailingWhitespace => Metrics.WidthIncludingTrailingWhitespace;

	public FormattedText(string textToFormat, CultureInfo culture, FlowDirection flowDirection, Typeface typeface, double emSize, IBrush? foreground)
	{
		if (culture == null)
		{
			throw new ArgumentNullException("culture");
		}
		ValidateFlowDirection(flowDirection, "flowDirection");
		ValidateFontSize(emSize);
		_text = textToFormat;
		GenericTextRunProperties genericTextRunProperties = new GenericTextRunProperties(typeface, emSize, null, foreground, null, BaselineAlignment.Baseline, culture);
		_latestPosition = _formatRuns.SetValue(0, _text.Length, genericTextRunProperties, _latestPosition);
		_defaultParaProps = new GenericTextParagraphProperties(flowDirection, TextAlignment.Left, firstLineInParagraph: false, alwaysCollapsible: false, genericTextRunProperties, TextWrapping.WrapWithOverflow, 0.0, 0.0, 0.0);
		InvalidateMetrics();
	}

	private static void ValidateFontSize(double emSize)
	{
		if (emSize <= 0.0)
		{
			throw new ArgumentOutOfRangeException("emSize", "The parameter value must be greater than zero.");
		}
		if (emSize > 35791.39406666667)
		{
			throw new ArgumentOutOfRangeException("emSize", $"The parameter value cannot be greater than '{35791.39406666667}'");
		}
		if (double.IsNaN(emSize))
		{
			throw new ArgumentOutOfRangeException("emSize", "The parameter value must be a number.");
		}
	}

	private static void ValidateFlowDirection(FlowDirection flowDirection, string parameterName)
	{
		if (flowDirection < FlowDirection.LeftToRight || flowDirection > FlowDirection.RightToLeft)
		{
			throw new InvalidEnumArgumentException(parameterName, (int)flowDirection, typeof(FlowDirection));
		}
	}

	private int ValidateRange(int startIndex, int count)
	{
		if (startIndex < 0 || startIndex > _text.Length)
		{
			throw new ArgumentOutOfRangeException("startIndex");
		}
		int num = startIndex + count;
		if (count < 0 || num < startIndex || num > _text.Length)
		{
			throw new ArgumentOutOfRangeException("count");
		}
		return num;
	}

	private void InvalidateMetrics()
	{
		_metrics = null;
	}

	public void SetForegroundBrush(IBrush foregroundBrush)
	{
		SetForegroundBrush(foregroundBrush, 0, _text.Length);
	}

	public void SetForegroundBrush(IBrush? foregroundBrush, int startIndex, int count)
	{
		int num = ValidateRange(startIndex, count);
		int num2 = startIndex;
		while (num2 < num)
		{
			SpanRider spanRider = new SpanRider(_formatRuns, _latestPosition, num2);
			num2 = Math.Min(num, num2 + spanRider.Length);
			if (!(spanRider.CurrentElement is GenericTextRunProperties genericTextRunProperties))
			{
				throw new NotSupportedException("runProps can not be null.");
			}
			if (genericTextRunProperties.ForegroundBrush != foregroundBrush)
			{
				GenericTextRunProperties element = new GenericTextRunProperties(genericTextRunProperties.Typeface, genericTextRunProperties.FontRenderingEmSize, genericTextRunProperties.TextDecorations, foregroundBrush, genericTextRunProperties.BackgroundBrush, genericTextRunProperties.BaselineAlignment, genericTextRunProperties.CultureInfo);
				_latestPosition = _formatRuns.SetValue(spanRider.CurrentPosition, num2 - spanRider.CurrentPosition, element, spanRider.SpanPosition);
			}
		}
	}

	public void SetFontFamily(string fontFamily)
	{
		SetFontFamily(fontFamily, 0, _text.Length);
	}

	public void SetFontFamily(string fontFamily, int startIndex, int count)
	{
		if (fontFamily == null)
		{
			throw new ArgumentNullException("fontFamily");
		}
		SetFontFamily(new FontFamily(fontFamily), startIndex, count);
	}

	public void SetFontFamily(FontFamily fontFamily)
	{
		SetFontFamily(fontFamily, 0, _text.Length);
	}

	public void SetFontFamily(FontFamily fontFamily, int startIndex, int count)
	{
		if (fontFamily == null)
		{
			throw new ArgumentNullException("fontFamily");
		}
		int num = ValidateRange(startIndex, count);
		int num2 = startIndex;
		while (num2 < num)
		{
			SpanRider spanRider = new SpanRider(_formatRuns, _latestPosition, num2);
			num2 = Math.Min(num, num2 + spanRider.Length);
			if (!(spanRider.CurrentElement is GenericTextRunProperties { Typeface: var typeface } genericTextRunProperties))
			{
				throw new NotSupportedException("runProps can not be null.");
			}
			if (!fontFamily.Equals(typeface.FontFamily))
			{
				GenericTextRunProperties element = new GenericTextRunProperties(new Typeface(fontFamily, typeface.Style, typeface.Weight), genericTextRunProperties.FontRenderingEmSize, genericTextRunProperties.TextDecorations, genericTextRunProperties.ForegroundBrush, genericTextRunProperties.BackgroundBrush, genericTextRunProperties.BaselineAlignment, genericTextRunProperties.CultureInfo);
				_latestPosition = _formatRuns.SetValue(spanRider.CurrentPosition, num2 - spanRider.CurrentPosition, element, spanRider.SpanPosition);
				InvalidateMetrics();
			}
		}
	}

	public void SetFontSize(double emSize)
	{
		SetFontSize(emSize, 0, _text.Length);
	}

	public void SetFontSize(double emSize, int startIndex, int count)
	{
		ValidateFontSize(emSize);
		int num = ValidateRange(startIndex, count);
		int num2 = startIndex;
		while (num2 < num)
		{
			SpanRider spanRider = new SpanRider(_formatRuns, _latestPosition, num2);
			num2 = Math.Min(num, num2 + spanRider.Length);
			if (!(spanRider.CurrentElement is GenericTextRunProperties genericTextRunProperties))
			{
				throw new NotSupportedException("runProps can not be null.");
			}
			if (genericTextRunProperties.FontRenderingEmSize != emSize)
			{
				GenericTextRunProperties element = new GenericTextRunProperties(genericTextRunProperties.Typeface, emSize, genericTextRunProperties.TextDecorations, genericTextRunProperties.ForegroundBrush, genericTextRunProperties.BackgroundBrush, genericTextRunProperties.BaselineAlignment, genericTextRunProperties.CultureInfo);
				_latestPosition = _formatRuns.SetValue(spanRider.CurrentPosition, num2 - spanRider.CurrentPosition, element, spanRider.SpanPosition);
				InvalidateMetrics();
			}
		}
	}

	public void SetCulture(CultureInfo culture)
	{
		SetCulture(culture, 0, _text.Length);
	}

	public void SetCulture(CultureInfo culture, int startIndex, int count)
	{
		if (culture == null)
		{
			throw new ArgumentNullException("culture");
		}
		int num = ValidateRange(startIndex, count);
		int num2 = startIndex;
		while (num2 < num)
		{
			SpanRider spanRider = new SpanRider(_formatRuns, _latestPosition, num2);
			num2 = Math.Min(num, num2 + spanRider.Length);
			if (!(spanRider.CurrentElement is GenericTextRunProperties genericTextRunProperties))
			{
				throw new NotSupportedException("runProps can not be null.");
			}
			if (genericTextRunProperties.CultureInfo != culture)
			{
				GenericTextRunProperties element = new GenericTextRunProperties(genericTextRunProperties.Typeface, genericTextRunProperties.FontRenderingEmSize, genericTextRunProperties.TextDecorations, genericTextRunProperties.ForegroundBrush, genericTextRunProperties.BackgroundBrush, genericTextRunProperties.BaselineAlignment, culture);
				_latestPosition = _formatRuns.SetValue(spanRider.CurrentPosition, num2 - spanRider.CurrentPosition, element, spanRider.SpanPosition);
				InvalidateMetrics();
			}
		}
	}

	public void SetFontWeight(FontWeight weight)
	{
		SetFontWeight(weight, 0, _text.Length);
	}

	public void SetFontWeight(FontWeight weight, int startIndex, int count)
	{
		int num = ValidateRange(startIndex, count);
		int num2 = startIndex;
		while (num2 < num)
		{
			SpanRider spanRider = new SpanRider(_formatRuns, _latestPosition, num2);
			num2 = Math.Min(num, num2 + spanRider.Length);
			if (!(spanRider.CurrentElement is GenericTextRunProperties { Typeface: var typeface } genericTextRunProperties))
			{
				throw new NotSupportedException("runProps can not be null.");
			}
			if (typeface.Weight != weight)
			{
				GenericTextRunProperties element = new GenericTextRunProperties(new Typeface(typeface.FontFamily, typeface.Style, weight), genericTextRunProperties.FontRenderingEmSize, genericTextRunProperties.TextDecorations, genericTextRunProperties.ForegroundBrush, genericTextRunProperties.BackgroundBrush, genericTextRunProperties.BaselineAlignment, genericTextRunProperties.CultureInfo);
				_latestPosition = _formatRuns.SetValue(spanRider.CurrentPosition, num2 - spanRider.CurrentPosition, element, spanRider.SpanPosition);
				InvalidateMetrics();
			}
		}
	}

	public void SetFontStyle(FontStyle style)
	{
		SetFontStyle(style, 0, _text.Length);
	}

	public void SetFontStyle(FontStyle style, int startIndex, int count)
	{
		int num = ValidateRange(startIndex, count);
		int num2 = startIndex;
		while (num2 < num)
		{
			SpanRider spanRider = new SpanRider(_formatRuns, _latestPosition, num2);
			num2 = Math.Min(num, num2 + spanRider.Length);
			if (!(spanRider.CurrentElement is GenericTextRunProperties { Typeface: var typeface } genericTextRunProperties))
			{
				throw new NotSupportedException("runProps can not be null.");
			}
			if (typeface.Style != style)
			{
				GenericTextRunProperties element = new GenericTextRunProperties(new Typeface(typeface.FontFamily, style, typeface.Weight), genericTextRunProperties.FontRenderingEmSize, genericTextRunProperties.TextDecorations, genericTextRunProperties.ForegroundBrush, genericTextRunProperties.BackgroundBrush, genericTextRunProperties.BaselineAlignment, genericTextRunProperties.CultureInfo);
				_latestPosition = _formatRuns.SetValue(spanRider.CurrentPosition, num2 - spanRider.CurrentPosition, element, spanRider.SpanPosition);
				InvalidateMetrics();
			}
		}
	}

	public void SetFontTypeface(Typeface typeface)
	{
		SetFontTypeface(typeface, 0, _text.Length);
	}

	public void SetFontTypeface(Typeface typeface, int startIndex, int count)
	{
		int num = ValidateRange(startIndex, count);
		int num2 = startIndex;
		while (num2 < num)
		{
			SpanRider spanRider = new SpanRider(_formatRuns, _latestPosition, num2);
			num2 = Math.Min(num, num2 + spanRider.Length);
			if (!(spanRider.CurrentElement is GenericTextRunProperties genericTextRunProperties))
			{
				throw new NotSupportedException("runProps can not be null.");
			}
			if (!(genericTextRunProperties.Typeface == typeface))
			{
				GenericTextRunProperties element = new GenericTextRunProperties(typeface, genericTextRunProperties.FontRenderingEmSize, genericTextRunProperties.TextDecorations, genericTextRunProperties.ForegroundBrush, genericTextRunProperties.BackgroundBrush, genericTextRunProperties.BaselineAlignment, genericTextRunProperties.CultureInfo);
				_latestPosition = _formatRuns.SetValue(spanRider.CurrentPosition, num2 - spanRider.CurrentPosition, element, spanRider.SpanPosition);
				InvalidateMetrics();
			}
		}
	}

	public void SetTextDecorations(TextDecorationCollection textDecorations)
	{
		SetTextDecorations(textDecorations, 0, _text.Length);
	}

	public void SetTextDecorations(TextDecorationCollection textDecorations, int startIndex, int count)
	{
		int num = ValidateRange(startIndex, count);
		int num2 = startIndex;
		while (num2 < num)
		{
			SpanRider spanRider = new SpanRider(_formatRuns, _latestPosition, num2);
			num2 = Math.Min(num, num2 + spanRider.Length);
			if (!(spanRider.CurrentElement is GenericTextRunProperties genericTextRunProperties))
			{
				throw new NotSupportedException("runProps can not be null.");
			}
			if (genericTextRunProperties.TextDecorations != textDecorations)
			{
				GenericTextRunProperties element = new GenericTextRunProperties(genericTextRunProperties.Typeface, genericTextRunProperties.FontRenderingEmSize, textDecorations, genericTextRunProperties.ForegroundBrush, genericTextRunProperties.BackgroundBrush, genericTextRunProperties.BaselineAlignment, genericTextRunProperties.CultureInfo);
				_latestPosition = _formatRuns.SetValue(spanRider.CurrentPosition, num2 - spanRider.CurrentPosition, element, spanRider.SpanPosition);
			}
		}
	}

	private LineEnumerator GetEnumerator()
	{
		return new LineEnumerator(this);
	}

	private void AdvanceLineOrigin(ref Point lineOrigin, TextLine currentLine)
	{
		double height = currentLine.Height;
		FlowDirection flowDirection = _defaultParaProps.FlowDirection;
		if ((uint)flowDirection <= 1u)
		{
			lineOrigin = lineOrigin.WithY(lineOrigin.Y + height);
		}
	}

	public void SetMaxTextWidths(double[] maxTextWidths)
	{
		if (maxTextWidths == null || maxTextWidths.Length == 0)
		{
			throw new ArgumentNullException("maxTextWidths");
		}
		_maxTextWidths = maxTextWidths;
		InvalidateMetrics();
	}

	public double[] GetMaxTextWidths()
	{
		if (_maxTextWidths == null)
		{
			return Array.Empty<double>();
		}
		return (double[])_maxTextWidths.Clone();
	}

	public Geometry? BuildGeometry(Point origin)
	{
		GeometryGroup accumulatedGeometry = null;
		Point lineOrigin = origin;
		DrawingGroup drawingGroup = new DrawingGroup();
		using (DrawingContext drawingContext = drawingGroup.Open())
		{
			using LineEnumerator lineEnumerator = GetEnumerator();
			while (lineEnumerator.MoveNext())
			{
				TextLine current = lineEnumerator.Current;
				if (current != null)
				{
					current.Draw(drawingContext, lineOrigin);
					AdvanceLineOrigin(ref lineOrigin, current);
				}
			}
		}
		Transform transform = new TranslateTransform(origin.X, origin.Y);
		CombineGeometryRecursive(drawingGroup, ref transform, ref accumulatedGeometry);
		return accumulatedGeometry;
	}

	public Geometry? BuildHighlightGeometry(Point origin)
	{
		return BuildHighlightGeometry(origin, 0, _text.Length);
	}

	public Geometry? BuildHighlightGeometry(Point origin, int startIndex, int count)
	{
		ValidateRange(startIndex, count);
		Geometry geometry = null;
		using (LineEnumerator lineEnumerator = GetEnumerator())
		{
			Point lineOrigin = origin;
			while (lineEnumerator.MoveNext())
			{
				TextLine current = lineEnumerator.Current;
				int num = Math.Max(lineEnumerator.Position, startIndex);
				int num2 = Math.Min(lineEnumerator.Position + lineEnumerator.Length, startIndex + count);
				if (num < num2)
				{
					IReadOnlyList<TextBounds> textBounds = current.GetTextBounds(num, num2 - num);
					if (textBounds.Count > 0)
					{
						foreach (TextBounds item in textBounds)
						{
							Rect rect = item.Rectangle;
							if (FlowDirection == FlowDirection.RightToLeft)
							{
								rect = rect.WithX(lineEnumerator.CurrentParagraphWidth - rect.Right);
							}
							rect = new Rect(new Point(rect.X + lineOrigin.X, rect.Y + lineOrigin.Y), rect.Size);
							RectangleGeometry rectangleGeometry = new RectangleGeometry(rect);
							geometry = ((geometry != null) ? Geometry.Combine(geometry, rectangleGeometry, GeometryCombineMode.Union) : rectangleGeometry);
						}
					}
				}
				AdvanceLineOrigin(ref lineOrigin, current);
			}
		}
		if (geometry?.PlatformImpl == null || (geometry.PlatformImpl.Bounds.Width == 0.0 && geometry.PlatformImpl.Bounds.Height == 0.0))
		{
			return null;
		}
		return geometry;
	}

	internal void Draw(DrawingContext drawingContext, Point origin)
	{
		Point lineOrigin = origin;
		if (_metrics != null && !double.IsNaN(_metrics.Extent))
		{
			using (LineEnumerator lineEnumerator = GetEnumerator())
			{
				while (lineEnumerator.MoveNext())
				{
					TextLine current = lineEnumerator.Current;
					current.Draw(drawingContext, lineOrigin);
					AdvanceLineOrigin(ref lineOrigin, current);
				}
				return;
			}
		}
		_metrics = DrawAndCalculateMetrics(drawingContext, origin, getBlackBoxMetrics: true);
	}

	private void CombineGeometryRecursive(Drawing drawing, ref Transform? transform, ref GeometryGroup? accumulatedGeometry)
	{
		if (drawing is DrawingGroup drawingGroup)
		{
			transform = drawingGroup.Transform;
			DrawingCollection children = drawingGroup.Children;
			if (children == null)
			{
				return;
			}
			{
				foreach (Drawing item in children)
				{
					CombineGeometryRecursive(item, ref transform, ref accumulatedGeometry);
				}
				return;
			}
		}
		if (drawing is GlyphRunDrawing glyphRunDrawing)
		{
			GlyphRun glyphRun = glyphRunDrawing.GlyphRun;
			if (glyphRun != null)
			{
				Geometry geometry = glyphRun.BuildGeometry();
				geometry.Transform = transform;
				if (accumulatedGeometry == null)
				{
					accumulatedGeometry = new GeometryGroup
					{
						FillRule = FillRule.NonZero
					};
				}
				accumulatedGeometry.Children.Add(geometry);
			}
		}
		else
		{
			if (!(drawing is GeometryDrawing geometryDrawing))
			{
				return;
			}
			Geometry geometry2 = geometryDrawing.Geometry;
			if (geometry2 == null)
			{
				return;
			}
			geometry2.Transform = transform;
			if (geometry2 is LineGeometry { Bounds: var rect })
			{
				if (rect.Height == 0.0)
				{
					rect = rect.WithHeight(geometryDrawing.Pen?.Thickness ?? 0.0);
				}
				else if (rect.Width == 0.0)
				{
					rect = rect.WithWidth(geometryDrawing.Pen?.Thickness ?? 0.0);
				}
				geometry2 = new RectangleGeometry(rect);
			}
			if (accumulatedGeometry == null)
			{
				accumulatedGeometry = new GeometryGroup
				{
					FillRule = FillRule.NonZero
				};
			}
			accumulatedGeometry.Children.Add(geometry2);
		}
	}

	private CachedMetrics DrawAndCalculateMetrics(DrawingContext? drawingContext, Point drawingOffset, bool getBlackBoxMetrics)
	{
		CachedMetrics cachedMetrics = new CachedMetrics();
		if (_text.Length == 0)
		{
			return cachedMetrics;
		}
		using (LineEnumerator lineEnumerator = GetEnumerator())
		{
			bool flag = true;
			double num;
			double num2 = (num = double.MaxValue);
			double num3;
			double num4 = (num3 = double.MinValue);
			Point lineOrigin = new Point(0.0, 0.0);
			double num5 = double.MaxValue;
			while (lineEnumerator.MoveNext())
			{
				TextLine current = lineEnumerator.Current;
				if (drawingContext != null)
				{
					current.Draw(drawingContext, new Point(lineOrigin.X + drawingOffset.X, lineOrigin.Y + drawingOffset.Y));
				}
				if (getBlackBoxMetrics)
				{
					double val = lineOrigin.X + current.Start + current.OverhangLeading;
					double val2 = lineOrigin.X + current.Start + current.Width - current.OverhangTrailing;
					double num6 = lineOrigin.Y + current.Height + current.OverhangAfter;
					double val3 = num6 - current.Extent;
					num2 = Math.Min(num2, val);
					num4 = Math.Max(num4, val2);
					num3 = Math.Max(num3, num6);
					num = Math.Min(num, val3);
					cachedMetrics.OverhangAfter = current.OverhangAfter;
				}
				cachedMetrics.Height += current.Height;
				cachedMetrics.Width = Math.Max(cachedMetrics.Width, current.Width);
				cachedMetrics.WidthIncludingTrailingWhitespace = Math.Max(cachedMetrics.WidthIncludingTrailingWhitespace, current.WidthIncludingTrailingWhitespace);
				num5 = Math.Min(num5, current.Start);
				if (flag)
				{
					cachedMetrics.Baseline = current.Baseline;
					flag = false;
				}
				AdvanceLineOrigin(ref lineOrigin, current);
			}
			if (getBlackBoxMetrics)
			{
				cachedMetrics.Extent = num3 - num;
				cachedMetrics.OverhangLeading = num2 - num5;
				cachedMetrics.OverhangTrailing = cachedMetrics.Width - (num4 - num5);
			}
			else
			{
				cachedMetrics.Extent = double.NaN;
			}
		}
		return cachedMetrics;
	}
}
