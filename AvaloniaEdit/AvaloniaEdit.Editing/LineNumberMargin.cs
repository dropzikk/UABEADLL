using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Editing;

public class LineNumberMargin : AbstractMargin
{
	private AnchorSegment _selectionStart;

	private bool _selecting;

	protected int MaxLineNumberLength = 1;

	protected Typeface Typeface { get; set; }

	protected double EmSize { get; set; }

	protected override Size MeasureOverride(Size availableSize)
	{
		Typeface = this.CreateTypeface();
		EmSize = GetValue(TextBlock.FontSizeProperty);
		return new Size(TextFormatterFactory.CreateFormattedText(this, new string('9', MaxLineNumberLength), Typeface, EmSize, GetValue(TextBlock.ForegroundProperty)).Width, 0.0);
	}

	public override void Render(DrawingContext drawingContext)
	{
		TextView textView = base.TextView;
		Size size = base.Bounds.Size;
		if (textView == null || !textView.VisualLinesValid)
		{
			return;
		}
		IBrush value = GetValue(TextBlock.ForegroundProperty);
		foreach (VisualLine visualLine in textView.VisualLines)
		{
			FormattedText formattedText = TextFormatterFactory.CreateFormattedText(this, visualLine.FirstDocumentLine.LineNumber.ToString(CultureInfo.CurrentCulture), Typeface, EmSize, value);
			double textLineVisualYPosition = visualLine.GetTextLineVisualYPosition(visualLine.TextLines[0], VisualYPosition.TextTop);
			drawingContext.DrawText(formattedText, new Point(size.Width - formattedText.Width, textLineVisualYPosition - textView.VerticalOffset));
		}
	}

	protected override void OnTextViewChanged(TextView oldTextView, TextView newTextView)
	{
		if (oldTextView != null)
		{
			oldTextView.VisualLinesChanged -= TextViewVisualLinesChanged;
		}
		base.OnTextViewChanged(oldTextView, newTextView);
		if (newTextView != null)
		{
			newTextView.VisualLinesChanged += TextViewVisualLinesChanged;
		}
		InvalidateVisual();
	}

	protected override void OnDocumentChanged(TextDocument oldDocument, TextDocument newDocument)
	{
		if (oldDocument != null)
		{
			WeakEventManagerBase<TextDocumentWeakEventManager.LineCountChanged, TextDocument, EventHandler, EventArgs>.RemoveHandler(oldDocument, OnDocumentLineCountChanged);
		}
		base.OnDocumentChanged(oldDocument, newDocument);
		if (newDocument != null)
		{
			WeakEventManagerBase<TextDocumentWeakEventManager.LineCountChanged, TextDocument, EventHandler, EventArgs>.AddHandler(newDocument, OnDocumentLineCountChanged);
		}
		OnDocumentLineCountChanged();
	}

	private void OnDocumentLineCountChanged(object sender, EventArgs e)
	{
		OnDocumentLineCountChanged();
	}

	private void TextViewVisualLinesChanged(object sender, EventArgs e)
	{
		InvalidateMeasure();
	}

	private void OnDocumentLineCountChanged()
	{
		int num = (base.Document?.LineCount ?? 1).ToString(CultureInfo.CurrentCulture).Length;
		if (num < 2)
		{
			num = 2;
		}
		if (num != MaxLineNumberLength)
		{
			MaxLineNumberLength = num;
			InvalidateMeasure();
		}
	}

	protected override void OnPointerPressed(PointerPressedEventArgs e)
	{
		base.OnPointerPressed(e);
		if (e.Handled || base.TextView == null || base.TextArea == null)
		{
			return;
		}
		e.Handled = true;
		base.TextArea.Focus();
		SimpleSegment textLineSegment = GetTextLineSegment(e);
		if (textLineSegment == SimpleSegment.Invalid)
		{
			return;
		}
		base.TextArea.Caret.Offset = textLineSegment.Offset + textLineSegment.Length;
		e.Pointer.Capture(this);
		if (e.Pointer.Captured == this)
		{
			_selecting = true;
			_selectionStart = new AnchorSegment(base.Document, textLineSegment.Offset, textLineSegment.Length);
			if (e.KeyModifiers.HasFlag(KeyModifiers.Shift) && base.TextArea.Selection is SimpleSelection simpleSelection)
			{
				_selectionStart = new AnchorSegment(base.Document, simpleSelection.SurroundingSegment);
			}
			base.TextArea.Selection = Selection.Create(base.TextArea, _selectionStart);
			if (e.KeyModifiers.HasFlag(KeyModifiers.Shift))
			{
				ExtendSelection(textLineSegment);
			}
			base.TextArea.Caret.BringCaretToView(0.0);
		}
	}

	private SimpleSegment GetTextLineSegment(PointerEventArgs e)
	{
		Point point = e.GetPosition(base.TextView);
		point = new Point(0.0, point.Y.CoerceValue(0.0, base.TextView.Bounds.Height) + base.TextView.VerticalOffset);
		VisualLine visualLineFromVisualTop = base.TextView.GetVisualLineFromVisualTop(point.Y);
		if (visualLineFromVisualTop == null)
		{
			return SimpleSegment.Invalid;
		}
		TextLine textLineByVisualYPosition = visualLineFromVisualTop.GetTextLineByVisualYPosition(point.Y);
		int textLineVisualStartColumn = visualLineFromVisualTop.GetTextLineVisualStartColumn(textLineByVisualYPosition);
		int visualColumn = textLineVisualStartColumn + textLineByVisualYPosition.Length;
		int offset = visualLineFromVisualTop.FirstDocumentLine.Offset;
		int num = visualLineFromVisualTop.GetRelativeOffset(textLineVisualStartColumn) + offset;
		int num2 = visualLineFromVisualTop.GetRelativeOffset(visualColumn) + offset;
		if (num2 == visualLineFromVisualTop.LastDocumentLine.Offset + visualLineFromVisualTop.LastDocumentLine.Length)
		{
			num2 += visualLineFromVisualTop.LastDocumentLine.DelimiterLength;
		}
		return new SimpleSegment(num, num2 - num);
	}

	private void ExtendSelection(SimpleSegment currentSeg)
	{
		if (currentSeg.Offset < _selectionStart.Offset)
		{
			base.TextArea.Caret.Offset = currentSeg.Offset;
			base.TextArea.Selection = Selection.Create(base.TextArea, currentSeg.Offset, _selectionStart.Offset + _selectionStart.Length);
		}
		else
		{
			base.TextArea.Caret.Offset = currentSeg.Offset + currentSeg.Length;
			base.TextArea.Selection = Selection.Create(base.TextArea, _selectionStart.Offset, currentSeg.Offset + currentSeg.Length);
		}
	}

	protected override void OnPointerMoved(PointerEventArgs e)
	{
		if (_selecting && base.TextArea != null && base.TextView != null)
		{
			e.Handled = true;
			SimpleSegment textLineSegment = GetTextLineSegment(e);
			if (textLineSegment == SimpleSegment.Invalid)
			{
				return;
			}
			ExtendSelection(textLineSegment);
			base.TextArea.Caret.BringCaretToView(0.0);
		}
		base.OnPointerMoved(e);
	}

	protected override void OnPointerReleased(PointerReleasedEventArgs e)
	{
		if (_selecting)
		{
			_selecting = false;
			_selectionStart = null;
			e.Pointer.Capture(null);
			e.Handled = true;
		}
		base.OnPointerReleased(e);
	}
}
