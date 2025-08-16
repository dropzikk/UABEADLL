using System;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Controls;

[TemplatePart("PART_AcceptButton", typeof(Button))]
[TemplatePart("PART_DismissButton", typeof(Button))]
[TemplatePart("PART_HourDownButton", typeof(RepeatButton))]
[TemplatePart("PART_HourSelector", typeof(DateTimePickerPanel))]
[TemplatePart("PART_HourUpButton", typeof(RepeatButton))]
[TemplatePart("PART_MinuteDownButton", typeof(RepeatButton))]
[TemplatePart("PART_MinuteSelector", typeof(DateTimePickerPanel))]
[TemplatePart("PART_MinuteUpButton", typeof(RepeatButton))]
[TemplatePart("PART_PeriodDownButton", typeof(RepeatButton))]
[TemplatePart("PART_PeriodHost", typeof(Panel))]
[TemplatePart("PART_PeriodSelector", typeof(DateTimePickerPanel))]
[TemplatePart("PART_PeriodUpButton", typeof(RepeatButton))]
[TemplatePart("PART_PickerContainer", typeof(Grid))]
[TemplatePart("PART_SecondSpacer", typeof(Rectangle))]
public class TimePickerPresenter : PickerPresenterBase
{
	public static readonly StyledProperty<int> MinuteIncrementProperty;

	public static readonly StyledProperty<string> ClockIdentifierProperty;

	public static readonly StyledProperty<TimeSpan> TimeProperty;

	private Grid? _pickerContainer;

	private Button? _acceptButton;

	private Button? _dismissButton;

	private Rectangle? _spacer2;

	private Panel? _periodHost;

	private DateTimePickerPanel? _hourSelector;

	private DateTimePickerPanel? _minuteSelector;

	private DateTimePickerPanel? _periodSelector;

	private Button? _hourUpButton;

	private Button? _minuteUpButton;

	private Button? _periodUpButton;

	private Button? _hourDownButton;

	private Button? _minuteDownButton;

	private Button? _periodDownButton;

	public int MinuteIncrement
	{
		get
		{
			return GetValue(MinuteIncrementProperty);
		}
		set
		{
			SetValue(MinuteIncrementProperty, value);
		}
	}

	public string ClockIdentifier
	{
		get
		{
			return GetValue(ClockIdentifierProperty);
		}
		set
		{
			SetValue(ClockIdentifierProperty, value);
		}
	}

	public TimeSpan Time
	{
		get
		{
			return GetValue(TimeProperty);
		}
		set
		{
			SetValue(TimeProperty, value);
		}
	}

	public TimePickerPresenter()
	{
		SetCurrentValue(TimeProperty, DateTime.Now.TimeOfDay);
	}

	static TimePickerPresenter()
	{
		MinuteIncrementProperty = TimePicker.MinuteIncrementProperty.AddOwner<TimePickerPresenter>();
		ClockIdentifierProperty = TimePicker.ClockIdentifierProperty.AddOwner<TimePickerPresenter>();
		TimeProperty = AvaloniaProperty.Register<TimePickerPresenter, TimeSpan>("Time");
		KeyboardNavigation.TabNavigationProperty.OverrideDefaultValue<TimePickerPresenter>(KeyboardNavigationMode.Cycle);
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);
		_pickerContainer = e.NameScope.Get<Grid>("PART_PickerContainer");
		_periodHost = e.NameScope.Get<Panel>("PART_PeriodHost");
		_hourSelector = e.NameScope.Get<DateTimePickerPanel>("PART_HourSelector");
		_minuteSelector = e.NameScope.Get<DateTimePickerPanel>("PART_MinuteSelector");
		_periodSelector = e.NameScope.Get<DateTimePickerPanel>("PART_PeriodSelector");
		_spacer2 = e.NameScope.Get<Rectangle>("PART_SecondSpacer");
		_acceptButton = e.NameScope.Get<Button>("PART_AcceptButton");
		_acceptButton.Click += OnAcceptButtonClicked;
		_hourUpButton = e.NameScope.Find<RepeatButton>("PART_HourUpButton");
		if (_hourUpButton != null)
		{
			_hourUpButton.Click += OnSelectorButtonClick;
		}
		_hourDownButton = e.NameScope.Find<RepeatButton>("PART_HourDownButton");
		if (_hourDownButton != null)
		{
			_hourDownButton.Click += OnSelectorButtonClick;
		}
		_minuteUpButton = e.NameScope.Find<RepeatButton>("PART_MinuteUpButton");
		if (_minuteUpButton != null)
		{
			_minuteUpButton.Click += OnSelectorButtonClick;
		}
		_minuteDownButton = e.NameScope.Find<RepeatButton>("PART_MinuteDownButton");
		if (_minuteDownButton != null)
		{
			_minuteDownButton.Click += OnSelectorButtonClick;
		}
		_periodUpButton = e.NameScope.Find<RepeatButton>("PART_PeriodUpButton");
		if (_periodUpButton != null)
		{
			_periodUpButton.Click += OnSelectorButtonClick;
		}
		_periodDownButton = e.NameScope.Find<RepeatButton>("PART_PeriodDownButton");
		if (_periodDownButton != null)
		{
			_periodDownButton.Click += OnSelectorButtonClick;
		}
		_dismissButton = e.NameScope.Find<Button>("PART_DismissButton");
		if (_dismissButton != null)
		{
			_dismissButton.Click += OnDismissButtonClicked;
		}
		InitPicker();
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == MinuteIncrementProperty || change.Property == ClockIdentifierProperty || change.Property == TimeProperty)
		{
			InitPicker();
		}
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		switch (e.Key)
		{
		case Key.Escape:
			OnDismiss();
			e.Handled = true;
			break;
		case Key.Tab:
		{
			IInputElement inputElement = FocusManager.GetFocusManager(this)?.GetFocusedElement();
			if (inputElement != null)
			{
				KeyboardNavigationHandler.GetNext(inputElement, NavigationDirection.Next)?.Focus(NavigationMethod.Tab);
				e.Handled = true;
			}
			break;
		}
		case Key.Return:
			OnConfirmed();
			e.Handled = true;
			break;
		}
		base.OnKeyDown(e);
	}

	protected override void OnConfirmed()
	{
		int num = _hourSelector.SelectedValue;
		int selectedValue = _minuteSelector.SelectedValue;
		int selectedValue2 = _periodSelector.SelectedValue;
		if (ClockIdentifier == "12HourClock")
		{
			num = ((selectedValue2 != 1) ? ((selectedValue2 != 0 || num != 12) ? num : 0) : ((num == 12) ? 12 : (num + 12)));
		}
		SetCurrentValue(TimeProperty, new TimeSpan(num, selectedValue, 0));
		base.OnConfirmed();
	}

	private void InitPicker()
	{
		if (_pickerContainer != null)
		{
			bool flag = ClockIdentifier == "12HourClock";
			_hourSelector.MaximumValue = (flag ? 12 : 23);
			_hourSelector.MinimumValue = (flag ? 1 : 0);
			_hourSelector.ItemFormat = "%h";
			int hours = Time.Hours;
			_hourSelector.SelectedValue = ((!flag) ? hours : ((hours > 12) ? (hours - 12) : ((hours == 0) ? 12 : hours)));
			_minuteSelector.MaximumValue = 59;
			_minuteSelector.MinimumValue = 0;
			_minuteSelector.Increment = MinuteIncrement;
			_minuteSelector.SelectedValue = Time.Minutes;
			_minuteSelector.ItemFormat = "mm";
			_periodSelector.MaximumValue = 1;
			_periodSelector.MinimumValue = 0;
			_periodSelector.SelectedValue = ((hours >= 12) ? 1 : 0);
			SetGrid();
			_hourSelector?.Focus(NavigationMethod.Pointer);
		}
	}

	private void SetGrid()
	{
		bool flag = ClockIdentifier == "24HourClock";
		string s = (flag ? "*, Auto, *" : "*, Auto, *, Auto, *");
		_pickerContainer.ColumnDefinitions = new ColumnDefinitions(s);
		_spacer2.IsVisible = !flag;
		_periodHost.IsVisible = !flag;
		Grid.SetColumn(_spacer2, (!flag) ? 3 : 0);
		Grid.SetColumn(_periodHost, (!flag) ? 4 : 0);
	}

	private void OnDismissButtonClicked(object? sender, RoutedEventArgs e)
	{
		OnDismiss();
	}

	private void OnAcceptButtonClicked(object? sender, RoutedEventArgs e)
	{
		OnConfirmed();
	}

	private void OnSelectorButtonClick(object? sender, RoutedEventArgs e)
	{
		if (sender == _hourUpButton)
		{
			_hourSelector.ScrollUp();
		}
		else if (sender == _hourDownButton)
		{
			_hourSelector.ScrollDown();
		}
		else if (sender == _minuteUpButton)
		{
			_minuteSelector.ScrollUp();
		}
		else if (sender == _minuteDownButton)
		{
			_minuteSelector.ScrollDown();
		}
		else if (sender == _periodUpButton)
		{
			_periodSelector.ScrollUp();
		}
		else if (sender == _periodDownButton)
		{
			_periodSelector.ScrollDown();
		}
	}

	internal double GetOffsetForPopup()
	{
		if (_hourSelector == null)
		{
			return 0.0;
		}
		double num = ((_acceptButton != null) ? _acceptButton.Bounds.Height : 41.0);
		return (0.0 - (base.MaxHeight - num)) / 2.0 - _hourSelector.ItemHeight / 2.0;
	}
}
