using System;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.Reactive;

namespace Avalonia.Controls;

[TemplatePart("PART_PaneRoot", typeof(Panel))]
[PseudoClasses(new string[] { ":open", ":closed" })]
[PseudoClasses(new string[] { ":compactoverlay", ":compactinline", ":overlay", ":inline" })]
[PseudoClasses(new string[] { ":left", ":right" })]
[PseudoClasses(new string[] { ":lightDismiss" })]
public class SplitView : ContentControl
{
	private const string pcOpen = ":open";

	private const string pcClosed = ":closed";

	private const string pcCompactOverlay = ":compactoverlay";

	private const string pcCompactInline = ":compactinline";

	private const string pcOverlay = ":overlay";

	private const string pcInline = ":inline";

	private const string pcLeft = ":left";

	private const string pcRight = ":right";

	private const string pcLightDismiss = ":lightDismiss";

	public static readonly StyledProperty<double> CompactPaneLengthProperty = AvaloniaProperty.Register<SplitView, double>("CompactPaneLength", 48.0);

	public static readonly StyledProperty<SplitViewDisplayMode> DisplayModeProperty = AvaloniaProperty.Register<SplitView, SplitViewDisplayMode>("DisplayMode", SplitViewDisplayMode.Overlay);

	public static readonly StyledProperty<bool> IsPaneOpenProperty = AvaloniaProperty.Register<SplitView, bool>("IsPaneOpen", defaultValue: false, inherits: false, BindingMode.OneWay, null, CoerceIsPaneOpen);

	public static readonly StyledProperty<double> OpenPaneLengthProperty = AvaloniaProperty.Register<SplitView, double>("OpenPaneLength", 320.0);

	public static readonly StyledProperty<IBrush?> PaneBackgroundProperty = AvaloniaProperty.Register<SplitView, IBrush>("PaneBackground");

	public static readonly StyledProperty<SplitViewPanePlacement> PanePlacementProperty = AvaloniaProperty.Register<SplitView, SplitViewPanePlacement>("PanePlacement", SplitViewPanePlacement.Left);

	public static readonly StyledProperty<object?> PaneProperty = AvaloniaProperty.Register<SplitView, object>("Pane");

	public static readonly StyledProperty<IDataTemplate> PaneTemplateProperty = AvaloniaProperty.Register<SplitView, IDataTemplate>("PaneTemplate");

	public static readonly StyledProperty<bool> UseLightDismissOverlayModeProperty = AvaloniaProperty.Register<SplitView, bool>("UseLightDismissOverlayMode", defaultValue: false);

	public static readonly DirectProperty<SplitView, SplitViewTemplateSettings> TemplateSettingsProperty = AvaloniaProperty.RegisterDirect("TemplateSettings", (SplitView x) => x.TemplateSettings);

	public static readonly RoutedEvent<RoutedEventArgs> PaneClosedEvent = RoutedEvent.Register<SplitView, RoutedEventArgs>("PaneClosed", RoutingStrategies.Bubble);

	public static readonly RoutedEvent<CancelRoutedEventArgs> PaneClosingEvent = RoutedEvent.Register<SplitView, CancelRoutedEventArgs>("PaneClosing", RoutingStrategies.Bubble);

	public static readonly RoutedEvent<RoutedEventArgs> PaneOpenedEvent = RoutedEvent.Register<SplitView, RoutedEventArgs>("PaneOpened", RoutingStrategies.Bubble);

	public static readonly RoutedEvent<CancelRoutedEventArgs> PaneOpeningEvent = RoutedEvent.Register<SplitView, CancelRoutedEventArgs>("PaneOpening", RoutingStrategies.Bubble);

	private Panel? _pane;

	private IDisposable? _pointerDisposable;

	private SplitViewTemplateSettings _templateSettings = new SplitViewTemplateSettings();

	private string? _lastDisplayModePseudoclass;

	private string? _lastPlacementPseudoclass;

	public double CompactPaneLength
	{
		get
		{
			return GetValue(CompactPaneLengthProperty);
		}
		set
		{
			SetValue(CompactPaneLengthProperty, value);
		}
	}

	public SplitViewDisplayMode DisplayMode
	{
		get
		{
			return GetValue(DisplayModeProperty);
		}
		set
		{
			SetValue(DisplayModeProperty, value);
		}
	}

	public bool IsPaneOpen
	{
		get
		{
			return GetValue(IsPaneOpenProperty);
		}
		set
		{
			SetValue(IsPaneOpenProperty, value);
		}
	}

	public double OpenPaneLength
	{
		get
		{
			return GetValue(OpenPaneLengthProperty);
		}
		set
		{
			SetValue(OpenPaneLengthProperty, value);
		}
	}

	public IBrush? PaneBackground
	{
		get
		{
			return GetValue(PaneBackgroundProperty);
		}
		set
		{
			SetValue(PaneBackgroundProperty, value);
		}
	}

	public SplitViewPanePlacement PanePlacement
	{
		get
		{
			return GetValue(PanePlacementProperty);
		}
		set
		{
			SetValue(PanePlacementProperty, value);
		}
	}

	[DependsOn("PaneTemplate")]
	public object? Pane
	{
		get
		{
			return GetValue(PaneProperty);
		}
		set
		{
			SetValue(PaneProperty, value);
		}
	}

	public IDataTemplate PaneTemplate
	{
		get
		{
			return GetValue(PaneTemplateProperty);
		}
		set
		{
			SetValue(PaneTemplateProperty, value);
		}
	}

	public bool UseLightDismissOverlayMode
	{
		get
		{
			return GetValue(UseLightDismissOverlayModeProperty);
		}
		set
		{
			SetValue(UseLightDismissOverlayModeProperty, value);
		}
	}

	public SplitViewTemplateSettings TemplateSettings
	{
		get
		{
			return _templateSettings;
		}
		private set
		{
			SetAndRaise(TemplateSettingsProperty, ref _templateSettings, value);
		}
	}

	public event EventHandler<RoutedEventArgs>? PaneClosed
	{
		add
		{
			AddHandler(PaneClosedEvent, value);
		}
		remove
		{
			RemoveHandler(PaneClosedEvent, value);
		}
	}

	public event EventHandler<CancelRoutedEventArgs>? PaneClosing
	{
		add
		{
			AddHandler(PaneClosingEvent, value);
		}
		remove
		{
			RemoveHandler(PaneClosingEvent, value);
		}
	}

	public event EventHandler<RoutedEventArgs>? PaneOpened
	{
		add
		{
			AddHandler(PaneOpenedEvent, value);
		}
		remove
		{
			RemoveHandler(PaneOpenedEvent, value);
		}
	}

	public event EventHandler<CancelRoutedEventArgs>? PaneOpening
	{
		add
		{
			AddHandler(PaneOpeningEvent, value);
		}
		remove
		{
			RemoveHandler(PaneOpeningEvent, value);
		}
	}

	protected override bool RegisterContentPresenter(ContentPresenter presenter)
	{
		bool result = base.RegisterContentPresenter(presenter);
		if (presenter.Name == "PART_PanePresenter")
		{
			return true;
		}
		return result;
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);
		_pane = e.NameScope.Find<Panel>("PART_PaneRoot");
		UpdateVisualStateForDisplayMode(DisplayMode);
	}

	protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnAttachedToVisualTree(e);
		UpdateVisualStateForPanePlacementProperty(PanePlacement);
	}

	protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnDetachedFromVisualTree(e);
		_pointerDisposable?.Dispose();
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == CompactPaneLengthProperty)
		{
			UpdateVisualStateForCompactPaneLength(change.GetNewValue<double>());
		}
		else if (change.Property == DisplayModeProperty)
		{
			UpdateVisualStateForDisplayMode(change.GetNewValue<SplitViewDisplayMode>());
		}
		else if (change.Property == IsPaneOpenProperty)
		{
			if (change.GetNewValue<bool>())
			{
				base.PseudoClasses.Add(":open");
				base.PseudoClasses.Remove(":closed");
				OnPaneOpened(new RoutedEventArgs(PaneOpenedEvent, this));
			}
			else
			{
				base.PseudoClasses.Add(":closed");
				base.PseudoClasses.Remove(":open");
				OnPaneClosed(new RoutedEventArgs(PaneClosedEvent, this));
			}
		}
		else if (change.Property == PaneProperty)
		{
			if (change.OldValue is ILogical item)
			{
				base.LogicalChildren.Remove(item);
			}
			if (change.NewValue is ILogical item2)
			{
				base.LogicalChildren.Add(item2);
			}
		}
		else if (change.Property == PanePlacementProperty)
		{
			UpdateVisualStateForPanePlacementProperty(change.GetNewValue<SplitViewPanePlacement>());
		}
		else if (change.Property == UseLightDismissOverlayModeProperty)
		{
			bool newValue = change.GetNewValue<bool>();
			base.PseudoClasses.Set(":lightDismiss", newValue);
		}
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		if (!e.Handled && e.Key == Key.Escape && IsPaneOpen && IsInOverlayMode())
		{
			SetCurrentValue(IsPaneOpenProperty, value: false);
			e.Handled = true;
		}
		base.OnKeyDown(e);
	}

	private void PointerReleasedOutside(object? sender, PointerReleasedEventArgs e)
	{
		if (!IsPaneOpen || _pane == null)
		{
			return;
		}
		bool flag = true;
		for (Visual visual = e.Source as Visual; visual != null; visual = visual.VisualParent)
		{
			if (visual == _pane || visual is PopupRoot)
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			SetCurrentValue(IsPaneOpenProperty, value: false);
			e.Handled = true;
		}
	}

	private bool IsInOverlayMode()
	{
		if (DisplayMode != SplitViewDisplayMode.CompactOverlay)
		{
			return DisplayMode == SplitViewDisplayMode.Overlay;
		}
		return true;
	}

	protected virtual void OnPaneOpening(CancelRoutedEventArgs args)
	{
		RaiseEvent(args);
	}

	protected virtual void OnPaneOpened(RoutedEventArgs args)
	{
		EnableLightDismiss();
		RaiseEvent(args);
	}

	protected virtual void OnPaneClosing(CancelRoutedEventArgs args)
	{
		RaiseEvent(args);
	}

	protected virtual void OnPaneClosed(RoutedEventArgs args)
	{
		_pointerDisposable?.Dispose();
		_pointerDisposable = null;
		RaiseEvent(args);
	}

	private static string GetPseudoClass(SplitViewDisplayMode mode)
	{
		return mode switch
		{
			SplitViewDisplayMode.Inline => ":inline", 
			SplitViewDisplayMode.CompactInline => ":compactinline", 
			SplitViewDisplayMode.Overlay => ":overlay", 
			SplitViewDisplayMode.CompactOverlay => ":compactoverlay", 
			_ => throw new ArgumentOutOfRangeException("mode", mode, null), 
		};
	}

	private static string GetPseudoClass(SplitViewPanePlacement placement)
	{
		return placement switch
		{
			SplitViewPanePlacement.Left => ":left", 
			SplitViewPanePlacement.Right => ":right", 
			_ => throw new ArgumentOutOfRangeException("placement", placement, null), 
		};
	}

	protected virtual bool OnCoerceIsPaneOpen(bool value)
	{
		CancelRoutedEventArgs cancelRoutedEventArgs;
		if (value)
		{
			cancelRoutedEventArgs = new CancelRoutedEventArgs(PaneOpeningEvent, this);
			OnPaneOpening(cancelRoutedEventArgs);
		}
		else
		{
			cancelRoutedEventArgs = new CancelRoutedEventArgs(PaneClosingEvent, this);
			OnPaneClosing(cancelRoutedEventArgs);
		}
		if (cancelRoutedEventArgs.Cancel)
		{
			return !value;
		}
		return value;
	}

	private void UpdateVisualStateForCompactPaneLength(double newLen)
	{
		switch (DisplayMode)
		{
		case SplitViewDisplayMode.CompactInline:
			TemplateSettings.ClosedPaneWidth = newLen;
			break;
		case SplitViewDisplayMode.CompactOverlay:
			TemplateSettings.ClosedPaneWidth = newLen;
			TemplateSettings.PaneColumnGridLength = new GridLength(newLen, GridUnitType.Pixel);
			break;
		}
	}

	private void UpdateVisualStateForDisplayMode(SplitViewDisplayMode newValue)
	{
		if (!string.IsNullOrEmpty(_lastDisplayModePseudoclass))
		{
			base.PseudoClasses.Remove(_lastDisplayModePseudoclass);
		}
		_lastDisplayModePseudoclass = GetPseudoClass(newValue);
		base.PseudoClasses.Add(_lastDisplayModePseudoclass);
		var (closedPaneWidth, paneColumnGridLength) = newValue switch
		{
			SplitViewDisplayMode.Overlay => (0.0, new GridLength(0.0, GridUnitType.Pixel)), 
			SplitViewDisplayMode.CompactOverlay => (CompactPaneLength, new GridLength(CompactPaneLength, GridUnitType.Pixel)), 
			SplitViewDisplayMode.Inline => (0.0, new GridLength(0.0, GridUnitType.Auto)), 
			SplitViewDisplayMode.CompactInline => (CompactPaneLength, new GridLength(0.0, GridUnitType.Auto)), 
			_ => throw new NotImplementedException(), 
		};
		TemplateSettings.ClosedPaneWidth = closedPaneWidth;
		TemplateSettings.PaneColumnGridLength = paneColumnGridLength;
	}

	private void UpdateVisualStateForPanePlacementProperty(SplitViewPanePlacement newValue)
	{
		if (!string.IsNullOrEmpty(_lastPlacementPseudoclass))
		{
			base.PseudoClasses.Remove(_lastPlacementPseudoclass);
		}
		_lastPlacementPseudoclass = GetPseudoClass(newValue);
		base.PseudoClasses.Add(_lastPlacementPseudoclass);
	}

	private void EnableLightDismiss()
	{
		if (_pane == null || !IsInOverlayMode())
		{
			return;
		}
		TopLevel topLevel = TopLevel.GetTopLevel(this);
		if (topLevel != null)
		{
			_pointerDisposable = Disposable.Create(delegate
			{
				topLevel.PointerReleased -= PointerReleasedOutside;
				topLevel.BackRequested -= TopLevelBackRequested;
			});
			topLevel.PointerReleased += PointerReleasedOutside;
			topLevel.BackRequested += TopLevelBackRequested;
		}
	}

	private void TopLevelBackRequested(object? sender, RoutedEventArgs e)
	{
		if (IsInOverlayMode())
		{
			SetCurrentValue(IsPaneOpenProperty, value: false);
			e.Handled = true;
		}
	}

	private static bool CoerceIsPaneOpen(AvaloniaObject instance, bool value)
	{
		if (instance is SplitView splitView)
		{
			return splitView.OnCoerceIsPaneOpen(value);
		}
		return value;
	}
}
