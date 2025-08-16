using System;
using System.Windows.Input;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Metadata;
using Avalonia.Utilities;

namespace Avalonia.Controls;

public class NativeMenuItem : NativeMenuItemBase, INativeMenuItemExporterEventsImplBridge
{
	private class CanExecuteChangedSubscriber : IWeakEventSubscriber<EventArgs>
	{
		private readonly NativeMenuItem _parent;

		public CanExecuteChangedSubscriber(NativeMenuItem parent)
		{
			_parent = parent;
		}

		public void OnEvent(object? sender, WeakEvent ev, EventArgs e)
		{
			_parent.CanExecuteChanged();
		}
	}

	private readonly CanExecuteChangedSubscriber _canExecuteChangedSubscriber;

	public static readonly StyledProperty<NativeMenu?> MenuProperty = AvaloniaProperty.Register<NativeMenuItem, NativeMenu>("Menu", null, inherits: false, BindingMode.OneWay, null, CoerceMenu);

	public static readonly StyledProperty<Bitmap?> IconProperty = AvaloniaProperty.Register<NativeMenuItem, Bitmap>("Icon");

	public static readonly StyledProperty<string?> HeaderProperty = AvaloniaProperty.Register<NativeMenuItem, string>("Header");

	public static readonly StyledProperty<KeyGesture?> GestureProperty = AvaloniaProperty.Register<NativeMenuItem, KeyGesture>("Gesture");

	public static readonly StyledProperty<bool> IsCheckedProperty = AvaloniaProperty.Register<NativeMenuItem, bool>("IsChecked", defaultValue: false);

	public static readonly StyledProperty<NativeMenuItemToggleType> ToggleTypeProperty = AvaloniaProperty.Register<NativeMenuItem, NativeMenuItemToggleType>("ToggleType", NativeMenuItemToggleType.None);

	public static readonly StyledProperty<ICommand?> CommandProperty = Button.CommandProperty.AddOwner<NativeMenuItem>(new StyledPropertyMetadata<ICommand>(default(Optional<ICommand>), BindingMode.Default, null, enableDataValidation: true));

	public static readonly StyledProperty<object?> CommandParameterProperty = Button.CommandParameterProperty.AddOwner<NativeMenuItem>();

	public static readonly StyledProperty<bool> IsEnabledProperty = AvaloniaProperty.Register<NativeMenuItem, bool>("IsEnabled", defaultValue: true);

	[Content]
	public NativeMenu? Menu
	{
		get
		{
			return GetValue(MenuProperty);
		}
		set
		{
			SetValue(MenuProperty, value);
		}
	}

	public Bitmap? Icon
	{
		get
		{
			return GetValue(IconProperty);
		}
		set
		{
			SetValue(IconProperty, value);
		}
	}

	public string? Header
	{
		get
		{
			return GetValue(HeaderProperty);
		}
		set
		{
			SetValue(HeaderProperty, value);
		}
	}

	public KeyGesture? Gesture
	{
		get
		{
			return GetValue(GestureProperty);
		}
		set
		{
			SetValue(GestureProperty, value);
		}
	}

	public bool IsChecked
	{
		get
		{
			return GetValue(IsCheckedProperty);
		}
		set
		{
			SetValue(IsCheckedProperty, value);
		}
	}

	public NativeMenuItemToggleType ToggleType
	{
		get
		{
			return GetValue(ToggleTypeProperty);
		}
		set
		{
			SetValue(ToggleTypeProperty, value);
		}
	}

	public bool IsEnabled
	{
		get
		{
			return GetValue(IsEnabledProperty);
		}
		set
		{
			SetValue(IsEnabledProperty, value);
		}
	}

	public bool HasClickHandlers => this.Click != null;

	public ICommand? Command
	{
		get
		{
			return GetValue(CommandProperty);
		}
		set
		{
			SetValue(CommandProperty, value);
		}
	}

	public object? CommandParameter
	{
		get
		{
			return GetValue(CommandParameterProperty);
		}
		set
		{
			SetValue(CommandParameterProperty, value);
		}
	}

	public event EventHandler? Click;

	public NativeMenuItem()
	{
		_canExecuteChangedSubscriber = new CanExecuteChangedSubscriber(this);
	}

	public NativeMenuItem(string header)
		: this()
	{
		Header = header;
	}

	private static NativeMenu? CoerceMenu(AvaloniaObject sender, NativeMenu? value)
	{
		if (value != null && value.Parent != null && value.Parent != sender)
		{
			throw new InvalidOperationException("NativeMenu already has a parent");
		}
		return value;
	}

	private void CanExecuteChanged()
	{
		SetCurrentValue(IsEnabledProperty, Command?.CanExecute(CommandParameter) ?? true);
	}

	void INativeMenuItemExporterEventsImplBridge.RaiseClicked()
	{
		this.Click?.Invoke(this, new EventArgs());
		ICommand? command = Command;
		if (command != null && command.CanExecute(CommandParameter))
		{
			Command.Execute(CommandParameter);
		}
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == MenuProperty && change.NewValue is NativeMenu nativeMenu)
		{
			if (nativeMenu.Parent != null && nativeMenu.Parent != this)
			{
				throw new InvalidOperationException("NativeMenu already has a parent");
			}
			nativeMenu.Parent = this;
		}
		else if (change.Property == CommandProperty)
		{
			if (change.OldValue is ICommand target)
			{
				WeakEvents.CommandCanExecuteChanged.Unsubscribe(target, _canExecuteChangedSubscriber);
			}
			if (change.NewValue is ICommand target2)
			{
				WeakEvents.CommandCanExecuteChanged.Subscribe(target2, _canExecuteChangedSubscriber);
			}
			CanExecuteChanged();
		}
	}
}
