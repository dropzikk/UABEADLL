using System;
using System.Linq;
using System.Windows.Input;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AvaloniaEdit;

public class RoutedCommand : ICommand
{
	private static IInputElement _inputElement;

	public string Name { get; }

	public KeyGesture Gesture { get; }

	public static RoutedEvent<CanExecuteRoutedEventArgs> CanExecuteEvent { get; }

	public static RoutedEvent<ExecutedRoutedEventArgs> ExecutedEvent { get; }

	event EventHandler ICommand.CanExecuteChanged
	{
		add
		{
		}
		remove
		{
		}
	}

	public RoutedCommand(string name, KeyGesture keyGesture = null)
	{
		Name = name;
		Gesture = keyGesture;
	}

	static RoutedCommand()
	{
		CanExecuteEvent = RoutedEvent.Register<CanExecuteRoutedEventArgs>("CanExecuteEvent", RoutingStrategies.Bubble, typeof(RoutedCommand));
		ExecutedEvent = RoutedEvent.Register<ExecutedRoutedEventArgs>("ExecutedEvent", RoutingStrategies.Bubble, typeof(RoutedCommand));
		CanExecuteEvent.AddClassHandler<Interactive>(CanExecuteEventHandler);
		ExecutedEvent.AddClassHandler<Interactive>(ExecutedEventHandler);
		InputElement.GotFocusEvent.AddClassHandler<Interactive>(GotFocusEventHandler);
	}

	private static void CanExecuteEventHandler(Interactive control, CanExecuteRoutedEventArgs args)
	{
		if (control is IRoutedCommandBindable routedCommandBindable)
		{
			RoutedCommandBinding routedCommandBinding = routedCommandBindable.CommandBindings.Where((RoutedCommandBinding c) => c != null).FirstOrDefault((RoutedCommandBinding c) => c.Command == args.Command && c.DoCanExecute(control, args));
			args.CanExecute = routedCommandBinding != null;
		}
	}

	private static void ExecutedEventHandler(Interactive control, ExecutedRoutedEventArgs args)
	{
		if (control is IRoutedCommandBindable routedCommandBindable)
		{
			routedCommandBindable.CommandBindings.Where((RoutedCommandBinding c) => c != null).FirstOrDefault((RoutedCommandBinding c) => c.Command == args.Command && c.DoExecuted(control, args));
		}
	}

	private static void GotFocusEventHandler(Interactive control, GotFocusEventArgs args)
	{
		_inputElement = args.Source as IInputElement;
	}

	public bool CanExecute(object parameter, IInputElement target)
	{
		if (target == null)
		{
			return false;
		}
		CanExecuteRoutedEventArgs canExecuteRoutedEventArgs = new CanExecuteRoutedEventArgs(this, parameter);
		target.RaiseEvent(canExecuteRoutedEventArgs);
		return canExecuteRoutedEventArgs.CanExecute;
	}

	bool ICommand.CanExecute(object parameter)
	{
		return CanExecute(parameter, _inputElement);
	}

	public void Execute(object parameter, IInputElement target)
	{
		if (target != null)
		{
			ExecutedRoutedEventArgs e = new ExecutedRoutedEventArgs(this, parameter);
			target.RaiseEvent(e);
		}
	}

	void ICommand.Execute(object parameter)
	{
		Execute(parameter, _inputElement);
	}
}
