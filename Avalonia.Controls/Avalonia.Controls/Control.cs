using System;
using System.Collections.Generic;
using System.ComponentModel;
using Avalonia.Automation.Peers;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Rendering;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace Avalonia.Controls;

public class Control : InputElement, IDataTemplateHost, INamed, IVisualBrushInitialize, ISetterValue
{
	public static readonly StyledProperty<ITemplate<Control>?> FocusAdornerProperty = AvaloniaProperty.Register<Control, ITemplate<Control>>("FocusAdorner");

	public static readonly StyledProperty<object?> TagProperty = AvaloniaProperty.Register<Control, object>("Tag");

	public static readonly StyledProperty<ContextMenu?> ContextMenuProperty = AvaloniaProperty.Register<Control, ContextMenu>("ContextMenu");

	public static readonly StyledProperty<FlyoutBase?> ContextFlyoutProperty = AvaloniaProperty.Register<Control, FlyoutBase>("ContextFlyout");

	public static readonly RoutedEvent<RequestBringIntoViewEventArgs> RequestBringIntoViewEvent = RoutedEvent.Register<Control, RequestBringIntoViewEventArgs>("RequestBringIntoView", RoutingStrategies.Bubble);

	public static readonly RoutedEvent<ContextRequestedEventArgs> ContextRequestedEvent = RoutedEvent.Register<Control, ContextRequestedEventArgs>("ContextRequested", RoutingStrategies.Tunnel | RoutingStrategies.Bubble);

	public static readonly RoutedEvent<RoutedEventArgs> LoadedEvent = RoutedEvent.Register<Control, RoutedEventArgs>("Loaded", RoutingStrategies.Direct);

	public static readonly RoutedEvent<RoutedEventArgs> UnloadedEvent = RoutedEvent.Register<Control, RoutedEventArgs>("Unloaded", RoutingStrategies.Direct);

	public static readonly RoutedEvent<SizeChangedEventArgs> SizeChangedEvent = RoutedEvent.Register<Control, SizeChangedEventArgs>("SizeChanged", RoutingStrategies.Direct);

	private static bool _isLoadedProcessing = false;

	private static readonly HashSet<Control> _loadedQueue = new HashSet<Control>();

	private static readonly HashSet<Control> _loadedProcessingQueue = new HashSet<Control>();

	private bool _isLoaded;

	private DataTemplates? _dataTemplates;

	private Control? _focusAdorner;

	private AutomationPeer? _automationPeer;

	private static Action loadedProcessingAction = delegate
	{
		_loadedProcessingQueue.Clear();
		foreach (Control item in _loadedQueue)
		{
			_loadedProcessingQueue.Add(item);
		}
		_loadedQueue.Clear();
		foreach (Control item2 in _loadedProcessingQueue)
		{
			item2.OnLoadedCore();
		}
		_loadedProcessingQueue.Clear();
		_isLoadedProcessing = false;
		if (_loadedQueue.Count > 0)
		{
			_isLoadedProcessing = true;
			Dispatcher.UIThread.Post(loadedProcessingAction, DispatcherPriority.Loaded);
		}
	};

	public ITemplate<Control>? FocusAdorner
	{
		get
		{
			return GetValue(FocusAdornerProperty);
		}
		set
		{
			SetValue(FocusAdornerProperty, value);
		}
	}

	public DataTemplates DataTemplates => _dataTemplates ?? (_dataTemplates = new DataTemplates());

	public ContextMenu? ContextMenu
	{
		get
		{
			return GetValue(ContextMenuProperty);
		}
		set
		{
			SetValue(ContextMenuProperty, value);
		}
	}

	public FlyoutBase? ContextFlyout
	{
		get
		{
			return GetValue(ContextFlyoutProperty);
		}
		set
		{
			SetValue(ContextFlyoutProperty, value);
		}
	}

	public bool IsLoaded => _isLoaded;

	public object? Tag
	{
		get
		{
			return GetValue(TagProperty);
		}
		set
		{
			SetValue(TagProperty, value);
		}
	}

	bool IDataTemplateHost.IsDataTemplatesInitialized => _dataTemplates != null;

	public event EventHandler<ContextRequestedEventArgs>? ContextRequested
	{
		add
		{
			AddHandler(ContextRequestedEvent, value);
		}
		remove
		{
			RemoveHandler(ContextRequestedEvent, value);
		}
	}

	public event EventHandler<RoutedEventArgs>? Loaded
	{
		add
		{
			AddHandler(LoadedEvent, value);
		}
		remove
		{
			RemoveHandler(LoadedEvent, value);
		}
	}

	public event EventHandler<RoutedEventArgs>? Unloaded
	{
		add
		{
			AddHandler(UnloadedEvent, value);
		}
		remove
		{
			RemoveHandler(UnloadedEvent, value);
		}
	}

	public event EventHandler<SizeChangedEventArgs>? SizeChanged
	{
		add
		{
			AddHandler(SizeChangedEvent, value);
		}
		remove
		{
			RemoveHandler(SizeChangedEvent, value);
		}
	}

	void ISetterValue.Initialize(SetterBase setter)
	{
		if (setter is Setter setter2 && setter2.Property == ContextFlyoutProperty)
		{
			return;
		}
		throw new InvalidOperationException("Cannot use a control as a Setter value. Wrap the control in a <Template>.");
	}

	void IVisualBrushInitialize.EnsureInitialized()
	{
		if (base.VisualRoot != null)
		{
			return;
		}
		if (!base.IsInitialized)
		{
			foreach (Visual selfAndVisualDescendant in this.GetSelfAndVisualDescendants())
			{
				if (selfAndVisualDescendant is Control { IsInitialized: false } control)
				{
					ISupportInitialize supportInitialize = control;
					if (supportInitialize != null)
					{
						supportInitialize.BeginInit();
						supportInitialize.EndInit();
					}
				}
			}
		}
		if (!base.IsArrangeValid)
		{
			Measure(Size.Infinity);
			Arrange(new Rect(base.DesiredSize));
		}
	}

	protected virtual Control? GetTemplateFocusTarget()
	{
		return this;
	}

	internal void ScheduleOnLoadedCore()
	{
		if (!_isLoaded && _loadedQueue.Add(this) && !_isLoadedProcessing)
		{
			_isLoadedProcessing = true;
			Dispatcher.UIThread.Post(loadedProcessingAction, DispatcherPriority.Loaded);
		}
	}

	internal void OnLoadedCore()
	{
		if (!_isLoaded && ((ILogical)this).IsAttachedToLogicalTree)
		{
			_isLoaded = true;
			OnLoaded(new RoutedEventArgs(LoadedEvent, this));
		}
	}

	internal void OnUnloadedCore()
	{
		if (_isLoaded)
		{
			_loadedQueue.Remove(this);
			_isLoaded = false;
			OnUnloaded(new RoutedEventArgs(UnloadedEvent, this));
		}
	}

	protected virtual void OnLoaded(RoutedEventArgs e)
	{
		RaiseEvent(e);
	}

	protected virtual void OnUnloaded(RoutedEventArgs e)
	{
		RaiseEvent(e);
	}

	protected virtual void OnSizeChanged(SizeChangedEventArgs e)
	{
		RaiseEvent(e);
	}

	protected sealed override void OnAttachedToVisualTreeCore(VisualTreeAttachmentEventArgs e)
	{
		base.OnAttachedToVisualTreeCore(e);
		AddHandler(Gestures.HoldingEvent, OnHoldEvent);
		InitializeIfNeeded();
		ScheduleOnLoadedCore();
	}

	private void OnHoldEvent(object? sender, HoldingRoutedEventArgs e)
	{
		if (e.HoldingState == HoldingState.Started)
		{
			RaiseEvent(new ContextRequestedEventArgs());
		}
	}

	protected sealed override void OnDetachedFromVisualTreeCore(VisualTreeAttachmentEventArgs e)
	{
		base.OnDetachedFromVisualTreeCore(e);
		RemoveHandler(Gestures.HoldingEvent, OnHoldEvent);
		OnUnloadedCore();
	}

	protected override void OnGotFocus(GotFocusEventArgs e)
	{
		base.OnGotFocus(e);
		if (!base.IsFocused || (e.NavigationMethod != NavigationMethod.Tab && e.NavigationMethod != NavigationMethod.Directional))
		{
			return;
		}
		AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
		if (adornerLayer == null)
		{
			return;
		}
		if (_focusAdorner == null)
		{
			ITemplate<Control> template;
			if (!IsSet(FocusAdornerProperty))
			{
				template = adornerLayer.DefaultFocusAdorner;
			}
			else
			{
				ITemplate<Control> value = GetValue(FocusAdornerProperty);
				template = value;
			}
			ITemplate<Control> template2 = template;
			if (template2 != null)
			{
				_focusAdorner = template2.Build();
			}
		}
		if (_focusAdorner != null)
		{
			Visual templateFocusTarget = GetTemplateFocusTarget();
			if (templateFocusTarget != null)
			{
				AdornerLayer.SetAdornedElement(_focusAdorner, templateFocusTarget);
				adornerLayer.Children.Add(_focusAdorner);
			}
		}
	}

	protected override void OnLostFocus(RoutedEventArgs e)
	{
		base.OnLostFocus(e);
		if (_focusAdorner?.Parent != null)
		{
			((Panel)_focusAdorner.Parent).Children.Remove(_focusAdorner);
			_focusAdorner = null;
		}
	}

	protected virtual AutomationPeer OnCreateAutomationPeer()
	{
		return new NoneAutomationPeer(this);
	}

	internal AutomationPeer? GetAutomationPeer()
	{
		VerifyAccess();
		return _automationPeer;
	}

	internal AutomationPeer GetOrCreateAutomationPeer()
	{
		VerifyAccess();
		if (_automationPeer != null)
		{
			return _automationPeer;
		}
		_automationPeer = OnCreateAutomationPeer();
		return _automationPeer;
	}

	protected override void OnPointerReleased(PointerReleasedEventArgs e)
	{
		base.OnPointerReleased(e);
		if (e.Source == this && !e.Handled && e.InitialPressMouseButton == MouseButton.Right)
		{
			ContextRequestedEventArgs contextRequestedEventArgs = new ContextRequestedEventArgs(e);
			RaiseEvent(contextRequestedEventArgs);
			e.Handled = contextRequestedEventArgs.Handled;
		}
	}

	protected override void OnKeyUp(KeyEventArgs e)
	{
		base.OnKeyUp(e);
		if (e.Source != this || e.Handled)
		{
			return;
		}
		List<KeyGesture> list = TopLevel.GetTopLevel(this)?.PlatformSettings?.HotkeyConfiguration.OpenContextMenu;
		if (list == null)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < list.Count; i++)
		{
			KeyGesture keyGesture = list[i];
			flag |= keyGesture.Matches(e);
			if (flag)
			{
				break;
			}
		}
		if (flag)
		{
			ContextRequestedEventArgs contextRequestedEventArgs = new ContextRequestedEventArgs();
			RaiseEvent(contextRequestedEventArgs);
			e.Handled = contextRequestedEventArgs.Handled;
		}
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == Visual.BoundsProperty)
		{
			Rect oldValue = change.GetOldValue<Rect>();
			Rect newValue = change.GetNewValue<Rect>();
			if (newValue.Size != oldValue.Size)
			{
				SizeChangedEventArgs e = new SizeChangedEventArgs(SizeChangedEvent, this, new Size(oldValue.Width, oldValue.Height), new Size(newValue.Width, newValue.Height));
				OnSizeChanged(e);
			}
		}
	}

	internal static void ResetLoadedQueueForUnitTests()
	{
		_loadedQueue.Clear();
		_loadedProcessingQueue.Clear();
		_isLoadedProcessing = false;
	}
}
