using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using AvaloniaEdit.Document;
using AvaloniaEdit.Editing;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Rendering;

public sealed class BackgroundGeometryBuilder
{
	private double _cornerRadius;

	private readonly PathFigures _figures = new PathFigures();

	private PathFigure _figure;

	private int _insertionIndex;

	private double _lastTop;

	private double _lastBottom;

	private double _lastLeft;

	private double _lastRight;

	public double CornerRadius
	{
		get
		{
			return _cornerRadius;
		}
		set
		{
			_cornerRadius = value;
		}
	}

	public bool AlignToWholePixels { get; set; }

	public double BorderThickness { get; set; }

	public bool ExtendToFullWidthAtLineEnd { get; set; }

	public void AddSegment(TextView textView, ISegment segment)
	{
		if (textView == null)
		{
			throw new ArgumentNullException("textView");
		}
		Size pixelSize = PixelSnapHelpers.GetPixelSize(textView);
		foreach (Rect item in GetRectsForSegment(textView, segment, ExtendToFullWidthAtLineEnd))
		{
			AddRectangle(pixelSize, item);
		}
	}

	public void AddRectangle(TextView textView, Rect rectangle)
	{
		AddRectangle(PixelSnapHelpers.GetPixelSize(textView), rectangle);
	}

	private void AddRectangle(Size pixelSize, Rect r)
	{
		if (AlignToWholePixels)
		{
			double num = 0.5 * BorderThickness;
			AddRectangle(PixelSnapHelpers.Round(r.Left - num, pixelSize.Width) + num, PixelSnapHelpers.Round(r.Top - num, pixelSize.Height) + num, PixelSnapHelpers.Round(r.Right + num, pixelSize.Width) - num, PixelSnapHelpers.Round(r.Bottom + num, pixelSize.Height) - num);
		}
		else
		{
			AddRectangle(r.Left, r.Top, r.Right, r.Bottom);
		}
	}

	public static IEnumerable<Rect> GetRectsForSegment(TextView textView, ISegment segment, bool extendToFullWidthAtLineEnd = false)
	{
		if (textView == null)
		{
			throw new ArgumentNullException("textView");
		}
		if (segment == null)
		{
			throw new ArgumentNullException("segment");
		}
		return GetRectsForSegmentImpl(textView, segment, extendToFullWidthAtLineEnd);
	}

	private static IEnumerable<Rect> GetRectsForSegmentImpl(TextView textView, ISegment segment, bool extendToFullWidthAtLineEnd)
	{
		int segmentStart = segment.Offset;
		int segmentEnd = segment.Offset + segment.Length;
		segmentStart = segmentStart.CoerceValue(0, textView.Document.TextLength);
		segmentEnd = segmentEnd.CoerceValue(0, textView.Document.TextLength);
		TextViewPosition start;
		TextViewPosition end;
		if (segment is SelectionSegment)
		{
			SelectionSegment selectionSegment = (SelectionSegment)segment;
			start = new TextViewPosition(textView.Document.GetLocation(selectionSegment.StartOffset), selectionSegment.StartVisualColumn);
			end = new TextViewPosition(textView.Document.GetLocation(selectionSegment.EndOffset), selectionSegment.EndVisualColumn);
		}
		else
		{
			start = new TextViewPosition(textView.Document.GetLocation(segmentStart));
			end = new TextViewPosition(textView.Document.GetLocation(segmentEnd));
		}
		foreach (VisualLine visualLine in textView.VisualLines)
		{
			int offset = visualLine.FirstDocumentLine.Offset;
			if (offset > segmentEnd)
			{
				break;
			}
			int num = visualLine.LastDocumentLine.Offset + visualLine.LastDocumentLine.Length;
			if (num < segmentStart)
			{
				continue;
			}
			int segmentStartVc = ((segmentStart >= offset) ? visualLine.ValidateVisualColumn(start, extendToFullWidthAtLineEnd) : 0);
			int segmentEndVc = ((segmentEnd <= num) ? visualLine.ValidateVisualColumn(end, extendToFullWidthAtLineEnd) : (extendToFullWidthAtLineEnd ? int.MaxValue : visualLine.VisualLengthWithEndOfLineMarker));
			foreach (Rect item in ProcessTextLines(textView, visualLine, segmentStartVc, segmentEndVc))
			{
				yield return item;
			}
		}
	}

	public static IEnumerable<Rect> GetRectsFromVisualSegment(TextView textView, VisualLine line, int startVc, int endVc)
	{
		if (textView == null)
		{
			throw new ArgumentNullException("textView");
		}
		if (line == null)
		{
			throw new ArgumentNullException("line");
		}
		return ProcessTextLines(textView, line, startVc, endVc);
	}

	private static IEnumerable<Rect> ProcessTextLines(TextView textView, VisualLine visualLine, int segmentStartVc, int segmentEndVc)
	{
		TextLine lastTextLine = visualLine.TextLines.Last();
		Vector scrollOffset = textView.ScrollOffset;
		for (int i = 0; i < visualLine.TextLines.Count; i++)
		{
			TextLine line = visualLine.TextLines[i];
			double y = visualLine.GetTextLineVisualYPosition(line, VisualYPosition.TextTop);
			int textLineVisualStartColumn = visualLine.GetTextLineVisualStartColumn(line);
			int visualEndCol = textLineVisualStartColumn + line.Length;
			visualEndCol = ((line != lastTextLine) ? (visualEndCol - line.TrailingWhitespaceLength) : (visualEndCol - 1));
			if (segmentEndVc < textLineVisualStartColumn)
			{
				break;
			}
			if (lastTextLine != line && segmentStartVc > visualEndCol)
			{
				continue;
			}
			int num = Math.Max(segmentStartVc, textLineVisualStartColumn);
			int num2 = Math.Min(segmentEndVc, visualEndCol);
			y -= scrollOffset.Y;
			Rect rect = default(Rect);
			if (num == num2)
			{
				double textLineVisualXPosition = visualLine.GetTextLineVisualXPosition(line, num);
				textLineVisualXPosition -= scrollOffset.X;
				if ((num2 == visualEndCol && i < visualLine.TextLines.Count - 1 && segmentEndVc > num2 && line.TrailingWhitespaceLength == 0) || (num == textLineVisualStartColumn && i > 0 && segmentStartVc < num && visualLine.TextLines[i - 1].TrailingWhitespaceLength == 0))
				{
					continue;
				}
				rect = new Rect(textLineVisualXPosition, y, textView.EmptyLineSelectionWidth, line.Height);
			}
			else if (num <= visualEndCol)
			{
				foreach (TextBounds textBound in line.GetTextBounds(num, num2 - num))
				{
					double left = textBound.Rectangle.Left - scrollOffset.X;
					double right = textBound.Rectangle.Right - scrollOffset.X;
					if (rect != default(Rect))
					{
						yield return rect;
					}
					rect = new Rect(Math.Min(left, right), y, Math.Abs(right - left), line.Height);
				}
			}
			if (segmentEndVc > visualEndCol)
			{
				double num3 = ((segmentStartVc <= visualLine.VisualLengthWithEndOfLineMarker) ? ((line == lastTextLine) ? line.WidthIncludingTrailingWhitespace : line.Width) : visualLine.GetTextLineVisualXPosition(lastTextLine, segmentStartVc));
				double num4 = ((line == lastTextLine && segmentEndVc != int.MaxValue) ? visualLine.GetTextLineVisualXPosition(lastTextLine, segmentEndVc) : Math.Max(((IScrollable)textView).Extent.Width, ((IScrollable)textView).Viewport.Width));
				Rect extendSelection = new Rect(Math.Min(num3, num4), y, Math.Abs(num4 - num3), line.Height);
				if (rect != default(Rect))
				{
					if (extendSelection.Intersects(rect))
					{
						rect.Union(extendSelection);
						yield return rect;
					}
					else
					{
						yield return rect;
						yield return extendSelection;
					}
				}
				else
				{
					yield return extendSelection;
				}
			}
			else
			{
				yield return rect;
			}
		}
	}

	public void AddRectangle(double left, double top, double right, double bottom)
	{
		if (!top.IsClose(_lastBottom))
		{
			CloseFigure();
		}
		if (_figure == null)
		{
			_figure = new PathFigure();
			_figure.StartPoint = new Point(left, top + _cornerRadius);
			if (Math.Abs(left - right) > _cornerRadius)
			{
				_figure.Segments.Add(MakeArc(left + _cornerRadius, top, SweepDirection.Clockwise));
				_figure.Segments.Add(MakeLineSegment(right - _cornerRadius, top));
				_figure.Segments.Add(MakeArc(right, top + _cornerRadius, SweepDirection.Clockwise));
			}
			_figure.Segments.Add(MakeLineSegment(right, bottom - _cornerRadius));
			_insertionIndex = _figure.Segments.Count;
		}
		else
		{
			if (!_lastRight.IsClose(right))
			{
				double num = ((right < _lastRight) ? (0.0 - _cornerRadius) : _cornerRadius);
				SweepDirection dir = ((right < _lastRight) ? SweepDirection.Clockwise : SweepDirection.CounterClockwise);
				SweepDirection dir2 = ((!(right < _lastRight)) ? SweepDirection.Clockwise : SweepDirection.CounterClockwise);
				_figure.Segments.Insert(_insertionIndex++, MakeArc(_lastRight + num, _lastBottom, dir));
				_figure.Segments.Insert(_insertionIndex++, MakeLineSegment(right - num, top));
				_figure.Segments.Insert(_insertionIndex++, MakeArc(right, top + _cornerRadius, dir2));
			}
			_figure.Segments.Insert(_insertionIndex++, MakeLineSegment(right, bottom - _cornerRadius));
			_figure.Segments.Insert(_insertionIndex, MakeLineSegment(_lastLeft, _lastTop + _cornerRadius));
			if (!_lastLeft.IsClose(left))
			{
				double num2 = ((left < _lastLeft) ? _cornerRadius : (0.0 - _cornerRadius));
				SweepDirection dir3 = ((!(left < _lastLeft)) ? SweepDirection.Clockwise : SweepDirection.CounterClockwise);
				SweepDirection dir4 = ((left < _lastLeft) ? SweepDirection.Clockwise : SweepDirection.CounterClockwise);
				_figure.Segments.Insert(_insertionIndex, MakeArc(_lastLeft, _lastBottom - _cornerRadius, dir3));
				_figure.Segments.Insert(_insertionIndex, MakeLineSegment(_lastLeft - num2, _lastBottom));
				_figure.Segments.Insert(_insertionIndex, MakeArc(left + num2, _lastBottom, dir4));
			}
		}
		_lastTop = top;
		_lastBottom = bottom;
		_lastLeft = left;
		_lastRight = right;
	}

	private ArcSegment MakeArc(double x, double y, SweepDirection dir)
	{
		return new ArcSegment
		{
			Point = new Point(x, y),
			Size = new Size(CornerRadius, CornerRadius),
			SweepDirection = dir
		};
	}

	private static LineSegment MakeLineSegment(double x, double y)
	{
		return new LineSegment
		{
			Point = new Point(x, y)
		};
	}

	public void CloseFigure()
	{
		if (_figure != null)
		{
			_figure.Segments.Insert(_insertionIndex, MakeLineSegment(_lastLeft, _lastTop + _cornerRadius));
			if (Math.Abs(_lastLeft - _lastRight) > _cornerRadius)
			{
				_figure.Segments.Insert(_insertionIndex, MakeArc(_lastLeft, _lastBottom - _cornerRadius, SweepDirection.Clockwise));
				_figure.Segments.Insert(_insertionIndex, MakeLineSegment(_lastLeft + _cornerRadius, _lastBottom));
				_figure.Segments.Insert(_insertionIndex, MakeArc(_lastRight - _cornerRadius, _lastBottom, SweepDirection.Clockwise));
			}
			_figure.IsClosed = true;
			_figures.Add(_figure);
			_figure = null;
		}
	}

	public Geometry CreateGeometry()
	{
		CloseFigure();
		if (_figures.Count == 0)
		{
			return null;
		}
		return new PathGeometry
		{
			Figures = _figures
		};
	}
}
