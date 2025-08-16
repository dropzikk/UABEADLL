using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Reactive;

namespace Avalonia.Controls;

[TemplatePart("PART_DecreaseButton", typeof(Button))]
[TemplatePart("PART_IncreaseButton", typeof(Button))]
[PseudoClasses(new string[] { ":left", ":right" })]
public class ButtonSpinner : Spinner
{
	public static readonly StyledProperty<bool> AllowSpinProperty;

	public static readonly StyledProperty<bool> ShowButtonSpinnerProperty;

	public static readonly StyledProperty<Location> ButtonSpinnerLocationProperty;

	private Button? _decreaseButton;

	private Button? _increaseButton;

	private Button? DecreaseButton
	{
		get
		{
			return _decreaseButton;
		}
		set
		{
			if (_decreaseButton != null)
			{
				_decreaseButton.Click -= OnButtonClick;
			}
			_decreaseButton = value;
			if (_decreaseButton != null)
			{
				_decreaseButton.Click += OnButtonClick;
			}
		}
	}

	private Button? IncreaseButton
	{
		get
		{
			return _increaseButton;
		}
		set
		{
			if (_increaseButton != null)
			{
				_increaseButton.Click -= OnButtonClick;
			}
			_increaseButton = value;
			if (_increaseButton != null)
			{
				_increaseButton.Click += OnButtonClick;
			}
		}
	}

	public bool AllowSpin
	{
		get
		{
			return GetValue(AllowSpinProperty);
		}
		set
		{
			SetValue(AllowSpinProperty, value);
		}
	}

	public bool ShowButtonSpinner
	{
		get
		{
			return GetValue(ShowButtonSpinnerProperty);
		}
		set
		{
			SetValue(ShowButtonSpinnerProperty, value);
		}
	}

	public Location ButtonSpinnerLocation
	{
		get
		{
			return GetValue(ButtonSpinnerLocationProperty);
		}
		set
		{
			SetValue(ButtonSpinnerLocationProperty, value);
		}
	}

	public ButtonSpinner()
	{
		UpdatePseudoClasses(ButtonSpinnerLocation);
	}

	static ButtonSpinner()
	{
		AllowSpinProperty = AvaloniaProperty.Register<ButtonSpinner, bool>("AllowSpin", defaultValue: true);
		ShowButtonSpinnerProperty = AvaloniaProperty.Register<ButtonSpinner, bool>("ShowButtonSpinner", defaultValue: true);
		ButtonSpinnerLocationProperty = AvaloniaProperty.Register<ButtonSpinner, Location>("ButtonSpinnerLocation", Location.Right);
		AllowSpinProperty.Changed.Subscribe(AllowSpinChanged);
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		IncreaseButton = e.NameScope.Find<Button>("PART_IncreaseButton");
		DecreaseButton = e.NameScope.Find<Button>("PART_DecreaseButton");
		SetButtonUsage();
	}

	protected override void OnPointerReleased(PointerReleasedEventArgs e)
	{
		base.OnPointerReleased(e);
		if (IncreaseButton != null && !IncreaseButton.IsEnabled)
		{
			Point position = e.GetPosition(IncreaseButton);
			if (position.X > 0.0 && position.X < IncreaseButton.Width && position.Y > 0.0 && position.Y < IncreaseButton.Height)
			{
				e.Handled = true;
			}
		}
		if (DecreaseButton != null && !DecreaseButton.IsEnabled)
		{
			Point position = e.GetPosition(DecreaseButton);
			if (position.X > 0.0 && position.X < DecreaseButton.Width && position.Y > 0.0 && position.Y < DecreaseButton.Height)
			{
				e.Handled = true;
			}
		}
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		switch (e.Key)
		{
		case Key.Up:
			if (AllowSpin)
			{
				OnSpin(new SpinEventArgs(Spinner.SpinEvent, SpinDirection.Increase));
				e.Handled = true;
			}
			break;
		case Key.Down:
			if (AllowSpin)
			{
				OnSpin(new SpinEventArgs(Spinner.SpinEvent, SpinDirection.Decrease));
				e.Handled = true;
			}
			break;
		case Key.Return:
			if ((IncreaseButton != null && IncreaseButton.IsFocused) || (DecreaseButton != null && DecreaseButton.IsFocused))
			{
				e.Handled = true;
			}
			break;
		}
	}

	protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
	{
		base.OnPointerWheelChanged(e);
		if (AllowSpin && base.IsKeyboardFocusWithin && e.Delta.Y != 0.0)
		{
			SpinEventArgs e2 = new SpinEventArgs(Spinner.SpinEvent, (e.Delta.Y < 0.0) ? SpinDirection.Decrease : SpinDirection.Increase, usingMouseWheel: true);
			OnSpin(e2);
			e.Handled = true;
		}
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == ButtonSpinnerLocationProperty)
		{
			UpdatePseudoClasses(change.GetNewValue<Location>());
		}
	}

	protected override void OnValidSpinDirectionChanged(ValidSpinDirections oldValue, ValidSpinDirections newValue)
	{
		SetButtonUsage();
	}

	protected virtual void OnAllowSpinChanged(bool oldValue, bool newValue)
	{
		SetButtonUsage();
	}

	private static void AllowSpinChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Sender is ButtonSpinner buttonSpinner)
		{
			bool oldValue = (bool)e.OldValue;
			bool newValue = (bool)e.NewValue;
			buttonSpinner.OnAllowSpinChanged(oldValue, newValue);
		}
	}

	private void SetButtonUsage()
	{
		if (IncreaseButton != null)
		{
			IncreaseButton.IsEnabled = AllowSpin && (base.ValidSpinDirection & ValidSpinDirections.Increase) == ValidSpinDirections.Increase;
		}
		if (DecreaseButton != null)
		{
			DecreaseButton.IsEnabled = AllowSpin && (base.ValidSpinDirection & ValidSpinDirections.Decrease) == ValidSpinDirections.Decrease;
		}
	}

	private void OnButtonClick(object? sender, RoutedEventArgs e)
	{
		if (AllowSpin)
		{
			SpinDirection direction = ((sender != IncreaseButton) ? SpinDirection.Decrease : SpinDirection.Increase);
			OnSpin(new SpinEventArgs(Spinner.SpinEvent, direction));
		}
	}

	private void UpdatePseudoClasses(Location location)
	{
		base.PseudoClasses.Set(":left", location == Location.Left);
		base.PseudoClasses.Set(":right", location == Location.Right);
	}
}
