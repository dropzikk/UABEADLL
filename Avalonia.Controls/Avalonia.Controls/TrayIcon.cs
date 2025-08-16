using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Platform;
using Avalonia.Data;
using Avalonia.Platform;
using Avalonia.Reactive;

namespace Avalonia.Controls;

public class TrayIcon : AvaloniaObject, INativeMenuExporterProvider, IDisposable
{
	private readonly ITrayIconImpl? _impl;

	public static readonly StyledProperty<ICommand?> CommandProperty;

	public static readonly StyledProperty<object?> CommandParameterProperty;

	public static readonly AttachedProperty<TrayIcons?> IconsProperty;

	public static readonly StyledProperty<NativeMenu?> MenuProperty;

	public static readonly StyledProperty<WindowIcon?> IconProperty;

	public static readonly StyledProperty<string?> ToolTipTextProperty;

	public static readonly StyledProperty<bool> IsVisibleProperty;

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

	public WindowIcon? Icon
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

	public string? ToolTipText
	{
		get
		{
			return GetValue(ToolTipTextProperty);
		}
		set
		{
			SetValue(ToolTipTextProperty, value);
		}
	}

	public bool IsVisible
	{
		get
		{
			return GetValue(IsVisibleProperty);
		}
		set
		{
			SetValue(IsVisibleProperty, value);
		}
	}

	public INativeMenuExporter? NativeMenuExporter => _impl?.MenuExporter;

	public event EventHandler? Clicked;

	private TrayIcon(ITrayIconImpl? impl)
	{
		if (impl == null)
		{
			return;
		}
		_impl = impl;
		_impl.SetIsVisible(IsVisible);
		_impl.OnClicked = delegate
		{
			this.Clicked?.Invoke(this, EventArgs.Empty);
			ICommand? command = Command;
			if (command != null && command.CanExecute(CommandParameter))
			{
				Command.Execute(CommandParameter);
			}
		};
	}

	public TrayIcon()
		: this(PlatformManager.CreateTrayIcon())
	{
	}

	static TrayIcon()
	{
		CommandProperty = Button.CommandProperty.AddOwner<TrayIcon>(new StyledPropertyMetadata<ICommand>(default(Optional<ICommand>), BindingMode.Default, null, enableDataValidation: true));
		CommandParameterProperty = Button.CommandParameterProperty.AddOwner<TrayIcon>();
		IconsProperty = AvaloniaProperty.RegisterAttached<TrayIcon, Application, TrayIcons>("Icons");
		MenuProperty = AvaloniaProperty.Register<TrayIcon, NativeMenu>("Menu");
		IconProperty = Window.IconProperty.AddOwner<TrayIcon>();
		ToolTipTextProperty = AvaloniaProperty.Register<TrayIcon, string>("ToolTipText");
		IsVisibleProperty = Visual.IsVisibleProperty.AddOwner<TrayIcon>();
		IconsProperty.Changed.Subscribe(delegate(AvaloniaPropertyChangedEventArgs<TrayIcons> args)
		{
			if (args.Sender is Application)
			{
				if (args.OldValue.Value != null)
				{
					RemoveIcons(args.OldValue.Value);
				}
				if (args.NewValue.Value != null)
				{
					args.NewValue.Value.CollectionChanged += Icons_CollectionChanged;
				}
				return;
			}
			throw new InvalidOperationException("TrayIcon.Icons must be set on the Application.");
		});
		if ((Application.Current ?? throw new InvalidOperationException("Application not yet initialized.")).ApplicationLifetime is IClassicDesktopStyleApplicationLifetime classicDesktopStyleApplicationLifetime)
		{
			classicDesktopStyleApplicationLifetime.Exit += Lifetime_Exit;
		}
	}

	public static void SetIcons(Application o, TrayIcons? trayIcons)
	{
		o.SetValue(IconsProperty, trayIcons);
	}

	public static TrayIcons? GetIcons(Application o)
	{
		return o.GetValue(IconsProperty);
	}

	private static void Lifetime_Exit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
	{
		TrayIcons icons = GetIcons(Application.Current ?? throw new InvalidOperationException("Application not yet initialized."));
		if (icons != null)
		{
			RemoveIcons(icons);
		}
	}

	private static void Icons_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		if (e.OldItems != null)
		{
			RemoveIcons(e.OldItems.Cast<TrayIcon>());
		}
	}

	private static void RemoveIcons(IEnumerable<TrayIcon> icons)
	{
		foreach (TrayIcon icon in icons)
		{
			icon.Dispose();
		}
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == IconProperty)
		{
			_impl?.SetIcon(Icon?.PlatformImpl);
		}
		else if (change.Property == IsVisibleProperty)
		{
			_impl?.SetIsVisible(change.GetNewValue<bool>());
		}
		else if (change.Property == ToolTipTextProperty)
		{
			_impl?.SetToolTipText(change.GetNewValue<string>());
		}
		else if (change.Property == MenuProperty)
		{
			_impl?.MenuExporter?.SetNativeMenu(change.GetNewValue<NativeMenu>());
		}
	}

	public void Dispose()
	{
		_impl?.Dispose();
	}
}
