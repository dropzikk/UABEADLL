using System;
using System.Windows.Input;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Reactive;

namespace Avalonia.Controls;

[TemplatePart("PART_PrimaryButton", typeof(Button))]
[TemplatePart("PART_SecondaryButton", typeof(Button))]
[PseudoClasses(new string[] { ":flyout-open", ":pressed" })]
public class SplitButton : ContentControl, ICommandSource
{
	internal const string pcChecked = ":checked";

	internal const string pcPressed = ":pressed";

	internal const string pcFlyoutOpen = ":flyout-open";

	public static readonly RoutedEvent<RoutedEventArgs> ClickEvent = RoutedEvent.Register<SplitButton, RoutedEventArgs>("Click", RoutingStrategies.Bubble);

	public static readonly StyledProperty<ICommand?> CommandProperty = Button.CommandProperty.AddOwner<SplitButton>();

	public static readonly StyledProperty<object?> CommandParameterProperty = Button.CommandParameterProperty.AddOwner<SplitButton>();

	public static readonly StyledProperty<FlyoutBase?> FlyoutProperty = Button.FlyoutProperty.AddOwner<SplitButton>();

	private Button? _primaryButton;

	private Button? _secondaryButton;

	private bool _commandCanExecute = true;

	private bool _isAttachedToLogicalTree;

	private bool _isFlyoutOpen;

	private bool _isKeyboardPressed;

	private IDisposable? _flyoutPropertyChangedDisposable;

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

	internal virtual bool InternalIsChecked => false;

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

	void ICommandSource.CanExecuteChanged(object sender, EventArgs e)
	{
		CanExecuteChanged(sender, e);
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

	protected void UpdatePseudoClasses()
	{
		base.PseudoClasses.Set(":flyout-open", _isFlyoutOpen);
		base.PseudoClasses.Set(":pressed", _isKeyboardPressed);
		base.PseudoClasses.Set(":checked", InternalIsChecked);
	}

	protected void OpenFlyout()
	{
		if (Flyout != null)
		{
			Flyout.ShowAt(this);
		}
	}

	protected void CloseFlyout()
	{
		if (Flyout != null)
		{
			Flyout.Hide();
		}
	}

	private void RegisterFlyoutEvents(FlyoutBase? flyout)
	{
		if (flyout != null)
		{
			flyout.Opened += Flyout_Opened;
			flyout.Closed += Flyout_Closed;
			_flyoutPropertyChangedDisposable = flyout.GetPropertyChangedObservable(Popup.PlacementProperty).Subscribe(Flyout_PlacementPropertyChanged);
		}
	}

	private void UnregisterFlyoutEvents(FlyoutBase? flyout)
	{
		if (flyout != null)
		{
			flyout.Opened -= Flyout_Opened;
			flyout.Closed -= Flyout_Closed;
			_flyoutPropertyChangedDisposable?.Dispose();
			_flyoutPropertyChangedDisposable = null;
		}
	}

	private void UnregisterEvents()
	{
		if (_primaryButton != null)
		{
			_primaryButton.Click -= PrimaryButton_Click;
		}
		if (_secondaryButton != null)
		{
			_secondaryButton.Click -= SecondaryButton_Click;
		}
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);
		UnregisterEvents();
		UnregisterFlyoutEvents(Flyout);
		_primaryButton = e.NameScope.Find<Button>("PART_PrimaryButton");
		_secondaryButton = e.NameScope.Find<Button>("PART_SecondaryButton");
		if (_primaryButton != null)
		{
			_primaryButton.Click += PrimaryButton_Click;
		}
		if (_secondaryButton != null)
		{
			_secondaryButton.Click += SecondaryButton_Click;
		}
		RegisterFlyoutEvents(Flyout);
		UpdatePseudoClasses();
	}

	protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
	{
		base.OnAttachedToLogicalTree(e);
		if (Command != null)
		{
			Command.CanExecuteChanged += CanExecuteChanged;
			CanExecuteChanged(this, EventArgs.Empty);
		}
		_isAttachedToLogicalTree = true;
	}

	protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
	{
		base.OnDetachedFromLogicalTree(e);
		if (Command != null)
		{
			Command.CanExecuteChanged -= CanExecuteChanged;
		}
		_isAttachedToLogicalTree = false;
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Property == CommandProperty)
		{
			if (_isAttachedToLogicalTree)
			{
				var (command, command2) = e.GetOldAndNewValue<ICommand>();
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
		else if (e.Property == CommandParameterProperty)
		{
			CanExecuteChanged(this, EventArgs.Empty);
		}
		else if (e.Property == FlyoutProperty)
		{
			var (flyoutBase, flyout) = e.GetOldAndNewValue<FlyoutBase>();
			if (flyoutBase != null && flyoutBase.IsOpen)
			{
				flyoutBase.Hide();
			}
			UnregisterFlyoutEvents(flyoutBase);
			RegisterFlyoutEvents(flyout);
			UpdatePseudoClasses();
		}
		base.OnPropertyChanged(e);
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		Key key = e.Key;
		if (key == Key.Space || key == Key.Return)
		{
			_isKeyboardPressed = true;
			UpdatePseudoClasses();
		}
		base.OnKeyDown(e);
	}

	protected override void OnKeyUp(KeyEventArgs e)
	{
		Key key = e.Key;
		if (key == Key.Space || key == Key.Return)
		{
			_isKeyboardPressed = false;
			UpdatePseudoClasses();
			if (base.IsEffectivelyEnabled)
			{
				OnClickPrimary(null);
				e.Handled = true;
			}
		}
		else if (key == Key.Down && e.KeyModifiers.HasAllFlags(KeyModifiers.Alt) && base.IsEffectivelyEnabled)
		{
			OpenFlyout();
			e.Handled = true;
		}
		else if (key == Key.F4 && base.IsEffectivelyEnabled)
		{
			OpenFlyout();
			e.Handled = true;
		}
		else if (e.Key == Key.Escape && _isFlyoutOpen)
		{
			CloseFlyout();
			e.Handled = true;
		}
		base.OnKeyUp(e);
	}

	protected virtual void OnClickPrimary(RoutedEventArgs? e)
	{
		if (!base.IsEffectivelyEnabled)
		{
			return;
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

	protected virtual void OnClickSecondary(RoutedEventArgs? e)
	{
		if (base.IsEffectivelyEnabled)
		{
			OpenFlyout();
		}
	}

	protected virtual void OnFlyoutOpened()
	{
	}

	protected virtual void OnFlyoutClosed()
	{
	}

	private void PrimaryButton_Click(object? sender, RoutedEventArgs e)
	{
		e.Handled = true;
		OnClickPrimary(e);
	}

	private void SecondaryButton_Click(object? sender, RoutedEventArgs e)
	{
		e.Handled = true;
		OnClickSecondary(e);
	}

	private void Flyout_PlacementPropertyChanged(AvaloniaPropertyChangedEventArgs e)
	{
		UpdatePseudoClasses();
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
