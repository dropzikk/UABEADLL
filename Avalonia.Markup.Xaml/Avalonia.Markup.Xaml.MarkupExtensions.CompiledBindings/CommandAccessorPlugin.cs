using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using Avalonia.Data;
using Avalonia.Data.Core.Plugins;
using Avalonia.Threading;
using Avalonia.Utilities;

namespace Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;

internal class CommandAccessorPlugin : IPropertyAccessorPlugin
{
	private sealed class CommandAccessor : PropertyAccessorBase
	{
		private sealed class Command : ICommand
		{
			private readonly WeakReference<object?> _target;

			private readonly Action<object, object?> _execute;

			private readonly Func<object, object?, bool>? _canExecute;

			public event EventHandler? CanExecuteChanged;

			public Command(WeakReference<object?> target, Action<object, object?> execute, Func<object, object?, bool>? canExecute)
			{
				_target = target;
				_execute = execute;
				_canExecute = canExecute;
			}

			public void RaiseCanExecuteChanged()
			{
				Dispatcher.UIThread.Post(delegate
				{
					this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
				}, DispatcherPriority.Input);
			}

			public bool CanExecute(object? parameter)
			{
				if (_target.TryGetTarget(out object target))
				{
					if (_canExecute == null)
					{
						return true;
					}
					return _canExecute(target, parameter);
				}
				return false;
			}

			public void Execute(object? parameter)
			{
				if (_target.TryGetTarget(out object target))
				{
					_execute(target, parameter);
				}
			}
		}

		private readonly WeakReference<object?> _reference;

		private readonly Command _command;

		private readonly ISet<string> _dependsOnProperties;

		public override object? Value
		{
			get
			{
				if (!_reference.TryGetTarget(out object _))
				{
					return null;
				}
				return _command;
			}
		}

		public override Type PropertyType => typeof(ICommand);

		public CommandAccessor(WeakReference<object?> reference, Action<object, object?> execute, Func<object, object?, bool>? canExecute, ISet<string> dependsOnProperties)
		{
			_reference = reference ?? throw new ArgumentNullException("reference");
			_dependsOnProperties = dependsOnProperties;
			_command = new Command(reference, execute, canExecute);
		}

		private void RaiseCanExecuteChanged()
		{
			_command.RaiseCanExecuteChanged();
		}

		public override bool SetValue(object? value, BindingPriority priority)
		{
			return false;
		}

		private void OnNotifyPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (string.IsNullOrEmpty(e.PropertyName) || _dependsOnProperties.Contains(e.PropertyName))
			{
				RaiseCanExecuteChanged();
			}
		}

		protected override void SubscribeCore()
		{
			SendCurrentValue();
			SubscribeToChanges();
		}

		protected override void UnsubscribeCore()
		{
			ISet<string> dependsOnProperties = _dependsOnProperties;
			if (dependsOnProperties != null && dependsOnProperties.Count > 0 && _reference.TryGetTarget(out object target) && target is INotifyPropertyChanged target2)
			{
				WeakEventHandlerManager.Unsubscribe<PropertyChangedEventArgs, CommandAccessor>(target2, "PropertyChanged", OnNotifyPropertyChanged);
			}
		}

		private void SendCurrentValue()
		{
			try
			{
				object value = Value;
				PublishValue(value);
			}
			catch
			{
			}
		}

		private void SubscribeToChanges()
		{
			ISet<string> dependsOnProperties = _dependsOnProperties;
			if (dependsOnProperties != null && dependsOnProperties.Count > 0 && _reference.TryGetTarget(out object target) && target is INotifyPropertyChanged target2)
			{
				WeakEventHandlerManager.Subscribe<INotifyPropertyChanged, PropertyChangedEventArgs, CommandAccessor>(target2, "PropertyChanged", OnNotifyPropertyChanged);
			}
		}
	}

	private readonly Action<object, object?> _execute;

	private readonly Func<object, object?, bool>? _canExecute;

	private readonly ISet<string> _dependsOnProperties;

	public CommandAccessorPlugin(Action<object, object?> execute, Func<object, object?, bool>? canExecute, ISet<string> dependsOnProperties)
	{
		_execute = execute;
		_canExecute = canExecute;
		_dependsOnProperties = dependsOnProperties;
	}

	[RequiresUnreferencedCode("PropertyAccessors might require unreferenced code.")]
	public bool Match(object obj, string propertyName)
	{
		throw new InvalidOperationException("The CommandAccessorPlugin does not support dynamic matching");
	}

	[RequiresUnreferencedCode("PropertyAccessors might require unreferenced code.")]
	public IPropertyAccessor Start(WeakReference<object?> reference, string propertyName)
	{
		return new CommandAccessor(reference, _execute, _canExecute, _dependsOnProperties);
	}
}
