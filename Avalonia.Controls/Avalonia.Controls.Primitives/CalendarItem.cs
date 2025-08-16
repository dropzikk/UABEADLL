using System;
using System.Globalization;
using Avalonia.Collections.Pooled;
using Avalonia.Controls.Metadata;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace Avalonia.Controls.Primitives;

[TemplatePart("PART_HeaderButton", typeof(Button))]
[TemplatePart("PART_MonthView", typeof(Grid))]
[TemplatePart("PART_NextButton", typeof(Button))]
[TemplatePart("PART_PreviousButton", typeof(Button))]
[TemplatePart("PART_YearView", typeof(Grid))]
[PseudoClasses(new string[] { ":calendardisabled" })]
public sealed class CalendarItem : TemplatedControl
{
	private const int NumberOfDaysPerWeek = 7;

	private const string PART_ElementHeaderButton = "PART_HeaderButton";

	private const string PART_ElementPreviousButton = "PART_PreviousButton";

	private const string PART_ElementNextButton = "PART_NextButton";

	private const string PART_ElementMonthView = "PART_MonthView";

	private const string PART_ElementYearView = "PART_YearView";

	private Button? _headerButton;

	private Button? _nextButton;

	private Button? _previousButton;

	private DateTime _currentMonth;

	private bool _isMouseLeftButtonDown;

	private bool _isMouseLeftButtonDownYearView;

	private bool _isControlPressed;

	private readonly System.Globalization.Calendar _calendar = new GregorianCalendar();

	public static readonly StyledProperty<IBrush?> HeaderBackgroundProperty = Calendar.HeaderBackgroundProperty.AddOwner<CalendarItem>();

	public static readonly StyledProperty<ITemplate<Control>?> DayTitleTemplateProperty = AvaloniaProperty.Register<CalendarItem, ITemplate<Control>>("DayTitleTemplate", null, inherits: false, BindingMode.OneTime);

	internal Calendar? Owner { get; set; }

	internal CalendarDayButton? CurrentButton { get; set; }

	public IBrush? HeaderBackground
	{
		get
		{
			return GetValue(HeaderBackgroundProperty);
		}
		set
		{
			SetValue(HeaderBackgroundProperty, value);
		}
	}

	public ITemplate<Control>? DayTitleTemplate
	{
		get
		{
			return GetValue(DayTitleTemplateProperty);
		}
		set
		{
			SetValue(DayTitleTemplateProperty, value);
		}
	}

	internal Button? HeaderButton
	{
		get
		{
			return _headerButton;
		}
		private set
		{
			if (_headerButton != null)
			{
				_headerButton.Click -= HeaderButton_Click;
			}
			_headerButton = value;
			if (_headerButton != null)
			{
				_headerButton.Click += HeaderButton_Click;
				_headerButton.Focusable = false;
			}
		}
	}

	internal Button? NextButton
	{
		get
		{
			return _nextButton;
		}
		private set
		{
			if (_nextButton != null)
			{
				_nextButton.Click -= NextButton_Click;
			}
			_nextButton = value;
			if (_nextButton != null)
			{
				if (_nextButton.Content == null)
				{
					_nextButton.Content = "next button";
				}
				_nextButton.IsVisible = true;
				_nextButton.Click += NextButton_Click;
				_nextButton.Focusable = false;
			}
		}
	}

	internal Button? PreviousButton
	{
		get
		{
			return _previousButton;
		}
		private set
		{
			if (_previousButton != null)
			{
				_previousButton.Click -= PreviousButton_Click;
			}
			_previousButton = value;
			if (_previousButton != null)
			{
				if (_previousButton.Content == null)
				{
					_previousButton.Content = "previous button";
				}
				_previousButton.IsVisible = true;
				_previousButton.Click += PreviousButton_Click;
				_previousButton.Focusable = false;
			}
		}
	}

	internal Grid? MonthView { get; set; }

	internal Grid? YearView { get; set; }

	private void PopulateGrids()
	{
		if (MonthView != null)
		{
			using PooledList<Control> pooledList = new PooledList<Control>(56);
			for (int i = 0; i < 7; i++)
			{
				Control control = DayTitleTemplate?.Build();
				if (control != null)
				{
					control.DataContext = string.Empty;
					control.SetValue(Grid.RowProperty, 0);
					control.SetValue(Grid.ColumnProperty, i);
					pooledList.Add(control);
				}
			}
			EventHandler<PointerPressedEventArgs> value = Cell_MouseLeftButtonDown;
			EventHandler<PointerReleasedEventArgs> value2 = Cell_MouseLeftButtonUp;
			EventHandler<PointerEventArgs> value3 = Cell_MouseEntered;
			EventHandler<RoutedEventArgs> value4 = Cell_Click;
			for (int j = 1; j < 7; j++)
			{
				for (int k = 0; k < 7; k++)
				{
					CalendarDayButton calendarDayButton = new CalendarDayButton();
					if (Owner != null)
					{
						calendarDayButton.Owner = Owner;
					}
					calendarDayButton.SetValue(Grid.RowProperty, j);
					calendarDayButton.SetValue(Grid.ColumnProperty, k);
					calendarDayButton.CalendarDayButtonMouseDown += value;
					calendarDayButton.CalendarDayButtonMouseUp += value2;
					calendarDayButton.PointerEntered += value3;
					calendarDayButton.Click += value4;
					pooledList.Add(calendarDayButton);
				}
			}
			MonthView.Children.AddRange(pooledList);
		}
		if (YearView == null)
		{
			return;
		}
		using PooledList<Control> pooledList2 = new PooledList<Control>(12);
		EventHandler<PointerPressedEventArgs> value5 = Month_CalendarButtonMouseDown;
		EventHandler<PointerReleasedEventArgs> value6 = Month_CalendarButtonMouseUp;
		EventHandler<PointerEventArgs> value7 = Month_MouseEntered;
		for (int l = 0; l < 3; l++)
		{
			for (int m = 0; m < 4; m++)
			{
				CalendarButton calendarButton = new CalendarButton();
				if (Owner != null)
				{
					calendarButton.Owner = Owner;
				}
				calendarButton.SetValue(Grid.RowProperty, l);
				calendarButton.SetValue(Grid.ColumnProperty, m);
				calendarButton.CalendarLeftMouseButtonDown += value5;
				calendarButton.CalendarLeftMouseButtonUp += value6;
				calendarButton.PointerEntered += value7;
				pooledList2.Add(calendarButton);
			}
		}
		YearView.Children.AddRange(pooledList2);
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		HeaderButton = e.NameScope.Find<Button>("PART_HeaderButton");
		PreviousButton = e.NameScope.Find<Button>("PART_PreviousButton");
		NextButton = e.NameScope.Find<Button>("PART_NextButton");
		MonthView = e.NameScope.Find<Grid>("PART_MonthView");
		YearView = e.NameScope.Find<Grid>("PART_YearView");
		if (Owner != null)
		{
			UpdateDisabled(Owner.IsEnabled);
		}
		PopulateGrids();
		if (MonthView == null || YearView == null)
		{
			return;
		}
		if (Owner != null)
		{
			Owner.SelectedMonth = Owner.DisplayDateInternal;
			Owner.SelectedYear = Owner.DisplayDateInternal;
			if (Owner.DisplayMode == CalendarMode.Year)
			{
				UpdateYearMode();
			}
			else if (Owner.DisplayMode == CalendarMode.Decade)
			{
				UpdateDecadeMode();
			}
			if (Owner.DisplayMode == CalendarMode.Month)
			{
				UpdateMonthMode();
				MonthView.IsVisible = true;
				YearView.IsVisible = false;
			}
			else
			{
				YearView.IsVisible = true;
				MonthView.IsVisible = false;
			}
		}
		else
		{
			UpdateMonthMode();
			MonthView.IsVisible = true;
			YearView.IsVisible = false;
		}
	}

	private void SetDayTitles()
	{
		for (int i = 0; i < 7; i++)
		{
			Control control = MonthView.Children[i];
			if (Owner != null)
			{
				control.DataContext = DateTimeHelper.GetCurrentDateFormat().ShortestDayNames[(int)(i + Owner.FirstDayOfWeek) % 7];
			}
			else
			{
				control.DataContext = DateTimeHelper.GetCurrentDateFormat().ShortestDayNames[(int)(i + DateTimeHelper.GetCurrentDateFormat().FirstDayOfWeek) % 7];
			}
		}
	}

	private int PreviousMonthDays(DateTime firstOfMonth)
	{
		DayOfWeek dayOfWeek = _calendar.GetDayOfWeek(firstOfMonth);
		int num = ((Owner == null) ? ((dayOfWeek - DateTimeHelper.GetCurrentDateFormat().FirstDayOfWeek + 7) % 7) : ((dayOfWeek - Owner.FirstDayOfWeek + 7) % 7));
		if (num == 0)
		{
			return 7;
		}
		return num;
	}

	internal void UpdateMonthMode()
	{
		if (Owner != null)
		{
			_currentMonth = Owner.DisplayDateInternal;
		}
		else
		{
			_currentMonth = DateTime.Today;
		}
		SetMonthModeHeaderButton();
		SetMonthModePreviousButton(_currentMonth);
		SetMonthModeNextButton(_currentMonth);
		if (MonthView != null)
		{
			SetDayTitles();
			SetCalendarDayButtons(_currentMonth);
		}
	}

	private void SetMonthModeHeaderButton()
	{
		if (HeaderButton != null)
		{
			if (Owner != null)
			{
				HeaderButton.Content = Owner.DisplayDateInternal.ToString("Y", DateTimeHelper.GetCurrentDateFormat());
				HeaderButton.IsEnabled = true;
			}
			else
			{
				HeaderButton.Content = DateTime.Today.ToString("Y", DateTimeHelper.GetCurrentDateFormat());
			}
		}
	}

	private void SetMonthModeNextButton(DateTime firstDayOfMonth)
	{
		if (Owner != null && NextButton != null)
		{
			if (DateTimeHelper.CompareYearMonth(firstDayOfMonth, DateTime.MaxValue) == 0)
			{
				NextButton.IsEnabled = false;
				return;
			}
			DateTime dt = _calendar.AddMonths(firstDayOfMonth, 1);
			NextButton.IsEnabled = DateTimeHelper.CompareDays(Owner.DisplayDateRangeEnd, dt) > -1;
		}
	}

	private void SetMonthModePreviousButton(DateTime firstDayOfMonth)
	{
		if (Owner != null && PreviousButton != null)
		{
			PreviousButton.IsEnabled = DateTimeHelper.CompareDays(Owner.DisplayDateRangeStart, firstDayOfMonth) < 0;
		}
	}

	private void SetButtonState(CalendarDayButton childButton, DateTime dateToAdd)
	{
		if (Owner == null)
		{
			return;
		}
		childButton.Opacity = 1.0;
		if (DateTimeHelper.CompareDays(dateToAdd, Owner.DisplayDateRangeStart) < 0 || DateTimeHelper.CompareDays(dateToAdd, Owner.DisplayDateRangeEnd) > 0)
		{
			childButton.IsEnabled = false;
			childButton.IsToday = false;
			childButton.IsSelected = false;
			childButton.Opacity = 0.0;
			return;
		}
		if (Owner.BlackoutDates.Contains(dateToAdd))
		{
			childButton.IsBlackout = true;
		}
		else
		{
			childButton.IsBlackout = false;
		}
		childButton.IsEnabled = true;
		childButton.IsInactive = DateTimeHelper.CompareYearMonth(dateToAdd, Owner.DisplayDateInternal) != 0;
		childButton.IsToday = Owner.IsTodayHighlighted && dateToAdd == DateTime.Today;
		childButton.IsSelected = false;
		foreach (DateTime selectedDate in Owner.SelectedDates)
		{
			childButton.IsSelected |= DateTimeHelper.CompareDays(dateToAdd, selectedDate) == 0;
		}
		if (!Owner.LastSelectedDate.HasValue)
		{
			return;
		}
		if (DateTimeHelper.CompareDays(Owner.LastSelectedDate.Value, dateToAdd) == 0)
		{
			if (Owner.FocusButton != null)
			{
				Owner.FocusButton.IsCurrent = false;
			}
			Owner.FocusButton = childButton;
			if (Owner.HasFocusInternal)
			{
				Owner.FocusButton.IsCurrent = true;
			}
		}
		else
		{
			childButton.IsCurrent = false;
		}
	}

	private void SetCalendarDayButtons(DateTime firstDayOfMonth)
	{
		int num = PreviousMonthDays(firstDayOfMonth);
		DateTime dateTime = ((DateTimeHelper.CompareYearMonth(firstDayOfMonth, DateTime.MinValue) <= 0) ? firstDayOfMonth : _calendar.AddDays(firstDayOfMonth, -num));
		if (Owner != null && Owner.HoverEnd.HasValue && Owner.HoverStart.HasValue)
		{
			Owner.HoverEndIndex = null;
			Owner.HoverStartIndex = null;
		}
		int num2 = 49;
		for (int i = 7; i < num2; i++)
		{
			CalendarDayButton calendarDayButton = (CalendarDayButton)MonthView.Children[i];
			calendarDayButton.Index = i;
			SetButtonState(calendarDayButton, dateTime);
			if (Owner != null && Owner.HoverEnd.HasValue && Owner.HoverStart.HasValue)
			{
				if (DateTimeHelper.CompareDays(dateTime, Owner.HoverEnd.Value) == 0)
				{
					Owner.HoverEndIndex = i;
				}
				if (DateTimeHelper.CompareDays(dateTime, Owner.HoverStart.Value) == 0)
				{
					Owner.HoverStartIndex = i;
				}
			}
			calendarDayButton.Content = dateTime.Day.ToString(DateTimeHelper.GetCurrentDateFormat());
			calendarDayButton.DataContext = dateTime;
			if (DateTime.Compare(DateTimeHelper.DiscardTime(DateTime.MaxValue), dateTime) > 0)
			{
				dateTime = _calendar.AddDays(dateTime, 1);
				continue;
			}
			i++;
			for (int j = i; j < num2; j++)
			{
				calendarDayButton = (CalendarDayButton)MonthView.Children[j];
				calendarDayButton.Content = j.ToString(DateTimeHelper.GetCurrentDateFormat());
				calendarDayButton.IsEnabled = false;
				calendarDayButton.Opacity = 0.0;
			}
			return;
		}
		if (Owner == null || !Owner.HoverStart.HasValue || !Owner.HoverEndInternal.HasValue)
		{
			return;
		}
		if (!Owner.HoverEndIndex.HasValue)
		{
			if (DateTimeHelper.CompareDays(Owner.HoverEndInternal.Value, Owner.HoverStart.Value) > 0)
			{
				Owner.HoverEndIndex = 48;
			}
			else
			{
				Owner.HoverEndIndex = 7;
			}
		}
		if (!Owner.HoverStartIndex.HasValue)
		{
			if (DateTimeHelper.CompareDays(Owner.HoverEndInternal.Value, Owner.HoverStart.Value) > 0)
			{
				Owner.HoverStartIndex = 7;
			}
			else
			{
				Owner.HoverStartIndex = 48;
			}
		}
	}

	internal void UpdateYearMode()
	{
		if (Owner != null)
		{
			_currentMonth = Owner.SelectedMonth;
		}
		else
		{
			_currentMonth = DateTime.Today;
		}
		SetYearModeHeaderButton();
		SetYearModePreviousButton();
		SetYearModeNextButton();
		if (YearView != null)
		{
			SetMonthButtonsForYearMode();
		}
	}

	private void SetYearModeHeaderButton()
	{
		if (HeaderButton != null)
		{
			HeaderButton.IsEnabled = true;
			HeaderButton.Content = _currentMonth.Year.ToString(DateTimeHelper.GetCurrentDateFormat());
		}
	}

	private void SetYearModePreviousButton()
	{
		if (Owner != null && PreviousButton != null)
		{
			PreviousButton.IsEnabled = Owner.DisplayDateRangeStart.Year != _currentMonth.Year;
		}
	}

	private void SetYearModeNextButton()
	{
		if (Owner != null && NextButton != null)
		{
			NextButton.IsEnabled = Owner.DisplayDateRangeEnd.Year != _currentMonth.Year;
		}
	}

	private void SetMonthButtonsForYearMode()
	{
		int num = 0;
		foreach (CalendarButton child in YearView.Children)
		{
			DateTime dateTime = new DateTime(_currentMonth.Year, num + 1, 1);
			child.DataContext = dateTime;
			child.Content = DateTimeHelper.GetCurrentDateFormat().AbbreviatedMonthNames[num];
			child.IsVisible = true;
			if (Owner != null)
			{
				if (dateTime.Year == _currentMonth.Year && dateTime.Month == _currentMonth.Month && dateTime.Day == _currentMonth.Day)
				{
					Owner.FocusCalendarButton = child;
					child.IsCalendarButtonFocused = Owner.HasFocusInternal;
				}
				else
				{
					child.IsCalendarButtonFocused = false;
				}
				child.IsSelected = DateTimeHelper.CompareYearMonth(dateTime, Owner.DisplayDateInternal) == 0;
				if (DateTimeHelper.CompareYearMonth(dateTime, Owner.DisplayDateRangeStart) < 0 || DateTimeHelper.CompareYearMonth(dateTime, Owner.DisplayDateRangeEnd) > 0)
				{
					child.IsEnabled = false;
					child.Opacity = 0.0;
				}
				else
				{
					child.IsEnabled = true;
					child.Opacity = 1.0;
				}
			}
			child.IsInactive = false;
			num++;
		}
	}

	internal void UpdateDecadeMode()
	{
		DateTime date;
		if (Owner != null)
		{
			date = Owner.SelectedYear;
			_currentMonth = Owner.SelectedMonth;
		}
		else
		{
			_currentMonth = DateTime.Today;
			date = DateTime.Today;
		}
		int num = DateTimeHelper.DecadeOfDate(date);
		int num2 = DateTimeHelper.EndOfDecade(date);
		SetDecadeModeHeaderButton(num, num2);
		SetDecadeModePreviousButton(num);
		SetDecadeModeNextButton(num2);
		if (YearView != null)
		{
			SetYearButtons(num, num2);
		}
	}

	internal void UpdateYearViewSelection(CalendarButton? calendarButton)
	{
		if (Owner != null && calendarButton?.DataContext is DateTime dateTime)
		{
			Owner.FocusCalendarButton.IsCalendarButtonFocused = false;
			Owner.FocusCalendarButton = calendarButton;
			calendarButton.IsCalendarButtonFocused = Owner.HasFocusInternal;
			if (Owner.DisplayMode == CalendarMode.Year)
			{
				Owner.SelectedMonth = dateTime;
			}
			else
			{
				Owner.SelectedYear = dateTime;
			}
		}
	}

	private void SetYearButtons(int decade, int decadeEnd)
	{
		int num = -1;
		foreach (CalendarButton child in YearView.Children)
		{
			int num2 = decade + num;
			if (num2 <= DateTime.MaxValue.Year && num2 >= DateTime.MinValue.Year)
			{
				DateTime dateTime = new DateTime(num2, 1, 1);
				child.DataContext = dateTime;
				child.Content = num2.ToString(DateTimeHelper.GetCurrentDateFormat());
				child.IsVisible = true;
				if (Owner != null)
				{
					if (num2 == Owner.SelectedYear.Year)
					{
						Owner.FocusCalendarButton = child;
						child.IsCalendarButtonFocused = Owner.HasFocusInternal;
					}
					else
					{
						child.IsCalendarButtonFocused = false;
					}
					child.IsSelected = Owner.DisplayDate.Year == num2;
					if (num2 < Owner.DisplayDateRangeStart.Year || num2 > Owner.DisplayDateRangeEnd.Year)
					{
						child.IsEnabled = false;
						child.Opacity = 0.0;
					}
					else
					{
						child.IsEnabled = true;
						child.Opacity = 1.0;
					}
				}
				child.IsInactive = num2 < decade || num2 > decadeEnd;
			}
			else
			{
				child.IsEnabled = false;
				child.Opacity = 0.0;
			}
			num++;
		}
	}

	private void SetDecadeModeHeaderButton(int decade, int decadeEnd)
	{
		if (HeaderButton != null)
		{
			HeaderButton.Content = decade.ToString(CultureInfo.CurrentCulture) + "-" + decadeEnd.ToString(CultureInfo.CurrentCulture);
			HeaderButton.IsEnabled = false;
		}
	}

	private void SetDecadeModeNextButton(int decadeEnd)
	{
		if (Owner != null && NextButton != null)
		{
			NextButton.IsEnabled = Owner.DisplayDateRangeEnd.Year > decadeEnd;
		}
	}

	private void SetDecadeModePreviousButton(int decade)
	{
		if (Owner != null && PreviousButton != null)
		{
			PreviousButton.IsEnabled = decade > Owner.DisplayDateRangeStart.Year;
		}
	}

	internal void HeaderButton_Click(object? sender, RoutedEventArgs e)
	{
		if (Owner == null)
		{
			return;
		}
		if (!Owner.HasFocusInternal)
		{
			Owner.Focus();
		}
		if (((Button)sender).IsEnabled)
		{
			if (Owner.DisplayMode == CalendarMode.Month)
			{
				DateTime displayDateInternal = Owner.DisplayDateInternal;
				Owner.SelectedMonth = new DateTime(displayDateInternal.Year, displayDateInternal.Month, 1);
				Owner.DisplayMode = CalendarMode.Year;
			}
			else
			{
				DateTime displayDateInternal = Owner.SelectedMonth;
				Owner.SelectedYear = new DateTime(displayDateInternal.Year, displayDateInternal.Month, 1);
				Owner.DisplayMode = CalendarMode.Decade;
			}
		}
	}

	internal void PreviousButton_Click(object? sender, RoutedEventArgs e)
	{
		if (Owner != null)
		{
			if (!Owner.HasFocusInternal)
			{
				Owner.Focus();
			}
			if (((Button)sender).IsEnabled)
			{
				Owner.OnPreviousClick();
			}
		}
	}

	internal void NextButton_Click(object? sender, RoutedEventArgs e)
	{
		if (Owner != null)
		{
			if (!Owner.HasFocusInternal)
			{
				Owner.Focus();
			}
			if (((Button)sender).IsEnabled)
			{
				Owner.OnNextClick();
			}
		}
	}

	internal void Cell_MouseEntered(object? sender, PointerEventArgs e)
	{
		if (Owner == null || !_isMouseLeftButtonDown || !(sender is CalendarDayButton { IsEnabled: not false, IsBlackout: false } calendarDayButton) || !(calendarDayButton.DataContext is DateTime dateTime))
		{
			return;
		}
		switch (Owner.SelectionMode)
		{
		case CalendarSelectionMode.SingleDate:
			Owner.CalendarDatePickerDisplayDateFlag = true;
			if (Owner.SelectedDates.Count == 0)
			{
				Owner.SelectedDates.Add(dateTime);
			}
			else
			{
				Owner.SelectedDates[0] = dateTime;
			}
			break;
		case CalendarSelectionMode.SingleRange:
		case CalendarSelectionMode.MultipleRange:
			Owner.UnHighlightDays();
			Owner.HoverEndIndex = calendarDayButton.Index;
			Owner.HoverEnd = dateTime;
			Owner.HighlightDays();
			break;
		}
	}

	internal void Cell_MouseLeftButtonDown(object? sender, PointerPressedEventArgs e)
	{
		if (Owner == null)
		{
			return;
		}
		if (!Owner.HasFocusInternal)
		{
			Owner.Focus();
		}
		CalendarExtensions.GetMetaKeyState(e.KeyModifiers, out var ctrl, out var shift);
		if (sender is CalendarDayButton calendarDayButton)
		{
			_isControlPressed = ctrl;
			if (calendarDayButton.IsEnabled && !calendarDayButton.IsBlackout && calendarDayButton.DataContext is DateTime dateTime)
			{
				_isMouseLeftButtonDown = true;
				switch (Owner.SelectionMode)
				{
				case CalendarSelectionMode.None:
					break;
				case CalendarSelectionMode.SingleDate:
					Owner.CalendarDatePickerDisplayDateFlag = true;
					if (Owner.SelectedDates.Count == 0)
					{
						Owner.SelectedDates.Add(dateTime);
					}
					else
					{
						Owner.SelectedDates[0] = dateTime;
					}
					break;
				case CalendarSelectionMode.SingleRange:
					if (shift)
					{
						Owner.UnHighlightDays();
						Owner.HoverEnd = dateTime;
						Owner.HoverEndIndex = calendarDayButton.Index;
						Owner.HighlightDays();
					}
					else
					{
						Owner.UnHighlightDays();
						Owner.HoverStart = dateTime;
						Owner.HoverStartIndex = calendarDayButton.Index;
					}
					break;
				case CalendarSelectionMode.MultipleRange:
					if (shift)
					{
						if (!ctrl)
						{
							foreach (DateTime selectedDate in Owner.SelectedDates)
							{
								Owner.RemovedItems.Add(selectedDate);
							}
							Owner.SelectedDates.ClearInternal();
						}
						Owner.HoverEnd = dateTime;
						Owner.HoverEndIndex = calendarDayButton.Index;
						Owner.HighlightDays();
						break;
					}
					if (!ctrl)
					{
						foreach (DateTime selectedDate2 in Owner.SelectedDates)
						{
							Owner.RemovedItems.Add(selectedDate2);
						}
						Owner.SelectedDates.ClearInternal();
						Owner.UnHighlightDays();
					}
					Owner.HoverStart = dateTime;
					Owner.HoverStartIndex = calendarDayButton.Index;
					break;
				}
			}
			else
			{
				Owner.HoverStart = null;
			}
		}
		else
		{
			_isControlPressed = false;
		}
	}

	private void AddSelection(CalendarDayButton b, DateTime selectedDate)
	{
		if (Owner != null)
		{
			Owner.HoverEndIndex = b.Index;
			Owner.HoverEnd = selectedDate;
			if (Owner.HoverEnd.HasValue && Owner.HoverStart.HasValue)
			{
				Owner.IsMouseSelection = true;
				Owner.SelectedDates.AddRange(Owner.HoverStart.Value, Owner.HoverEnd.Value);
				Owner.OnDayClick(selectedDate);
			}
		}
	}

	internal void Cell_MouseLeftButtonUp(object? sender, PointerReleasedEventArgs e)
	{
		if (Owner == null)
		{
			return;
		}
		CalendarDayButton calendarDayButton = sender as CalendarDayButton;
		if (calendarDayButton != null && !calendarDayButton.IsBlackout)
		{
			Owner.OnDayButtonMouseUp(e);
		}
		_isMouseLeftButtonDown = false;
		if (calendarDayButton == null || !(calendarDayButton.DataContext is DateTime selectedDate))
		{
			return;
		}
		if (Owner.SelectionMode == CalendarSelectionMode.None || Owner.SelectionMode == CalendarSelectionMode.SingleDate)
		{
			Owner.OnDayClick(selectedDate);
		}
		else if (Owner.HoverStart.HasValue)
		{
			switch (Owner.SelectionMode)
			{
			case CalendarSelectionMode.SingleRange:
				foreach (DateTime selectedDate2 in Owner.SelectedDates)
				{
					Owner.RemovedItems.Add(selectedDate2);
				}
				Owner.SelectedDates.ClearInternal();
				AddSelection(calendarDayButton, selectedDate);
				break;
			case CalendarSelectionMode.MultipleRange:
				AddSelection(calendarDayButton, selectedDate);
				break;
			}
		}
		else if (calendarDayButton.IsInactive && calendarDayButton.IsBlackout)
		{
			Owner.OnDayClick(selectedDate);
		}
	}

	private void Cell_Click(object? sender, RoutedEventArgs e)
	{
		if (Owner != null && _isControlPressed && Owner.SelectionMode == CalendarSelectionMode.MultipleRange)
		{
			CalendarDayButton calendarDayButton = (CalendarDayButton)sender;
			if (calendarDayButton.IsSelected)
			{
				Owner.HoverStart = null;
				_isMouseLeftButtonDown = false;
				calendarDayButton.IsSelected = false;
				if (calendarDayButton.DataContext is DateTime item)
				{
					Owner.SelectedDates.Remove(item);
				}
			}
		}
		_isControlPressed = false;
	}

	private void Month_CalendarButtonMouseDown(object? sender, PointerPressedEventArgs e)
	{
		_isMouseLeftButtonDownYearView = true;
		UpdateYearViewSelection(sender as CalendarButton);
	}

	internal void Month_CalendarButtonMouseUp(object? sender, PointerReleasedEventArgs e)
	{
		_isMouseLeftButtonDownYearView = false;
		if (Owner != null && (sender as CalendarButton)?.DataContext is DateTime dateTime)
		{
			if (Owner.DisplayMode == CalendarMode.Year)
			{
				Owner.DisplayDate = dateTime;
				Owner.DisplayMode = CalendarMode.Month;
			}
			else
			{
				Owner.SelectedMonth = dateTime;
				Owner.DisplayMode = CalendarMode.Year;
			}
		}
	}

	private void Month_MouseEntered(object? sender, PointerEventArgs e)
	{
		if (_isMouseLeftButtonDownYearView)
		{
			UpdateYearViewSelection(sender as CalendarButton);
		}
	}

	internal void UpdateDisabled(bool isEnabled)
	{
		base.PseudoClasses.Set(":calendardisabled", !isEnabled);
	}
}
