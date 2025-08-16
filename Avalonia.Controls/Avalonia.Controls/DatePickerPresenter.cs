using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Controls;

[TemplatePart("PART_AcceptButton", typeof(Button))]
[TemplatePart("PART_DayDownButton", typeof(RepeatButton))]
[TemplatePart("PART_DayHost", typeof(Panel))]
[TemplatePart("PART_DaySelector", typeof(DateTimePickerPanel))]
[TemplatePart("PART_DayUpButton", typeof(RepeatButton))]
[TemplatePart("PART_DismissButton", typeof(Button))]
[TemplatePart("PART_FirstSpacer", typeof(Rectangle))]
[TemplatePart("PART_MonthDownButton", typeof(RepeatButton))]
[TemplatePart("PART_MonthHost", typeof(Panel))]
[TemplatePart("PART_MonthSelector", typeof(DateTimePickerPanel))]
[TemplatePart("PART_MonthUpButton", typeof(RepeatButton))]
[TemplatePart("PART_PickerContainer", typeof(Grid))]
[TemplatePart("PART_SecondSpacer", typeof(Rectangle))]
[TemplatePart("PART_YearDownButton", typeof(RepeatButton))]
[TemplatePart("PART_YearHost", typeof(Panel))]
[TemplatePart("PART_YearSelector", typeof(DateTimePickerPanel))]
[TemplatePart("PART_YearUpButton", typeof(RepeatButton))]
public class DatePickerPresenter : PickerPresenterBase
{
	public static readonly StyledProperty<DateTimeOffset> DateProperty;

	public static readonly StyledProperty<string> DayFormatProperty;

	public static readonly StyledProperty<bool> DayVisibleProperty;

	public static readonly StyledProperty<DateTimeOffset> MaxYearProperty;

	public static readonly StyledProperty<DateTimeOffset> MinYearProperty;

	public static readonly StyledProperty<string> MonthFormatProperty;

	public static readonly StyledProperty<bool> MonthVisibleProperty;

	public static readonly StyledProperty<string> YearFormatProperty;

	public static readonly StyledProperty<bool> YearVisibleProperty;

	private Grid? _pickerContainer;

	private Button? _acceptButton;

	private Button? _dismissButton;

	private Rectangle? _spacer1;

	private Rectangle? _spacer2;

	private Panel? _monthHost;

	private Panel? _yearHost;

	private Panel? _dayHost;

	private DateTimePickerPanel? _monthSelector;

	private DateTimePickerPanel? _yearSelector;

	private DateTimePickerPanel? _daySelector;

	private Button? _monthUpButton;

	private Button? _dayUpButton;

	private Button? _yearUpButton;

	private Button? _monthDownButton;

	private Button? _dayDownButton;

	private Button? _yearDownButton;

	private DateTimeOffset _syncDate;

	private readonly GregorianCalendar _calendar;

	private bool _suppressUpdateSelection;

	public DateTimeOffset Date
	{
		get
		{
			return GetValue(DateProperty);
		}
		set
		{
			SetValue(DateProperty, value);
		}
	}

	public string DayFormat
	{
		get
		{
			return GetValue(DayFormatProperty);
		}
		set
		{
			SetValue(DayFormatProperty, value);
		}
	}

	public bool DayVisible
	{
		get
		{
			return GetValue(DayVisibleProperty);
		}
		set
		{
			SetValue(DayVisibleProperty, value);
		}
	}

	public DateTimeOffset MaxYear
	{
		get
		{
			return GetValue(MaxYearProperty);
		}
		set
		{
			SetValue(MaxYearProperty, value);
		}
	}

	public DateTimeOffset MinYear
	{
		get
		{
			return GetValue(MinYearProperty);
		}
		set
		{
			SetValue(MinYearProperty, value);
		}
	}

	public string MonthFormat
	{
		get
		{
			return GetValue(MonthFormatProperty);
		}
		set
		{
			SetValue(MonthFormatProperty, value);
		}
	}

	public bool MonthVisible
	{
		get
		{
			return GetValue(MonthVisibleProperty);
		}
		set
		{
			SetValue(MonthVisibleProperty, value);
		}
	}

	public string YearFormat
	{
		get
		{
			return GetValue(YearFormatProperty);
		}
		set
		{
			SetValue(YearFormatProperty, value);
		}
	}

	public bool YearVisible
	{
		get
		{
			return GetValue(YearVisibleProperty);
		}
		set
		{
			SetValue(YearVisibleProperty, value);
		}
	}

	private static DateTimeOffset CoerceDate(AvaloniaObject sender, DateTimeOffset value)
	{
		DateTimeOffset value2 = sender.GetValue(MaxYearProperty);
		if (value > value2)
		{
			return value2;
		}
		DateTimeOffset value3 = sender.GetValue(MinYearProperty);
		if (value < value3)
		{
			return value3;
		}
		return value;
	}

	public DatePickerPresenter()
	{
		DateTimeOffset now = DateTimeOffset.Now;
		SetCurrentValue(MinYearProperty, new DateTimeOffset(now.Year - 100, 1, 1, 0, 0, 0, now.Offset));
		SetCurrentValue(MaxYearProperty, new DateTimeOffset(now.Year + 100, 12, 31, 0, 0, 0, now.Offset));
		SetCurrentValue(DateProperty, now);
		_calendar = new GregorianCalendar();
	}

	static DatePickerPresenter()
	{
		Func<AvaloniaObject, DateTimeOffset, DateTimeOffset> coerce = CoerceDate;
		DateProperty = AvaloniaProperty.Register<DatePickerPresenter, DateTimeOffset>("Date", default(DateTimeOffset), inherits: false, BindingMode.OneWay, null, coerce);
		DayFormatProperty = DatePicker.DayFormatProperty.AddOwner<DatePickerPresenter>();
		DayVisibleProperty = DatePicker.DayVisibleProperty.AddOwner<DatePickerPresenter>();
		MaxYearProperty = DatePicker.MaxYearProperty.AddOwner<DatePickerPresenter>();
		MinYearProperty = DatePicker.MinYearProperty.AddOwner<DatePickerPresenter>();
		MonthFormatProperty = DatePicker.MonthFormatProperty.AddOwner<DatePickerPresenter>();
		MonthVisibleProperty = DatePicker.MonthVisibleProperty.AddOwner<DatePickerPresenter>();
		YearFormatProperty = DatePicker.YearFormatProperty.AddOwner<DatePickerPresenter>();
		YearVisibleProperty = DatePicker.YearVisibleProperty.AddOwner<DatePickerPresenter>();
		KeyboardNavigation.TabNavigationProperty.OverrideDefaultValue<DatePickerPresenter>(KeyboardNavigationMode.Cycle);
	}

	private static void OnDateRangeChanged(DatePickerPresenter sender, AvaloniaPropertyChangedEventArgs e)
	{
		sender.CoerceValue(DateProperty);
	}

	private void OnDateChanged(DateTimeOffset newValue)
	{
		_syncDate = newValue;
		InitPicker();
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);
		_pickerContainer = e.NameScope.Get<Grid>("PART_PickerContainer");
		_monthHost = e.NameScope.Get<Panel>("PART_MonthHost");
		_dayHost = e.NameScope.Get<Panel>("PART_DayHost");
		_yearHost = e.NameScope.Get<Panel>("PART_YearHost");
		_monthSelector = e.NameScope.Get<DateTimePickerPanel>("PART_MonthSelector");
		_monthSelector.SelectionChanged += OnMonthChanged;
		_daySelector = e.NameScope.Get<DateTimePickerPanel>("PART_DaySelector");
		_daySelector.SelectionChanged += OnDayChanged;
		_yearSelector = e.NameScope.Get<DateTimePickerPanel>("PART_YearSelector");
		_yearSelector.SelectionChanged += OnYearChanged;
		_acceptButton = e.NameScope.Get<Button>("PART_AcceptButton");
		_monthUpButton = e.NameScope.Find<RepeatButton>("PART_MonthUpButton");
		if (_monthUpButton != null)
		{
			_monthUpButton.Click += OnSelectorButtonClick;
		}
		_monthDownButton = e.NameScope.Find<RepeatButton>("PART_MonthDownButton");
		if (_monthDownButton != null)
		{
			_monthDownButton.Click += OnSelectorButtonClick;
		}
		_dayUpButton = e.NameScope.Find<RepeatButton>("PART_DayUpButton");
		if (_dayUpButton != null)
		{
			_dayUpButton.Click += OnSelectorButtonClick;
		}
		_dayDownButton = e.NameScope.Find<RepeatButton>("PART_DayDownButton");
		if (_dayDownButton != null)
		{
			_dayDownButton.Click += OnSelectorButtonClick;
		}
		_yearUpButton = e.NameScope.Find<RepeatButton>("PART_YearUpButton");
		if (_yearUpButton != null)
		{
			_yearUpButton.Click += OnSelectorButtonClick;
		}
		_yearDownButton = e.NameScope.Find<RepeatButton>("PART_YearDownButton");
		if (_yearDownButton != null)
		{
			_yearDownButton.Click += OnSelectorButtonClick;
		}
		_dismissButton = e.NameScope.Find<Button>("PART_DismissButton");
		_spacer1 = e.NameScope.Find<Rectangle>("PART_FirstSpacer");
		_spacer2 = e.NameScope.Find<Rectangle>("PART_SecondSpacer");
		if (_acceptButton != null)
		{
			_acceptButton.Click += OnAcceptButtonClicked;
		}
		if (_dismissButton != null)
		{
			_dismissButton.Click += OnDismissButtonClicked;
		}
		InitPicker();
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == DateProperty)
		{
			OnDateChanged(change.GetNewValue<DateTimeOffset>());
		}
		else if (change.Property == MaxYearProperty || change.Property == MinYearProperty)
		{
			OnDateRangeChanged(this, change);
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
			SetCurrentValue(DateProperty, _syncDate);
			OnConfirmed();
			e.Handled = true;
			break;
		}
		base.OnKeyDown(e);
	}

	private void InitPicker()
	{
		if (_pickerContainer != null)
		{
			_suppressUpdateSelection = true;
			_monthSelector.MaximumValue = 12;
			_monthSelector.MinimumValue = 1;
			_monthSelector.ItemFormat = MonthFormat;
			_daySelector.ItemFormat = DayFormat;
			_yearSelector.MaximumValue = MaxYear.Year;
			_yearSelector.MinimumValue = MinYear.Year;
			_yearSelector.ItemFormat = YearFormat;
			SetGrid();
			DateTimeOffset date = Date;
			if (DayVisible)
			{
				_daySelector.FormatDate = date.Date;
				int daysInMonth = _calendar.GetDaysInMonth(date.Year, date.Month);
				_daySelector.MaximumValue = daysInMonth;
				_daySelector.MinimumValue = 1;
				_daySelector.SelectedValue = date.Day;
			}
			if (MonthVisible)
			{
				_monthSelector.SelectedValue = date.Month;
				_monthSelector.FormatDate = date.Date;
			}
			if (YearVisible)
			{
				_yearSelector.SelectedValue = date.Year;
				_yearSelector.FormatDate = date.Date;
			}
			_suppressUpdateSelection = false;
			SetInitialFocus();
		}
	}

	private void SetGrid()
	{
		string shortDatePattern = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
		List<(Panel, int)> list = new List<(Panel, int)>();
		list.Add((_monthHost, MonthVisible ? shortDatePattern.IndexOf("m", StringComparison.OrdinalIgnoreCase) : (-1)));
		list.Add((_yearHost, YearVisible ? shortDatePattern.IndexOf("y", StringComparison.OrdinalIgnoreCase) : (-1)));
		list.Add((_dayHost, DayVisible ? shortDatePattern.IndexOf("d", StringComparison.OrdinalIgnoreCase) : (-1)));
		list.Sort(((Panel?, int) x, (Panel?, int) y) => x.Item2 - y.Item2);
		_pickerContainer.ColumnDefinitions.Clear();
		int num = 0;
		foreach (var item in list)
		{
			if (item.Item1 == null)
			{
				continue;
			}
			item.Item1.IsVisible = item.Item2 != -1;
			if (item.Item2 != -1)
			{
				if (num > 0)
				{
					_pickerContainer.ColumnDefinitions.Add(new ColumnDefinition(0.0, GridUnitType.Auto));
				}
				_pickerContainer.ColumnDefinitions.Add(new ColumnDefinition((item.Item1 == _monthHost) ? 138 : 78, GridUnitType.Star));
				if (item.Item1.Parent == null)
				{
					_pickerContainer.Children.Add(item.Item1);
				}
				Grid.SetColumn(item.Item1, num++ * 2);
			}
		}
		bool flag = num > 1;
		bool flag2 = num > 2;
		Grid.SetColumn(_spacer1, flag ? 1 : 0);
		Grid.SetColumn(_spacer2, flag2 ? 3 : 0);
		_spacer1.IsVisible = flag;
		_spacer2.IsVisible = flag2;
	}

	private void SetInitialFocus()
	{
		int num = (MonthVisible ? Grid.GetColumn(_monthHost) : int.MaxValue);
		int num2 = (DayVisible ? Grid.GetColumn(_dayHost) : int.MaxValue);
		int num3 = (YearVisible ? Grid.GetColumn(_yearHost) : int.MaxValue);
		if (num < num2 && num < num3)
		{
			_monthSelector?.Focus(NavigationMethod.Pointer);
		}
		else if (num2 < num && num2 < num3)
		{
			_monthSelector?.Focus(NavigationMethod.Pointer);
		}
		else if (num3 < num && num3 < num2)
		{
			_yearSelector?.Focus(NavigationMethod.Pointer);
		}
	}

	private void OnDismissButtonClicked(object? sender, RoutedEventArgs e)
	{
		OnDismiss();
	}

	private void OnAcceptButtonClicked(object? sender, RoutedEventArgs e)
	{
		SetCurrentValue(DateProperty, _syncDate);
		OnConfirmed();
	}

	private void OnSelectorButtonClick(object? sender, RoutedEventArgs e)
	{
		if (sender == _monthUpButton)
		{
			_monthSelector.ScrollUp();
		}
		else if (sender == _monthDownButton)
		{
			_monthSelector.ScrollDown();
		}
		else if (sender == _yearUpButton)
		{
			_yearSelector.ScrollUp();
		}
		else if (sender == _yearDownButton)
		{
			_yearSelector.ScrollDown();
		}
		else if (sender == _dayUpButton)
		{
			_daySelector.ScrollUp();
		}
		else if (sender == _dayDownButton)
		{
			_daySelector.ScrollDown();
		}
	}

	private void OnYearChanged(object? sender, EventArgs e)
	{
		if (_suppressUpdateSelection)
		{
			return;
		}
		int daysInMonth = _calendar.GetDaysInMonth(_yearSelector.SelectedValue, _syncDate.Month);
		DateTimeOffset dateTimeOffset = (_syncDate = new DateTimeOffset(_yearSelector.SelectedValue, _syncDate.Month, (_syncDate.Day > daysInMonth) ? daysInMonth : _syncDate.Day, 0, 0, 0, _syncDate.Offset));
		if (DayVisible && _syncDate.Month == 2)
		{
			_suppressUpdateSelection = true;
			_daySelector.FormatDate = dateTimeOffset.Date;
			if (_daySelector.MaximumValue != daysInMonth)
			{
				_daySelector.MaximumValue = daysInMonth;
			}
			else
			{
				_daySelector.RefreshItems();
			}
			_suppressUpdateSelection = false;
		}
	}

	private void OnDayChanged(object? sender, EventArgs e)
	{
		if (!_suppressUpdateSelection)
		{
			_syncDate = new DateTimeOffset(_syncDate.Year, _syncDate.Month, _daySelector.SelectedValue, 0, 0, 0, _syncDate.Offset);
		}
	}

	private void OnMonthChanged(object? sender, EventArgs e)
	{
		if (_suppressUpdateSelection)
		{
			return;
		}
		int daysInMonth = _calendar.GetDaysInMonth(_syncDate.Year, _monthSelector.SelectedValue);
		DateTimeOffset syncDate = new DateTimeOffset(_syncDate.Year, _monthSelector.SelectedValue, (_syncDate.Day > daysInMonth) ? daysInMonth : _syncDate.Day, 0, 0, 0, _syncDate.Offset);
		if (!DayVisible)
		{
			_syncDate = syncDate;
			return;
		}
		_suppressUpdateSelection = true;
		_daySelector.FormatDate = syncDate.Date;
		_syncDate = syncDate;
		if (_daySelector.MaximumValue != daysInMonth)
		{
			_daySelector.MaximumValue = daysInMonth;
		}
		else
		{
			_daySelector.RefreshItems();
		}
		_suppressUpdateSelection = false;
	}

	internal double GetOffsetForPopup()
	{
		if (_monthSelector == null)
		{
			return 0.0;
		}
		double num = ((_acceptButton != null) ? _acceptButton.Bounds.Height : 41.0);
		return (0.0 - (base.MaxHeight - num)) / 2.0 - _monthSelector.ItemHeight / 2.0;
	}
}
