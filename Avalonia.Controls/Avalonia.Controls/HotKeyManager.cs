using System;
using System.Windows.Input;
using Avalonia.Controls.Utils;
using Avalonia.Input;
using Avalonia.Logging;
using Avalonia.Reactive;

namespace Avalonia.Controls;

public class HotKeyManager
{
	private class HotkeyCommandWrapper : ICommand
	{
		private readonly WeakReference reference;

		public event EventHandler? CanExecuteChanged;

		public HotkeyCommandWrapper(Control control)
		{
			reference = new WeakReference(control);
		}

		public ICommand? GetCommand()
		{
			object target = reference.Target;
			if (target != null)
			{
				if (target is ICommandSource commandSource)
				{
					ICommand command = commandSource.Command;
					if (command != null)
					{
						return command;
					}
				}
				if (target is IClickableControl)
				{
					return this;
				}
			}
			return null;
		}

		public bool CanExecute(object? parameter)
		{
			object target = reference.Target;
			if (target != null)
			{
				if (target is ICommandSource commandSource)
				{
					ICommand command = commandSource.Command;
					if (command != null)
					{
						if (commandSource.IsEffectivelyEnabled)
						{
							return command.CanExecute(commandSource.CommandParameter);
						}
						return false;
					}
				}
				if (target is IClickableControl clickableControl)
				{
					return clickableControl.IsEffectivelyEnabled;
				}
			}
			return false;
		}

		public void Execute(object? parameter)
		{
			object target = reference.Target;
			if (target == null)
			{
				return;
			}
			if (target is ICommandSource commandSource)
			{
				ICommand command = commandSource.Command;
				if (command != null)
				{
					command.Execute(commandSource.CommandParameter);
					return;
				}
			}
			if (target is IClickableControl { IsEffectivelyEnabled: not false } clickableControl)
			{
				clickableControl.RaiseClick();
			}
		}
	}

	private class Manager
	{
		private readonly Control _control;

		private TopLevel? _root;

		private IDisposable? _parentSub;

		private IDisposable? _hotkeySub;

		private KeyGesture? _hotkey;

		private readonly HotkeyCommandWrapper _wrapper;

		private KeyBinding? _binding;

		public Manager(Control control)
		{
			_control = control;
			_wrapper = new HotkeyCommandWrapper(_control);
		}

		public void Init()
		{
			_hotkeySub = _control.GetObservable(HotKeyProperty).Subscribe(OnHotkeyChanged);
			_parentSub = AncestorFinder.Create<TopLevel>(_control).Subscribe(OnParentChanged);
		}

		private void OnParentChanged(TopLevel? control)
		{
			Unregister();
			_root = control;
			Register();
		}

		private void OnHotkeyChanged(KeyGesture? hotkey)
		{
			if (hotkey == null)
			{
				Stop();
				return;
			}
			Unregister();
			_hotkey = hotkey;
			Register();
		}

		private void Unregister()
		{
			if (_root != null && _binding != null)
			{
				_root.KeyBindings.Remove(_binding);
			}
			_binding = null;
		}

		private void Register()
		{
			if (_root != null && _hotkey != null)
			{
				_binding = new KeyBinding
				{
					Gesture = _hotkey,
					Command = _wrapper
				};
				_root.KeyBindings.Add(_binding);
			}
		}

		private void Stop()
		{
			Unregister();
			_parentSub?.Dispose();
			_hotkeySub?.Dispose();
		}
	}

	public static readonly AttachedProperty<KeyGesture?> HotKeyProperty;

	static HotKeyManager()
	{
		HotKeyProperty = AvaloniaProperty.RegisterAttached<Control, KeyGesture>("HotKey", typeof(HotKeyManager));
		HotKeyProperty.Changed.Subscribe(delegate(AvaloniaPropertyChangedEventArgs<KeyGesture> args)
		{
			if ((object)args.NewValue.Value != null)
			{
				Control control = args.Sender as Control;
				if (!(control is IClickableControl))
				{
					Logger.TryGet(LogEventLevel.Warning, "Control")?.Log(control, $"The element {args.Sender.GetType().Name} does not implement IClickableControl and does not support binding a HotKey ({args.NewValue}).");
				}
				else
				{
					new Manager(control).Init();
				}
			}
		});
	}

	public static void SetHotKey(AvaloniaObject target, KeyGesture value)
	{
		target.SetValue(HotKeyProperty, value);
	}

	public static KeyGesture? GetHotKey(AvaloniaObject target)
	{
		return target.GetValue(HotKeyProperty);
	}
}
