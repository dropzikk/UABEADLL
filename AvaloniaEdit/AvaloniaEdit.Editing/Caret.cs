using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Avalonia.Threading;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Editing;

public sealed class Caret
{
	private const double CaretWidth = 0.5;

	private readonly TextArea _textArea;

	private readonly TextView _textView;

	private readonly CaretLayer _caretAdorner;

	private bool _visible;

	private TextViewPosition _position;

	private bool _isInVirtualSpace;

	private int _storedCaretOffset;

	private bool _raisePositionChangedOnUpdateFinished;

	private bool _visualColumnValid;

	internal const double MinimumDistanceToViewBorder = 0.0;

	private bool _showScheduled;

	private bool _hasWin32Caret;

	public TextViewPosition Position
	{
		get
		{
			ValidateVisualColumn();
			return _position;
		}
		set
		{
			if (_position != value)
			{
				_position = value;
				_storedCaretOffset = -1;
				ValidatePosition();
				InvalidateVisualColumn();
				RaisePositionChanged();
				if (_visible)
				{
					Show();
				}
			}
		}
	}

	internal TextViewPosition NonValidatedPosition => _position;

	public TextLocation Location
	{
		get
		{
			return _position.Location;
		}
		set
		{
			Position = new TextViewPosition(value);
		}
	}

	public int Line
	{
		get
		{
			return _position.Line;
		}
		set
		{
			Position = new TextViewPosition(value, _position.Column);
		}
	}

	public int Column
	{
		get
		{
			return _position.Column;
		}
		set
		{
			Position = new TextViewPosition(_position.Line, value);
		}
	}

	public int VisualColumn
	{
		get
		{
			ValidateVisualColumn();
			return _position.VisualColumn;
		}
		set
		{
			Position = new TextViewPosition(_position.Line, _position.Column, value);
		}
	}

	public bool IsInVirtualSpace
	{
		get
		{
			ValidateVisualColumn();
			return _isInVirtualSpace;
		}
	}

	public int Offset
	{
		get
		{
			return _textArea.Document?.GetOffset(_position.Location) ?? 0;
		}
		set
		{
			TextDocument document = _textArea.Document;
			if (document != null)
			{
				Position = new TextViewPosition(document.GetLocation(value));
				DesiredXPos = double.NaN;
			}
		}
	}

	public double DesiredXPos { get; set; } = double.NaN;

	public IBrush CaretBrush
	{
		get
		{
			return _caretAdorner.CaretBrush;
		}
		set
		{
			_caretAdorner.CaretBrush = value;
		}
	}

	public event EventHandler PositionChanged;

	internal Caret(TextArea textArea)
	{
		_textArea = textArea;
		_textView = textArea.TextView;
		_position = new TextViewPosition(1, 1, 0);
		_caretAdorner = new CaretLayer(textArea);
		_textView.InsertLayer(_caretAdorner, KnownLayer.Caret, LayerInsertionPosition.Replace);
		_textView.VisualLinesChanged += TextView_VisualLinesChanged;
		_textView.ScrollOffsetChanged += TextView_ScrollOffsetChanged;
	}

	internal void UpdateIfVisible()
	{
		if (_visible)
		{
			Show();
		}
	}

	private void TextView_VisualLinesChanged(object sender, EventArgs e)
	{
		if (_visible)
		{
			Show();
		}
		InvalidateVisualColumn();
	}

	private void TextView_ScrollOffsetChanged(object sender, EventArgs e)
	{
		_caretAdorner?.InvalidateVisual();
	}

	internal void OnDocumentChanging()
	{
		_storedCaretOffset = Offset;
		InvalidateVisualColumn();
	}

	internal void OnDocumentChanged(DocumentChangeEventArgs e)
	{
		InvalidateVisualColumn();
		if (_storedCaretOffset >= 0)
		{
			int offset = e.GetNewOffset(movementType: (!_textArea.Selection.IsEmpty && _storedCaretOffset == _textArea.Selection.SurroundingSegment.EndOffset) ? AnchorMovementType.BeforeInsertion : AnchorMovementType.Default, offset: _storedCaretOffset);
			TextDocument document = _textArea.Document;
			if (document != null)
			{
				Position = new TextViewPosition(document.GetLocation(offset), _position.VisualColumn);
			}
		}
		_storedCaretOffset = -1;
	}

	private void ValidatePosition()
	{
		if (_position.Line < 1)
		{
			_position.Line = 1;
		}
		if (_position.Column < 1)
		{
			_position.Column = 1;
		}
		if (_position.VisualColumn < -1)
		{
			_position.VisualColumn = -1;
		}
		TextDocument document = _textArea.Document;
		if (document == null)
		{
			return;
		}
		if (_position.Line > document.LineCount)
		{
			_position.Line = document.LineCount;
			_position.Column = document.GetLineByNumber(_position.Line).Length + 1;
			_position.VisualColumn = -1;
			return;
		}
		DocumentLine lineByNumber = document.GetLineByNumber(_position.Line);
		if (_position.Column > lineByNumber.Length + 1)
		{
			_position.Column = lineByNumber.Length + 1;
			_position.VisualColumn = -1;
		}
	}

	private void RaisePositionChanged()
	{
		if (_textArea.Document != null && _textArea.Document.IsInUpdate)
		{
			_raisePositionChangedOnUpdateFinished = true;
		}
		else
		{
			this.PositionChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	internal void OnDocumentUpdateFinished()
	{
		if (_raisePositionChangedOnUpdateFinished)
		{
			_raisePositionChangedOnUpdateFinished = false;
			this.PositionChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	private void ValidateVisualColumn()
	{
		if (!_visualColumnValid)
		{
			TextDocument document = _textArea.Document;
			if (document != null)
			{
				DocumentLine lineByNumber = document.GetLineByNumber(_position.Line);
				RevalidateVisualColumn(_textView.GetOrConstructVisualLine(lineByNumber));
			}
		}
	}

	private void InvalidateVisualColumn()
	{
		_visualColumnValid = false;
	}

	private void RevalidateVisualColumn(VisualLine visualLine)
	{
		if (visualLine == null)
		{
			throw new ArgumentNullException("visualLine");
		}
		_visualColumnValid = true;
		int offset = _textView.Document.GetOffset(_position.Location);
		int offset2 = visualLine.FirstDocumentLine.Offset;
		_position.VisualColumn = visualLine.ValidateVisualColumn(_position, _textArea.Selection.EnableVirtualSpace);
		int nextCaretPosition = visualLine.GetNextCaretPosition(_position.VisualColumn - 1, AvaloniaEdit.Document.LogicalDirection.Forward, CaretPositioningMode.Normal, _textArea.Selection.EnableVirtualSpace);
		if (nextCaretPosition != _position.VisualColumn)
		{
			int nextCaretPosition2 = visualLine.GetNextCaretPosition(_position.VisualColumn + 1, AvaloniaEdit.Document.LogicalDirection.Backward, CaretPositioningMode.Normal, _textArea.Selection.EnableVirtualSpace);
			if (nextCaretPosition < 0 && nextCaretPosition2 < 0)
			{
				throw ThrowUtil.NoValidCaretPosition();
			}
			int num = ((nextCaretPosition < 0) ? (-1) : (visualLine.GetRelativeOffset(nextCaretPosition) + offset2));
			int num2 = ((nextCaretPosition2 < 0) ? (-1) : (visualLine.GetRelativeOffset(nextCaretPosition2) + offset2));
			int visualColumn;
			int offset3;
			if (nextCaretPosition < 0)
			{
				visualColumn = nextCaretPosition2;
				offset3 = num2;
			}
			else if (nextCaretPosition2 < 0)
			{
				visualColumn = nextCaretPosition;
				offset3 = num;
			}
			else if (Math.Abs(num2 - offset) < Math.Abs(num - offset))
			{
				visualColumn = nextCaretPosition2;
				offset3 = num2;
			}
			else
			{
				visualColumn = nextCaretPosition;
				offset3 = num;
			}
			Position = new TextViewPosition(_textView.Document.GetLocation(offset3), visualColumn);
		}
		_isInVirtualSpace = _position.VisualColumn > visualLine.VisualLength;
	}

	private Rect CalcCaretRectangle(VisualLine visualLine)
	{
		if (!_visualColumnValid)
		{
			RevalidateVisualColumn(visualLine);
		}
		TextLine textLine = visualLine.GetTextLine(_position.VisualColumn, _position.IsAtEndOfLine);
		double textLineVisualXPosition = visualLine.GetTextLineVisualXPosition(textLine, _position.VisualColumn);
		double textLineVisualYPosition = visualLine.GetTextLineVisualYPosition(textLine, VisualYPosition.TextTop);
		double textLineVisualYPosition2 = visualLine.GetTextLineVisualYPosition(textLine, VisualYPosition.TextBottom);
		return new Rect(textLineVisualXPosition, textLineVisualYPosition, 0.5, textLineVisualYPosition2 - textLineVisualYPosition);
	}

	private Rect CalcCaretOverstrikeRectangle(VisualLine visualLine)
	{
		if (!_visualColumnValid)
		{
			RevalidateVisualColumn(visualLine);
		}
		int visualColumn = _position.VisualColumn;
		int nextCaretPosition = visualLine.GetNextCaretPosition(visualColumn, AvaloniaEdit.Document.LogicalDirection.Forward, CaretPositioningMode.Normal, allowVirtualSpace: true);
		TextLine textLine = visualLine.GetTextLine(visualColumn);
		Rect rectangle;
		if (visualColumn < visualLine.VisualLength)
		{
			rectangle = textLine.GetTextBounds(visualColumn, nextCaretPosition - visualColumn)[0].Rectangle;
			double y = rectangle.Y + visualLine.GetTextLineVisualYPosition(textLine, VisualYPosition.LineTop);
			rectangle = rectangle.WithY(y);
		}
		else
		{
			double textLineVisualXPosition = visualLine.GetTextLineVisualXPosition(textLine, visualColumn);
			double textLineVisualXPosition2 = visualLine.GetTextLineVisualXPosition(textLine, nextCaretPosition);
			double textLineVisualYPosition = visualLine.GetTextLineVisualYPosition(textLine, VisualYPosition.TextTop);
			double textLineVisualYPosition2 = visualLine.GetTextLineVisualYPosition(textLine, VisualYPosition.TextBottom);
			rectangle = new Rect(textLineVisualXPosition, textLineVisualYPosition, textLineVisualXPosition2 - textLineVisualXPosition, textLineVisualYPosition2 - textLineVisualYPosition);
		}
		if (rectangle.Width < 0.5)
		{
			rectangle = rectangle.WithWidth(0.5);
		}
		return rectangle;
	}

	public Rect CalculateCaretRectangle()
	{
		if (_textView?.Document != null)
		{
			VisualLine orConstructVisualLine = _textView.GetOrConstructVisualLine(_textView.Document.GetLineByNumber(_position.Line));
			if (!_textArea.OverstrikeMode)
			{
				return CalcCaretRectangle(orConstructVisualLine);
			}
			return CalcCaretOverstrikeRectangle(orConstructVisualLine);
		}
		return default(Rect);
	}

	public void BringCaretToView()
	{
		BringCaretToView(0.0);
	}

	public void BringCaretToView(double border)
	{
		Rect rect = CalculateCaretRectangle();
		if (rect != default(Rect))
		{
			rect = rect.Inflate(border);
			_textView.MakeVisible(rect);
		}
	}

	public void Show()
	{
		_visible = true;
		if (!_showScheduled)
		{
			_showScheduled = true;
			Dispatcher.UIThread.Post(ShowInternal);
		}
	}

	private void ShowInternal()
	{
		_showScheduled = false;
		if (_visible && _caretAdorner != null && _textView != null)
		{
			VisualLine visualLine = _textView.GetVisualLine(_position.Line);
			if (visualLine != null)
			{
				Rect caretRectangle = (_textArea.OverstrikeMode ? CalcCaretOverstrikeRectangle(visualLine) : CalcCaretRectangle(visualLine));
				_caretAdorner.Show(caretRectangle);
			}
			else
			{
				_caretAdorner.Hide();
			}
		}
	}

	public void Hide()
	{
		_visible = false;
		if (_hasWin32Caret)
		{
			_hasWin32Caret = false;
		}
		_caretAdorner?.Hide();
	}

	[Conditional("DEBUG")]
	private static void Log(string text)
	{
	}
}
