using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Media.TextFormatting;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Editing;

internal static class CaretNavigationCommandHandler
{
	private static readonly List<RoutedCommandBinding> CommandBindings;

	private static readonly List<KeyBinding> KeyBindings;

	public static TextAreaInputHandler Create(TextArea textArea)
	{
		TextAreaInputHandler textAreaInputHandler = new TextAreaInputHandler(textArea);
		textAreaInputHandler.CommandBindings.AddRange(CommandBindings);
		textAreaInputHandler.KeyBindings.AddRange(KeyBindings);
		return textAreaInputHandler;
	}

	private static void AddBinding(RoutedCommand command, EventHandler<ExecutedRoutedEventArgs> handler)
	{
		CommandBindings.Add(new RoutedCommandBinding(command, handler));
	}

	private static void AddBinding(RoutedCommand command, KeyModifiers modifiers, Key key, EventHandler<ExecutedRoutedEventArgs> handler)
	{
		AddBinding(command, new KeyGesture(key, modifiers), handler);
	}

	private static void AddBinding(RoutedCommand command, KeyGesture gesture, EventHandler<ExecutedRoutedEventArgs> handler)
	{
		AddBinding(command, handler);
		KeyBindings.Add(TextAreaDefaultInputHandler.CreateKeyBinding(command, gesture));
	}

	static CaretNavigationCommandHandler()
	{
		CommandBindings = new List<RoutedCommandBinding>();
		KeyBindings = new List<KeyBinding>();
		PlatformHotkeyConfiguration hotkeyConfiguration = Application.Current.PlatformSettings.HotkeyConfiguration;
		AddBinding(EditingCommands.MoveLeftByCharacter, KeyModifiers.None, Key.Left, OnMoveCaret(CaretMovementType.CharLeft));
		AddBinding(EditingCommands.SelectLeftByCharacter, hotkeyConfiguration.SelectionModifiers, Key.Left, OnMoveCaretExtendSelection(CaretMovementType.CharLeft));
		AddBinding(RectangleSelection.BoxSelectLeftByCharacter, KeyModifiers.Alt | hotkeyConfiguration.SelectionModifiers, Key.Left, OnMoveCaretBoxSelection(CaretMovementType.CharLeft));
		AddBinding(EditingCommands.MoveRightByCharacter, KeyModifiers.None, Key.Right, OnMoveCaret(CaretMovementType.CharRight));
		AddBinding(EditingCommands.SelectRightByCharacter, hotkeyConfiguration.SelectionModifiers, Key.Right, OnMoveCaretExtendSelection(CaretMovementType.CharRight));
		AddBinding(RectangleSelection.BoxSelectRightByCharacter, KeyModifiers.Alt | hotkeyConfiguration.SelectionModifiers, Key.Right, OnMoveCaretBoxSelection(CaretMovementType.CharRight));
		AddBinding(EditingCommands.MoveLeftByWord, hotkeyConfiguration.WholeWordTextActionModifiers, Key.Left, OnMoveCaret(CaretMovementType.WordLeft));
		AddBinding(EditingCommands.SelectLeftByWord, hotkeyConfiguration.WholeWordTextActionModifiers | hotkeyConfiguration.SelectionModifiers, Key.Left, OnMoveCaretExtendSelection(CaretMovementType.WordLeft));
		AddBinding(RectangleSelection.BoxSelectLeftByWord, hotkeyConfiguration.WholeWordTextActionModifiers | KeyModifiers.Alt | hotkeyConfiguration.SelectionModifiers, Key.Left, OnMoveCaretBoxSelection(CaretMovementType.WordLeft));
		AddBinding(EditingCommands.MoveRightByWord, hotkeyConfiguration.WholeWordTextActionModifiers, Key.Right, OnMoveCaret(CaretMovementType.WordRight));
		AddBinding(EditingCommands.SelectRightByWord, hotkeyConfiguration.WholeWordTextActionModifiers | hotkeyConfiguration.SelectionModifiers, Key.Right, OnMoveCaretExtendSelection(CaretMovementType.WordRight));
		AddBinding(RectangleSelection.BoxSelectRightByWord, hotkeyConfiguration.WholeWordTextActionModifiers | KeyModifiers.Alt | hotkeyConfiguration.SelectionModifiers, Key.Right, OnMoveCaretBoxSelection(CaretMovementType.WordRight));
		AddBinding(EditingCommands.MoveUpByLine, KeyModifiers.None, Key.Up, OnMoveCaret(CaretMovementType.LineUp));
		AddBinding(EditingCommands.SelectUpByLine, hotkeyConfiguration.SelectionModifiers, Key.Up, OnMoveCaretExtendSelection(CaretMovementType.LineUp));
		AddBinding(RectangleSelection.BoxSelectUpByLine, KeyModifiers.Alt | hotkeyConfiguration.SelectionModifiers, Key.Up, OnMoveCaretBoxSelection(CaretMovementType.LineUp));
		AddBinding(EditingCommands.MoveDownByLine, KeyModifiers.None, Key.Down, OnMoveCaret(CaretMovementType.LineDown));
		AddBinding(EditingCommands.SelectDownByLine, hotkeyConfiguration.SelectionModifiers, Key.Down, OnMoveCaretExtendSelection(CaretMovementType.LineDown));
		AddBinding(RectangleSelection.BoxSelectDownByLine, KeyModifiers.Alt | hotkeyConfiguration.SelectionModifiers, Key.Down, OnMoveCaretBoxSelection(CaretMovementType.LineDown));
		AddBinding(EditingCommands.MoveDownByPage, KeyModifiers.None, Key.PageDown, OnMoveCaret(CaretMovementType.PageDown));
		AddBinding(EditingCommands.SelectDownByPage, hotkeyConfiguration.SelectionModifiers, Key.PageDown, OnMoveCaretExtendSelection(CaretMovementType.PageDown));
		AddBinding(EditingCommands.MoveUpByPage, KeyModifiers.None, Key.PageUp, OnMoveCaret(CaretMovementType.PageUp));
		AddBinding(EditingCommands.SelectUpByPage, hotkeyConfiguration.SelectionModifiers, Key.PageUp, OnMoveCaretExtendSelection(CaretMovementType.PageUp));
		foreach (KeyGesture item in hotkeyConfiguration.MoveCursorToTheStartOfLine)
		{
			AddBinding(EditingCommands.MoveToLineStart, item, OnMoveCaret(CaretMovementType.LineStart));
		}
		foreach (KeyGesture item2 in hotkeyConfiguration.MoveCursorToTheStartOfLineWithSelection)
		{
			AddBinding(EditingCommands.SelectToLineStart, item2, OnMoveCaretExtendSelection(CaretMovementType.LineStart));
		}
		foreach (KeyGesture item3 in hotkeyConfiguration.MoveCursorToTheEndOfLine)
		{
			AddBinding(EditingCommands.MoveToLineEnd, item3, OnMoveCaret(CaretMovementType.LineEnd));
		}
		foreach (KeyGesture item4 in hotkeyConfiguration.MoveCursorToTheEndOfLineWithSelection)
		{
			AddBinding(EditingCommands.SelectToLineEnd, item4, OnMoveCaretExtendSelection(CaretMovementType.LineEnd));
		}
		AddBinding(RectangleSelection.BoxSelectToLineStart, KeyModifiers.Alt | hotkeyConfiguration.SelectionModifiers, Key.Home, OnMoveCaretBoxSelection(CaretMovementType.LineStart));
		AddBinding(RectangleSelection.BoxSelectToLineEnd, KeyModifiers.Alt | hotkeyConfiguration.SelectionModifiers, Key.End, OnMoveCaretBoxSelection(CaretMovementType.LineEnd));
		foreach (KeyGesture item5 in hotkeyConfiguration.MoveCursorToTheStartOfDocument)
		{
			AddBinding(EditingCommands.MoveToDocumentStart, item5, OnMoveCaret(CaretMovementType.DocumentStart));
		}
		foreach (KeyGesture item6 in hotkeyConfiguration.MoveCursorToTheStartOfDocumentWithSelection)
		{
			AddBinding(EditingCommands.SelectToDocumentStart, item6, OnMoveCaretExtendSelection(CaretMovementType.DocumentStart));
		}
		foreach (KeyGesture item7 in hotkeyConfiguration.MoveCursorToTheEndOfDocument)
		{
			AddBinding(EditingCommands.MoveToDocumentEnd, item7, OnMoveCaret(CaretMovementType.DocumentEnd));
		}
		foreach (KeyGesture item8 in hotkeyConfiguration.MoveCursorToTheEndOfDocumentWithSelection)
		{
			AddBinding(EditingCommands.SelectToDocumentEnd, item8, OnMoveCaretExtendSelection(CaretMovementType.DocumentEnd));
		}
		AddBinding(ApplicationCommands.SelectAll, OnSelectAll);
	}

	private static void OnSelectAll(object target, ExecutedRoutedEventArgs args)
	{
		TextArea textArea = GetTextArea(target);
		if (textArea?.Document != null)
		{
			args.Handled = true;
			textArea.Caret.Offset = textArea.Document.TextLength;
			textArea.Selection = Selection.Create(textArea, 0, textArea.Document.TextLength);
		}
	}

	private static TextArea GetTextArea(object target)
	{
		return target as TextArea;
	}

	private static EventHandler<ExecutedRoutedEventArgs> OnMoveCaret(CaretMovementType direction)
	{
		return delegate(object target, ExecutedRoutedEventArgs args)
		{
			TextArea textArea = GetTextArea(target);
			if (textArea?.Document != null)
			{
				args.Handled = true;
				textArea.ClearSelection();
				MoveCaret(textArea, direction);
				textArea.Caret.BringCaretToView();
			}
		};
	}

	private static EventHandler<ExecutedRoutedEventArgs> OnMoveCaretExtendSelection(CaretMovementType direction)
	{
		return delegate(object target, ExecutedRoutedEventArgs args)
		{
			TextArea textArea = GetTextArea(target);
			if (textArea?.Document != null)
			{
				args.Handled = true;
				TextViewPosition position = textArea.Caret.Position;
				MoveCaret(textArea, direction);
				textArea.Selection = textArea.Selection.StartSelectionOrSetEndpoint(position, textArea.Caret.Position);
				textArea.Caret.BringCaretToView();
			}
		};
	}

	private static EventHandler<ExecutedRoutedEventArgs> OnMoveCaretBoxSelection(CaretMovementType direction)
	{
		return delegate(object target, ExecutedRoutedEventArgs args)
		{
			TextArea textArea = GetTextArea(target);
			if (textArea?.Document != null)
			{
				args.Handled = true;
				if (textArea.Options.EnableRectangularSelection && !(textArea.Selection is RectangleSelection))
				{
					textArea.Selection = (textArea.Selection.IsEmpty ? new RectangleSelection(textArea, textArea.Caret.Position, textArea.Caret.Position) : new RectangleSelection(textArea, textArea.Selection.StartPosition, textArea.Caret.Position));
				}
				TextViewPosition position = textArea.Caret.Position;
				MoveCaret(textArea, direction);
				textArea.Selection = textArea.Selection.StartSelectionOrSetEndpoint(position, textArea.Caret.Position);
				textArea.Caret.BringCaretToView();
			}
		};
	}

	internal static void MoveCaret(TextArea textArea, CaretMovementType direction)
	{
		double desiredXPos = textArea.Caret.DesiredXPos;
		textArea.Caret.Position = GetNewCaretPosition(textArea.TextView, textArea.Caret.Position, direction, textArea.Selection.EnableVirtualSpace, ref desiredXPos);
		textArea.Caret.DesiredXPos = desiredXPos;
	}

	internal static TextViewPosition GetNewCaretPosition(TextView textView, TextViewPosition caretPosition, CaretMovementType direction, bool enableVirtualSpace, ref double desiredXPos)
	{
		switch (direction)
		{
		case CaretMovementType.None:
			return caretPosition;
		case CaretMovementType.DocumentStart:
			desiredXPos = double.NaN;
			return new TextViewPosition(0, 0);
		case CaretMovementType.DocumentEnd:
			desiredXPos = double.NaN;
			return new TextViewPosition(textView.Document.GetLocation(textView.Document.TextLength));
		default:
		{
			DocumentLine lineByNumber = textView.Document.GetLineByNumber(caretPosition.Line);
			VisualLine orConstructVisualLine = textView.GetOrConstructVisualLine(lineByNumber);
			TextLine textLine = orConstructVisualLine.GetTextLine(caretPosition.VisualColumn, caretPosition.IsAtEndOfLine);
			switch (direction)
			{
			case CaretMovementType.CharLeft:
				desiredXPos = double.NaN;
				if (caretPosition.VisualColumn == 0 && enableVirtualSpace)
				{
					return caretPosition;
				}
				return GetPrevCaretPosition(textView, caretPosition, orConstructVisualLine, CaretPositioningMode.Normal, enableVirtualSpace);
			case CaretMovementType.Backspace:
				desiredXPos = double.NaN;
				return GetPrevCaretPosition(textView, caretPosition, orConstructVisualLine, CaretPositioningMode.EveryCodepoint, enableVirtualSpace);
			case CaretMovementType.CharRight:
				desiredXPos = double.NaN;
				return GetNextCaretPosition(textView, caretPosition, orConstructVisualLine, CaretPositioningMode.Normal, enableVirtualSpace);
			case CaretMovementType.WordLeft:
				desiredXPos = double.NaN;
				return GetPrevCaretPosition(textView, caretPosition, orConstructVisualLine, CaretPositioningMode.WordStart, enableVirtualSpace);
			case CaretMovementType.WordRight:
				desiredXPos = double.NaN;
				return GetNextCaretPosition(textView, caretPosition, orConstructVisualLine, CaretPositioningMode.WordStart, enableVirtualSpace);
			case CaretMovementType.LineUp:
			case CaretMovementType.LineDown:
			case CaretMovementType.PageUp:
			case CaretMovementType.PageDown:
				return GetUpDownCaretPosition(textView, caretPosition, direction, orConstructVisualLine, textLine, enableVirtualSpace, ref desiredXPos);
			case CaretMovementType.LineStart:
				desiredXPos = double.NaN;
				return GetStartOfLineCaretPosition(caretPosition.VisualColumn, orConstructVisualLine, textLine, enableVirtualSpace);
			case CaretMovementType.LineEnd:
				desiredXPos = double.NaN;
				return GetEndOfLineCaretPosition(orConstructVisualLine, textLine);
			default:
				throw new NotSupportedException(direction.ToString());
			}
		}
		}
	}

	private static TextViewPosition GetStartOfLineCaretPosition(int oldVisualColumn, VisualLine visualLine, TextLine textLine, bool enableVirtualSpace)
	{
		int num = visualLine.GetTextLineVisualStartColumn(textLine);
		if (num == 0)
		{
			num = visualLine.GetNextCaretPosition(num - 1, AvaloniaEdit.Document.LogicalDirection.Forward, CaretPositioningMode.WordStart, enableVirtualSpace);
		}
		if (num < 0)
		{
			throw ThrowUtil.NoValidCaretPosition();
		}
		if (num == oldVisualColumn)
		{
			num = 0;
		}
		return visualLine.GetTextViewPosition(num);
	}

	private static TextViewPosition GetEndOfLineCaretPosition(VisualLine visualLine, TextLine textLine)
	{
		int visualColumn = visualLine.GetTextLineVisualStartColumn(textLine) + textLine.Length - textLine.TrailingWhitespaceLength;
		TextViewPosition textViewPosition = visualLine.GetTextViewPosition(visualColumn);
		textViewPosition.IsAtEndOfLine = true;
		return textViewPosition;
	}

	private static TextViewPosition GetNextCaretPosition(TextView textView, TextViewPosition caretPosition, VisualLine visualLine, CaretPositioningMode mode, bool enableVirtualSpace)
	{
		int nextCaretPosition = visualLine.GetNextCaretPosition(caretPosition.VisualColumn, AvaloniaEdit.Document.LogicalDirection.Forward, mode, enableVirtualSpace);
		if (nextCaretPosition >= 0)
		{
			return visualLine.GetTextViewPosition(nextCaretPosition);
		}
		DocumentLine nextLine = visualLine.LastDocumentLine.NextLine;
		if (nextLine != null)
		{
			VisualLine orConstructVisualLine = textView.GetOrConstructVisualLine(nextLine);
			nextCaretPosition = orConstructVisualLine.GetNextCaretPosition(-1, AvaloniaEdit.Document.LogicalDirection.Forward, mode, enableVirtualSpace);
			if (nextCaretPosition < 0)
			{
				throw ThrowUtil.NoValidCaretPosition();
			}
			return orConstructVisualLine.GetTextViewPosition(nextCaretPosition);
		}
		return new TextViewPosition(textView.Document.GetLocation(textView.Document.TextLength));
	}

	private static TextViewPosition GetPrevCaretPosition(TextView textView, TextViewPosition caretPosition, VisualLine visualLine, CaretPositioningMode mode, bool enableVirtualSpace)
	{
		int nextCaretPosition = visualLine.GetNextCaretPosition(caretPosition.VisualColumn, AvaloniaEdit.Document.LogicalDirection.Backward, mode, enableVirtualSpace);
		if (nextCaretPosition >= 0)
		{
			return visualLine.GetTextViewPosition(nextCaretPosition);
		}
		DocumentLine previousLine = visualLine.FirstDocumentLine.PreviousLine;
		if (previousLine != null)
		{
			VisualLine orConstructVisualLine = textView.GetOrConstructVisualLine(previousLine);
			nextCaretPosition = orConstructVisualLine.GetNextCaretPosition(orConstructVisualLine.VisualLength + 1, AvaloniaEdit.Document.LogicalDirection.Backward, mode, enableVirtualSpace);
			if (nextCaretPosition < 0)
			{
				throw ThrowUtil.NoValidCaretPosition();
			}
			return orConstructVisualLine.GetTextViewPosition(nextCaretPosition);
		}
		return new TextViewPosition(0, 0);
	}

	private static TextViewPosition GetUpDownCaretPosition(TextView textView, TextViewPosition caretPosition, CaretMovementType direction, VisualLine visualLine, TextLine textLine, bool enableVirtualSpace, ref double xPos)
	{
		if (double.IsNaN(xPos))
		{
			xPos = visualLine.GetTextLineVisualXPosition(textLine, caretPosition.VisualColumn);
		}
		VisualLine visualLine2 = visualLine;
		int num = visualLine.TextLines.IndexOf(textLine);
		TextLine textLine2;
		switch (direction)
		{
		case CaretMovementType.LineUp:
		{
			int num3 = visualLine.FirstDocumentLine.LineNumber - 1;
			if (num > 0)
			{
				textLine2 = visualLine.TextLines[num - 1];
			}
			else if (num3 >= 1)
			{
				DocumentLine lineByNumber2 = textView.Document.GetLineByNumber(num3);
				visualLine2 = textView.GetOrConstructVisualLine(lineByNumber2);
				textLine2 = visualLine2.TextLines[visualLine2.TextLines.Count - 1];
			}
			else
			{
				textLine2 = null;
			}
			break;
		}
		case CaretMovementType.LineDown:
		{
			int num2 = visualLine.LastDocumentLine.LineNumber + 1;
			if (num < visualLine.TextLines.Count - 1)
			{
				textLine2 = visualLine.TextLines[num + 1];
			}
			else if (num2 <= textView.Document.LineCount)
			{
				DocumentLine lineByNumber = textView.Document.GetLineByNumber(num2);
				visualLine2 = textView.GetOrConstructVisualLine(lineByNumber);
				textLine2 = visualLine2.TextLines[0];
			}
			else
			{
				textLine2 = null;
			}
			break;
		}
		case CaretMovementType.PageUp:
		case CaretMovementType.PageDown:
		{
			double textLineVisualYPosition = visualLine.GetTextLineVisualYPosition(textLine, VisualYPosition.LineMiddle);
			textLineVisualYPosition = ((direction != CaretMovementType.PageUp) ? (textLineVisualYPosition + textView.Bounds.Height) : (textLineVisualYPosition - textView.Bounds.Height));
			DocumentLine documentLineByVisualTop = textView.GetDocumentLineByVisualTop(textLineVisualYPosition);
			visualLine2 = textView.GetOrConstructVisualLine(documentLineByVisualTop);
			textLine2 = visualLine2.GetTextLineByVisualYPosition(textLineVisualYPosition);
			break;
		}
		default:
			throw new NotSupportedException(direction.ToString());
		}
		if (textLine2 != null)
		{
			double textLineVisualYPosition2 = visualLine2.GetTextLineVisualYPosition(textLine2, VisualYPosition.LineMiddle);
			int num4 = visualLine2.GetVisualColumn(new Point(xPos, textLineVisualYPosition2), enableVirtualSpace);
			int textLineVisualStartColumn = visualLine2.GetTextLineVisualStartColumn(textLine2);
			if (num4 >= textLineVisualStartColumn + textLine2.Length && num4 <= visualLine2.VisualLength)
			{
				num4 = textLineVisualStartColumn + textLine2.Length - 1;
			}
			return visualLine2.GetTextViewPosition(num4);
		}
		return caretPosition;
	}
}
