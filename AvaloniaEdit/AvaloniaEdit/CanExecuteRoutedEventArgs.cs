using System;
using System.Windows.Input;
using Avalonia.Interactivity;

namespace AvaloniaEdit;

public sealed class CanExecuteRoutedEventArgs : RoutedEventArgs
{
	public ICommand Command { get; }

	public object Parameter { get; }

	public bool CanExecute { get; set; }

	internal CanExecuteRoutedEventArgs(ICommand command, object parameter)
	{
		Command = command ?? throw new ArgumentNullException("command");
		Parameter = parameter;
		base.RoutedEvent = RoutedCommand.CanExecuteEvent;
	}
}
