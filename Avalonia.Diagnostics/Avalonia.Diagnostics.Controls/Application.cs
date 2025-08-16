using System;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Rendering;
using Avalonia.Styling;

namespace Avalonia.Diagnostics.Controls;

internal class Application : TopLevelGroup, ICloseable, IDisposable
{
	private readonly Avalonia.Application _application;

	public static readonly StyledProperty<ThemeVariant?> RequestedThemeVariantProperty = ThemeVariantScope.RequestedThemeVariantProperty.AddOwner<Application>();

	internal Avalonia.Application Instance => _application;

	public object? DataContext => _application.DataContext;

	public DataTemplates DataTemplates => _application.DataTemplates;

	public InputManager? InputManager => _application.InputManager;

	public IResourceDictionary Resources => _application.Resources;

	public Styles Styles => _application.Styles;

	public IApplicationLifetime? ApplicationLifetime => _application.ApplicationLifetime;

	public string? Name => _application.Name;

	internal IRenderer? RendererRoot { get; }

	public ThemeVariant? RequestedThemeVariant
	{
		get
		{
			return GetValue(RequestedThemeVariantProperty);
		}
		set
		{
			SetValue(RequestedThemeVariantProperty, value);
		}
	}

	public event EventHandler? Closed;

	public Application(ClassicDesktopStyleApplicationLifetimeTopLevelGroup group, Avalonia.Application application)
		: base(group)
	{
		Application application2 = this;
		_application = application;
		IApplicationLifetime applicationLifetime = _application.ApplicationLifetime;
		IControlledApplicationLifetime controller = applicationLifetime as IControlledApplicationLifetime;
		if (controller != null)
		{
			EventHandler<ControlledApplicationLifetimeExitEventArgs> eh = null;
			eh = delegate(object? s, ControlledApplicationLifetimeExitEventArgs e)
			{
				controller.Exit -= eh;
				application2.Closed?.Invoke(s, e);
			};
			controller.Exit += eh;
		}
		applicationLifetime = application.ApplicationLifetime;
		RendererRoot = ((applicationLifetime is IClassicDesktopStyleApplicationLifetime classicDesktopStyleApplicationLifetime) ? classicDesktopStyleApplicationLifetime.MainWindow?.Renderer : ((!(applicationLifetime is ISingleViewApplicationLifetime singleViewApplicationLifetime)) ? null : singleViewApplicationLifetime.MainView?.VisualRoot?.Renderer));
		SetCurrentValue(RequestedThemeVariantProperty, application.RequestedThemeVariant);
		_application.PropertyChanged += ApplicationOnPropertyChanged;
	}

	public void Dispose()
	{
		_application.PropertyChanged -= ApplicationOnPropertyChanged;
	}

	private void ApplicationOnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Property == Avalonia.Application.RequestedThemeVariantProperty)
		{
			SetCurrentValue(RequestedThemeVariantProperty, e.GetNewValue<ThemeVariant>());
		}
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == RequestedThemeVariantProperty)
		{
			_application.RequestedThemeVariant = change.GetNewValue<ThemeVariant>();
		}
	}
}
