using System;
using System.Collections.Generic;
using AvaloniaEdit.Document;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Editing;

public sealed class SimpleSelection : Selection
{
	private readonly TextViewPosition _start;

	private readonly TextViewPosition _end;

	private readonly int _startOffset;

	private readonly int _endOffset;

	public override IEnumerable<SelectionSegment> Segments => ExtensionMethods.Sequence(new SelectionSegment(_startOffset, _start.VisualColumn, _endOffset, _end.VisualColumn));

	public override ISegment SurroundingSegment => new SelectionSegment(_startOffset, _endOffset);

	public override TextViewPosition StartPosition => _start;

	public override TextViewPosition EndPosition => _end;

	public override bool IsEmpty
	{
		get
		{
			if (_startOffset == _endOffset)
			{
				return _start.VisualColumn == _end.VisualColumn;
			}
			return false;
		}
	}

	public override int Length => Math.Abs(_endOffset - _startOffset);

	internal SimpleSelection(TextArea textArea, TextViewPosition start, TextViewPosition end)
		: base(textArea)
	{
		_start = start;
		_end = end;
		_startOffset = textArea.Document.GetOffset(start.Location);
		_endOffset = textArea.Document.GetOffset(end.Location);
	}

	public override void ReplaceSelectionWithText(string newText)
	{
		if (newText == null)
		{
			throw new ArgumentNullException("newText");
		}
		using (base.TextArea.Document.RunUpdate())
		{
			ISegment[] deletableSegments = base.TextArea.GetDeletableSegments(SurroundingSegment);
			for (int num = deletableSegments.Length - 1; num >= 0; num--)
			{
				if (num == deletableSegments.Length - 1)
				{
					if (deletableSegments[num].Offset == SurroundingSegment.Offset && deletableSegments[num].Length == SurroundingSegment.Length)
					{
						newText = AddSpacesIfRequired(newText, _start, _end);
					}
					if (string.IsNullOrEmpty(newText))
					{
						base.TextArea.Caret.Position = ((_start.CompareTo(_end) <= 0) ? _start : _end);
					}
					else
					{
						base.TextArea.Caret.Offset = deletableSegments[num].EndOffset;
					}
					base.TextArea.Document.Replace(deletableSegments[num], newText);
				}
				else
				{
					base.TextArea.Document.Remove(deletableSegments[num]);
				}
			}
			if (deletableSegments.Length != 0)
			{
				base.TextArea.ClearSelection();
			}
		}
	}

	public override Selection UpdateOnDocumentChange(DocumentChangeEventArgs e)
	{
		if (e == null)
		{
			throw new ArgumentNullException("e");
		}
		int num;
		int num2;
		if (_startOffset <= _endOffset)
		{
			num = e.GetNewOffset(_startOffset);
			num2 = Math.Max(num, e.GetNewOffset(_endOffset, AnchorMovementType.BeforeInsertion));
		}
		else
		{
			num2 = e.GetNewOffset(_endOffset);
			num = Math.Max(num2, e.GetNewOffset(_startOffset, AnchorMovementType.BeforeInsertion));
		}
		return Selection.Create(base.TextArea, new TextViewPosition(base.TextArea.Document.GetLocation(num), _start.VisualColumn), new TextViewPosition(base.TextArea.Document.GetLocation(num2), _end.VisualColumn));
	}

	public override Selection SetEndpoint(TextViewPosition endPosition)
	{
		return Selection.Create(base.TextArea, _start, endPosition);
	}

	public override Selection StartSelectionOrSetEndpoint(TextViewPosition startPosition, TextViewPosition endPosition)
	{
		if (base.TextArea.Document == null)
		{
			throw ThrowUtil.NoDocumentAssigned();
		}
		return Selection.Create(base.TextArea, _start, endPosition);
	}

	public override int GetHashCode()
	{
		return _startOffset * 27811 + _endOffset + base.TextArea.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		if (!(obj is SimpleSelection simpleSelection))
		{
			return false;
		}
		if (_start.Equals(simpleSelection._start) && _end.Equals(simpleSelection._end) && _startOffset == simpleSelection._startOffset && _endOffset == simpleSelection._endOffset)
		{
			return base.TextArea == simpleSelection.TextArea;
		}
		return false;
	}

	public override string ToString()
	{
		return "[SimpleSelection Start=" + _start.ToString() + " End=" + _end.ToString() + "]";
	}
}
