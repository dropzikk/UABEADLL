using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avalonia;
using Avalonia.Media.TextFormatting;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Editing;

public sealed class RectangleSelection : Selection
{
	public static readonly RoutedCommand BoxSelectLeftByCharacter = new RoutedCommand("BoxSelectLeftByCharacter");

	public static readonly RoutedCommand BoxSelectRightByCharacter = new RoutedCommand("BoxSelectRightByCharacter");

	public static readonly RoutedCommand BoxSelectLeftByWord = new RoutedCommand("BoxSelectLeftByWord");

	public static readonly RoutedCommand BoxSelectRightByWord = new RoutedCommand("BoxSelectRightByWord");

	public static readonly RoutedCommand BoxSelectUpByLine = new RoutedCommand("BoxSelectUpByLine");

	public static readonly RoutedCommand BoxSelectDownByLine = new RoutedCommand("BoxSelectDownByLine");

	public static readonly RoutedCommand BoxSelectToLineStart = new RoutedCommand("BoxSelectToLineStart");

	public static readonly RoutedCommand BoxSelectToLineEnd = new RoutedCommand("BoxSelectToLineEnd");

	private TextDocument _document;

	private readonly int _startLine;

	private readonly int _endLine;

	private readonly double _startXPos;

	private readonly double _endXPos;

	private readonly int _topLeftOffset;

	private readonly int _bottomRightOffset;

	private readonly List<SelectionSegment> _segments = new List<SelectionSegment>();

	public override int Length => Segments.Sum((SelectionSegment s) => s.Length);

	public override bool EnableVirtualSpace => true;

	public override ISegment SurroundingSegment => new SimpleSegment(_topLeftOffset, _bottomRightOffset - _topLeftOffset);

	public override IEnumerable<SelectionSegment> Segments => _segments;

	public override TextViewPosition StartPosition { get; }

	public override TextViewPosition EndPosition { get; }

	public RectangleSelection(TextArea textArea, TextViewPosition start, TextViewPosition end)
		: base(textArea)
	{
		InitDocument();
		_startLine = start.Line;
		_endLine = end.Line;
		_startXPos = GetXPos(textArea, start);
		_endXPos = GetXPos(textArea, end);
		CalculateSegments();
		_topLeftOffset = _segments.First().StartOffset;
		_bottomRightOffset = _segments.Last().EndOffset;
		StartPosition = start;
		EndPosition = end;
	}

	private RectangleSelection(TextArea textArea, int startLine, double startXPos, TextViewPosition end)
		: base(textArea)
	{
		InitDocument();
		_startLine = startLine;
		_endLine = end.Line;
		_startXPos = startXPos;
		_endXPos = GetXPos(textArea, end);
		CalculateSegments();
		_topLeftOffset = _segments.First().StartOffset;
		_bottomRightOffset = _segments.Last().EndOffset;
		StartPosition = GetStart();
		EndPosition = end;
	}

	private RectangleSelection(TextArea textArea, TextViewPosition start, int endLine, double endXPos)
		: base(textArea)
	{
		InitDocument();
		_startLine = start.Line;
		_endLine = endLine;
		_startXPos = GetXPos(textArea, start);
		_endXPos = endXPos;
		CalculateSegments();
		_topLeftOffset = _segments.First().StartOffset;
		_bottomRightOffset = _segments.Last().EndOffset;
		StartPosition = start;
		EndPosition = GetEnd();
	}

	private void InitDocument()
	{
		_document = base.TextArea.Document;
		if (_document == null)
		{
			throw ThrowUtil.NoDocumentAssigned();
		}
	}

	private static double GetXPos(TextArea textArea, TextViewPosition pos)
	{
		DocumentLine lineByNumber = textArea.Document.GetLineByNumber(pos.Line);
		VisualLine orConstructVisualLine = textArea.TextView.GetOrConstructVisualLine(lineByNumber);
		int visualColumn = orConstructVisualLine.ValidateVisualColumn(pos, allowVirtualSpace: true);
		TextLine textLine = orConstructVisualLine.GetTextLine(visualColumn, pos.IsAtEndOfLine);
		return orConstructVisualLine.GetTextLineVisualXPosition(textLine, visualColumn);
	}

	private void CalculateSegments()
	{
		DocumentLine documentLine = _document.GetLineByNumber(Math.Min(_startLine, _endLine));
		do
		{
			VisualLine orConstructVisualLine = base.TextArea.TextView.GetOrConstructVisualLine(documentLine);
			int visualColumn = orConstructVisualLine.GetVisualColumn(new Point(_startXPos, 0.0), allowVirtualSpace: true);
			int visualColumn2 = orConstructVisualLine.GetVisualColumn(new Point(_endXPos, 0.0), allowVirtualSpace: true);
			int offset = orConstructVisualLine.FirstDocumentLine.Offset;
			int startOffset = offset + orConstructVisualLine.GetRelativeOffset(visualColumn);
			int endOffset = offset + orConstructVisualLine.GetRelativeOffset(visualColumn2);
			_segments.Add(new SelectionSegment(startOffset, visualColumn, endOffset, visualColumn2));
			documentLine = orConstructVisualLine.LastDocumentLine.NextLine;
		}
		while (documentLine != null && documentLine.LineNumber <= Math.Max(_startLine, _endLine));
	}

	private TextViewPosition GetStart()
	{
		SelectionSegment selectionSegment = ((_startLine < _endLine) ? _segments.First() : _segments.Last());
		if (_startXPos < _endXPos)
		{
			return new TextViewPosition(_document.GetLocation(selectionSegment.StartOffset), selectionSegment.StartVisualColumn);
		}
		return new TextViewPosition(_document.GetLocation(selectionSegment.EndOffset), selectionSegment.EndVisualColumn);
	}

	private TextViewPosition GetEnd()
	{
		SelectionSegment selectionSegment = ((_startLine < _endLine) ? _segments.Last() : _segments.First());
		if (_startXPos < _endXPos)
		{
			return new TextViewPosition(_document.GetLocation(selectionSegment.EndOffset), selectionSegment.EndVisualColumn);
		}
		return new TextViewPosition(_document.GetLocation(selectionSegment.StartOffset), selectionSegment.StartVisualColumn);
	}

	public override string GetText()
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (SelectionSegment segment in Segments)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.AppendLine();
			}
			stringBuilder.Append(_document.GetText(segment));
		}
		return stringBuilder.ToString();
	}

	public override Selection StartSelectionOrSetEndpoint(TextViewPosition startPosition, TextViewPosition endPosition)
	{
		return SetEndpoint(endPosition);
	}

	public override bool Equals(object obj)
	{
		if (obj is RectangleSelection rectangleSelection && rectangleSelection.TextArea == base.TextArea && rectangleSelection._topLeftOffset == _topLeftOffset && rectangleSelection._bottomRightOffset == _bottomRightOffset && rectangleSelection._startLine == _startLine && rectangleSelection._endLine == _endLine && rectangleSelection._startXPos == _startXPos)
		{
			return rectangleSelection._endXPos == _endXPos;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return _topLeftOffset ^ _bottomRightOffset;
	}

	public override Selection SetEndpoint(TextViewPosition endPosition)
	{
		return new RectangleSelection(base.TextArea, _startLine, _startXPos, endPosition);
	}

	private int GetVisualColumnFromXPos(int line, double xPos)
	{
		return base.TextArea.TextView.GetOrConstructVisualLine(base.TextArea.Document.GetLineByNumber(line)).GetVisualColumn(new Point(xPos, 0.0), allowVirtualSpace: true);
	}

	public override Selection UpdateOnDocumentChange(DocumentChangeEventArgs e)
	{
		TextLocation location = base.TextArea.Document.GetLocation(e.GetNewOffset(_topLeftOffset, AnchorMovementType.AfterInsertion));
		TextLocation location2 = base.TextArea.Document.GetLocation(e.GetNewOffset(_bottomRightOffset, AnchorMovementType.BeforeInsertion));
		return new RectangleSelection(base.TextArea, new TextViewPosition(location, GetVisualColumnFromXPos(location.Line, _startXPos)), new TextViewPosition(location2, GetVisualColumnFromXPos(location2.Line, _endXPos)));
	}

	public override void ReplaceSelectionWithText(string newText)
	{
		if (newText == null)
		{
			throw new ArgumentNullException("newText");
		}
		using (base.TextArea.Document.RunUpdate())
		{
			int num = 0;
			int num2 = Math.Min(_topLeftOffset, _bottomRightOffset);
			int insertionLength;
			TextViewPosition textViewPosition;
			if (NewLineFinder.NextNewLine(newText, 0) == SimpleSegment.Invalid)
			{
				foreach (SelectionSegment item in Segments.Reverse())
				{
					ReplaceSingleLineText(base.TextArea, item, newText, out insertionLength);
					num = insertionLength;
				}
				textViewPosition = new TextViewPosition(_document.GetLocation(num2 + num));
				base.TextArea.Selection = new RectangleSelection(base.TextArea, textViewPosition, Math.Max(_startLine, _endLine), GetXPos(base.TextArea, textViewPosition));
			}
			else
			{
				string[] array = newText.Split(NewLineFinder.NewlineStrings, _segments.Count, StringSplitOptions.None);
				for (int num3 = array.Length - 1; num3 >= 0; num3--)
				{
					ReplaceSingleLineText(base.TextArea, _segments[num3], array[num3], out insertionLength);
					num = insertionLength;
				}
				textViewPosition = new TextViewPosition(_document.GetLocation(num2 + num));
				base.TextArea.ClearSelection();
			}
			base.TextArea.Caret.Position = new TextViewPosition(Math.Max(_startLine, _endLine), textViewPosition.Column);
		}
	}

	private void ReplaceSingleLineText(TextArea textArea, SelectionSegment lineSegment, string newText, out int insertionLength)
	{
		if (lineSegment.Length == 0)
		{
			if (newText.Length > 0 && textArea.ReadOnlySectionProvider.CanInsert(lineSegment.StartOffset))
			{
				newText = AddSpacesIfRequired(newText, new TextViewPosition(_document.GetLocation(lineSegment.StartOffset), lineSegment.StartVisualColumn), new TextViewPosition(_document.GetLocation(lineSegment.EndOffset), lineSegment.EndVisualColumn));
				textArea.Document.Insert(lineSegment.StartOffset, newText);
			}
		}
		else
		{
			ISegment[] deletableSegments = textArea.GetDeletableSegments(lineSegment);
			for (int num = deletableSegments.Length - 1; num >= 0; num--)
			{
				if (num == deletableSegments.Length - 1)
				{
					if (deletableSegments[num].Offset == SurroundingSegment.Offset && deletableSegments[num].Length == SurroundingSegment.Length)
					{
						newText = AddSpacesIfRequired(newText, new TextViewPosition(_document.GetLocation(lineSegment.StartOffset), lineSegment.StartVisualColumn), new TextViewPosition(_document.GetLocation(lineSegment.EndOffset), lineSegment.EndVisualColumn));
					}
					textArea.Document.Replace(deletableSegments[num], newText);
				}
				else
				{
					textArea.Document.Remove(deletableSegments[num]);
				}
			}
		}
		insertionLength = newText.Length;
	}

	public static bool PerformRectangularPaste(TextArea textArea, TextViewPosition startPosition, string text, bool selectInsertedText)
	{
		if (textArea == null)
		{
			throw new ArgumentNullException("textArea");
		}
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		int num = text.AsEnumerable().Count((char c) => c == '\n');
		TextLocation textLocation = new TextLocation(startPosition.Line + num, startPosition.Column);
		if (textLocation.Line <= textArea.Document.LineCount)
		{
			int offset = textArea.Document.GetOffset(textLocation);
			if (textArea.Selection.EnableVirtualSpace || textArea.Document.GetLocation(offset) == textLocation)
			{
				new RectangleSelection(textArea, startPosition, textLocation.Line, GetXPos(textArea, startPosition)).ReplaceSelectionWithText(text);
				if (selectInsertedText && textArea.Selection is RectangleSelection)
				{
					RectangleSelection rectangleSelection = (RectangleSelection)textArea.Selection;
					textArea.Selection = new RectangleSelection(textArea, startPosition, rectangleSelection._endLine, rectangleSelection._endXPos);
				}
				return true;
			}
		}
		return false;
	}

	public override string ToString()
	{
		return $"[RectangleSelection {_startLine} {_topLeftOffset} {_startXPos} to {_endLine} {_bottomRightOffset} {_endXPos}]";
	}
}
