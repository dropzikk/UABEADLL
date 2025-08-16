using System;
using System.Linq;
using System.Windows.Input;
using Avalonia.Automation.Peers;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

namespace Avalonia.Controls;

[PseudoClasses(new string[] { ":flyout-open", ":pressed" })]
public class Button : ContentControl, ICommandSource, IClickableControl
{
	private const string pcPressed = ":pressed";

	private const string pcFlyoutOpen = ":flyout-open";

	public static readonly StyledProperty<ClickMode> ClickModeProperty;

	public static readonly StyledProperty<ICommand?> CommandProperty;

	public static readonly StyledProperty<KeyGesture?> HotKeyProperty;

	public static readonly StyledProperty<object?> CommandParameterProperty;

	public static readonly StyledProperty<bool> IsDefaultProperty;

	public static readonly StyledProperty<bool> IsCancelProperty;

	public static readonly RoutedEvent<RoutedEventArgs> ClickEvent;

	public static readonly DirectProperty<Button, bool> IsPressedProperty;

	public static readonly StyledProperty<FlyoutBase?> FlyoutProperty;

	private bool _commandCanExecute = true;

	private KeyGesture? _hotkey;

	private bool _isFlyoutOpen;

	private bool _isPressed;

	public ClickMode ClickMode
	{
		get
		{
			return GetValue(ClickModeProperty);
		}
		set
		{
			SetValue(ClickModeProperty, value);
		}
	}

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

	public KeyGesture? HotKey
	{
		get
		{
			return GetValue(HotKeyProperty);
		}
		set
		{
			SetValue(HotKeyProperty, value);
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

	public bool IsDefault
	{
		get
		{
			return GetValue(IsDefaultProperty);
		}
		set
		{
			SetValue(IsDefaultProperty, value);
		}
	}

	public bool IsCancel
	{
		get
		{
			return GetValue(IsCancelProperty);
		}
		set
		{
			SetValue(IsCancelProperty, value);
		}
	}

	public bool IsPressed
	{
		get
		{
			return _isPressed;
		}
		private set
		{
			SetAndRaise(IsPressedProperty, ref _isPressed, value);
		}
	}

	public FlyoutBase? Flyout
	{
		get
		{
			return GetValue(FlyoutProperty);
		}
		set
		{
			SetValue(FlyoutProperty, value);
		}
	}

	protected override bool IsEnabledCore
	{
		get
		{
			if (base.IsEnabledCore)
			{
				return _commandCanExecute;
			}
			return false;
		}
	}

	public event EventHandler<RoutedEventArgs>? Click
	{
		add
		{
			AddHandler(ClickEvent, value);
		}
		remove
		{
			RemoveHandler(ClickEvent, value);
		}
	}

	static Button()
	{
		ClickModeProperty = AvaloniaProperty.Register<Button, ClickMode>("ClickMode", ClickMode.Release);
		CommandProperty = AvaloniaProperty.Register<Button, ICommand>("Command", null, inherits: false, BindingMode.OneWay, null, null, enableDataValidation: true);
		HotKeyProperty = HotKeyManager.HotKeyProperty.AddOwner<Button>();
		CommandParameterProperty = AvaloniaProperty.Register<Button, object>("CommandParameter");
		IsDefaultProperty = AvaloniaProperty.Register<Button, bool>("IsDefault", defaultValue: false);
		IsCancelProperty = AvaloniaProperty.Register<Button, bool>("IsCancel", defaultValue: false);
		ClickEvent = RoutedEvent.Register<Button, RoutedEventArgs>("Click", RoutingStrategies.Bubble);
		IsPressedProperty = AvaloniaProperty.RegisterDirect("IsPressed", (Button b) => b.IsPressed, null, unsetValue: false);
		FlyoutProperty = AvaloniaProperty.Register<Button, FlyoutBase>("Flyout");
		InputElement.FocusableProperty.OverrideDefaultValue(typeof(Button), defaultValue: true);
		AccessKeyHandler.AccessKeyPressedEvent.AddClassHandler(delegate(Button lbl, RoutedEventArgs args)
		{
			lbl.OnAccessKey(args);
		});
	}

	protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnAttachedToVisualTree(e);
		if (IsDefault && e.Root is IInputElement root)
		{
			ListenForDefault(root);
		}
		if (IsCancel && e.Root is IInputElement root2)
		{
			ListenForCancel(root2);
		}
	}

	protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnDetachedFromVisualTree(e);
		if (IsDefault && e.Root is IInputElement root)
		{
			StopListeningForDefault(root);
		}
		if (IsCancel && e.Root is IInputElement root2)
		{
			StopListeningForCancel(root2);
		}
	}

	protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
	{
		if (_hotkey != null)
		{
			SetCurrentValue(HotKeyProperty, _hotkey);
		}
		base.OnAttachedToLogicalTree(e);
		if (Command != null)
		{
			Command.CanExecuteChanged += CanExecuteChanged;
			CanExecuteChanged(this, EventArgs.Empty);
		}
	}

	protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
	{
		if (HotKey != null)
		{
			_hotkey = HotKey;
			SetCurrentValue(HotKeyProperty, null);
		}
		base.OnDetachedFromLogicalTree(e);
		if (Command != null)
		{
			Command.CanExecuteChanged -= CanExecuteChanged;
		}
	}

	protected virtual void OnAccessKey(RoutedEventArgs e)
	{
		OnClick();
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		switch (e.Key)
		{
		case Key.Return:
			OnClick();
			e.Handled = true;
			break;
		case Key.Space:
			if (ClickMode == ClickMode.Press)
			{
				OnClick();
			}
			IsPressed = true;
			e.Handled = true;
			break;
		case Key.Escape:
			if (Flyout != null)
			{
				CloseFlyout();
			}
			break;
		}
		base.OnKeyDown(e);
	}

	protected override void OnKeyUp(KeyEventArgs e)
	{
		if (e.Key == Key.Space)
		{
			if (ClickMode == ClickMode.Release)
			{
				OnClick();
			}
			IsPressed = false;
			e.Handled = true;
		}
		base.OnKeyUp(e);
	}

	protected virtual void OnClick()
	{
		if (!base.IsEffectivelyEnabled)
		{
			return;
		}
		if (_isFlyoutOpen)
		{
			CloseFlyout();
		}
		else
		{
			OpenFlyout();
		}
		RoutedEventArgs routedEventArgs = new RoutedEventArgs(ClickEvent);
		RaiseEvent(routedEventArgs);
		if (!routedEventArgs.Handled)
		{
			ICommand? command = Command;
			if (command != null && command.CanExecute(CommandParameter))
			{
				Command.Execute(CommandParameter);
				routedEventArgs.Handled = true;
			}
		}
	}

	protected virtual void OpenFlyout()
	{
		Flyout?.ShowAt(this);
	}

	protected virtual void CloseFlyout()
	{
		Flyout?.Hide();
	}

	protected virtual void OnFlyoutOpened()
	{
	}

	protected virtual void OnFlyoutClosed()
	{
	}

	protected override void OnPointerPressed(PointerPressedEventArgs e)
	{
		base.OnPointerPressed(e);
		if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
		{
			IsPressed = true;
			e.Handled = true;
			if (ClickMode == ClickMode.Press)
			{
				OnClick();
			}
		}
	}

	protected override void OnPointerReleased(PointerReleasedEventArgs e)
	{
		base.OnPointerReleased(e);
		if (IsPressed && e.InitialPressMouseButton == MouseButton.Left)
		{
			IsPressed = false;
			e.Handled = true;
			if (ClickMode == ClickMode.Release && this.GetVisualsAt(e.GetPosition(this)).Any((Visual c) => this == c || this.IsVisualAncestorOf(c)))
			{
				OnClick();
			}
		}
	}

	protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
	{
		base.OnPointerCaptureLost(e);
		IsPressed = false;
	}

	protected override void OnLostFocus(RoutedEventArgs e)
	{
		base.OnLostFocus(e);
		IsPressed = false;
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);
		UnregisterFlyoutEvents(Flyout);
		RegisterFlyoutEvents(Flyout);
		UpdatePseudoClasses();
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == CommandProperty)
		{
			if (((ILogical)this).IsAttachedToLogicalTree)
			{
				var (command, command2) = change.GetOldAndNewValue<ICommand>();
				if (command != null)
				{
					ICommand command3 = command;
					command3.CanExecuteChanged -= CanExecuteChanged;
				}
				if (command2 != null)
				{
					ICommand command4 = command2;
					command4.CanExecuteChanged += CanExecuteChanged;
				}
			}
			CanExecuteChanged(this, EventArgs.Empty);
		}
		else if (change.Property == CommandParameterProperty)
		{
			CanExecuteChanged(this, EventArgs.Empty);
		}
		else if (change.Property == IsCancelProperty)
		{
			bool newValue = change.GetNewValue<bool>();
			if (base.VisualRoot is IInputElement root)
			{
				if (newValue)
				{
					ListenForCancel(root);
				}
				else
				{
					StopListeningForCancel(root);
				}
			}
		}
		else if (change.Property == IsDefaultProperty)
		{
			bool newValue2 = change.GetNewValue<bool>();
			if (base.VisualRoot is IInputElement root2)
			{
				if (newValue2)
				{
					ListenForDefault(root2);
				}
				else
				{
					StopListeningForDefault(root2);
				}
			}
		}
		else if (change.Property == IsPressedProperty)
		{
			UpdatePseudoClasses();
		}
		else if (change.Property == FlyoutProperty)
		{
			var (flyoutBase, flyout) = change.GetOldAndNewValue<FlyoutBase>();
			if (flyoutBase != null && flyoutBase.IsOpen)
			{
				flyoutBase.Hide();
			}
			UnregisterFlyoutEvents(flyoutBase);
			RegisterFlyoutEvents(flyout);
			UpdatePseudoClasses();
		}
	}

	protected override AutomationPeer OnCreateAutomationPeer()
	{
		return new ButtonAutomationPeer(this);
	}

	protected override void UpdateDataValidation(AvaloniaProperty property, BindingValueType state, Exception? error)
	{
		base.UpdateDataValidation(property, state, error);
		if (property == CommandProperty && state == BindingValueType.BindingError && _commandCanExecute)
		{
			_commandCanExecute = false;
			UpdateIsEffectivelyEnabled();
		}
	}

	internal void PerformClick()
	{
		OnClick();
	}

	private void CanExecuteChanged(object? sender, EventArgs e)
	{
		bool flag = Command == null || Command.CanExecute(CommandParameter);
		if (flag != _commandCanExecute)
		{
			_commandCanExecute = flag;
			UpdateIsEffectivelyEnabled();
		}
	}

	private void RegisterFlyoutEvents(FlyoutBase? flyout)
	{
		if (flyout != null)
		{
			flyout.Opened += Flyout_Opened;
			flyout.Closed += Flyout_Closed;
		}
	}

	private void UnregisterFlyoutEvents(FlyoutBase? flyout)
	{
		if (flyout != null)
		{
			flyout.Opened -= Flyout_Opened;
			flyout.Closed -= Flyout_Closed;
		}
	}

	private void ListenForDefault(IInputElement root)
	{
		root.AddHandler(InputElement.KeyDownEvent, new Action<object, KeyEventArgs>(RootDefaultKeyDown));
	}

	private void ListenForCancel(IInputElement root)
	{
		root.AddHandler(InputElement.KeyDownEvent, new Action<object, KeyEventArgs>(RootCancelKeyDown));
	}

	private void StopListeningForDefault(IInputElement root)
	{
		root.RemoveHandler(InputElement.KeyDownEvent, new Action<object, KeyEventArgs>(RootDefaultKeyDown));
	}

	private void StopListeningForCancel(IInputElement root)
	{
		root.RemoveHandler(InputElement.KeyDownEvent, new Action<object, KeyEventArgs>(RootCancelKeyDown));
	}

	private void RootDefaultKeyDown(object? sender, KeyEventArgs e)
	{
		if (e.Key == Key.Return && base.IsVisible && base.IsEnabled)
		{
			OnClick();
			e.Handled = true;
		}
	}

	private void RootCancelKeyDown(object? sender, KeyEventArgs e)
	{
		if (e.Key == Key.Escape && base.IsVisible && base.IsEnabled)
		{
			OnClick();
			e.Handled = true;
		}
	}

	private void UpdatePseudoClasses()
	{
		base.PseudoClasses.Set(":flyout-open", _isFlyoutOpen);
		base.PseudoClasses.Set(":pressed", IsPressed);
	}

	void ICommandSource.CanExecuteChanged(object sender, EventArgs e)
	{
		CanExecuteChanged(sender, e);
	}

	void IClickableControl.RaiseClick()
	{
		OnClick();
	}

	private void Flyout_Opened(object? sender, EventArgs e)
	{
		if ((sender as FlyoutBase)?.Target == this)
		{
			_isFlyoutOpen = true;
			UpdatePseudoClasses();
			OnFlyoutOpened();
		}
	}

	private void Flyout_Closed(object? sender, EventArgs e)
	{
		if ((sender as FlyoutBase)?.Target == this)
		{
			_isFlyoutOpen = false;
			UpdatePseudoClasses();
			OnFlyoutClosed();
		}
	}
}
