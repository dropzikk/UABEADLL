using System;
using System.ComponentModel;
using System.Linq;
using Avalonia.Controls.Diagnostics;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Input.Raw;
using Avalonia.Layout;
using Avalonia.Logging;
using Avalonia.LogicalTree;
using Avalonia.Reactive;

namespace Avalonia.Controls.Primitives;

public abstract class PopupFlyoutBase : FlyoutBase, IPopupHostProvider
{
	public static readonly StyledProperty<PlacementMode> PlacementProperty;

	public static readonly StyledProperty<double> HorizontalOffsetProperty;

	public static readonly StyledProperty<double> VerticalOffsetProperty;

	public static readonly StyledProperty<PopupAnchor> PlacementAnchorProperty;

	public static readonly StyledProperty<PopupGravity> PlacementGravityProperty;

	public static readonly StyledProperty<FlyoutShowMode> ShowModeProperty;

	public static readonly StyledProperty<IInputElement?> OverlayInputPassThroughElementProperty;

	private readonly Lazy<Popup> _popupLazy;

	private Rect? _enlargedPopupRect;

	private PixelRect? _enlargePopupRectScreenPixelRect;

	private IDisposable? _transientDisposable;

	private Action<IPopupHost?>? _popupHostChangedHandler;

	protected Popup Popup => _popupLazy.Value;

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

	public FlyoutShowMode ShowMode
	{
		get
		{
			return GetValue(ShowModeProperty);
		}
		set
		{
			SetValue(ShowModeProperty, value);
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

	IPopupHost? IPopupHostProvider.PopupHost => Popup?.Host;

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

	public event EventHandler<CancelEventArgs>? Closing;

	public event EventHandler? Opening;

	static PopupFlyoutBase()
	{
		PlacementProperty = Avalonia.Controls.Primitives.Popup.PlacementProperty.AddOwner<PopupFlyoutBase>();
		HorizontalOffsetProperty = Avalonia.Controls.Primitives.Popup.HorizontalOffsetProperty.AddOwner<PopupFlyoutBase>();
		VerticalOffsetProperty = Avalonia.Controls.Primitives.Popup.VerticalOffsetProperty.AddOwner<PopupFlyoutBase>();
		PlacementAnchorProperty = Avalonia.Controls.Primitives.Popup.PlacementAnchorProperty.AddOwner<PopupFlyoutBase>();
		PlacementGravityProperty = Avalonia.Controls.Primitives.Popup.PlacementGravityProperty.AddOwner<PopupFlyoutBase>();
		ShowModeProperty = AvaloniaProperty.Register<PopupFlyoutBase, FlyoutShowMode>("ShowMode", FlyoutShowMode.Standard);
		OverlayInputPassThroughElementProperty = Avalonia.Controls.Primitives.Popup.OverlayInputPassThroughElementProperty.AddOwner<PopupFlyoutBase>();
		Control.ContextFlyoutProperty.Changed.Subscribe(OnContextFlyoutPropertyChanged);
	}

	public PopupFlyoutBase()
	{
		_popupLazy = new Lazy<Popup>(() => CreatePopup());
	}

	public sealed override void ShowAt(Control placementTarget)
	{
		ShowAtCore(placementTarget);
	}

	public void ShowAt(Control placementTarget, bool showAtPointer)
	{
		ShowAtCore(placementTarget, showAtPointer);
	}

	public sealed override void Hide()
	{
		HideCore();
	}

	protected virtual bool HideCore(bool canCancel = true)
	{
		if (!base.IsOpen)
		{
			return false;
		}
		if (canCancel && CancelClosing())
		{
			return false;
		}
		base.IsOpen = false;
		Popup.IsOpen = false;
		((ISetLogicalParent)Popup).SetParent((ILogical?)null);
		_transientDisposable?.Dispose();
		_transientDisposable = null;
		_enlargedPopupRect = null;
		_enlargePopupRectScreenPixelRect = null;
		if (base.Target != null)
		{
			base.Target.DetachedFromVisualTree -= PlacementTarget_DetachedFromVisualTree;
			base.Target.KeyUp -= OnPlacementTargetOrPopupKeyUp;
		}
		OnClosed();
		return true;
	}

	protected virtual bool ShowAtCore(Control placementTarget, bool showAtPointer = false)
	{
		if (placementTarget == null)
		{
			throw new ArgumentNullException("placementTarget");
		}
		if (base.IsOpen)
		{
			if (placementTarget == base.Target)
			{
				return false;
			}
			HideCore(canCancel: false);
		}
		if (Popup.Parent != null && Popup.Parent != placementTarget)
		{
			((ISetLogicalParent)Popup).SetParent((ILogical?)null);
		}
		if (Popup.Parent == null || Popup.PlacementTarget != placementTarget)
		{
			Popup popup = Popup;
			Control placementTarget2 = (base.Target = placementTarget);
			popup.PlacementTarget = placementTarget2;
			((ISetLogicalParent)Popup).SetParent((ILogical?)placementTarget);
			Popup.TemplatedParent = placementTarget.TemplatedParent;
		}
		if (Popup.Child == null)
		{
			Popup.Child = CreatePresenter();
		}
		Popup.OverlayInputPassThroughElement = OverlayInputPassThroughElement;
		if (CancelOpening())
		{
			return false;
		}
		PositionPopup(showAtPointer);
		bool isOpen = (Popup.IsOpen = true);
		base.IsOpen = isOpen;
		OnOpened();
		placementTarget.DetachedFromVisualTree += PlacementTarget_DetachedFromVisualTree;
		placementTarget.KeyUp += OnPlacementTargetOrPopupKeyUp;
		if (ShowMode == FlyoutShowMode.Standard)
		{
			if (Popup.Child.Focusable)
			{
				Popup.Child.Focus();
			}
			else
			{
				KeyboardNavigationHandler.GetNext(Popup.Child, NavigationDirection.Next)?.Focus();
			}
		}
		else if (ShowMode == FlyoutShowMode.TransientWithDismissOnPointerMoveAway)
		{
			_transientDisposable = InputManager.Instance?.Process.Subscribe(HandleTransientDismiss);
		}
		return true;
	}

	private void PlacementTarget_DetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
	{
		HideCore(canCancel: false);
	}

	private void HandleTransientDismiss(RawInputEventArgs args)
	{
		if (!(args is RawPointerEventArgs { Type: RawPointerEventType.Move } rawPointerEventArgs))
		{
			return;
		}
		if (!_enlargedPopupRect.HasValue && !_enlargePopupRectScreenPixelRect.HasValue)
		{
			if (Popup?.Host is PopupRoot { Bounds: var bounds } popupRoot)
			{
				Rect rect = bounds.Inflate(100.0);
				_enlargePopupRectScreenPixelRect = new PixelRect(popupRoot.PointToScreen(rect.TopLeft), popupRoot.PointToScreen(rect.BottomRight));
			}
			else if (Popup?.Host is OverlayPopupHost overlayPopupHost)
			{
				_enlargedPopupRect = overlayPopupHost.Bounds.Inflate(100.0);
			}
		}
		else if (Popup?.Host is PopupRoot && rawPointerEventArgs.Root is Visual visual)
		{
			PixelPoint p = visual.PointToScreen(rawPointerEventArgs.Position);
			ref PixelRect? enlargePopupRectScreenPixelRect = ref _enlargePopupRectScreenPixelRect;
			if (enlargePopupRectScreenPixelRect.HasValue && !enlargePopupRectScreenPixelRect.GetValueOrDefault().Contains(p))
			{
				HideCore(canCancel: false);
			}
		}
		else if (Popup?.Host is OverlayPopupHost)
		{
			ref Rect? enlargedPopupRect = ref _enlargedPopupRect;
			if (enlargedPopupRect.HasValue && !enlargedPopupRect.GetValueOrDefault().Contains(rawPointerEventArgs.Position))
			{
				HideCore(canCancel: false);
			}
		}
	}

	protected virtual void OnOpening(CancelEventArgs args)
	{
		this.Opening?.Invoke(this, args);
	}

	protected virtual void OnClosing(CancelEventArgs args)
	{
		this.Closing?.Invoke(this, args);
	}

	protected abstract Control CreatePresenter();

	private Popup CreatePopup()
	{
		Popup popup = new Popup();
		popup.WindowManagerAddShadowHint = false;
		popup.IsLightDismissEnabled = true;
		popup.OverlayDismissEventPassThrough = false;
		popup.Opened += OnPopupOpened;
		popup.Closed += OnPopupClosed;
		popup.Closing += OnPopupClosing;
		popup.KeyUp += OnPlacementTargetOrPopupKeyUp;
		return popup;
	}

	private void OnPopupOpened(object? sender, EventArgs e)
	{
		base.IsOpen = true;
		_popupHostChangedHandler?.Invoke(Popup.Host);
	}

	private void OnPopupClosing(object? sender, CancelEventArgs e)
	{
		if (base.IsOpen)
		{
			e.Cancel = CancelClosing();
		}
	}

	private void OnPopupClosed(object? sender, EventArgs e)
	{
		HideCore(canCancel: false);
		_popupHostChangedHandler?.Invoke(null);
	}

	private void OnPlacementTargetOrPopupKeyUp(object? sender, KeyEventArgs e)
	{
		if (!e.Handled && base.IsOpen && base.Target?.ContextFlyout == this)
		{
			PlatformHotkeyConfiguration obj = Application.Current.PlatformSettings?.HotkeyConfiguration;
			if (obj != null && obj.OpenContextMenu.Any((KeyGesture k) => k.Matches(e)))
			{
				e.Handled = HideCore();
			}
		}
	}

	private void PositionPopup(bool showAtPointer)
	{
		if (Popup.Child.DesiredSize == default(Size))
		{
			LayoutHelper.MeasureChild(Popup.Child, Size.Infinity, default(Thickness));
		}
		else
		{
			_ = Popup.Child.DesiredSize;
		}
		Popup.VerticalOffset = VerticalOffset;
		Popup.HorizontalOffset = HorizontalOffset;
		Popup.PlacementAnchor = PlacementAnchor;
		Popup.PlacementGravity = PlacementGravity;
		if (showAtPointer)
		{
			Popup.Placement = PlacementMode.Pointer;
			return;
		}
		Popup.Placement = Placement;
		Popup.PlacementConstraintAdjustment = PopupPositionerConstraintAdjustment.SlideX | PopupPositionerConstraintAdjustment.SlideY;
	}

	private static void OnContextFlyoutPropertyChanged(AvaloniaPropertyChangedEventArgs args)
	{
		if (args.Sender is Control control)
		{
			if (args.OldValue is FlyoutBase)
			{
				control.ContextRequested -= OnControlContextRequested;
			}
			if (args.NewValue is FlyoutBase)
			{
				control.ContextRequested += OnControlContextRequested;
			}
		}
	}

	private static void OnControlContextRequested(object? sender, ContextRequestedEventArgs e)
	{
		if (e.Handled || !(sender is Control control))
		{
			return;
		}
		FlyoutBase contextFlyout = control.ContextFlyout;
		if (contextFlyout != null)
		{
			if (control.ContextMenu != null)
			{
				Logger.TryGet(LogEventLevel.Verbose, "FlyoutBase")?.Log(control, "ContextMenu and ContextFlyout are both set, defaulting to ContextMenu");
			}
			else if (contextFlyout is PopupFlyoutBase popupFlyoutBase)
			{
				Point point;
				bool showAtPointer = e.TryGetPosition(null, out point);
				e.Handled = popupFlyoutBase.ShowAtCore(control, showAtPointer);
			}
			else
			{
				contextFlyout.ShowAt(control);
				e.Handled = true;
			}
		}
	}

	private bool CancelClosing()
	{
		CancelEventArgs cancelEventArgs = new CancelEventArgs();
		OnClosing(cancelEventArgs);
		return cancelEventArgs.Cancel;
	}

	private bool CancelOpening()
	{
		CancelEventArgs cancelEventArgs = new CancelEventArgs();
		OnOpening(cancelEventArgs);
		return cancelEventArgs.Cancel;
	}

	internal static void SetPresenterClasses(Control? presenter, Classes classes)
	{
		if (presenter == null)
		{
			return;
		}
		for (int num = presenter.Classes.Count - 1; num >= 0; num--)
		{
			if (!classes.Contains(presenter.Classes[num]) && !presenter.Classes[num].Contains(':'))
			{
				presenter.Classes.RemoveAt(num);
			}
		}
		presenter.Classes.AddRange(classes);
	}
}
