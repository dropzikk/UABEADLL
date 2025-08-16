using System;
using System.Collections.ObjectModel;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace Avalonia.Controls;

[TemplatePart("PART_CalendarItem", typeof(CalendarItem))]
[TemplatePart("PART_Root", typeof(Panel))]
public class Calendar : TemplatedControl
{
	internal const int RowsPerMonth = 7;

	internal const int ColumnsPerMonth = 7;

	internal const int RowsPerYear = 3;

	internal const int ColumnsPerYear = 4;

	private DateTime _selectedMonth;

	private DateTime _selectedYear;

	private bool _isShiftPressed;

	private bool _displayDateIsChanging;

	public static readonly StyledProperty<DayOfWeek> FirstDayOfWeekProperty;

	public static readonly StyledProperty<bool> IsTodayHighlightedProperty;

	public static readonly StyledProperty<IBrush?> HeaderBackgroundProperty;

	public static readonly StyledProperty<CalendarMode> DisplayModeProperty;

	public static readonly StyledProperty<CalendarSelectionMode> SelectionModeProperty;

	public static readonly StyledProperty<DateTime?> SelectedDateProperty;

	public static readonly StyledProperty<DateTime> DisplayDateProperty;

	public static readonly StyledProperty<DateTime?> DisplayDateStartProperty;

	public static readonly StyledProperty<DateTime?> DisplayDateEndProperty;

	private const string PART_ElementRoot = "PART_Root";

	private const string PART_ElementMonth = "PART_CalendarItem";

	internal CalendarDayButton? FocusButton { get; set; }

	internal CalendarButton? FocusCalendarButton { get; set; }

	internal Panel? Root { get; set; }

	internal CalendarItem? MonthControl
	{
		get
		{
			if (Root != null && Root.Children.Count > 0)
			{
				return Root.Children[0] as CalendarItem;
			}
			return null;
		}
	}

	public DayOfWeek FirstDayOfWeek
	{
		get
		{
			return GetValue(FirstDayOfWeekProperty);
		}
		set
		{
			SetValue(FirstDayOfWeekProperty, value);
		}
	}

	public bool IsTodayHighlighted
	{
		get
		{
			return GetValue(IsTodayHighlightedProperty);
		}
		set
		{
			SetValue(IsTodayHighlightedProperty, value);
		}
	}

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

	public CalendarMode DisplayMode
	{
		get
		{
			return GetValue(DisplayModeProperty);
		}
		set
		{
			SetValue(DisplayModeProperty, value);
		}
	}

	public CalendarSelectionMode SelectionMode
	{
		get
		{
			return GetValue(SelectionModeProperty);
		}
		set
		{
			SetValue(SelectionModeProperty, value);
		}
	}

	public DateTime? SelectedDate
	{
		get
		{
			return GetValue(SelectedDateProperty);
		}
		set
		{
			SetValue(SelectedDateProperty, value);
		}
	}

	public SelectedDatesCollection SelectedDates { get; private set; }

	internal Collection<DateTime> RemovedItems { get; set; }

	internal DateTime? LastSelectedDateInternal { get; set; }

	internal DateTime? LastSelectedDate
	{
		get
		{
			return LastSelectedDateInternal;
		}
		set
		{
			LastSelectedDateInternal = value;
			if (SelectionMode == CalendarSelectionMode.None)
			{
				if (FocusButton != null)
				{
					FocusButton.IsCurrent = false;
				}
				FocusButton = FindDayButtonFromDay(LastSelectedDate.Value);
				if (FocusButton != null)
				{
					FocusButton.IsCurrent = HasFocusInternal;
				}
			}
		}
	}

	internal DateTime SelectedMonth
	{
		get
		{
			return _selectedMonth;
		}
		set
		{
			int num = DateTimeHelper.CompareYearMonth(value, DisplayDateRangeStart);
			int num2 = DateTimeHelper.CompareYearMonth(value, DisplayDateRangeEnd);
			if (num >= 0 && num2 <= 0)
			{
				_selectedMonth = DateTimeHelper.DiscardDayTime(value);
			}
			else if (num < 0)
			{
				_selectedMonth = DateTimeHelper.DiscardDayTime(DisplayDateRangeStart);
			}
			else
			{
				_selectedMonth = DateTimeHelper.DiscardDayTime(DisplayDateRangeEnd);
			}
		}
	}

	internal DateTime SelectedYear
	{
		get
		{
			return _selectedYear;
		}
		set
		{
			if (value.Year < DisplayDateRangeStart.Year)
			{
				_selectedYear = DisplayDateRangeStart;
			}
			else if (value.Year > DisplayDateRangeEnd.Year)
			{
				_selectedYear = DisplayDateRangeEnd;
			}
			else
			{
				_selectedYear = value;
			}
		}
	}

	public DateTime DisplayDate
	{
		get
		{
			return GetValue(DisplayDateProperty);
		}
		set
		{
			SetValue(DisplayDateProperty, value);
		}
	}

	internal DateTime DisplayDateInternal { get; private set; }

	public DateTime? DisplayDateStart
	{
		get
		{
			return GetValue(DisplayDateStartProperty);
		}
		set
		{
			SetValue(DisplayDateStartProperty, value);
		}
	}

	public CalendarBlackoutDatesCollection BlackoutDates { get; private set; }

	internal DateTime DisplayDateRangeStart => DisplayDateStart.GetValueOrDefault(DateTime.MinValue);

	public DateTime? DisplayDateEnd
	{
		get
		{
			return GetValue(DisplayDateEndProperty);
		}
		set
		{
			SetValue(DisplayDateEndProperty, value);
		}
	}

	internal DateTime DisplayDateRangeEnd => DisplayDateEnd.GetValueOrDefault(DateTime.MaxValue);

	internal DateTime? HoverStart { get; set; }

	internal int? HoverStartIndex { get; set; }

	internal DateTime? HoverEndInternal { get; set; }

	internal DateTime? HoverEnd
	{
		get
		{
			return HoverEndInternal;
		}
		set
		{
			HoverEndInternal = value;
			LastSelectedDate = value;
		}
	}

	internal int? HoverEndIndex { get; set; }

	internal bool HasFocusInternal { get; set; }

	internal bool IsMouseSelection { get; set; }

	internal bool CalendarDatePickerDisplayDateFlag { get; set; }

	public event EventHandler<SelectionChangedEventArgs>? SelectedDatesChanged;

	public event EventHandler<CalendarDateChangedEventArgs>? DisplayDateChanged;

	public event EventHandler<CalendarModeChangedEventArgs>? DisplayModeChanged;

	internal event EventHandler<PointerReleasedEventArgs>? DayButtonMouseUp;

	private void OnFirstDayOfWeekChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (IsValidFirstDayOfWeek(e.NewValue))
		{
			UpdateMonths();
			return;
		}
		throw new ArgumentOutOfRangeException("e", "Invalid DayOfWeek");
	}

	private static bool IsValidFirstDayOfWeek(object value)
	{
		DayOfWeek dayOfWeek = (DayOfWeek)value;
		if (dayOfWeek != 0 && dayOfWeek != DayOfWeek.Monday && dayOfWeek != DayOfWeek.Tuesday && dayOfWeek != DayOfWeek.Wednesday && dayOfWeek != DayOfWeek.Thursday && dayOfWeek != DayOfWeek.Friday)
		{
			return dayOfWeek == DayOfWeek.Saturday;
		}
		return true;
	}

	private void OnIsTodayHighlightedChanged(AvaloniaPropertyChangedEventArgs e)
	{
		int num = DateTimeHelper.CompareYearMonth(DisplayDateInternal, DateTime.Today);
		if (num > -2 && num < 2)
		{
			UpdateMonths();
		}
	}

	private void OnDisplayModePropertyChanged(AvaloniaPropertyChangedEventArgs e)
	{
		CalendarMode calendarMode = (CalendarMode)e.NewValue;
		CalendarMode calendarMode2 = (CalendarMode)e.OldValue;
		if (MonthControl != null)
		{
			switch (calendarMode2)
			{
			case CalendarMode.Month:
				SelectedYear = DisplayDateInternal;
				SelectedMonth = DisplayDateInternal;
				break;
			case CalendarMode.Year:
				SetCurrentValue(DisplayDateProperty, SelectedMonth);
				SelectedYear = SelectedMonth;
				break;
			case CalendarMode.Decade:
				SetCurrentValue(DisplayDateProperty, SelectedYear);
				SelectedMonth = SelectedYear;
				break;
			}
			switch (calendarMode)
			{
			case CalendarMode.Month:
				OnMonthClick();
				break;
			case CalendarMode.Year:
			case CalendarMode.Decade:
				OnHeaderClick();
				break;
			}
		}
		OnDisplayModeChanged(new CalendarModeChangedEventArgs((CalendarMode)e.OldValue, calendarMode));
	}

	private static bool IsValidDisplayMode(CalendarMode mode)
	{
		if (mode != 0 && mode != CalendarMode.Year)
		{
			return mode == CalendarMode.Decade;
		}
		return true;
	}

	private void OnDisplayModeChanged(CalendarModeChangedEventArgs args)
	{
		this.DisplayModeChanged?.Invoke(this, args);
	}

	private void OnSelectionModeChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (IsValidSelectionMode(e.NewValue))
		{
			_displayDateIsChanging = true;
			SetCurrentValue(SelectedDateProperty, null);
			_displayDateIsChanging = false;
			SelectedDates.Clear();
			return;
		}
		throw new ArgumentOutOfRangeException("e", "Invalid SelectionMode");
	}

	private static bool IsValidSelectionMode(object value)
	{
		CalendarSelectionMode calendarSelectionMode = (CalendarSelectionMode)value;
		if (calendarSelectionMode != 0 && calendarSelectionMode != CalendarSelectionMode.SingleRange && calendarSelectionMode != CalendarSelectionMode.MultipleRange)
		{
			return calendarSelectionMode == CalendarSelectionMode.None;
		}
		return true;
	}

	private void OnSelectedDateChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (_displayDateIsChanging)
		{
			return;
		}
		if (SelectionMode != CalendarSelectionMode.None)
		{
			DateTime? dateTime = (DateTime?)e.NewValue;
			if (IsValidDateSelection(this, dateTime))
			{
				if (!dateTime.HasValue)
				{
					SelectedDates.Clear();
				}
				else if (SelectedDates.Count <= 0 || !(SelectedDates[0] == dateTime.Value))
				{
					foreach (DateTime selectedDate in SelectedDates)
					{
						RemovedItems.Add(selectedDate);
					}
					SelectedDates.ClearInternal();
					SelectedDates.AddRange(dateTime.Value, dateTime.Value);
				}
				if (SelectionMode == CalendarSelectionMode.SingleDate)
				{
					LastSelectedDate = dateTime;
				}
				return;
			}
			throw new ArgumentOutOfRangeException("e", "SelectedDate value is not valid.");
		}
		throw new InvalidOperationException("The SelectedDate property cannot be set when the selection mode is None.");
	}

	private static bool IsSelectionChanged(SelectionChangedEventArgs e)
	{
		if (e.AddedItems.Count != e.RemovedItems.Count)
		{
			return true;
		}
		foreach (DateTime addedItem in e.AddedItems)
		{
			if (!e.RemovedItems.Contains(addedItem))
			{
				return true;
			}
		}
		return false;
	}

	internal void OnSelectedDatesCollectionChanged(SelectionChangedEventArgs e)
	{
		if (IsSelectionChanged(e))
		{
			e.RoutedEvent = SelectingItemsControl.SelectionChangedEvent;
			e.Source = this;
			this.SelectedDatesChanged?.Invoke(this, e);
		}
	}

	private void OnDisplayDateChanged(AvaloniaPropertyChangedEventArgs e)
	{
		UpdateDisplayDate(this, (DateTime)e.NewValue, (DateTime)e.OldValue);
	}

	private static void UpdateDisplayDate(Calendar c, DateTime addedDate, DateTime removedDate)
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		if (DateTime.Compare(addedDate, c.DisplayDateRangeStart) < 0)
		{
			c.DisplayDate = c.DisplayDateRangeStart;
			return;
		}
		if (DateTime.Compare(addedDate, c.DisplayDateRangeEnd) > 0)
		{
			c.DisplayDate = c.DisplayDateRangeEnd;
			return;
		}
		c.DisplayDateInternal = DateTimeHelper.DiscardDayTime(addedDate);
		c.UpdateMonths();
		c.OnDisplayDate(new CalendarDateChangedEventArgs(removedDate, addedDate));
	}

	private void OnDisplayDate(CalendarDateChangedEventArgs e)
	{
		this.DisplayDateChanged?.Invoke(this, e);
	}

	private void OnDisplayDateStartChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (_displayDateIsChanging)
		{
			return;
		}
		DateTime? dateTime = e.NewValue as DateTime?;
		if (dateTime.HasValue)
		{
			DateTime? dateTime2 = SelectedDateMin(this);
			if (dateTime2.HasValue && DateTime.Compare(dateTime2.Value, dateTime.Value) < 0)
			{
				SetCurrentValue(DisplayDateStartProperty, dateTime2.Value);
				return;
			}
			if (DateTime.Compare(dateTime.Value, DisplayDateRangeEnd) > 0)
			{
				SetCurrentValue(DisplayDateEndProperty, DisplayDateStart);
			}
			if (DateTimeHelper.CompareYearMonth(dateTime.Value, DisplayDateInternal) > 0)
			{
				SetCurrentValue(DisplayDateProperty, dateTime.Value);
			}
		}
		UpdateMonths();
	}

	private static DateTime? SelectedDateMin(Calendar cal)
	{
		if (cal.SelectedDates.Count > 0)
		{
			DateTime dateTime = cal.SelectedDates[0];
			foreach (DateTime selectedDate in cal.SelectedDates)
			{
				if (DateTime.Compare(selectedDate, dateTime) < 0)
				{
					dateTime = selectedDate;
				}
			}
			return dateTime;
		}
		return null;
	}

	private void OnDisplayDateEndChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (_displayDateIsChanging)
		{
			return;
		}
		DateTime? dateTime = e.NewValue as DateTime?;
		if (dateTime.HasValue)
		{
			DateTime? dateTime2 = SelectedDateMax(this);
			if (dateTime2.HasValue && DateTime.Compare(dateTime2.Value, dateTime.Value) > 0)
			{
				SetCurrentValue(DisplayDateEndProperty, dateTime2.Value);
				return;
			}
			if (DateTime.Compare(dateTime.Value, DisplayDateRangeStart) < 0)
			{
				SetCurrentValue(DisplayDateEndProperty, DisplayDateStart);
				return;
			}
			if (DateTimeHelper.CompareYearMonth(dateTime.Value, DisplayDateInternal) < 0)
			{
				SetCurrentValue(DisplayDateProperty, dateTime.Value);
			}
		}
		UpdateMonths();
	}

	private static DateTime? SelectedDateMax(Calendar cal)
	{
		if (cal.SelectedDates.Count > 0)
		{
			DateTime dateTime = cal.SelectedDates[0];
			foreach (DateTime selectedDate in cal.SelectedDates)
			{
				if (DateTime.Compare(selectedDate, dateTime) > 0)
				{
					dateTime = selectedDate;
				}
			}
			return dateTime;
		}
		return null;
	}

	internal CalendarDayButton? FindDayButtonFromDay(DateTime day)
	{
		CalendarItem monthControl = MonthControl;
		int num = 49;
		if (monthControl != null && monthControl.MonthView != null)
		{
			for (int i = 7; i < num; i++)
			{
				if (monthControl.MonthView.Children[i] is CalendarDayButton calendarDayButton)
				{
					DateTime? dateTime = calendarDayButton.DataContext as DateTime?;
					if (dateTime.HasValue && DateTimeHelper.CompareDays(dateTime.Value, day) == 0)
					{
						return calendarDayButton;
					}
				}
			}
		}
		return null;
	}

	private void OnSelectedMonthChanged(DateTime? selectedMonth)
	{
		if (selectedMonth.HasValue)
		{
			SelectedMonth = selectedMonth.Value;
			UpdateMonths();
		}
	}

	private void OnSelectedYearChanged(DateTime? selectedYear)
	{
		if (selectedYear.HasValue)
		{
			SelectedYear = selectedYear.Value;
			UpdateMonths();
		}
	}

	internal void OnHeaderClick()
	{
		CalendarItem monthControl = MonthControl;
		if (monthControl != null && monthControl.MonthView != null && monthControl.YearView != null)
		{
			monthControl.MonthView.IsVisible = false;
			monthControl.YearView.IsVisible = true;
			UpdateMonths();
		}
	}

	internal void ResetStates()
	{
		CalendarItem monthControl = MonthControl;
		int num = 49;
		if (monthControl != null && monthControl.MonthView != null)
		{
			for (int i = 7; i < num; i++)
			{
				((CalendarDayButton)monthControl.MonthView.Children[i]).IgnoreMouseOverState();
			}
		}
	}

	internal void UpdateMonths()
	{
		CalendarItem monthControl = MonthControl;
		if (monthControl != null)
		{
			switch (DisplayMode)
			{
			case CalendarMode.Month:
				monthControl.UpdateMonthMode();
				break;
			case CalendarMode.Year:
				monthControl.UpdateYearMode();
				break;
			case CalendarMode.Decade:
				monthControl.UpdateDecadeMode();
				break;
			}
		}
	}

	internal static bool IsValidDateSelection(Calendar cal, DateTime? value)
	{
		if (!value.HasValue)
		{
			return true;
		}
		if (cal.BlackoutDates.Contains(value.Value))
		{
			return false;
		}
		cal._displayDateIsChanging = true;
		if (DateTime.Compare(value.Value, cal.DisplayDateRangeStart) < 0)
		{
			cal.DisplayDateStart = value;
		}
		else if (DateTime.Compare(value.Value, cal.DisplayDateRangeEnd) > 0)
		{
			cal.DisplayDateEnd = value;
		}
		cal._displayDateIsChanging = false;
		return true;
	}

	private static bool IsValidKeyboardSelection(Calendar cal, DateTime? value)
	{
		if (!value.HasValue)
		{
			return true;
		}
		if (cal.BlackoutDates.Contains(value.Value))
		{
			return false;
		}
		if (DateTime.Compare(value.Value, cal.DisplayDateRangeStart) >= 0)
		{
			return DateTime.Compare(value.Value, cal.DisplayDateRangeEnd) <= 0;
		}
		return false;
	}

	internal void HighlightDays()
	{
		if (!HoverEnd.HasValue || !HoverStart.HasValue)
		{
			return;
		}
		CalendarItem monthControl = MonthControl;
		if (!HoverEndIndex.HasValue || !HoverStartIndex.HasValue)
		{
			return;
		}
		SortHoverIndexes(out var startIndex, out var endIndex);
		for (int i = startIndex; i <= endIndex; i++)
		{
			if (!(monthControl.MonthView.Children[i] is CalendarDayButton calendarDayButton))
			{
				continue;
			}
			calendarDayButton.IsSelected = true;
			DateTime? dateTime = calendarDayButton.DataContext as DateTime?;
			if (dateTime.HasValue && DateTimeHelper.CompareDays(HoverEnd.Value, dateTime.Value) == 0)
			{
				if (FocusButton != null)
				{
					FocusButton.IsCurrent = false;
				}
				calendarDayButton.IsCurrent = HasFocusInternal;
				FocusButton = calendarDayButton;
			}
		}
	}

	internal void UnHighlightDays()
	{
		if (!HoverEnd.HasValue || !HoverStart.HasValue)
		{
			return;
		}
		CalendarItem monthControl = MonthControl;
		if (!HoverEndIndex.HasValue || !HoverStartIndex.HasValue)
		{
			return;
		}
		SortHoverIndexes(out var startIndex, out var endIndex);
		if (SelectionMode == CalendarSelectionMode.MultipleRange)
		{
			for (int i = startIndex; i <= endIndex; i++)
			{
				if (monthControl.MonthView.Children[i] is CalendarDayButton calendarDayButton)
				{
					DateTime? dateTime = calendarDayButton.DataContext as DateTime?;
					if (dateTime.HasValue && !SelectedDates.Contains(dateTime.Value))
					{
						calendarDayButton.IsSelected = false;
					}
				}
			}
		}
		else
		{
			for (int i = startIndex; i <= endIndex; i++)
			{
				((CalendarDayButton)monthControl.MonthView.Children[i]).IsSelected = false;
			}
		}
	}

	internal void SortHoverIndexes(out int startIndex, out int endIndex)
	{
		if (DateTimeHelper.CompareDays(HoverEnd.Value, HoverStart.Value) > 0)
		{
			startIndex = HoverStartIndex.Value;
			endIndex = HoverEndIndex.Value;
		}
		else
		{
			startIndex = HoverEndIndex.Value;
			endIndex = HoverStartIndex.Value;
		}
	}

	internal void OnPreviousClick()
	{
		if (DisplayMode == CalendarMode.Month)
		{
			DateTime? dateTime = DateTimeHelper.AddMonths(DateTimeHelper.DiscardDayTime(DisplayDate), -1);
			if (dateTime.HasValue)
			{
				if (!LastSelectedDate.HasValue || DateTimeHelper.CompareYearMonth(LastSelectedDate.Value, dateTime.Value) != 0)
				{
					LastSelectedDate = dateTime.Value;
				}
				SetCurrentValue(DisplayDateProperty, dateTime.Value);
			}
			return;
		}
		if (DisplayMode == CalendarMode.Year)
		{
			DateTime? dateTime2 = DateTimeHelper.AddYears(new DateTime(SelectedMonth.Year, 1, 1), -1);
			if (dateTime2.HasValue)
			{
				SelectedMonth = dateTime2.Value;
			}
			else
			{
				SelectedMonth = DateTimeHelper.DiscardDayTime(DisplayDateRangeStart);
			}
		}
		else
		{
			DateTime? dateTime3 = DateTimeHelper.AddYears(new DateTime(SelectedYear.Year, 1, 1), -10);
			if (dateTime3.HasValue)
			{
				int year = Math.Max(1, DateTimeHelper.DecadeOfDate(dateTime3.Value));
				SelectedYear = new DateTime(year, 1, 1);
			}
			else
			{
				SelectedYear = DateTimeHelper.DiscardDayTime(DisplayDateRangeStart);
			}
		}
		UpdateMonths();
	}

	internal void OnNextClick()
	{
		if (DisplayMode == CalendarMode.Month)
		{
			DateTime? dateTime = DateTimeHelper.AddMonths(DateTimeHelper.DiscardDayTime(DisplayDate), 1);
			if (dateTime.HasValue)
			{
				if (!LastSelectedDate.HasValue || DateTimeHelper.CompareYearMonth(LastSelectedDate.Value, dateTime.Value) != 0)
				{
					LastSelectedDate = dateTime.Value;
				}
				SetCurrentValue(DisplayDateProperty, dateTime.Value);
			}
			return;
		}
		if (DisplayMode == CalendarMode.Year)
		{
			DateTime? dateTime2 = DateTimeHelper.AddYears(new DateTime(SelectedMonth.Year, 1, 1), 1);
			if (dateTime2.HasValue)
			{
				SelectedMonth = dateTime2.Value;
			}
			else
			{
				SelectedMonth = DateTimeHelper.DiscardDayTime(DisplayDateRangeEnd);
			}
		}
		else
		{
			DateTime? dateTime3 = DateTimeHelper.AddYears(new DateTime(SelectedYear.Year, 1, 1), 10);
			if (dateTime3.HasValue)
			{
				int year = Math.Max(1, DateTimeHelper.DecadeOfDate(dateTime3.Value));
				SelectedYear = new DateTime(year, 1, 1);
			}
			else
			{
				SelectedYear = DateTimeHelper.DiscardDayTime(DisplayDateRangeEnd);
			}
		}
		UpdateMonths();
	}

	internal void OnDayClick(DateTime selectedDate)
	{
		int num = DateTimeHelper.CompareYearMonth(selectedDate, DisplayDateInternal);
		if (SelectionMode == CalendarSelectionMode.None)
		{
			LastSelectedDate = selectedDate;
		}
		if (num > 0)
		{
			OnNextClick();
		}
		else if (num < 0)
		{
			OnPreviousClick();
		}
	}

	private void OnMonthClick()
	{
		CalendarItem monthControl = MonthControl;
		if (monthControl != null && monthControl.YearView != null && monthControl.MonthView != null)
		{
			monthControl.YearView.IsVisible = false;
			monthControl.MonthView.IsVisible = true;
			if (!LastSelectedDate.HasValue || DateTimeHelper.CompareYearMonth(LastSelectedDate.Value, DisplayDate) != 0)
			{
				LastSelectedDate = DisplayDate;
			}
			UpdateMonths();
		}
	}

	public override string ToString()
	{
		if (SelectedDate.HasValue)
		{
			return SelectedDate.Value.ToString(DateTimeHelper.GetCurrentDateFormat());
		}
		return string.Empty;
	}

	private void AddSelection()
	{
		if (!HoverEnd.HasValue || !HoverStart.HasValue)
		{
			return;
		}
		foreach (DateTime selectedDate in SelectedDates)
		{
			RemovedItems.Add(selectedDate);
		}
		SelectedDates.ClearInternal();
		SelectedDates.AddRange(HoverStart.Value, HoverEnd.Value);
	}

	private void ProcessSelection(bool shift, DateTime? lastSelectedDate, int? index)
	{
		if (SelectionMode == CalendarSelectionMode.None && lastSelectedDate.HasValue)
		{
			OnDayClick(lastSelectedDate.Value);
		}
		else
		{
			if (!lastSelectedDate.HasValue || !IsValidKeyboardSelection(this, lastSelectedDate.Value))
			{
				return;
			}
			if (SelectionMode == CalendarSelectionMode.SingleRange || SelectionMode == CalendarSelectionMode.MultipleRange)
			{
				foreach (DateTime selectedDate in SelectedDates)
				{
					RemovedItems.Add(selectedDate);
				}
				SelectedDates.ClearInternal();
				if (shift)
				{
					_isShiftPressed = true;
					if (!HoverStart.HasValue)
					{
						if (LastSelectedDate.HasValue)
						{
							HoverStart = LastSelectedDate;
						}
						else if (DateTimeHelper.CompareYearMonth(DisplayDateInternal, DateTime.Today) == 0)
						{
							HoverStart = DateTime.Today;
						}
						else
						{
							HoverStart = DisplayDateInternal;
						}
						CalendarDayButton calendarDayButton = FindDayButtonFromDay(HoverStart.Value);
						if (calendarDayButton != null)
						{
							HoverStartIndex = calendarDayButton.Index;
						}
					}
					UnHighlightDays();
					CalendarDateRange range = ((DateTime.Compare(HoverStart.Value, lastSelectedDate.Value) >= 0) ? new CalendarDateRange(lastSelectedDate.Value, HoverStart.Value) : new CalendarDateRange(HoverStart.Value, lastSelectedDate.Value));
					if (!BlackoutDates.ContainsAny(range))
					{
						HoverEnd = lastSelectedDate;
						if (index.HasValue)
						{
							HoverEndIndex += index;
						}
						else
						{
							CalendarDayButton calendarDayButton = FindDayButtonFromDay(HoverEndInternal.Value);
							if (calendarDayButton != null)
							{
								HoverEndIndex = calendarDayButton.Index;
							}
						}
					}
					OnDayClick(HoverEnd.Value);
					HighlightDays();
				}
				else
				{
					HoverStart = lastSelectedDate;
					HoverEnd = lastSelectedDate;
					AddSelection();
					OnDayClick(lastSelectedDate.Value);
				}
			}
			else
			{
				LastSelectedDate = lastSelectedDate.Value;
				if (SelectedDates.Count > 0)
				{
					SelectedDates[0] = lastSelectedDate.Value;
				}
				else
				{
					SelectedDates.Add(lastSelectedDate.Value);
				}
				OnDayClick(lastSelectedDate.Value);
			}
		}
	}

	protected override void OnPointerReleased(PointerReleasedEventArgs e)
	{
		base.OnPointerReleased(e);
		if (!HasFocusInternal && e.InitialPressMouseButton == MouseButton.Left)
		{
			Focus();
		}
	}

	internal void OnDayButtonMouseUp(PointerReleasedEventArgs e)
	{
		this.DayButtonMouseUp?.Invoke(this, e);
	}

	protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
	{
		base.OnPointerWheelChanged(e);
		if (e.Handled)
		{
			return;
		}
		CalendarExtensions.GetMetaKeyState(e.KeyModifiers, out var ctrl, out var shift);
		if (!ctrl)
		{
			if (e.Delta.Y > 0.0)
			{
				ProcessPageUpKey(shift: false);
			}
			else
			{
				ProcessPageDownKey(shift: false);
			}
		}
		else if (e.Delta.Y > 0.0)
		{
			ProcessDownKey(ctrl, shift);
		}
		else
		{
			ProcessUpKey(ctrl, shift);
		}
		e.Handled = true;
	}

	internal void Calendar_KeyDown(KeyEventArgs e)
	{
		if (!e.Handled && base.IsEnabled)
		{
			e.Handled = ProcessCalendarKey(e);
		}
	}

	internal bool ProcessCalendarKey(KeyEventArgs e)
	{
		if (DisplayMode == CalendarMode.Month && LastSelectedDate.HasValue && DateTimeHelper.CompareYearMonth(LastSelectedDate.Value, DisplayDateInternal) != 0 && FocusButton != null && !FocusButton.IsInactive)
		{
			return true;
		}
		Key key = e.Key;
		CalendarExtensions.GetMetaKeyState(e.KeyModifiers, out var ctrl, out var shift);
		switch (key)
		{
		case Key.Up:
			ProcessUpKey(ctrl, shift);
			return true;
		case Key.Down:
			ProcessDownKey(ctrl, shift);
			return true;
		case Key.Left:
			ProcessLeftKey(shift);
			return true;
		case Key.Right:
			ProcessRightKey(shift);
			return true;
		case Key.PageDown:
			ProcessPageDownKey(shift);
			return true;
		case Key.PageUp:
			ProcessPageUpKey(shift);
			return true;
		case Key.Home:
			ProcessHomeKey(shift);
			return true;
		case Key.End:
			ProcessEndKey(shift);
			return true;
		case Key.Return:
		case Key.Space:
			return ProcessEnterKey();
		default:
			return false;
		}
	}

	internal void ProcessUpKey(bool ctrl, bool shift)
	{
		switch (DisplayMode)
		{
		case CalendarMode.Month:
			if (ctrl)
			{
				SelectedMonth = DisplayDateInternal;
				SetCurrentValue(DisplayModeProperty, CalendarMode.Year);
			}
			else
			{
				DateTime? lastSelectedDate = DateTimeHelper.AddDays(LastSelectedDate.GetValueOrDefault(DateTime.Today), -7);
				ProcessSelection(shift, lastSelectedDate, -7);
			}
			break;
		case CalendarMode.Year:
			if (ctrl)
			{
				SelectedYear = SelectedMonth;
				SetCurrentValue(DisplayModeProperty, CalendarMode.Decade);
			}
			else
			{
				DateTime? selectedMonth = DateTimeHelper.AddMonths(_selectedMonth, -4);
				OnSelectedMonthChanged(selectedMonth);
			}
			break;
		case CalendarMode.Decade:
			if (!ctrl)
			{
				DateTime? selectedYear = DateTimeHelper.AddYears(SelectedYear, -4);
				OnSelectedYearChanged(selectedYear);
			}
			break;
		}
	}

	internal void ProcessDownKey(bool ctrl, bool shift)
	{
		switch (DisplayMode)
		{
		case CalendarMode.Month:
			if (!ctrl || shift)
			{
				DateTime? lastSelectedDate = DateTimeHelper.AddDays(LastSelectedDate.GetValueOrDefault(DateTime.Today), 7);
				ProcessSelection(shift, lastSelectedDate, 7);
			}
			break;
		case CalendarMode.Year:
			if (ctrl)
			{
				SetCurrentValue(DisplayDateProperty, SelectedMonth);
				SetCurrentValue(DisplayModeProperty, CalendarMode.Month);
			}
			else
			{
				DateTime? selectedMonth = DateTimeHelper.AddMonths(_selectedMonth, 4);
				OnSelectedMonthChanged(selectedMonth);
			}
			break;
		case CalendarMode.Decade:
			if (ctrl)
			{
				SelectedMonth = SelectedYear;
				SetCurrentValue(DisplayModeProperty, CalendarMode.Year);
			}
			else
			{
				DateTime? selectedYear = DateTimeHelper.AddYears(SelectedYear, 4);
				OnSelectedYearChanged(selectedYear);
			}
			break;
		}
	}

	internal void ProcessLeftKey(bool shift)
	{
		switch (DisplayMode)
		{
		case CalendarMode.Month:
		{
			DateTime? lastSelectedDate = DateTimeHelper.AddDays(LastSelectedDate.GetValueOrDefault(DateTime.Today), -1);
			ProcessSelection(shift, lastSelectedDate, -1);
			break;
		}
		case CalendarMode.Year:
		{
			DateTime? selectedMonth = DateTimeHelper.AddMonths(_selectedMonth, -1);
			OnSelectedMonthChanged(selectedMonth);
			break;
		}
		case CalendarMode.Decade:
		{
			DateTime? selectedYear = DateTimeHelper.AddYears(SelectedYear, -1);
			OnSelectedYearChanged(selectedYear);
			break;
		}
		}
	}

	internal void ProcessRightKey(bool shift)
	{
		switch (DisplayMode)
		{
		case CalendarMode.Month:
		{
			DateTime? lastSelectedDate = DateTimeHelper.AddDays(LastSelectedDate.GetValueOrDefault(DateTime.Today), 1);
			ProcessSelection(shift, lastSelectedDate, 1);
			break;
		}
		case CalendarMode.Year:
		{
			DateTime? selectedMonth = DateTimeHelper.AddMonths(_selectedMonth, 1);
			OnSelectedMonthChanged(selectedMonth);
			break;
		}
		case CalendarMode.Decade:
		{
			DateTime? selectedYear = DateTimeHelper.AddYears(SelectedYear, 1);
			OnSelectedYearChanged(selectedYear);
			break;
		}
		}
	}

	private bool ProcessEnterKey()
	{
		switch (DisplayMode)
		{
		case CalendarMode.Year:
			SetCurrentValue(DisplayDateProperty, SelectedMonth);
			SetCurrentValue(DisplayModeProperty, CalendarMode.Month);
			return true;
		case CalendarMode.Decade:
			SelectedMonth = SelectedYear;
			SetCurrentValue(DisplayModeProperty, CalendarMode.Year);
			return true;
		default:
			return false;
		}
	}

	internal void ProcessHomeKey(bool shift)
	{
		switch (DisplayMode)
		{
		case CalendarMode.Month:
		{
			DateTime? lastSelectedDate = new DateTime(DisplayDateInternal.Year, DisplayDateInternal.Month, 1);
			ProcessSelection(shift, lastSelectedDate, null);
			break;
		}
		case CalendarMode.Year:
		{
			DateTime value = new DateTime(_selectedMonth.Year, 1, 1);
			OnSelectedMonthChanged(value);
			break;
		}
		case CalendarMode.Decade:
		{
			DateTime? selectedYear = new DateTime(DateTimeHelper.DecadeOfDate(SelectedYear), 1, 1);
			OnSelectedYearChanged(selectedYear);
			break;
		}
		}
	}

	internal void ProcessEndKey(bool shift)
	{
		switch (DisplayMode)
		{
		case CalendarMode.Month:
		{
			DateTime? lastSelectedDate = new DateTime(DisplayDateInternal.Year, DisplayDateInternal.Month, 1);
			if (DateTimeHelper.CompareYearMonth(DateTime.MaxValue, lastSelectedDate.Value) > 0)
			{
				lastSelectedDate = DateTimeHelper.AddMonths(lastSelectedDate.Value, 1).Value;
				lastSelectedDate = DateTimeHelper.AddDays(lastSelectedDate.Value, -1).Value;
			}
			else
			{
				lastSelectedDate = DateTime.MaxValue;
			}
			ProcessSelection(shift, lastSelectedDate, null);
			break;
		}
		case CalendarMode.Year:
		{
			DateTime value = new DateTime(_selectedMonth.Year, 12, 1);
			OnSelectedMonthChanged(value);
			break;
		}
		case CalendarMode.Decade:
		{
			DateTime? selectedYear = new DateTime(DateTimeHelper.EndOfDecade(SelectedYear), 1, 1);
			OnSelectedYearChanged(selectedYear);
			break;
		}
		}
	}

	internal void ProcessPageDownKey(bool shift)
	{
		if (!shift)
		{
			OnNextClick();
			return;
		}
		switch (DisplayMode)
		{
		case CalendarMode.Month:
		{
			DateTime? lastSelectedDate = DateTimeHelper.AddMonths(LastSelectedDate.GetValueOrDefault(DateTime.Today), 1);
			ProcessSelection(shift, lastSelectedDate, null);
			break;
		}
		case CalendarMode.Year:
		{
			DateTime? selectedMonth = DateTimeHelper.AddYears(_selectedMonth, 1);
			OnSelectedMonthChanged(selectedMonth);
			break;
		}
		case CalendarMode.Decade:
		{
			DateTime? selectedYear = DateTimeHelper.AddYears(SelectedYear, 10);
			OnSelectedYearChanged(selectedYear);
			break;
		}
		}
	}

	internal void ProcessPageUpKey(bool shift)
	{
		if (!shift)
		{
			OnPreviousClick();
			return;
		}
		switch (DisplayMode)
		{
		case CalendarMode.Month:
		{
			DateTime? lastSelectedDate = DateTimeHelper.AddMonths(LastSelectedDate.GetValueOrDefault(DateTime.Today), -1);
			ProcessSelection(shift, lastSelectedDate, null);
			break;
		}
		case CalendarMode.Year:
		{
			DateTime? selectedMonth = DateTimeHelper.AddYears(_selectedMonth, -1);
			OnSelectedMonthChanged(selectedMonth);
			break;
		}
		case CalendarMode.Decade:
		{
			DateTime? selectedYear = DateTimeHelper.AddYears(SelectedYear, -10);
			OnSelectedYearChanged(selectedYear);
			break;
		}
		}
	}

	private void Calendar_KeyUp(KeyEventArgs e)
	{
		if (!e.Handled && (e.Key == Key.LeftShift || e.Key == Key.RightShift))
		{
			ProcessShiftKeyUp();
		}
	}

	internal void ProcessShiftKeyUp()
	{
		if (_isShiftPressed && (SelectionMode == CalendarSelectionMode.SingleRange || SelectionMode == CalendarSelectionMode.MultipleRange))
		{
			AddSelection();
			_isShiftPressed = false;
		}
	}

	protected override void OnGotFocus(GotFocusEventArgs e)
	{
		base.OnGotFocus(e);
		HasFocusInternal = true;
		switch (DisplayMode)
		{
		case CalendarMode.Month:
		{
			DateTime day;
			if (LastSelectedDate.HasValue && DateTimeHelper.CompareYearMonth(DisplayDateInternal, LastSelectedDate.Value) == 0)
			{
				day = LastSelectedDate.Value;
			}
			else
			{
				day = DisplayDate;
				LastSelectedDate = DisplayDate;
			}
			FocusButton = FindDayButtonFromDay(day);
			if (FocusButton != null)
			{
				FocusButton.IsCurrent = true;
			}
			break;
		}
		case CalendarMode.Year:
		case CalendarMode.Decade:
			if (FocusCalendarButton != null)
			{
				FocusCalendarButton.IsCalendarButtonFocused = true;
			}
			break;
		}
	}

	protected override void OnLostFocus(RoutedEventArgs e)
	{
		base.OnLostFocus(e);
		HasFocusInternal = false;
		switch (DisplayMode)
		{
		case CalendarMode.Month:
			if (FocusButton != null)
			{
				FocusButton.IsCurrent = false;
			}
			break;
		case CalendarMode.Year:
		case CalendarMode.Decade:
			if (FocusCalendarButton != null)
			{
				FocusCalendarButton.IsCalendarButtonFocused = false;
			}
			break;
		}
	}

	private void OnIsEnabledChanged(AvaloniaPropertyChangedEventArgs e)
	{
		bool isEnabled = (bool)e.NewValue;
		if (MonthControl != null)
		{
			MonthControl.UpdateDisabled(isEnabled);
		}
	}

	static Calendar()
	{
		FirstDayOfWeekProperty = AvaloniaProperty.Register<Calendar, DayOfWeek>("FirstDayOfWeek", DateTimeHelper.GetCurrentDateFormat().FirstDayOfWeek);
		IsTodayHighlightedProperty = AvaloniaProperty.Register<Calendar, bool>("IsTodayHighlighted", defaultValue: true);
		HeaderBackgroundProperty = AvaloniaProperty.Register<Calendar, IBrush>("HeaderBackground");
		DisplayModeProperty = AvaloniaProperty.Register<Calendar, CalendarMode>("DisplayMode", CalendarMode.Month, inherits: false, BindingMode.OneWay, IsValidDisplayMode);
		SelectionModeProperty = AvaloniaProperty.Register<Calendar, CalendarSelectionMode>("SelectionMode", CalendarSelectionMode.SingleDate);
		SelectedDateProperty = AvaloniaProperty.Register<Calendar, DateTime?>("SelectedDate", null, inherits: false, BindingMode.TwoWay);
		DisplayDateProperty = AvaloniaProperty.Register<Calendar, DateTime>("DisplayDate", default(DateTime), inherits: false, BindingMode.TwoWay);
		DisplayDateStartProperty = AvaloniaProperty.Register<Calendar, DateTime?>("DisplayDateStart", null, inherits: false, BindingMode.TwoWay);
		DisplayDateEndProperty = AvaloniaProperty.Register<Calendar, DateTime?>("DisplayDateEnd", null, inherits: false, BindingMode.TwoWay);
		InputElement.IsEnabledProperty.Changed.AddClassHandler(delegate(Calendar x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnIsEnabledChanged(e);
		});
		FirstDayOfWeekProperty.Changed.AddClassHandler(delegate(Calendar x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnFirstDayOfWeekChanged(e);
		});
		IsTodayHighlightedProperty.Changed.AddClassHandler(delegate(Calendar x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnIsTodayHighlightedChanged(e);
		});
		DisplayModeProperty.Changed.AddClassHandler(delegate(Calendar x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnDisplayModePropertyChanged(e);
		});
		SelectionModeProperty.Changed.AddClassHandler(delegate(Calendar x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnSelectionModeChanged(e);
		});
		SelectedDateProperty.Changed.AddClassHandler(delegate(Calendar x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnSelectedDateChanged(e);
		});
		DisplayDateProperty.Changed.AddClassHandler(delegate(Calendar x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnDisplayDateChanged(e);
		});
		DisplayDateStartProperty.Changed.AddClassHandler(delegate(Calendar x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnDisplayDateStartChanged(e);
		});
		DisplayDateEndProperty.Changed.AddClassHandler(delegate(Calendar x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnDisplayDateEndChanged(e);
		});
		InputElement.KeyDownEvent.AddClassHandler(delegate(Calendar x, KeyEventArgs e)
		{
			x.Calendar_KeyDown(e);
		});
		InputElement.KeyUpEvent.AddClassHandler(delegate(Calendar x, KeyEventArgs e)
		{
			x.Calendar_KeyUp(e);
		});
	}

	public Calendar()
	{
		SetCurrentValue(DisplayDateProperty, DateTime.Today);
		UpdateDisplayDate(this, DisplayDate, DateTime.MinValue);
		BlackoutDates = new CalendarBlackoutDatesCollection(this);
		SelectedDates = new SelectedDatesCollection(this);
		RemovedItems = new Collection<DateTime>();
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		Root = e.NameScope.Find<Panel>("PART_Root");
		SelectedMonth = DisplayDate;
		SelectedYear = DisplayDate;
		if (Root != null)
		{
			CalendarItem calendarItem = e.NameScope.Find<CalendarItem>("PART_CalendarItem");
			if (calendarItem != null)
			{
				calendarItem.Owner = this;
			}
		}
	}
}
