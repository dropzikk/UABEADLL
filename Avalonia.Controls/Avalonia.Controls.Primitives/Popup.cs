using System;
using System.ComponentModel;
using Avalonia.Automation.Peers;
using Avalonia.Controls.Diagnostics;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Input.TextInput;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.Platform;
using Avalonia.Reactive;
using Avalonia.VisualTree;

namespace Avalonia.Controls.Primitives;

public class Popup : Control, IPopupHostProvider
{
	private readonly struct IgnoreIsOpenScope : IDisposable
	{
		private readonly Popup _owner;

		public IgnoreIsOpenScope(Popup owner)
		{
			_owner = owner;
			_owner._ignoreIsOpenChanged = true;
		}

		public void Dispose()
		{
			_owner._ignoreIsOpenChanged = false;
		}
	}

	private class PopupOpenState : IDisposable
	{
		private readonly IDisposable _cleanup;

		private IDisposable? _presenterCleanup;

		public TopLevel TopLevel { get; }

		public Control PlacementTarget { get; set; }

		public IPopupHost PopupHost { get; }

		public PopupOpenState(Control placementTarget, TopLevel topLevel, IPopupHost popupHost, IDisposable cleanup)
		{
			PlacementTarget = placementTarget;
			TopLevel = topLevel;
			PopupHost = popupHost;
			_cleanup = cleanup;
		}

		public void SetPresenterSubscription(IDisposable? presenterCleanup)
		{
			_presenterCleanup?.Dispose();
			_presenterCleanup = presenterCleanup;
		}

		public void Dispose()
		{
			_presenterCleanup?.Dispose();
			_cleanup.Dispose();
		}
	}

	public static readonly StyledProperty<bool> WindowManagerAddShadowHintProperty;

	public static readonly StyledProperty<Control?> ChildProperty;

	public static readonly StyledProperty<bool> InheritsTransformProperty;

	public static readonly StyledProperty<bool> IsOpenProperty;

	public static readonly StyledProperty<PopupAnchor> PlacementAnchorProperty;

	public static readonly StyledProperty<PopupPositionerConstraintAdjustment> PlacementConstraintAdjustmentProperty;

	public static readonly StyledProperty<PopupGravity> PlacementGravityProperty;

	public static readonly StyledProperty<PlacementMode> PlacementProperty;

	[Obsolete("Use the Placement property instead.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static readonly StyledProperty<PlacementMode> PlacementModeProperty;

	public static readonly StyledProperty<Rect?> PlacementRectProperty;

	public static readonly StyledProperty<Control?> PlacementTargetProperty;

	public static readonly StyledProperty<bool> OverlayDismissEventPassThroughProperty;

	public static readonly StyledProperty<IInputElement?> OverlayInputPassThroughElementProperty;

	public static readonly StyledProperty<double> HorizontalOffsetProperty;

	public static readonly StyledProperty<bool> IsLightDismissEnabledProperty;

	public static readonly StyledProperty<double> VerticalOffsetProperty;

	public static readonly StyledProperty<bool> TopmostProperty;

	private bool _isOpenRequested;

	private bool _ignoreIsOpenChanged;

	private PopupOpenState? _openState;

	private Action<IPopupHost?>? _popupHostChangedHandler;

	public IPopupHost? Host => _openState?.PopupHost;

	public bool WindowManagerAddShadowHint
	{
		get
		{
			return GetValue(WindowManagerAddShadowHintProperty);
		}
		set
		{
			SetValue(WindowManagerAddShadowHintProperty, value);
		}
	}

	[Content]
	public Control? Child
	{
		get
		{
			return GetValue(ChildProperty);
		}
		set
		{
			SetValue(ChildProperty, value);
		}
	}

	public IAvaloniaDependencyResolver? DependencyResolver { get; set; }

	public bool InheritsTransform
	{
		get
		{
			return GetValue(InheritsTransformProperty);
		}
		set
		{
			SetValue(InheritsTransformProperty, value);
		}
	}

	public bool IsLightDismissEnabled
	{
		get
		{
			return GetValue(IsLightDismissEnabledProperty);
		}
		set
		{
			SetValue(IsLightDismissEnabledProperty, value);
		}
	}

	public bool IsOpen
	{
		get
		{
			return GetValue(IsOpenProperty);
		}
		set
		{
			SetValue(IsOpenProperty, value);
		}
	}

	public PopupAnchor PlacementAnchor
	{
		get
		{
			return GetValue(PlacementAnchorProperty);
		}
		set
		{
			SetValue(PlacementAnchorProperty, value);
		}
	}

	public PopupPositionerConstraintAdjustment PlacementConstraintAdjustment
	{
		get
		{
			return GetValue(PlacementConstraintAdjustmentProperty);
		}
		set
		{
			SetValue(PlacementConstraintAdjustmentProperty, value);
		}
	}

	public PopupGravity PlacementGravity
	{
		get
		{
			return GetValue(PlacementGravityProperty);
		}
		set
		{
			SetValue(PlacementGravityProperty, value);
		}
	}

	[Obsolete("Use the Placement property instead.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public PlacementMode PlacementMode
	{
		get
		{
			return GetValue(PlacementProperty);
		}
		set
		{
			SetValue(PlacementProperty, value);
		}
	}

	public PlacementMode Placement
	{
		get
		{
			return GetValue(PlacementProperty);
		}
		set
		{
			SetValue(PlacementProperty, value);
		}
	}

	public Rect? PlacementRect
	{
		get
		{
			return GetValue(PlacementRectProperty);
		}
		set
		{
			SetValue(PlacementRectProperty, value);
		}
	}

	[ResolveByName]
	public Control? PlacementTarget
	{
		get
		{
			return GetValue(PlacementTargetProperty);
		}
		set
		{
			SetValue(PlacementTargetProperty, value);
		}
	}

	public bool OverlayDismissEventPassThrough
	{
		get
		{
			return GetValue(OverlayDismissEventPassThroughProperty);
		}
		set
		{
			SetValue(OverlayDismissEventPassThroughProperty, value);
		}
	}

	public IInputElement? OverlayInputPassThroughElement
	{
		get
		{
			return GetValue(OverlayInputPassThroughElementProperty);
		}
		set
		{
			SetValue(OverlayInputPassThroughElementProperty, value);
		}
	}

	public double HorizontalOffset
	{
		get
		{
			return GetValue(HorizontalOffsetProperty);
		}
		set
		{
			SetValue(HorizontalOffsetProperty, value);
		}
	}

	public double VerticalOffset
	{
		get
		{
			return GetValue(VerticalOffsetProperty);
		}
		set
		{
			SetValue(VerticalOffsetProperty, value);
		}
	}

	public bool Topmost
	{
		get
		{
			return GetValue(TopmostProperty);
		}
		set
		{
			SetValue(TopmostProperty, value);
		}
	}

	IPopupHost? IPopupHostProvider.PopupHost => Host;

	public bool IsPointerOverPopup => ((IInputElement)(_openState?.PopupHost))?.IsPointerOver ?? false;

	public event EventHandler<EventArgs>? Closed;

	public event EventHandler? Opened;

	internal event EventHandler<CancelEventArgs>? Closing;

	event Action<IPopupHost?>? IPopupHostProvider.PopupHostChanged
	{
		add
		{
			_popupHostChangedHandler = (Action<IPopupHost>)Delegate.Combine(_popupHostChangedHandler, value);
		}
		remove
		{
			_popupHostChangedHandler = (Action<IPopupHost>)Delegate.Remove(_popupHostChangedHandler, value);
		}
	}

	static Popup()
	{
		WindowManagerAddShadowHintProperty = AvaloniaProperty.Register<Popup, bool>("WindowManagerAddShadowHint", defaultValue: false);
		ChildProperty = AvaloniaProperty.Register<Popup, Control>("Child");
		InheritsTransformProperty = AvaloniaProperty.Register<Popup, bool>("InheritsTransform", defaultValue: false);
		IsOpenProperty = AvaloniaProperty.Register<Popup, bool>("IsOpen", defaultValue: false);
		PlacementAnchorProperty = AvaloniaProperty.Register<Popup, PopupAnchor>("PlacementAnchor", PopupAnchor.None);
		PlacementConstraintAdjustmentProperty = AvaloniaProperty.Register<Popup, PopupPositionerConstraintAdjustment>("PlacementConstraintAdjustment", PopupPositionerConstraintAdjustment.All);
		PlacementGravityProperty = AvaloniaProperty.Register<Popup, PopupGravity>("PlacementGravity", PopupGravity.None);
		PlacementProperty = AvaloniaProperty.Register<Popup, PlacementMode>("Placement", PlacementMode.Bottom);
		PlacementModeProperty = PlacementProperty;
		PlacementRectProperty = AvaloniaProperty.Register<Popup, Rect?>("PlacementRect", null);
		PlacementTargetProperty = AvaloniaProperty.Register<Popup, Control>("PlacementTarget");
		OverlayDismissEventPassThroughProperty = AvaloniaProperty.Register<Popup, bool>("OverlayDismissEventPassThrough", defaultValue: false);
		OverlayInputPassThroughElementProperty = AvaloniaProperty.Register<Popup, IInputElement>("OverlayInputPassThroughElement");
		HorizontalOffsetProperty = AvaloniaProperty.Register<Popup, double>("HorizontalOffset", 0.0);
		IsLightDismissEnabledProperty = AvaloniaProperty.Register<Popup, bool>("IsLightDismissEnabled", defaultValue: false);
		VerticalOffsetProperty = AvaloniaProperty.Register<Popup, double>("VerticalOffset", 0.0);
		TopmostProperty = AvaloniaProperty.Register<Popup, bool>("Topmost", defaultValue: false);
		InputElement.IsHitTestVisibleProperty.OverrideDefaultValue<Popup>(defaultValue: false);
		ChildProperty.Changed.AddClassHandler(delegate(Popup x, AvaloniaPropertyChangedEventArgs e)
		{
			x.ChildChanged(e);
		});
		IsOpenProperty.Changed.AddClassHandler(delegate(Popup x, AvaloniaPropertyChangedEventArgs e)
		{
			x.IsOpenChanged((AvaloniaPropertyChangedEventArgs<bool>)e);
		});
		VerticalOffsetProperty.Changed.AddClassHandler(delegate(Popup x, AvaloniaPropertyChangedEventArgs _)
		{
			x.HandlePositionChange();
		});
		HorizontalOffsetProperty.Changed.AddClassHandler(delegate(Popup x, AvaloniaPropertyChangedEventArgs _)
		{
			x.HandlePositionChange();
		});
	}

	public void Open()
	{
		if (_openState != null)
		{
			return;
		}
		Control control = PlacementTarget ?? this.FindLogicalAncestorOfType<Control>();
		if (control == null)
		{
			_isOpenRequested = true;
			return;
		}
		TopLevel topLevel = TopLevel.GetTopLevel(control);
		if (topLevel == null)
		{
			_isOpenRequested = true;
			return;
		}
		_isOpenRequested = false;
		IPopupHost popupHost = OverlayPopupHost.CreatePopupHost(control, DependencyResolver);
		CompositeDisposable compositeDisposable = new CompositeDisposable(7);
		UpdateHostSizing(popupHost, topLevel, control);
		popupHost.Topmost = Topmost;
		popupHost.SetChild(Child);
		((ISetLogicalParent)popupHost).SetParent(this);
		if (InheritsTransform)
		{
			TransformTrackingHelper.Track(control, PlacementTargetTransformChanged).DisposeWith(compositeDisposable);
		}
		else
		{
			popupHost.Transform = null;
		}
		if (popupHost is PopupRoot popupRoot)
		{
			popupRoot.Bind(ThemeVariantScope.ActualThemeVariantProperty, this.GetBindingObservable(ThemeVariantScope.ActualThemeVariantProperty)).DisposeWith(compositeDisposable);
		}
		UpdateHostPosition(popupHost, control);
		SubscribeToEventHandler(popupHost, RootTemplateApplied, delegate(IPopupHost x, EventHandler<TemplateAppliedEventArgs> handler)
		{
			x.TemplateApplied += handler;
		}, delegate(IPopupHost x, EventHandler<TemplateAppliedEventArgs> handler)
		{
			x.TemplateApplied -= handler;
		}).DisposeWith(compositeDisposable);
		if (topLevel is Window { PlatformImpl: not null } window)
		{
			SubscribeToEventHandler(window, WindowDeactivated, delegate(Window x, EventHandler handler)
			{
				x.Deactivated += handler;
			}, delegate(Window x, EventHandler handler)
			{
				x.Deactivated -= handler;
			}).DisposeWith(compositeDisposable);
			SubscribeToEventHandler(window.PlatformImpl, WindowLostFocus, delegate(IWindowImpl x, Action handler)
			{
				x.LostFocus = (Action)Delegate.Combine(x.LostFocus, handler);
			}, delegate(IWindowImpl x, Action handler)
			{
				x.LostFocus = (Action)Delegate.Remove(x.LostFocus, handler);
			}).DisposeWith(compositeDisposable);
			if (Placement != 0)
			{
				SubscribeToEventHandler(window.PlatformImpl, WindowPositionChanged, delegate(IWindowImpl x, Action<PixelPoint> handler)
				{
					x.PositionChanged = (Action<PixelPoint>)Delegate.Combine(x.PositionChanged, handler);
				}, delegate(IWindowImpl x, Action<PixelPoint> handler)
				{
					x.PositionChanged = (Action<PixelPoint>)Delegate.Remove(x.PositionChanged, handler);
				}).DisposeWith(compositeDisposable);
				Layoutable layoutable = control;
				if (layoutable != null)
				{
					SubscribeToEventHandler(layoutable, PlacementTargetLayoutUpdated, delegate(Layoutable x, EventHandler handler)
					{
						x.LayoutUpdated += handler;
					}, delegate(Layoutable x, EventHandler handler)
					{
						x.LayoutUpdated -= handler;
					}).DisposeWith(compositeDisposable);
				}
			}
		}
		else if (topLevel is PopupRoot popupRoot2)
		{
			SubscribeToEventHandler(popupRoot2, ParentPopupPositionChanged, delegate(PopupRoot x, EventHandler<PixelPointEventArgs> handler)
			{
				x.PositionChanged += handler;
			}, delegate(PopupRoot x, EventHandler<PixelPointEventArgs> handler)
			{
				x.PositionChanged -= handler;
			}).DisposeWith(compositeDisposable);
			if (popupRoot2.Parent is Popup target)
			{
				SubscribeToEventHandler(target, ParentClosed, delegate(Popup x, EventHandler<EventArgs> handler)
				{
					x.Closed += handler;
				}, delegate(Popup x, EventHandler<EventArgs> handler)
				{
					x.Closed -= handler;
				}).DisposeWith(compositeDisposable);
			}
		}
		InputManager.Instance?.Process.Subscribe(ListenForNonClientClick).DisposeWith(compositeDisposable);
		IDisposable cleanup = Disposable.Create((popupHost, compositeDisposable), delegate((IPopupHost popupHost, CompositeDisposable handlerCleanup) state)
		{
			state.handlerCleanup.Dispose();
			state.popupHost.SetChild(null);
			state.popupHost.Hide();
			((ISetLogicalParent)state.popupHost).SetParent(null);
			state.popupHost.Dispose();
		});
		if (IsLightDismissEnabled)
		{
			LightDismissOverlayLayer dismissLayer = LightDismissOverlayLayer.GetLightDismissOverlayLayer(control);
			if (dismissLayer != null)
			{
				dismissLayer.IsVisible = true;
				dismissLayer.InputPassThroughElement = OverlayInputPassThroughElement;
				Disposable.Create(delegate
				{
					dismissLayer.IsVisible = false;
					dismissLayer.InputPassThroughElement = null;
				}).DisposeWith(compositeDisposable);
				SubscribeToEventHandler(dismissLayer, PointerPressedDismissOverlay, delegate(LightDismissOverlayLayer x, EventHandler<PointerPressedEventArgs> handler)
				{
					x.PointerPressed += handler;
				}, delegate(LightDismissOverlayLayer x, EventHandler<PointerPressedEventArgs> handler)
				{
					x.PointerPressed -= handler;
				}).DisposeWith(compositeDisposable);
			}
		}
		_openState = new PopupOpenState(control, topLevel, popupHost, cleanup);
		WindowManagerAddShadowHintChanged(popupHost, WindowManagerAddShadowHint);
		popupHost.Show();
		using (BeginIgnoringIsOpen())
		{
			SetCurrentValue(IsOpenProperty, value: true);
		}
		this.Opened?.Invoke(this, EventArgs.Empty);
		_popupHostChangedHandler?.Invoke(Host);
	}

	public void Close()
	{
		CloseCore();
	}

	protected override Size MeasureCore(Size availableSize)
	{
		return default(Size);
	}

	protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnAttachedToVisualTree(e);
		if (_isOpenRequested)
		{
			Open();
		}
	}

	protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
	{
		base.OnDetachedFromLogicalTree(e);
		Close();
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (_openState == null)
		{
			return;
		}
		if (change.Property == Layoutable.WidthProperty || change.Property == Layoutable.MinWidthProperty || change.Property == Layoutable.MaxWidthProperty || change.Property == Layoutable.HeightProperty || change.Property == Layoutable.MinHeightProperty || change.Property == Layoutable.MaxHeightProperty)
		{
			UpdateHostSizing(_openState.PopupHost, _openState.TopLevel, _openState.PlacementTarget);
		}
		else if (change.Property == PlacementTargetProperty || change.Property == PlacementProperty || change.Property == HorizontalOffsetProperty || change.Property == VerticalOffsetProperty || change.Property == PlacementAnchorProperty || change.Property == PlacementConstraintAdjustmentProperty || change.Property == PlacementRectProperty)
		{
			if (change.Property == PlacementTargetProperty)
			{
				Control control = change.GetNewValue<Control>() ?? this.FindLogicalAncestorOfType<Control>();
				if (control == null || control.GetVisualRoot() != _openState.TopLevel)
				{
					Close();
					return;
				}
				_openState.PlacementTarget = control;
			}
			UpdateHostPosition(_openState.PopupHost, _openState.PlacementTarget);
		}
		else if (change.Property == TopmostProperty)
		{
			_openState.PopupHost.Topmost = change.GetNewValue<bool>();
		}
	}

	private void UpdateHostPosition(IPopupHost popupHost, Control placementTarget)
	{
		popupHost.ConfigurePosition(placementTarget, Placement, new Point(HorizontalOffset, VerticalOffset), PlacementAnchor, PlacementGravity, PlacementConstraintAdjustment, PlacementRect ?? new Rect(default(Point), placementTarget.Bounds.Size));
	}

	private void UpdateHostSizing(IPopupHost popupHost, TopLevel topLevel, Control placementTarget)
	{
		double num = 1.0;
		double num2 = 1.0;
		if (InheritsTransform)
		{
			Matrix? matrix = placementTarget.TransformToVisual(topLevel);
			if (matrix.HasValue)
			{
				Matrix valueOrDefault = matrix.GetValueOrDefault();
				num = Math.Sqrt(valueOrDefault.M11 * valueOrDefault.M11 + valueOrDefault.M12 * valueOrDefault.M12);
				num2 = Math.Sqrt(valueOrDefault.M11 * valueOrDefault.M11 + valueOrDefault.M12 * valueOrDefault.M12);
				popupHost.Transform = new ScaleTransform(num, num2);
				goto IL_0095;
			}
		}
		popupHost.Transform = null;
		goto IL_0095;
		IL_0095:
		popupHost.Width = base.Width * num;
		popupHost.MinWidth = base.MinWidth * num;
		popupHost.MaxWidth = base.MaxWidth * num;
		popupHost.Height = base.Height * num2;
		popupHost.MinHeight = base.MinHeight * num2;
		popupHost.MaxHeight = base.MaxHeight * num2;
	}

	private void HandlePositionChange()
	{
		if (_openState != null)
		{
			Control control = PlacementTarget ?? this.FindLogicalAncestorOfType<Control>();
			if (control != null)
			{
				_openState.PopupHost.ConfigurePosition(control, Placement, new Point(HorizontalOffset, VerticalOffset), PlacementAnchor, PlacementGravity, PlacementConstraintAdjustment, PlacementRect);
			}
		}
	}

	protected override AutomationPeer OnCreateAutomationPeer()
	{
		return new PopupAutomationPeer(this);
	}

	private static IDisposable SubscribeToEventHandler<T, TEventHandler>(T target, TEventHandler handler, Action<T, TEventHandler> subscribe, Action<T, TEventHandler> unsubscribe)
	{
		subscribe(target, handler);
		return Disposable.Create((unsubscribe, target, handler), delegate((Action<T, TEventHandler> unsubscribe, T target, TEventHandler handler) state)
		{
			state.unsubscribe(state.target, state.handler);
		});
	}

	private static void WindowManagerAddShadowHintChanged(IPopupHost host, bool hint)
	{
		if (host is PopupRoot { PlatformImpl: not null } popupRoot)
		{
			popupRoot.PlatformImpl.SetWindowManagerAddShadowHint(hint);
		}
	}

	private void IsOpenChanged(AvaloniaPropertyChangedEventArgs<bool> e)
	{
		if (!_ignoreIsOpenChanged)
		{
			if (e.NewValue.Value)
			{
				Open();
			}
			else
			{
				Close();
			}
		}
	}

	private void ChildChanged(AvaloniaPropertyChangedEventArgs e)
	{
		base.LogicalChildren.Clear();
		((ISetLogicalParent)e.OldValue)?.SetParent(null);
		if (e.NewValue != null)
		{
			((ISetLogicalParent)e.NewValue).SetParent(this);
			base.LogicalChildren.Add((ILogical)e.NewValue);
		}
	}

	private void CloseCore()
	{
		CancelEventArgs cancelEventArgs = new CancelEventArgs();
		this.Closing?.Invoke(this, cancelEventArgs);
		if (cancelEventArgs.Cancel)
		{
			return;
		}
		_isOpenRequested = false;
		if (_openState == null)
		{
			using (BeginIgnoringIsOpen())
			{
				SetCurrentValue(IsOpenProperty, value: false);
				return;
			}
		}
		_openState.Dispose();
		_openState = null;
		_popupHostChangedHandler?.Invoke(null);
		using (BeginIgnoringIsOpen())
		{
			SetCurrentValue(IsOpenProperty, value: false);
		}
		this.Closed?.Invoke(this, EventArgs.Empty);
		if (FocusManager.GetFocusManager(this)?.GetFocusedElement() != null)
		{
			return;
		}
		if (PlacementTarget != null)
		{
			Control control = PlacementTarget;
			while (control != null && (!control.Focusable || !control.IsEffectivelyEnabled || !control.IsVisible))
			{
				control = control.VisualParent as Control;
			}
			control?.Focus();
		}
		else
		{
			this.FindLogicalAncestorOfType<Control>()?.Focus();
		}
	}

	private void ListenForNonClientClick(RawInputEventArgs e)
	{
		RawPointerEventArgs rawPointerEventArgs = e as RawPointerEventArgs;
		if (IsLightDismissEnabled && rawPointerEventArgs != null && rawPointerEventArgs.Type == RawPointerEventType.NonClientLeftButtonDown)
		{
			CloseCore();
		}
	}

	private void PointerPressedDismissOverlay(object? sender, PointerPressedEventArgs e)
	{
		if (IsLightDismissEnabled && e.Source is Visual child && !IsChildOrThis(child))
		{
			CloseCore();
			if (OverlayDismissEventPassThrough)
			{
				PassThroughEvent(e);
			}
		}
	}

	private static void PassThroughEvent(PointerPressedEventArgs e)
	{
		object source = e.Source;
		LightDismissOverlayLayer layer = source as LightDismissOverlayLayer;
		if (layer != null && layer.GetVisualRoot() is InputElement inputElement)
		{
			IInputElement inputElement2 = inputElement.InputHitTest(e.GetCurrentPoint(inputElement).Position, (Visual x) => x != layer);
			if (inputElement2 != null)
			{
				e.Pointer.Capture(inputElement2);
				inputElement2.RaiseEvent(e);
				e.Handled = true;
			}
		}
	}

	private void RootTemplateApplied(object? sender, TemplateAppliedEventArgs e)
	{
		if (_openState == null)
		{
			return;
		}
		IPopupHost popupHost = _openState.PopupHost;
		popupHost.TemplateApplied -= RootTemplateApplied;
		_openState.SetPresenterSubscription(null);
		if (base.TemplatedParent != null)
		{
			Control presenter = popupHost.Presenter;
			if (presenter != null)
			{
				presenter.ApplyTemplate();
				IDisposable presenterSubscription = presenter.GetObservable(ContentPresenter.ChildProperty).Subscribe(SetTemplatedParentAndApplyChildTemplates);
				_openState.SetPresenterSubscription(presenterSubscription);
			}
		}
	}

	private void SetTemplatedParentAndApplyChildTemplates(Control? control)
	{
		if (control != null)
		{
			TemplatedControl.ApplyTemplatedParent(control, base.TemplatedParent);
		}
	}

	private bool IsChildOrThis(Visual child)
	{
		if (_openState == null)
		{
			return false;
		}
		IPopupHost popupHost = _openState.PopupHost;
		for (Visual visual = child.VisualRoot as Visual; visual is IHostedVisualTreeRoot hostedVisualTreeRoot; visual = hostedVisualTreeRoot.Host?.VisualRoot as Visual)
		{
			if (visual == popupHost)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsInsidePopup(Visual visual)
	{
		if (_openState == null)
		{
			return false;
		}
		return ((Visual)_openState.PopupHost).IsVisualAncestorOf(visual);
	}

	private void WindowDeactivated(object? sender, EventArgs e)
	{
		if (IsLightDismissEnabled)
		{
			Close();
		}
	}

	private void ParentClosed(object? sender, EventArgs e)
	{
		if (IsLightDismissEnabled)
		{
			Close();
		}
	}

	private void PlacementTargetTransformChanged(Visual v, Matrix? matrix)
	{
		if (_openState != null)
		{
			UpdateHostSizing(_openState.PopupHost, _openState.TopLevel, _openState.PlacementTarget);
		}
	}

	private void WindowLostFocus()
	{
		if (IsLightDismissEnabled)
		{
			Close();
		}
	}

	private void WindowPositionChanged(PixelPoint pp)
	{
		HandlePositionChange();
	}

	private void PlacementTargetLayoutUpdated(object? src, EventArgs e)
	{
		HandlePositionChange();
	}

	private void ParentPopupPositionChanged(object? src, PixelPointEventArgs e)
	{
		HandlePositionChange();
	}

	private IgnoreIsOpenScope BeginIgnoringIsOpen()
	{
		return new IgnoreIsOpenScope(this);
	}
}
