using System;
using System.Windows.Input;
using Avalonia.Interactivity;

namespace AvaloniaEdit;

public sealed class ExecutedRoutedEventArgs : RoutedEventArgs
{
	public ICommand Command { get; }

	public object Parameter { get; }

	internal ExecutedRoutedEventArgs(ICommand command, object parameter)
	{
		Command = command ?? throw new ArgumentNullException("command");
		Parameter = parameter;
		base.RoutedEvent = RoutedCommand.ExecutedEvent;
	}
}
