using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Controls.Diagnostics;
using Avalonia.Controls.Platform;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Reactive;
using Avalonia.Styling;

namespace Avalonia.Controls;

public class ContextMenu : MenuBase, ISetterValue, IPopupHostProvider
{
	public static readonly StyledProperty<double> HorizontalOffsetProperty;

	public static readonly StyledProperty<double> VerticalOffsetProperty;

	public static readonly StyledProperty<PopupAnchor> PlacementAnchorProperty;

	public static readonly StyledProperty<PopupPositionerConstraintAdjustment> PlacementConstraintAdjustmentProperty;

	public static readonly StyledProperty<PopupGravity> PlacementGravityProperty;

	public static readonly StyledProperty<PlacementMode> PlacementProperty;

	[Obsolete("Use the Placement property instead.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static readonly StyledProperty<PlacementMode> PlacementModeProperty;

	public static readonly StyledProperty<Rect?> PlacementRectProperty;

	public static readonly StyledProperty<bool> WindowManagerAddShadowHintProperty;

	public static readonly StyledProperty<Control?> PlacementTargetProperty;

	private static readonly FuncTemplate<Panel?> DefaultPanel;

	private Popup? _popup;

	private List<Control>? _attachedControls;

	private IInputElement? _previousFocus;

	private Action<IPopupHost?>? _popupHostChangedHandler;

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

	IPopupHost? IPopupHostProvider.PopupHost => _popup?.Host;

	public event CancelEventHandler? Opening;

	public event CancelEventHandler? Closing;

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

	public ContextMenu()
		: this(new DefaultMenuInteractionHandler(isContextMenu: true))
	{
	}

	public ContextMenu(IMenuInteractionHandler interactionHandler)
		: base(interactionHandler)
	{
	}

	static ContextMenu()
	{
		HorizontalOffsetProperty = Popup.HorizontalOffsetProperty.AddOwner<ContextMenu>();
		VerticalOffsetProperty = Popup.VerticalOffsetProperty.AddOwner<ContextMenu>();
		PlacementAnchorProperty = Popup.PlacementAnchorProperty.AddOwner<ContextMenu>();
		PlacementConstraintAdjustmentProperty = Popup.PlacementConstraintAdjustmentProperty.AddOwner<ContextMenu>();
		PlacementGravityProperty = Popup.PlacementGravityProperty.AddOwner<ContextMenu>();
		PlacementProperty = Popup.PlacementProperty.AddOwner<ContextMenu>();
		PlacementModeProperty = PlacementProperty;
		PlacementRectProperty = Popup.PlacementRectProperty.AddOwner<ContextMenu>();
		WindowManagerAddShadowHintProperty = Popup.WindowManagerAddShadowHintProperty.AddOwner<ContextMenu>();
		PlacementTargetProperty = Popup.PlacementTargetProperty.AddOwner<ContextMenu>();
		DefaultPanel = new FuncTemplate<Panel>(() => new StackPanel
		{
			Orientation = Orientation.Vertical
		});
		ItemsControl.ItemsPanelProperty.OverrideDefaultValue<ContextMenu>(DefaultPanel);
		PlacementProperty.OverrideDefaultValue<ContextMenu>(PlacementMode.Pointer);
		Control.ContextMenuProperty.Changed.Subscribe(ContextMenuChanged);
		AutomationProperties.AccessibilityViewProperty.OverrideDefaultValue<ContextMenu>(AccessibilityView.Control);
		AutomationProperties.ControlTypeOverrideProperty.OverrideDefaultValue<ContextMenu>(AutomationControlType.Menu);
	}

	private static void ContextMenuChanged(AvaloniaPropertyChangedEventArgs e)
	{
		Control control = (Control)e.Sender;
		if (e.OldValue is ContextMenu contextMenu)
		{
			control.ContextRequested -= ControlContextRequested;
			control.DetachedFromVisualTree -= ControlDetachedFromVisualTree;
			contextMenu._attachedControls?.Remove(control);
			((ISetLogicalParent)contextMenu._popup)?.SetParent((ILogical?)null);
		}
		if (e.NewValue is ContextMenu contextMenu2)
		{
			ContextMenu contextMenu3 = contextMenu2;
			if (contextMenu3._attachedControls == null)
			{
				contextMenu3._attachedControls = new List<Control>();
			}
			contextMenu2._attachedControls.Add(control);
			control.ContextRequested += ControlContextRequested;
			control.DetachedFromVisualTree += ControlDetachedFromVisualTree;
		}
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == WindowManagerAddShadowHintProperty && _popup != null)
		{
			_popup.WindowManagerAddShadowHint = change.GetNewValue<bool>();
		}
	}

	public override void Open()
	{
		Open(null);
	}

	public void Open(Control? control)
	{
		if (control == null && (_attachedControls == null || _attachedControls.Count == 0))
		{
			throw new ArgumentNullException("control");
		}
		if (control != null && _attachedControls != null && !_attachedControls.Contains(control))
		{
			throw new ArgumentException("Cannot show ContentMenu on a different control to the one it is attached to.", "control");
		}
		if (control == null)
		{
			control = _attachedControls[0];
		}
		Open(control, PlacementTarget ?? control, requestedByPointer: false);
	}

	public override void Close()
	{
		if (base.IsOpen && _popup != null && _popup.IsVisible)
		{
			_popup.IsOpen = false;
		}
	}

	void ISetterValue.Initialize(SetterBase setter)
	{
		if (!(setter is Setter setter2) || !(setter2.Property == Control.ContextMenuProperty))
		{
			throw new InvalidOperationException("Cannot use a control as a Setter value. Wrap the control in a <Template>.");
		}
	}

	private void Open(Control control, Control placementTarget, bool requestedByPointer)
	{
		if (!base.IsOpen)
		{
			if (_popup == null)
			{
				_popup = new Popup
				{
					IsLightDismissEnabled = true,
					OverlayDismissEventPassThrough = true
				};
				_popup.Opened += PopupOpened;
				_popup.Closed += PopupClosed;
				_popup.Closing += PopupClosing;
				_popup.KeyUp += PopupKeyUp;
			}
			if (_popup.Parent != control)
			{
				((ISetLogicalParent)_popup).SetParent((ILogical?)null);
				((ISetLogicalParent)_popup).SetParent((ILogical?)control);
			}
			_popup.Placement = ((!requestedByPointer && Placement == PlacementMode.Pointer) ? PlacementMode.Bottom : Placement);
			_popup.Child = this;
			_popup.PlacementTarget = placementTarget;
			_popup.HorizontalOffset = HorizontalOffset;
			_popup.VerticalOffset = VerticalOffset;
			_popup.PlacementAnchor = PlacementAnchor;
			_popup.PlacementConstraintAdjustment = PlacementConstraintAdjustment;
			_popup.PlacementGravity = PlacementGravity;
			_popup.PlacementRect = PlacementRect;
			_popup.WindowManagerAddShadowHint = WindowManagerAddShadowHint;
			base.IsOpen = true;
			_popup.IsOpen = true;
			RaiseEvent(new RoutedEventArgs
			{
				RoutedEvent = MenuBase.OpenedEvent,
				Source = this
			});
		}
	}

	private void PopupOpened(object? sender, EventArgs e)
	{
		_previousFocus = FocusManager.GetFocusManager(this)?.GetFocusedElement();
		Focus();
		_popupHostChangedHandler?.Invoke(_popup.Host);
	}

	private void PopupClosing(object? sender, CancelEventArgs e)
	{
		e.Cancel = CancelClosing();
	}

	private void PopupClosed(object? sender, EventArgs e)
	{
		foreach (ILogical logicalChild in base.LogicalChildren)
		{
			if (logicalChild is MenuItem menuItem)
			{
				menuItem.IsSubMenuOpen = false;
			}
		}
		base.SelectedIndex = -1;
		base.IsOpen = false;
		if (_attachedControls == null || _attachedControls.Count == 0)
		{
			((ISetLogicalParent)_popup).SetParent((ILogical?)null);
		}
		_previousFocus?.Focus();
		RaiseEvent(new RoutedEventArgs
		{
			RoutedEvent = MenuBase.ClosedEvent,
			Source = this
		});
		_popupHostChangedHandler?.Invoke(null);
	}

	private void PopupKeyUp(object? sender, KeyEventArgs e)
	{
		if (base.IsOpen)
		{
			PlatformHotkeyConfiguration hotkeyConfiguration = Application.Current.PlatformSettings.HotkeyConfiguration;
			if (hotkeyConfiguration != null && hotkeyConfiguration.OpenContextMenu.Any((KeyGesture k) => k.Matches(e)) && !CancelClosing())
			{
				Close();
				e.Handled = true;
			}
		}
	}

	private static void ControlContextRequested(object? sender, ContextRequestedEventArgs e)
	{
		if (sender is Control control)
		{
			ContextMenu contextMenu = control.ContextMenu;
			if (contextMenu != null && !e.Handled && !contextMenu.CancelOpening())
			{
				Point point;
				bool requestedByPointer = e.TryGetPosition(null, out point);
				contextMenu.Open(control, (e.Source as Control) ?? control, requestedByPointer);
				e.Handled = true;
			}
		}
	}

	private static void ControlDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
	{
		if (!(sender is Control control))
		{
			return;
		}
		ContextMenu contextMenu = control.ContextMenu;
		if (contextMenu != null)
		{
			if (contextMenu._popup?.Parent == control)
			{
				((ISetLogicalParent)contextMenu._popup).SetParent((ILogical?)null);
			}
			contextMenu.Close();
		}
	}

	private bool CancelClosing()
	{
		CancelEventArgs cancelEventArgs = new CancelEventArgs();
		this.Closing?.Invoke(this, cancelEventArgs);
		return cancelEventArgs.Cancel;
	}

	private bool CancelOpening()
	{
		CancelEventArgs cancelEventArgs = new CancelEventArgs();
		this.Opening?.Invoke(this, cancelEventArgs);
		return cancelEventArgs.Cancel;
	}
}
