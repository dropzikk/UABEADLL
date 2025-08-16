using System;
using System.Collections.Generic;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input.GestureRecognizers;
using Avalonia.Input.TextInput;
using Avalonia.Interactivity;
using Avalonia.Reactive;
using Avalonia.VisualTree;

namespace Avalonia.Input;

[PseudoClasses(new string[] { ":disabled", ":focus", ":focus-visible", ":focus-within", ":pointerover" })]
public class InputElement : Interactive, IInputElement
{
	public static readonly StyledProperty<bool> FocusableProperty;

	public static readonly StyledProperty<bool> IsEnabledProperty;

	public static readonly DirectProperty<InputElement, bool> IsEffectivelyEnabledProperty;

	public static readonly StyledProperty<Cursor?> CursorProperty;

	public static readonly DirectProperty<InputElement, bool> IsKeyboardFocusWithinProperty;

	public static readonly DirectProperty<InputElement, bool> IsFocusedProperty;

	public static readonly StyledProperty<bool> IsHitTestVisibleProperty;

	public static readonly DirectProperty<InputElement, bool> IsPointerOverProperty;

	public static readonly StyledProperty<bool> IsTabStopProperty;

	public static readonly RoutedEvent<GotFocusEventArgs> GotFocusEvent;

	public static readonly RoutedEvent<RoutedEventArgs> LostFocusEvent;

	public static readonly RoutedEvent<KeyEventArgs> KeyDownEvent;

	public static readonly RoutedEvent<KeyEventArgs> KeyUpEvent;

	public static readonly StyledProperty<int> TabIndexProperty;

	public static readonly RoutedEvent<TextInputEventArgs> TextInputEvent;

	public static readonly RoutedEvent<TextInputMethodClientRequestedEventArgs> TextInputMethodClientRequestedEvent;

	public static readonly RoutedEvent<PointerEventArgs> PointerEnteredEvent;

	public static readonly RoutedEvent<PointerEventArgs> PointerExitedEvent;

	public static readonly RoutedEvent<PointerEventArgs> PointerMovedEvent;

	public static readonly RoutedEvent<PointerPressedEventArgs> PointerPressedEvent;

	public static readonly RoutedEvent<PointerReleasedEventArgs> PointerReleasedEvent;

	public static readonly RoutedEvent<PointerCaptureLostEventArgs> PointerCaptureLostEvent;

	public static readonly RoutedEvent<PointerWheelEventArgs> PointerWheelChangedEvent;

	public static readonly RoutedEvent<TappedEventArgs> TappedEvent;

	public static readonly RoutedEvent<HoldingRoutedEventArgs> HoldingEvent;

	public static readonly RoutedEvent<TappedEventArgs> DoubleTappedEvent;

	private bool _isEffectivelyEnabled = true;

	private bool _isFocused;

	private bool _isKeyboardFocusWithin;

	private bool _isFocusVisible;

	private bool _isPointerOver;

	private GestureRecognizerCollection? _gestureRecognizers;

	public bool Focusable
	{
		get
		{
			return GetValue(FocusableProperty);
		}
		set
		{
			SetValue(FocusableProperty, value);
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

	public Cursor? Cursor
	{
		get
		{
			return GetValue(CursorProperty);
		}
		set
		{
			SetValue(CursorProperty, value);
		}
	}

	public bool IsKeyboardFocusWithin
	{
		get
		{
			return _isKeyboardFocusWithin;
		}
		internal set
		{
			SetAndRaise(IsKeyboardFocusWithinProperty, ref _isKeyboardFocusWithin, value);
		}
	}

	public bool IsFocused
	{
		get
		{
			return _isFocused;
		}
		private set
		{
			SetAndRaise(IsFocusedProperty, ref _isFocused, value);
		}
	}

	public bool IsHitTestVisible
	{
		get
		{
			return GetValue(IsHitTestVisibleProperty);
		}
		set
		{
			SetValue(IsHitTestVisibleProperty, value);
		}
	}

	public bool IsPointerOver
	{
		get
		{
			return _isPointerOver;
		}
		internal set
		{
			SetAndRaise(IsPointerOverProperty, ref _isPointerOver, value);
		}
	}

	public bool IsTabStop
	{
		get
		{
			return GetValue(IsTabStopProperty);
		}
		set
		{
			SetValue(IsTabStopProperty, value);
		}
	}

	public bool IsEffectivelyEnabled
	{
		get
		{
			return _isEffectivelyEnabled;
		}
		private set
		{
			SetAndRaise(IsEffectivelyEnabledProperty, ref _isEffectivelyEnabled, value);
			base.PseudoClasses.Set(":disabled", !value);
			if (!IsEffectivelyEnabled)
			{
				FocusManager focusManager = FocusManager.GetFocusManager(this);
				if (focusManager != null && object.Equals(focusManager.GetFocusedElement(), this))
				{
					focusManager.ClearFocus();
				}
			}
		}
	}

	public int TabIndex
	{
		get
		{
			return GetValue(TabIndexProperty);
		}
		set
		{
			SetValue(TabIndexProperty, value);
		}
	}

	public List<KeyBinding> KeyBindings { get; } = new List<KeyBinding>();

	protected virtual bool IsEnabledCore => IsEnabled;

	public GestureRecognizerCollection GestureRecognizers => _gestureRecognizers ?? (_gestureRecognizers = new GestureRecognizerCollection(this));

	public event EventHandler<GotFocusEventArgs>? GotFocus
	{
		add
		{
			AddHandler(GotFocusEvent, value);
		}
		remove
		{
			RemoveHandler(GotFocusEvent, value);
		}
	}

	public event EventHandler<RoutedEventArgs>? LostFocus
	{
		add
		{
			AddHandler(LostFocusEvent, value);
		}
		remove
		{
			RemoveHandler(LostFocusEvent, value);
		}
	}

	public event EventHandler<KeyEventArgs>? KeyDown
	{
		add
		{
			AddHandler(KeyDownEvent, value);
		}
		remove
		{
			RemoveHandler(KeyDownEvent, value);
		}
	}

	public event EventHandler<KeyEventArgs>? KeyUp
	{
		add
		{
			AddHandler(KeyUpEvent, value);
		}
		remove
		{
			RemoveHandler(KeyUpEvent, value);
		}
	}

	public event EventHandler<TextInputEventArgs>? TextInput
	{
		add
		{
			AddHandler(TextInputEvent, value);
		}
		remove
		{
			RemoveHandler(TextInputEvent, value);
		}
	}

	public event EventHandler<TextInputMethodClientRequestedEventArgs>? TextInputMethodClientRequested
	{
		add
		{
			AddHandler(TextInputMethodClientRequestedEvent, value);
		}
		remove
		{
			RemoveHandler(TextInputMethodClientRequestedEvent, value);
		}
	}

	public event EventHandler<PointerEventArgs>? PointerEntered
	{
		add
		{
			AddHandler(PointerEnteredEvent, value);
		}
		remove
		{
			RemoveHandler(PointerEnteredEvent, value);
		}
	}

	public event EventHandler<PointerEventArgs>? PointerExited
	{
		add
		{
			AddHandler(PointerExitedEvent, value);
		}
		remove
		{
			RemoveHandler(PointerExitedEvent, value);
		}
	}

	public event EventHandler<PointerEventArgs>? PointerMoved
	{
		add
		{
			AddHandler(PointerMovedEvent, value);
		}
		remove
		{
			RemoveHandler(PointerMovedEvent, value);
		}
	}

	public event EventHandler<PointerPressedEventArgs>? PointerPressed
	{
		add
		{
			AddHandler(PointerPressedEvent, value);
		}
		remove
		{
			RemoveHandler(PointerPressedEvent, value);
		}
	}

	public event EventHandler<PointerReleasedEventArgs>? PointerReleased
	{
		add
		{
			AddHandler(PointerReleasedEvent, value);
		}
		remove
		{
			RemoveHandler(PointerReleasedEvent, value);
		}
	}

	public event EventHandler<PointerCaptureLostEventArgs>? PointerCaptureLost
	{
		add
		{
			AddHandler(PointerCaptureLostEvent, value);
		}
		remove
		{
			RemoveHandler(PointerCaptureLostEvent, value);
		}
	}

	public event EventHandler<PointerWheelEventArgs>? PointerWheelChanged
	{
		add
		{
			AddHandler(PointerWheelChangedEvent, value);
		}
		remove
		{
			RemoveHandler(PointerWheelChangedEvent, value);
		}
	}

	public event EventHandler<TappedEventArgs>? Tapped
	{
		add
		{
			AddHandler(TappedEvent, value);
		}
		remove
		{
			RemoveHandler(TappedEvent, value);
		}
	}

	public event EventHandler<HoldingRoutedEventArgs>? Holding
	{
		add
		{
			AddHandler(HoldingEvent, value);
		}
		remove
		{
			RemoveHandler(HoldingEvent, value);
		}
	}

	public event EventHandler<TappedEventArgs>? DoubleTapped
	{
		add
		{
			AddHandler(DoubleTappedEvent, value);
		}
		remove
		{
			RemoveHandler(DoubleTappedEvent, value);
		}
	}

	static InputElement()
	{
		FocusableProperty = AvaloniaProperty.Register<InputElement, bool>("Focusable", defaultValue: false);
		IsEnabledProperty = AvaloniaProperty.Register<InputElement, bool>("IsEnabled", defaultValue: true);
		IsEffectivelyEnabledProperty = AvaloniaProperty.RegisterDirect("IsEffectivelyEnabled", (InputElement o) => o.IsEffectivelyEnabled, null, unsetValue: false);
		CursorProperty = AvaloniaProperty.Register<InputElement, Cursor>("Cursor", null, inherits: true);
		IsKeyboardFocusWithinProperty = AvaloniaProperty.RegisterDirect("IsKeyboardFocusWithin", (InputElement o) => o.IsKeyboardFocusWithin, null, unsetValue: false);
		IsFocusedProperty = AvaloniaProperty.RegisterDirect("IsFocused", (InputElement o) => o.IsFocused, null, unsetValue: false);
		IsHitTestVisibleProperty = AvaloniaProperty.Register<InputElement, bool>("IsHitTestVisible", defaultValue: true);
		IsPointerOverProperty = AvaloniaProperty.RegisterDirect("IsPointerOver", (InputElement o) => o.IsPointerOver, null, unsetValue: false);
		IsTabStopProperty = KeyboardNavigation.IsTabStopProperty.AddOwner<InputElement>();
		GotFocusEvent = RoutedEvent.Register<InputElement, GotFocusEventArgs>("GotFocus", RoutingStrategies.Bubble);
		LostFocusEvent = RoutedEvent.Register<InputElement, RoutedEventArgs>("LostFocus", RoutingStrategies.Bubble);
		KeyDownEvent = RoutedEvent.Register<InputElement, KeyEventArgs>("KeyDown", RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
		KeyUpEvent = RoutedEvent.Register<InputElement, KeyEventArgs>("KeyUp", RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
		TabIndexProperty = KeyboardNavigation.TabIndexProperty.AddOwner<InputElement>();
		TextInputEvent = RoutedEvent.Register<InputElement, TextInputEventArgs>("TextInput", RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
		TextInputMethodClientRequestedEvent = RoutedEvent.Register<InputElement, TextInputMethodClientRequestedEventArgs>("TextInputMethodClientRequested", RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
		PointerEnteredEvent = RoutedEvent.Register<InputElement, PointerEventArgs>("PointerEntered", RoutingStrategies.Direct);
		PointerExitedEvent = RoutedEvent.Register<InputElement, PointerEventArgs>("PointerExited", RoutingStrategies.Direct);
		PointerMovedEvent = RoutedEvent.Register<InputElement, PointerEventArgs>("PointerMoved", RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
		PointerPressedEvent = RoutedEvent.Register<InputElement, PointerPressedEventArgs>("PointerPressed", RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
		PointerReleasedEvent = RoutedEvent.Register<InputElement, PointerReleasedEventArgs>("PointerReleased", RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
		PointerCaptureLostEvent = RoutedEvent.Register<InputElement, PointerCaptureLostEventArgs>("PointerCaptureLost", RoutingStrategies.Direct);
		PointerWheelChangedEvent = RoutedEvent.Register<InputElement, PointerWheelEventArgs>("PointerWheelChanged", RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
		TappedEvent = Gestures.TappedEvent;
		HoldingEvent = Gestures.HoldingEvent;
		DoubleTappedEvent = Gestures.DoubleTappedEvent;
		IsEnabledProperty.Changed.Subscribe(IsEnabledChanged);
		GotFocusEvent.AddClassHandler(delegate(InputElement x, GotFocusEventArgs e)
		{
			x.OnGotFocus(e);
		});
		LostFocusEvent.AddClassHandler(delegate(InputElement x, RoutedEventArgs e)
		{
			x.OnLostFocus(e);
		});
		KeyDownEvent.AddClassHandler(delegate(InputElement x, KeyEventArgs e)
		{
			x.OnKeyDown(e);
		});
		KeyUpEvent.AddClassHandler(delegate(InputElement x, KeyEventArgs e)
		{
			x.OnKeyUp(e);
		});
		TextInputEvent.AddClassHandler(delegate(InputElement x, TextInputEventArgs e)
		{
			x.OnTextInput(e);
		});
		PointerEnteredEvent.AddClassHandler(delegate(InputElement x, PointerEventArgs e)
		{
			x.OnPointerEnteredCore(e);
		});
		PointerExitedEvent.AddClassHandler(delegate(InputElement x, PointerEventArgs e)
		{
			x.OnPointerExitedCore(e);
		});
		PointerMovedEvent.AddClassHandler(delegate(InputElement x, PointerEventArgs e)
		{
			x.OnPointerMoved(e);
		});
		PointerPressedEvent.AddClassHandler(delegate(InputElement x, PointerPressedEventArgs e)
		{
			x.OnPointerPressed(e);
		});
		PointerReleasedEvent.AddClassHandler(delegate(InputElement x, PointerReleasedEventArgs e)
		{
			x.OnPointerReleased(e);
		});
		PointerCaptureLostEvent.AddClassHandler(delegate(InputElement x, PointerCaptureLostEventArgs e)
		{
			x.OnPointerCaptureLost(e);
		});
		PointerWheelChangedEvent.AddClassHandler(delegate(InputElement x, PointerWheelEventArgs e)
		{
			x.OnPointerWheelChanged(e);
		});
		PointerMovedEvent.AddClassHandler(delegate(InputElement x, PointerEventArgs e)
		{
			x.OnGesturePointerMoved(e);
		}, RoutingStrategies.Direct | RoutingStrategies.Bubble, handledEventsToo: true);
		PointerPressedEvent.AddClassHandler(delegate(InputElement x, PointerPressedEventArgs e)
		{
			x.OnGesturePointerPressed(e);
		}, RoutingStrategies.Direct | RoutingStrategies.Bubble, handledEventsToo: true);
		PointerReleasedEvent.AddClassHandler(delegate(InputElement x, PointerReleasedEventArgs e)
		{
			x.OnGesturePointerReleased(e);
		}, RoutingStrategies.Direct | RoutingStrategies.Bubble, handledEventsToo: true);
	}

	public InputElement()
	{
		UpdatePseudoClasses(IsFocused, IsPointerOver);
	}

	public bool Focus(NavigationMethod method = NavigationMethod.Unspecified, KeyModifiers keyModifiers = KeyModifiers.None)
	{
		return FocusManager.GetFocusManager(this)?.Focus(this, method, keyModifiers) ?? false;
	}

	protected override void OnDetachedFromVisualTreeCore(VisualTreeAttachmentEventArgs e)
	{
		base.OnDetachedFromVisualTreeCore(e);
		if (IsFocused)
		{
			FocusManager.GetFocusManager(this)?.ClearFocus();
		}
	}

	protected override void OnAttachedToVisualTreeCore(VisualTreeAttachmentEventArgs e)
	{
		base.OnAttachedToVisualTreeCore(e);
		UpdateIsEffectivelyEnabled();
	}

	protected virtual void OnGotFocus(GotFocusEventArgs e)
	{
		bool flag = e.Source == this;
		_isFocusVisible = flag && (e.NavigationMethod == NavigationMethod.Directional || e.NavigationMethod == NavigationMethod.Tab);
		IsFocused = flag;
	}

	protected virtual void OnLostFocus(RoutedEventArgs e)
	{
		_isFocusVisible = false;
		IsFocused = false;
	}

	protected virtual void OnKeyDown(KeyEventArgs e)
	{
	}

	protected virtual void OnKeyUp(KeyEventArgs e)
	{
	}

	protected virtual void OnTextInput(TextInputEventArgs e)
	{
	}

	protected virtual void OnPointerEntered(PointerEventArgs e)
	{
	}

	protected virtual void OnPointerExited(PointerEventArgs e)
	{
	}

	protected virtual void OnPointerMoved(PointerEventArgs e)
	{
	}

	protected virtual void OnPointerPressed(PointerPressedEventArgs e)
	{
	}

	protected virtual void OnPointerReleased(PointerReleasedEventArgs e)
	{
	}

	private void OnGesturePointerReleased(PointerReleasedEventArgs e)
	{
		if (!e.IsGestureRecognitionSkipped)
		{
			GestureRecognizerCollection? gestureRecognizers = _gestureRecognizers;
			if (gestureRecognizers != null && gestureRecognizers.HandlePointerReleased(e))
			{
				e.Handled = true;
			}
		}
	}

	private void OnGesturePointerPressed(PointerPressedEventArgs e)
	{
		if (!e.IsGestureRecognitionSkipped)
		{
			GestureRecognizerCollection? gestureRecognizers = _gestureRecognizers;
			if (gestureRecognizers != null && gestureRecognizers.HandlePointerPressed(e))
			{
				e.Handled = true;
			}
		}
	}

	private void OnGesturePointerMoved(PointerEventArgs e)
	{
		if (!e.IsGestureRecognitionSkipped)
		{
			GestureRecognizerCollection? gestureRecognizers = _gestureRecognizers;
			if (gestureRecognizers != null && gestureRecognizers.HandlePointerMoved(e))
			{
				e.Handled = true;
			}
		}
	}

	protected virtual void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
	{
	}

	protected virtual void OnPointerWheelChanged(PointerWheelEventArgs e)
	{
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == IsFocusedProperty)
		{
			UpdatePseudoClasses(change.GetNewValue<bool>(), null);
		}
		else if (change.Property == IsPointerOverProperty)
		{
			UpdatePseudoClasses(null, change.GetNewValue<bool>());
		}
		else if (change.Property == IsKeyboardFocusWithinProperty)
		{
			base.PseudoClasses.Set(":focus-within", change.GetNewValue<bool>());
		}
		else if (change.Property == Visual.IsVisibleProperty && !change.GetNewValue<bool>() && IsFocused)
		{
			FocusManager.GetFocusManager(this)?.ClearFocus();
		}
	}

	protected void UpdateIsEffectivelyEnabled()
	{
		UpdateIsEffectivelyEnabled(this.GetVisualParent<InputElement>());
	}

	private static void IsEnabledChanged(AvaloniaPropertyChangedEventArgs e)
	{
		((InputElement)e.Sender).UpdateIsEffectivelyEnabled();
	}

	private void OnPointerEnteredCore(PointerEventArgs e)
	{
		IsPointerOver = true;
		OnPointerEntered(e);
	}

	private void OnPointerExitedCore(PointerEventArgs e)
	{
		IsPointerOver = false;
		OnPointerExited(e);
	}

	private void UpdateIsEffectivelyEnabled(InputElement? parent)
	{
		IsEffectivelyEnabled = IsEnabledCore && (parent?.IsEffectivelyEnabled ?? true);
		IAvaloniaList<Visual> visualChildren = base.VisualChildren;
		for (int i = 0; i < visualChildren.Count; i++)
		{
			(visualChildren[i] as InputElement)?.UpdateIsEffectivelyEnabled(this);
		}
	}

	private void UpdatePseudoClasses(bool? isFocused, bool? isPointerOver)
	{
		if (isFocused.HasValue)
		{
			base.PseudoClasses.Set(":focus", isFocused.Value);
			base.PseudoClasses.Set(":focus-visible", _isFocusVisible);
		}
		if (isPointerOver.HasValue)
		{
			base.PseudoClasses.Set(":pointerover", isPointerOver.Value);
		}
	}
}
