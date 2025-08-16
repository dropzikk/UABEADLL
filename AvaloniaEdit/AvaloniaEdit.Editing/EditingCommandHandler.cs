using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using AvaloniaEdit.Document;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Editing;

internal class EditingCommandHandler
{
	private enum DefaultSegmentType
	{
		WholeDocument,
		CurrentLine
	}

	private static readonly List<RoutedCommandBinding> CommandBindings;

	private static readonly List<KeyBinding> KeyBindings;

	public static TextAreaInputHandler Create(TextArea textArea)
	{
		TextAreaInputHandler textAreaInputHandler = new TextAreaInputHandler(textArea);
		textAreaInputHandler.CommandBindings.AddRange(CommandBindings);
		textAreaInputHandler.KeyBindings.AddRange(KeyBindings);
		return textAreaInputHandler;
	}

	private static void AddBinding(RoutedCommand command, KeyModifiers modifiers, Key key, EventHandler<ExecutedRoutedEventArgs> handler)
	{
		CommandBindings.Add(new RoutedCommandBinding(command, handler));
		KeyBindings.Add(TextAreaDefaultInputHandler.CreateKeyBinding(command, modifiers, key));
	}

	private static void AddBinding(RoutedCommand command, EventHandler<ExecutedRoutedEventArgs> handler, EventHandler<CanExecuteRoutedEventArgs> canExecuteHandler = null)
	{
		CommandBindings.Add(new RoutedCommandBinding(command, handler, canExecuteHandler));
	}

	static EditingCommandHandler()
	{
		CommandBindings = new List<RoutedCommandBinding>();
		KeyBindings = new List<KeyBinding>();
		AddBinding(EditingCommands.Delete, KeyModifiers.None, Key.Delete, OnDelete(CaretMovementType.CharRight));
		AddBinding(EditingCommands.DeleteNextWord, KeyModifiers.Control, Key.Delete, OnDelete(CaretMovementType.WordRight));
		AddBinding(EditingCommands.Backspace, KeyModifiers.None, Key.Back, OnDelete(CaretMovementType.Backspace));
		KeyBindings.Add(TextAreaDefaultInputHandler.CreateKeyBinding(EditingCommands.Backspace, KeyModifiers.Shift, Key.Back));
		AddBinding(EditingCommands.DeletePreviousWord, KeyModifiers.Control, Key.Back, OnDelete(CaretMovementType.WordLeft));
		AddBinding(EditingCommands.EnterParagraphBreak, KeyModifiers.None, Key.Return, OnEnter);
		AddBinding(EditingCommands.EnterLineBreak, KeyModifiers.Shift, Key.Return, OnEnter);
		AddBinding(EditingCommands.TabForward, KeyModifiers.None, Key.Tab, OnTab);
		AddBinding(EditingCommands.TabBackward, KeyModifiers.Shift, Key.Tab, OnShiftTab);
		AddBinding(ApplicationCommands.Delete, OnDelete(CaretMovementType.None), CanDelete);
		AddBinding(ApplicationCommands.Copy, OnCopy, CanCopy);
		AddBinding(ApplicationCommands.Cut, OnCut, CanCut);
		AddBinding(ApplicationCommands.Paste, OnPaste, CanPaste);
		AddBinding(AvaloniaEditCommands.ToggleOverstrike, OnToggleOverstrike);
		AddBinding(AvaloniaEditCommands.DeleteLine, OnDeleteLine);
		AddBinding(AvaloniaEditCommands.RemoveLeadingWhitespace, OnRemoveLeadingWhitespace);
		AddBinding(AvaloniaEditCommands.RemoveTrailingWhitespace, OnRemoveTrailingWhitespace);
		AddBinding(AvaloniaEditCommands.ConvertToUppercase, OnConvertToUpperCase);
		AddBinding(AvaloniaEditCommands.ConvertToLowercase, OnConvertToLowerCase);
		AddBinding(AvaloniaEditCommands.ConvertToTitleCase, OnConvertToTitleCase);
		AddBinding(AvaloniaEditCommands.InvertCase, OnInvertCase);
		AddBinding(AvaloniaEditCommands.ConvertTabsToSpaces, OnConvertTabsToSpaces);
		AddBinding(AvaloniaEditCommands.ConvertSpacesToTabs, OnConvertSpacesToTabs);
		AddBinding(AvaloniaEditCommands.ConvertLeadingTabsToSpaces, OnConvertLeadingTabsToSpaces);
		AddBinding(AvaloniaEditCommands.ConvertLeadingSpacesToTabs, OnConvertLeadingSpacesToTabs);
		AddBinding(AvaloniaEditCommands.IndentSelection, OnIndentSelection);
	}

	private static TextArea GetTextArea(object target)
	{
		return target as TextArea;
	}

	private static void TransformSelectedLines(Action<TextArea, DocumentLine> transformLine, object target, ExecutedRoutedEventArgs args, DefaultSegmentType defaultSegmentType)
	{
		TextArea textArea = GetTextArea(target);
		if (textArea?.Document == null)
		{
			return;
		}
		using (textArea.Document.RunUpdate())
		{
			DocumentLine documentLine2;
			DocumentLine documentLine;
			if (textArea.Selection.IsEmpty)
			{
				switch (defaultSegmentType)
				{
				case DefaultSegmentType.CurrentLine:
					documentLine2 = (documentLine = textArea.Document.GetLineByNumber(textArea.Caret.Line));
					break;
				case DefaultSegmentType.WholeDocument:
					documentLine2 = textArea.Document.Lines.First();
					documentLine = textArea.Document.Lines.Last();
					break;
				default:
					documentLine2 = (documentLine = null);
					break;
				}
			}
			else
			{
				ISegment surroundingSegment = textArea.Selection.SurroundingSegment;
				documentLine2 = textArea.Document.GetLineByOffset(surroundingSegment.Offset);
				documentLine = textArea.Document.GetLineByOffset(surroundingSegment.EndOffset);
				if (documentLine2 != documentLine && documentLine.Offset == surroundingSegment.EndOffset)
				{
					documentLine = documentLine.PreviousLine;
				}
			}
			if (documentLine2 != null)
			{
				transformLine(textArea, documentLine2);
				while (documentLine2 != documentLine)
				{
					documentLine2 = documentLine2.NextLine;
					transformLine(textArea, documentLine2);
				}
			}
		}
		textArea.Caret.BringCaretToView();
		args.Handled = true;
	}

	private static void TransformSelectedSegments(Action<TextArea, ISegment> transformSegment, object target, ExecutedRoutedEventArgs args, DefaultSegmentType defaultSegmentType)
	{
		TextArea textArea = GetTextArea(target);
		if (textArea?.Document == null)
		{
			return;
		}
		using (textArea.Document.RunUpdate())
		{
			IEnumerable<ISegment> enumerable = ((!textArea.Selection.IsEmpty) ? textArea.Selection.Segments : (defaultSegmentType switch
			{
				DefaultSegmentType.CurrentLine => new ISegment[1] { textArea.Document.GetLineByNumber(textArea.Caret.Line) }, 
				DefaultSegmentType.WholeDocument => textArea.Document.Lines, 
				_ => null, 
			}));
			if (enumerable != null)
			{
				foreach (ISegment item in enumerable.Reverse())
				{
					foreach (ISegment item2 in textArea.GetDeletableSegments(item).Reverse())
					{
						transformSegment(textArea, item2);
					}
				}
			}
		}
		textArea.Caret.BringCaretToView();
		args.Handled = true;
	}

	private static void OnEnter(object target, ExecutedRoutedEventArgs args)
	{
		TextArea textArea = GetTextArea(target);
		if (textArea != null && textArea.IsFocused)
		{
			textArea.PerformTextInput("\n");
			args.Handled = true;
		}
	}

	private static void OnTab(object target, ExecutedRoutedEventArgs args)
	{
		TextArea textArea = GetTextArea(target);
		if (textArea?.Document == null)
		{
			return;
		}
		using (textArea.Document.RunUpdate())
		{
			if (textArea.Selection.IsMultiline)
			{
				ISegment surroundingSegment = textArea.Selection.SurroundingSegment;
				DocumentLine lineByOffset = textArea.Document.GetLineByOffset(surroundingSegment.Offset);
				DocumentLine documentLine = textArea.Document.GetLineByOffset(surroundingSegment.EndOffset);
				if (lineByOffset != documentLine && documentLine.Offset == surroundingSegment.EndOffset)
				{
					documentLine = documentLine.PreviousLine;
				}
				DocumentLine documentLine2 = lineByOffset;
				while (true)
				{
					int offset = documentLine2.Offset;
					if (textArea.ReadOnlySectionProvider.CanInsert(offset))
					{
						textArea.Document.Replace(offset, 0, textArea.Options.IndentationString, OffsetChangeMappingType.KeepAnchorBeforeInsertion);
					}
					if (documentLine2 == documentLine)
					{
						break;
					}
					documentLine2 = documentLine2.NextLine;
				}
			}
			else
			{
				string indentationString = textArea.Options.GetIndentationString(textArea.Caret.Column);
				textArea.ReplaceSelectionWithText(indentationString);
			}
		}
		textArea.Caret.BringCaretToView();
		args.Handled = true;
	}

	private static void OnShiftTab(object target, ExecutedRoutedEventArgs args)
	{
		TransformSelectedLines(delegate(TextArea textArea, DocumentLine line)
		{
			int offset = line.Offset;
			ISegment singleIndentationSegment = TextUtilities.GetSingleIndentationSegment(textArea.Document, offset, textArea.Options.IndentationSize);
			if (singleIndentationSegment.Length > 0)
			{
				singleIndentationSegment = textArea.GetDeletableSegments(singleIndentationSegment).FirstOrDefault();
				if (singleIndentationSegment != null && singleIndentationSegment.Length > 0)
				{
					textArea.Document.Remove(singleIndentationSegment.Offset, singleIndentationSegment.Length);
				}
			}
		}, target, args, DefaultSegmentType.CurrentLine);
	}

	private static EventHandler<ExecutedRoutedEventArgs> OnDelete(CaretMovementType caretMovement)
	{
		return delegate(object target, ExecutedRoutedEventArgs args)
		{
			TextArea textArea = GetTextArea(target);
			if (textArea?.Document != null)
			{
				if (textArea.Selection.IsEmpty)
				{
					TextViewPosition position = textArea.Caret.Position;
					bool enableVirtualSpace = textArea.Options.EnableVirtualSpace;
					if (caretMovement == CaretMovementType.CharRight)
					{
						enableVirtualSpace = false;
					}
					double desiredXPos = textArea.Caret.DesiredXPos;
					TextViewPosition endPosition = CaretNavigationCommandHandler.GetNewCaretPosition(textArea.TextView, position, caretMovement, enableVirtualSpace, ref desiredXPos);
					if (endPosition.Line < 1 || endPosition.Column < 1)
					{
						endPosition = new TextViewPosition(Math.Max(endPosition.Line, 1), Math.Max(endPosition.Column, 1));
					}
					if (textArea.Selection is RectangleSelection && position.Line != endPosition.Line)
					{
						return;
					}
					textArea.Selection.StartSelectionOrSetEndpoint(position, endPosition).ReplaceSelectionWithText(string.Empty);
				}
				else
				{
					textArea.RemoveSelectedText();
				}
				textArea.Caret.BringCaretToView();
				args.Handled = true;
			}
		};
	}

	private static void CanDelete(object target, CanExecuteRoutedEventArgs args)
	{
		if (GetTextArea(target)?.Document != null)
		{
			args.CanExecute = true;
			args.Handled = true;
		}
	}

	private static void CanCut(object target, CanExecuteRoutedEventArgs args)
	{
		TextArea textArea = GetTextArea(target);
		if (textArea?.Document != null)
		{
			args.CanExecute = (textArea.Options.CutCopyWholeLine || !textArea.Selection.IsEmpty) && !textArea.IsReadOnly;
			args.Handled = true;
		}
	}

	private static void CanCopy(object target, CanExecuteRoutedEventArgs args)
	{
		TextArea textArea = GetTextArea(target);
		if (textArea?.Document != null)
		{
			args.CanExecute = textArea.Options.CutCopyWholeLine || !textArea.Selection.IsEmpty;
			args.Handled = true;
		}
	}

	private static void OnCopy(object target, ExecutedRoutedEventArgs args)
	{
		TextArea textArea = GetTextArea(target);
		if (textArea?.Document != null)
		{
			if (textArea.Selection.IsEmpty && textArea.Options.CutCopyWholeLine)
			{
				DocumentLine lineByNumber = textArea.Document.GetLineByNumber(textArea.Caret.Line);
				CopyWholeLine(textArea, lineByNumber);
			}
			else
			{
				CopySelectedText(textArea);
			}
			args.Handled = true;
		}
	}

	private static void OnCut(object target, ExecutedRoutedEventArgs args)
	{
		TextArea textArea = GetTextArea(target);
		if (textArea?.Document == null)
		{
			return;
		}
		if (textArea.Selection.IsEmpty && textArea.Options.CutCopyWholeLine)
		{
			DocumentLine lineByNumber = textArea.Document.GetLineByNumber(textArea.Caret.Line);
			if (CopyWholeLine(textArea, lineByNumber))
			{
				ISegment[] deletableSegments = textArea.GetDeletableSegments(new SimpleSegment(lineByNumber.Offset, lineByNumber.TotalLength));
				for (int num = deletableSegments.Length - 1; num >= 0; num--)
				{
					textArea.Document.Remove(deletableSegments[num]);
				}
			}
		}
		else if (CopySelectedText(textArea))
		{
			textArea.RemoveSelectedText();
		}
		textArea.Caret.BringCaretToView();
		args.Handled = true;
	}

	private static bool CopySelectedText(TextArea textArea)
	{
		string text = textArea.Selection.GetText();
		text = TextUtilities.NormalizeNewLines(text, Environment.NewLine);
		SetClipboardText(text, textArea);
		textArea.OnTextCopied(new TextEventArgs(text));
		return true;
	}

	private static void SetClipboardText(string text, Visual visual)
	{
		try
		{
			TopLevel.GetTopLevel(visual)?.Clipboard?.SetTextAsync(text).GetAwaiter().GetResult();
		}
		catch (Exception)
		{
		}
	}

	private static bool CopyWholeLine(TextArea textArea, DocumentLine line)
	{
		ISegment segment = new SimpleSegment(line.Offset, line.TotalLength);
		string text = textArea.Document.GetText(segment);
		if (string.IsNullOrEmpty(text))
		{
			return false;
		}
		text = TextUtilities.NormalizeNewLines(text, Environment.NewLine);
		SetClipboardText(text, textArea);
		textArea.OnTextCopied(new TextEventArgs(text));
		return true;
	}

	private static void CanPaste(object target, CanExecuteRoutedEventArgs args)
	{
		TextArea textArea = GetTextArea(target);
		if (textArea?.Document != null)
		{
			args.CanExecute = textArea.ReadOnlySectionProvider.CanInsert(textArea.Caret.Offset);
			args.Handled = true;
		}
	}

	private static async void OnPaste(object target, ExecutedRoutedEventArgs args)
	{
		TextArea textArea = GetTextArea(target);
		if (textArea?.Document == null)
		{
			return;
		}
		textArea.Document.BeginUpdate();
		string text;
		try
		{
			text = await (TopLevel.GetTopLevel(textArea)?.Clipboard?.GetTextAsync());
		}
		catch (Exception)
		{
			textArea.Document.EndUpdate();
			return;
		}
		if (text != null)
		{
			text = GetTextToPaste(text, textArea);
			if (!string.IsNullOrEmpty(text))
			{
				textArea.ReplaceSelectionWithText(text);
			}
			textArea.Caret.BringCaretToView();
			args.Handled = true;
			textArea.Document.EndUpdate();
		}
	}

	internal static string GetTextToPaste(string text, TextArea textArea)
	{
		try
		{
			string newLineFromDocument = TextUtilities.GetNewLineFromDocument(textArea.Document, textArea.Caret.Line);
			text = TextUtilities.NormalizeNewLines(text, newLineFromDocument);
			text = (textArea.Options.ConvertTabsToSpaces ? text.Replace("\t", new string(' ', textArea.Options.IndentationSize)) : text);
			return text;
		}
		catch (OutOfMemoryException)
		{
			return null;
		}
	}

	private static void OnToggleOverstrike(object target, ExecutedRoutedEventArgs args)
	{
		TextArea textArea = GetTextArea(target);
		if (textArea != null && textArea.Options.AllowToggleOverstrikeMode)
		{
			textArea.OverstrikeMode = !textArea.OverstrikeMode;
		}
	}

	private static void OnDeleteLine(object target, ExecutedRoutedEventArgs args)
	{
		TextArea textArea = GetTextArea(target);
		if (textArea?.Document != null)
		{
			int number2;
			int number;
			if (textArea.Selection.Length == 0)
			{
				number2 = (number = textArea.Caret.Line);
			}
			else
			{
				number2 = Math.Min(textArea.Selection.StartPosition.Line, textArea.Selection.EndPosition.Line);
				number = Math.Max(textArea.Selection.StartPosition.Line, textArea.Selection.EndPosition.Line);
			}
			DocumentLine lineByNumber = textArea.Document.GetLineByNumber(number2);
			DocumentLine lineByNumber2 = textArea.Document.GetLineByNumber(number);
			textArea.Selection = Selection.Create(textArea, lineByNumber.Offset, lineByNumber2.Offset + lineByNumber2.TotalLength);
			textArea.RemoveSelectedText();
			args.Handled = true;
		}
	}

	private static void OnRemoveLeadingWhitespace(object target, ExecutedRoutedEventArgs args)
	{
		TransformSelectedLines(delegate(TextArea textArea, DocumentLine line)
		{
			textArea.Document.Remove(TextUtilities.GetLeadingWhitespace(textArea.Document, line));
		}, target, args, DefaultSegmentType.WholeDocument);
	}

	private static void OnRemoveTrailingWhitespace(object target, ExecutedRoutedEventArgs args)
	{
		TransformSelectedLines(delegate(TextArea textArea, DocumentLine line)
		{
			textArea.Document.Remove(TextUtilities.GetTrailingWhitespace(textArea.Document, line));
		}, target, args, DefaultSegmentType.WholeDocument);
	}

	private static void OnConvertTabsToSpaces(object target, ExecutedRoutedEventArgs args)
	{
		TransformSelectedSegments(ConvertTabsToSpaces, target, args, DefaultSegmentType.WholeDocument);
	}

	private static void OnConvertLeadingTabsToSpaces(object target, ExecutedRoutedEventArgs args)
	{
		TransformSelectedLines(delegate(TextArea textArea, DocumentLine line)
		{
			ConvertTabsToSpaces(textArea, TextUtilities.GetLeadingWhitespace(textArea.Document, line));
		}, target, args, DefaultSegmentType.WholeDocument);
	}

	private static void ConvertTabsToSpaces(TextArea textArea, ISegment segment)
	{
		TextDocument document = textArea.Document;
		int num = segment.EndOffset;
		string text = new string(' ', textArea.Options.IndentationSize);
		for (int i = segment.Offset; i < num; i++)
		{
			if (document.GetCharAt(i) == '\t')
			{
				document.Replace(i, 1, text, OffsetChangeMappingType.CharacterReplace);
				num += text.Length - 1;
			}
		}
	}

	private static void OnConvertSpacesToTabs(object target, ExecutedRoutedEventArgs args)
	{
		TransformSelectedSegments(ConvertSpacesToTabs, target, args, DefaultSegmentType.WholeDocument);
	}

	private static void OnConvertLeadingSpacesToTabs(object target, ExecutedRoutedEventArgs args)
	{
		TransformSelectedLines(delegate(TextArea textArea, DocumentLine line)
		{
			ConvertSpacesToTabs(textArea, TextUtilities.GetLeadingWhitespace(textArea.Document, line));
		}, target, args, DefaultSegmentType.WholeDocument);
	}

	private static void ConvertSpacesToTabs(TextArea textArea, ISegment segment)
	{
		TextDocument document = textArea.Document;
		int num = segment.EndOffset;
		int indentationSize = textArea.Options.IndentationSize;
		int num2 = 0;
		for (int i = segment.Offset; i < num; i++)
		{
			if (document.GetCharAt(i) == ' ')
			{
				num2++;
				if (num2 == indentationSize)
				{
					document.Replace(i - (indentationSize - 1), indentationSize, "\t", OffsetChangeMappingType.CharacterReplace);
					num2 = 0;
					i -= indentationSize - 1;
					num -= indentationSize - 1;
				}
			}
			else
			{
				num2 = 0;
			}
		}
	}

	private static void ConvertCase(Func<string, string> transformText, object target, ExecutedRoutedEventArgs args)
	{
		TransformSelectedSegments(delegate(TextArea textArea, ISegment segment)
		{
			string text = textArea.Document.GetText(segment);
			string text2 = transformText(text);
			textArea.Document.Replace(segment.Offset, segment.Length, text2, OffsetChangeMappingType.CharacterReplace);
		}, target, args, DefaultSegmentType.WholeDocument);
	}

	private static void OnConvertToUpperCase(object target, ExecutedRoutedEventArgs args)
	{
		ConvertCase(CultureInfo.CurrentCulture.TextInfo.ToUpper, target, args);
	}

	private static void OnConvertToLowerCase(object target, ExecutedRoutedEventArgs args)
	{
		ConvertCase(CultureInfo.CurrentCulture.TextInfo.ToLower, target, args);
	}

	private static void OnConvertToTitleCase(object target, ExecutedRoutedEventArgs args)
	{
		throw new NotSupportedException();
	}

	private static void OnInvertCase(object target, ExecutedRoutedEventArgs args)
	{
		ConvertCase(InvertCase, target, args);
	}

	private static string InvertCase(string text)
	{
		char[] array = text.ToCharArray();
		for (int i = 0; i < array.Length; i++)
		{
			char c = array[i];
			array[i] = (char.IsUpper(c) ? char.ToLower(c) : char.ToUpper(c));
		}
		return new string(array);
	}

	private static void OnIndentSelection(object target, ExecutedRoutedEventArgs args)
	{
		TextArea textArea = GetTextArea(target);
		if (textArea?.Document == null || textArea.IndentationStrategy == null)
		{
			return;
		}
		using (textArea.Document.RunUpdate())
		{
			int beginLine;
			int endLine;
			if (textArea.Selection.IsEmpty)
			{
				beginLine = 1;
				endLine = textArea.Document.LineCount;
			}
			else
			{
				beginLine = textArea.Document.GetLineByOffset(textArea.Selection.SurroundingSegment.Offset).LineNumber;
				endLine = textArea.Document.GetLineByOffset(textArea.Selection.SurroundingSegment.EndOffset).LineNumber;
			}
			textArea.IndentationStrategy.IndentLines(textArea.Document, beginLine, endLine);
		}
		textArea.Caret.BringCaretToView();
		args.Handled = true;
	}
}
