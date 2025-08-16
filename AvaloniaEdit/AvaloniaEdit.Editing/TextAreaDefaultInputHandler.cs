using System;
using System.Windows.Input;
using Avalonia.Input;
using AvaloniaEdit.Document;

namespace AvaloniaEdit.Editing;

public class TextAreaDefaultInputHandler : TextAreaInputHandler
{
	public TextAreaInputHandler CaretNavigation { get; }

	public TextAreaInputHandler Editing { get; }

	public ITextAreaInputHandler MouseSelection { get; }

	public TextAreaDefaultInputHandler(TextArea textArea)
		: base(textArea)
	{
		base.NestedInputHandlers.Add(CaretNavigation = CaretNavigationCommandHandler.Create(textArea));
		base.NestedInputHandlers.Add(Editing = EditingCommandHandler.Create(textArea));
		base.NestedInputHandlers.Add(MouseSelection = new SelectionMouseHandler(textArea));
		AddBinding(ApplicationCommands.Undo, ExecuteUndo, CanExecuteUndo);
		AddBinding(ApplicationCommands.Redo, ExecuteRedo, CanExecuteRedo);
	}

	private void AddBinding(RoutedCommand command, EventHandler<ExecutedRoutedEventArgs> handler, EventHandler<CanExecuteRoutedEventArgs> canExecuteHandler = null)
	{
		base.CommandBindings.Add(new RoutedCommandBinding(command, handler, canExecuteHandler));
	}

	internal static KeyBinding CreateKeyBinding(ICommand command, KeyModifiers modifiers, Key key)
	{
		return CreateKeyBinding(command, new KeyGesture(key, modifiers));
	}

	internal static KeyBinding CreateKeyBinding(ICommand command, KeyGesture gesture)
	{
		return new KeyBinding
		{
			Command = command,
			Gesture = gesture
		};
	}

	private UndoStack GetUndoStack()
	{
		return base.TextArea.Document?.UndoStack;
	}

	private void ExecuteUndo(object sender, ExecutedRoutedEventArgs e)
	{
		UndoStack undoStack = GetUndoStack();
		if (undoStack != null)
		{
			if (undoStack.CanUndo)
			{
				undoStack.Undo();
				base.TextArea.Caret.BringCaretToView();
			}
			e.Handled = true;
		}
	}

	private void CanExecuteUndo(object sender, CanExecuteRoutedEventArgs e)
	{
		UndoStack undoStack = GetUndoStack();
		if (undoStack != null)
		{
			e.Handled = true;
			e.CanExecute = undoStack.CanUndo;
		}
	}

	private void ExecuteRedo(object sender, ExecutedRoutedEventArgs e)
	{
		UndoStack undoStack = GetUndoStack();
		if (undoStack != null)
		{
			if (undoStack.CanRedo)
			{
				undoStack.Redo();
				base.TextArea.Caret.BringCaretToView();
			}
			e.Handled = true;
		}
	}

	private void CanExecuteRedo(object sender, CanExecuteRoutedEventArgs e)
	{
		UndoStack undoStack = GetUndoStack();
		if (undoStack != null)
		{
			e.Handled = true;
			e.CanExecute = undoStack.CanRedo;
		}
	}
}
