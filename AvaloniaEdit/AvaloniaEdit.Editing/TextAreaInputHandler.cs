using System;
using System.Collections.Generic;
using Avalonia.Input;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Editing;

public class TextAreaInputHandler : ITextAreaInputHandler
{
	private readonly ObserveAddRemoveCollection<RoutedCommandBinding> _commandBindings;

	private readonly List<KeyBinding> _keyBindings;

	private readonly ObserveAddRemoveCollection<ITextAreaInputHandler> _nestedInputHandlers;

	public TextArea TextArea { get; }

	public bool IsAttached { get; private set; }

	public ICollection<RoutedCommandBinding> CommandBindings => _commandBindings;

	public ICollection<KeyBinding> KeyBindings => _keyBindings;

	public ICollection<ITextAreaInputHandler> NestedInputHandlers => _nestedInputHandlers;

	public TextAreaInputHandler(TextArea textArea)
	{
		TextArea = textArea ?? throw new ArgumentNullException("textArea");
		_commandBindings = new ObserveAddRemoveCollection<RoutedCommandBinding>(CommandBinding_Added, CommandBinding_Removed);
		_keyBindings = new List<KeyBinding>();
		_nestedInputHandlers = new ObserveAddRemoveCollection<ITextAreaInputHandler>(NestedInputHandler_Added, NestedInputHandler_Removed);
	}

	private void CommandBinding_Added(RoutedCommandBinding commandBinding)
	{
		if (IsAttached)
		{
			TextArea.CommandBindings.Add(commandBinding);
		}
	}

	private void CommandBinding_Removed(RoutedCommandBinding commandBinding)
	{
		if (IsAttached)
		{
			TextArea.CommandBindings.Remove(commandBinding);
		}
	}

	public void AddBinding(RoutedCommand command, KeyModifiers modifiers, Key key, EventHandler<ExecutedRoutedEventArgs> handler)
	{
		CommandBindings.Add(new RoutedCommandBinding(command, handler));
		KeyBindings.Add(new KeyBinding
		{
			Command = command,
			Gesture = new KeyGesture(key, modifiers)
		});
	}

	private void NestedInputHandler_Added(ITextAreaInputHandler handler)
	{
		if (handler == null)
		{
			throw new ArgumentNullException("handler");
		}
		if (handler.TextArea != TextArea)
		{
			throw new ArgumentException("The nested handler must be working for the same text area!");
		}
		if (IsAttached)
		{
			handler.Attach();
		}
	}

	private void NestedInputHandler_Removed(ITextAreaInputHandler handler)
	{
		if (IsAttached)
		{
			handler.Detach();
		}
	}

	private void TextAreaOnKeyDown(object sender, KeyEventArgs keyEventArgs)
	{
		foreach (KeyBinding keyBinding in _keyBindings)
		{
			if (keyEventArgs.Handled)
			{
				break;
			}
			keyBinding.TryHandle(keyEventArgs);
		}
		foreach (RoutedCommandBinding commandBinding in CommandBindings)
		{
			KeyGesture gesture = commandBinding.Command.Gesture;
			if ((object)gesture != null && gesture.Matches(keyEventArgs))
			{
				commandBinding.Command.Execute(null, (IInputElement)sender);
				keyEventArgs.Handled = true;
				break;
			}
		}
	}

	public virtual void Attach()
	{
		if (IsAttached)
		{
			throw new InvalidOperationException("Input handler is already attached");
		}
		IsAttached = true;
		TextArea.CommandBindings.AddRange(_commandBindings);
		TextArea.KeyDown += TextAreaOnKeyDown;
		foreach (ITextAreaInputHandler nestedInputHandler in _nestedInputHandlers)
		{
			nestedInputHandler.Attach();
		}
	}

	public virtual void Detach()
	{
		if (!IsAttached)
		{
			throw new InvalidOperationException("Input handler is not attached");
		}
		IsAttached = false;
		foreach (RoutedCommandBinding commandBinding in _commandBindings)
		{
			TextArea.CommandBindings.Remove(commandBinding);
		}
		TextArea.KeyDown -= TextAreaOnKeyDown;
		foreach (ITextAreaInputHandler nestedInputHandler in _nestedInputHandlers)
		{
			nestedInputHandler.Detach();
		}
	}
}
