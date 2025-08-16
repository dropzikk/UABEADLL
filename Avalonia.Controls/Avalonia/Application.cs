using System;
using System.Collections.Generic;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Input.Raw;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Threading;

namespace Avalonia;

public class Application : AvaloniaObject, IDataContextProvider, IGlobalDataTemplates, IDataTemplateHost, IGlobalStyles, IStyleHost, IThemeVariantHost, IResourceHost, IResourceNode, IApplicationPlatformEvents
{
	private DataTemplates? _dataTemplates;

	private Styles? _styles;

	private IResourceDictionary? _resources;

	private bool _notifyingResourcesChanged;

	private Action<IReadOnlyList<IStyle>>? _stylesAdded;

	private Action<IReadOnlyList<IStyle>>? _stylesRemoved;

	public static readonly StyledProperty<object?> DataContextProperty = StyledElement.DataContextProperty.AddOwner<Application>();

	public static readonly StyledProperty<ThemeVariant> ActualThemeVariantProperty = ThemeVariantScope.ActualThemeVariantProperty.AddOwner<Application>();

	public static readonly StyledProperty<ThemeVariant?> RequestedThemeVariantProperty = ThemeVariantScope.RequestedThemeVariantProperty.AddOwner<Application>();

	private string? _name;

	public static readonly DirectProperty<Application, string?> NameProperty = AvaloniaProperty.RegisterDirect("Name", (Application o) => o.Name, delegate(Application o, string? v)
	{
		o.Name = v;
	});

	public object? DataContext
	{
		get
		{
			return GetValue(DataContextProperty);
		}
		set
		{
			SetValue(DataContextProperty, value);
		}
	}

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

	public ThemeVariant ActualThemeVariant => GetValue(ActualThemeVariantProperty);

	public static Application? Current => AvaloniaLocator.Current.GetService<Application>();

	public DataTemplates DataTemplates => _dataTemplates ?? (_dataTemplates = new DataTemplates());

	internal InputManager? InputManager { get; private set; }

	public IResourceDictionary Resources
	{
		get
		{
			return _resources ?? (_resources = new ResourceDictionary(this));
		}
		set
		{
			value = value ?? throw new ArgumentNullException("value");
			_resources?.RemoveOwner(this);
			_resources = value;
			_resources.AddOwner(this);
		}
	}

	public Styles Styles => _styles ?? (_styles = new Styles(this));

	bool IDataTemplateHost.IsDataTemplatesInitialized => _dataTemplates != null;

	bool IResourceNode.HasResources
	{
		get
		{
			IResourceDictionary? resources = _resources;
			if (resources == null || !resources.HasResources)
			{
				return ((IResourceNode)_styles)?.HasResources ?? false;
			}
			return true;
		}
	}

	IStyleHost? IStyleHost.StylingParent => null;

	bool IStyleHost.IsStylesInitialized => _styles != null;

	public IApplicationLifetime? ApplicationLifetime { get; set; }

	public IPlatformSettings? PlatformSettings => AvaloniaLocator.Current.GetService<IPlatformSettings>();

	public string? Name
	{
		get
		{
			return _name;
		}
		set
		{
			SetAndRaise(NameProperty, ref _name, value);
		}
	}

	public event EventHandler<ResourcesChangedEventArgs>? ResourcesChanged;

	public event EventHandler<UrlOpenedEventArgs>? UrlsOpened;

	public event EventHandler? ActualThemeVariantChanged;

	event Action<IReadOnlyList<IStyle>>? IGlobalStyles.GlobalStylesAdded
	{
		add
		{
			_stylesAdded = (Action<IReadOnlyList<IStyle>>)Delegate.Combine(_stylesAdded, value);
		}
		remove
		{
			_stylesAdded = (Action<IReadOnlyList<IStyle>>)Delegate.Remove(_stylesAdded, value);
		}
	}

	event Action<IReadOnlyList<IStyle>>? IGlobalStyles.GlobalStylesRemoved
	{
		add
		{
			_stylesRemoved = (Action<IReadOnlyList<IStyle>>)Delegate.Combine(_stylesRemoved, value);
		}
		remove
		{
			_stylesRemoved = (Action<IReadOnlyList<IStyle>>)Delegate.Remove(_stylesRemoved, value);
		}
	}

	public Application()
	{
		Name = "Avalonia Application";
	}

	public virtual void Initialize()
	{
	}

	public bool TryGetResource(object key, ThemeVariant? theme, out object? value)
	{
		value = null;
		IResourceDictionary? resources = _resources;
		if (resources == null || !resources.TryGetResource(key, theme, out value))
		{
			return Styles.TryGetResource(key, theme, out value);
		}
		return true;
	}

	void IResourceHost.NotifyHostedResourcesChanged(ResourcesChangedEventArgs e)
	{
		this.ResourcesChanged?.Invoke(this, e);
	}

	void IStyleHost.StylesAdded(IReadOnlyList<IStyle> styles)
	{
		_stylesAdded?.Invoke(styles);
	}

	void IStyleHost.StylesRemoved(IReadOnlyList<IStyle> styles)
	{
		_stylesRemoved?.Invoke(styles);
	}

	public virtual void RegisterServices()
	{
		AvaloniaSynchronizationContext.InstallIfNeeded();
		FocusManager constant = new FocusManager();
		InputManager = new InputManager();
		IPlatformSettings platformSettings = PlatformSettings;
		if (platformSettings != null)
		{
			platformSettings.ColorValuesChanged += OnColorValuesChanged;
			OnColorValuesChanged(platformSettings, platformSettings.GetColorValues());
		}
		AvaloniaLocator.CurrentMutable.Bind<IAccessKeyHandler>().ToTransient<AccessKeyHandler>().Bind<IGlobalDataTemplates>()
			.ToConstant(this)
			.Bind<IGlobalStyles>()
			.ToConstant(this)
			.Bind<IThemeVariantHost>()
			.ToConstant(this)
			.Bind<IFocusManager>()
			.ToConstant(constant)
			.Bind<IInputManager>()
			.ToConstant(InputManager)
			.Bind<IKeyboardNavigationHandler>()
			.ToTransient<KeyboardNavigationHandler>()
			.Bind<IDragDropDevice>()
			.ToConstant(DragDropDevice.Instance);
		if (AvaloniaLocator.Current.GetService<IPlatformDragSource>() == null)
		{
			AvaloniaLocator.CurrentMutable.Bind<IPlatformDragSource>().ToTransient<InProcessDragSource>();
		}
		AvaloniaLocator.CurrentMutable.Bind<IGlobalClock>().ToConstant(MediaContext.Instance.Clock);
	}

	public virtual void OnFrameworkInitializationCompleted()
	{
	}

	void IApplicationPlatformEvents.RaiseUrlsOpened(string[] urls)
	{
		this.UrlsOpened?.Invoke(this, new UrlOpenedEventArgs(urls));
	}

	private void NotifyResourcesChanged(ResourcesChangedEventArgs e)
	{
		if (_notifyingResourcesChanged)
		{
			return;
		}
		try
		{
			_notifyingResourcesChanged = true;
			this.ResourcesChanged?.Invoke(this, ResourcesChangedEventArgs.Empty);
		}
		finally
		{
			_notifyingResourcesChanged = false;
		}
	}

	private void ThisResourcesChanged(object sender, ResourcesChangedEventArgs e)
	{
		NotifyResourcesChanged(e);
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == RequestedThemeVariantProperty)
		{
			ThemeVariant newValue = change.GetNewValue<ThemeVariant>();
			if ((object)newValue != null && newValue != ThemeVariant.Default)
			{
				SetValue(ActualThemeVariantProperty, newValue);
			}
			else
			{
				ClearValue(ActualThemeVariantProperty);
			}
		}
		else if (change.Property == ActualThemeVariantProperty)
		{
			this.ActualThemeVariantChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	private void OnColorValuesChanged(object? sender, PlatformColorValues e)
	{
		SetValue(ActualThemeVariantProperty, (ThemeVariant)e.ThemeVariant, BindingPriority.Template);
	}
}
