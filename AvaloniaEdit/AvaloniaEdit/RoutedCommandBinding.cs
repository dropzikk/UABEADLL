using System;

namespace AvaloniaEdit;

public class RoutedCommandBinding
{
	public RoutedCommand Command { get; }

	public event EventHandler<CanExecuteRoutedEventArgs> CanExecute;

	public event EventHandler<ExecutedRoutedEventArgs> Executed;

	public RoutedCommandBinding(RoutedCommand command, EventHandler<ExecutedRoutedEventArgs> executed = null, EventHandler<CanExecuteRoutedEventArgs> canExecute = null)
	{
		Command = command;
		if (executed != null)
		{
			Executed += executed;
		}
		if (canExecute != null)
		{
			CanExecute += canExecute;
		}
	}

	internal bool DoCanExecute(object sender, CanExecuteRoutedEventArgs e)
	{
		if (e.Handled)
		{
			return true;
		}
		EventHandler<CanExecuteRoutedEventArgs> eventHandler = this.CanExecute;
		if (eventHandler == null)
		{
			if (this.Executed != null)
			{
				e.Handled = true;
				e.CanExecute = true;
			}
		}
		else
		{
			eventHandler(sender, e);
			if (e.CanExecute)
			{
				e.Handled = true;
			}
		}
		return e.CanExecute;
	}

	internal bool DoExecuted(object sender, ExecutedRoutedEventArgs e)
	{
		if (!e.Handled)
		{
			EventHandler<ExecutedRoutedEventArgs> eventHandler = this.Executed;
			if (eventHandler != null && DoCanExecute(sender, new CanExecuteRoutedEventArgs(e.Command, e.Parameter)))
			{
				eventHandler(sender, e);
				e.Handled = true;
				return true;
			}
		}
		return false;
	}
}
