using System;
using System.Collections.ObjectModel;
using System.Globalization;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Reactive;
using Avalonia.Threading;

namespace Avalonia.Controls;

[TemplatePart("PART_Button", typeof(Button))]
[TemplatePart("PART_Calendar", typeof(Calendar))]
[TemplatePart("PART_Popup", typeof(Popup))]
[TemplatePart("PART_TextBox", typeof(TextBox))]
[PseudoClasses(new string[] { ":flyout-open", ":pressed" })]
public class CalendarDatePicker : TemplatedControl
{
	private const string pcPressed = ":pressed";

	private const string pcFlyoutOpen = ":flyout-open";

	private const string ElementTextBox = "PART_TextBox";

	private const string ElementButton = "PART_Button";

	private const string ElementPopup = "PART_Popup";

	private const string ElementCalendar = "PART_Calendar";

	private Calendar? _calendar;

	private string _defaultText;

	private Button? _dropDownButton;

	private Popup? _popUp;

	private TextBox? _textBox;

	private IDisposable? _textBoxTextChangedSubscription;

	private IDisposable? _buttonPointerPressedSubscription;

	private DateTime? _onOpenSelectedDate;

	private bool _settingSelectedDate;

	private bool _suspendTextChangeHandler;

	private bool _isPopupClosing;

	private bool _ignoreButtonClick;

	private bool _isFlyoutOpen;

	private bool _isPressed;

	public static readonly StyledProperty<DateTime> DisplayDateProperty;

	public static readonly StyledProperty<DateTime?> DisplayDateStartProperty;

	public static readonly StyledProperty<DateTime?> DisplayDateEndProperty;

	public static readonly StyledProperty<DayOfWeek> FirstDayOfWeekProperty;

	public static readonly StyledProperty<bool> IsDropDownOpenProperty;

	public static readonly StyledProperty<bool> IsTodayHighlightedProperty;

	public static readonly StyledProperty<DateTime?> SelectedDateProperty;

	public static readonly StyledProperty<CalendarDatePickerFormat> SelectedDateFormatProperty;

	public static readonly StyledProperty<string> CustomDateFormatStringProperty;

	public static readonly StyledProperty<string?> TextProperty;

	public static readonly StyledProperty<string?> WatermarkProperty;

	public static readonly StyledProperty<bool> UseFloatingWatermarkProperty;

	public static readonly StyledProperty<HorizontalAlignment> HorizontalContentAlignmentProperty;

	public static readonly StyledProperty<VerticalAlignment> VerticalContentAlignmentProperty;

	public CalendarBlackoutDatesCollection? BlackoutDates { get; private set; }

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

	public bool IsDropDownOpen
	{
		get
		{
			return GetValue(IsDropDownOpenProperty);
		}
		set
		{
			SetValue(IsDropDownOpenProperty, value);
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

	public CalendarDatePickerFormat SelectedDateFormat
	{
		get
		{
			return GetValue(SelectedDateFormatProperty);
		}
		set
		{
			SetValue(SelectedDateFormatProperty, value);
		}
	}

	public string CustomDateFormatString
	{
		get
		{
			return GetValue(CustomDateFormatStringProperty);
		}
		set
		{
			SetValue(CustomDateFormatStringProperty, value);
		}
	}

	public string? Text
	{
		get
		{
			return GetValue(TextProperty);
		}
		set
		{
			SetValue(TextProperty, value);
		}
	}

	public string? Watermark
	{
		get
		{
			return GetValue(WatermarkProperty);
		}
		set
		{
			SetValue(WatermarkProperty, value);
		}
	}

	public bool UseFloatingWatermark
	{
		get
		{
			return GetValue(UseFloatingWatermarkProperty);
		}
		set
		{
			SetValue(UseFloatingWatermarkProperty, value);
		}
	}

	public HorizontalAlignment HorizontalContentAlignment
	{
		get
		{
			return GetValue(HorizontalContentAlignmentProperty);
		}
		set
		{
			SetValue(HorizontalContentAlignmentProperty, value);
		}
	}

	public VerticalAlignment VerticalContentAlignment
	{
		get
		{
			return GetValue(VerticalContentAlignmentProperty);
		}
		set
		{
			SetValue(VerticalContentAlignmentProperty, value);
		}
	}

	public event EventHandler? CalendarClosed;

	public event EventHandler? CalendarOpened;

	public event EventHandler<CalendarDatePickerDateValidationErrorEventArgs>? DateValidationError;

	public event EventHandler<SelectionChangedEventArgs>? SelectedDateChanged;

	static CalendarDatePicker()
	{
		DisplayDateProperty = AvaloniaProperty.Register<CalendarDatePicker, DateTime>("DisplayDate");
		DisplayDateStartProperty = AvaloniaProperty.Register<CalendarDatePicker, DateTime?>("DisplayDateStart", null);
		DisplayDateEndProperty = AvaloniaProperty.Register<CalendarDatePicker, DateTime?>("DisplayDateEnd", null);
		FirstDayOfWeekProperty = AvaloniaProperty.Register<CalendarDatePicker, DayOfWeek>("FirstDayOfWeek", DayOfWeek.Sunday);
		IsDropDownOpenProperty = AvaloniaProperty.Register<CalendarDatePicker, bool>("IsDropDownOpen", defaultValue: false);
		IsTodayHighlightedProperty = AvaloniaProperty.Register<CalendarDatePicker, bool>("IsTodayHighlighted", defaultValue: false);
		SelectedDateProperty = AvaloniaProperty.Register<CalendarDatePicker, DateTime?>("SelectedDate", null, inherits: false, BindingMode.TwoWay, null, null, enableDataValidation: true);
		SelectedDateFormatProperty = AvaloniaProperty.Register<CalendarDatePicker, CalendarDatePickerFormat>("SelectedDateFormat", CalendarDatePickerFormat.Short, inherits: false, BindingMode.OneWay, IsValidSelectedDateFormat);
		CustomDateFormatStringProperty = AvaloniaProperty.Register<CalendarDatePicker, string>("CustomDateFormatString", "d", inherits: false, BindingMode.OneWay, IsValidDateFormatString);
		TextProperty = AvaloniaProperty.Register<CalendarDatePicker, string>("Text");
		WatermarkProperty = TextBox.WatermarkProperty.AddOwner<CalendarDatePicker>();
		UseFloatingWatermarkProperty = TextBox.UseFloatingWatermarkProperty.AddOwner<CalendarDatePicker>();
		HorizontalContentAlignmentProperty = ContentControl.HorizontalContentAlignmentProperty.AddOwner<CalendarDatePicker>();
		VerticalContentAlignmentProperty = ContentControl.VerticalContentAlignmentProperty.AddOwner<CalendarDatePicker>();
		InputElement.FocusableProperty.OverrideDefaultValue<CalendarDatePicker>(defaultValue: true);
	}

	public CalendarDatePicker()
	{
		SetCurrentValue(FirstDayOfWeekProperty, DateTimeHelper.GetCurrentDateFormat().FirstDayOfWeek);
		_defaultText = string.Empty;
		SetCurrentValue(DisplayDateProperty, DateTime.Today);
	}

	protected void UpdatePseudoClasses()
	{
		base.PseudoClasses.Set(":flyout-open", _isFlyoutOpen);
		base.PseudoClasses.Set(":pressed", _isPressed);
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		if (_calendar != null)
		{
			_calendar.DayButtonMouseUp -= Calendar_DayButtonMouseUp;
			_calendar.DisplayDateChanged -= Calendar_DisplayDateChanged;
			_calendar.SelectedDatesChanged -= Calendar_SelectedDatesChanged;
			_calendar.PointerReleased -= Calendar_PointerReleased;
			_calendar.KeyDown -= Calendar_KeyDown;
		}
		_calendar = e.NameScope.Find<Calendar>("PART_Calendar");
		if (_calendar != null)
		{
			_calendar.SelectionMode = CalendarSelectionMode.SingleDate;
			_calendar.DayButtonMouseUp += Calendar_DayButtonMouseUp;
			_calendar.DisplayDateChanged += Calendar_DisplayDateChanged;
			_calendar.SelectedDatesChanged += Calendar_SelectedDatesChanged;
			_calendar.PointerReleased += Calendar_PointerReleased;
			_calendar.KeyDown += Calendar_KeyDown;
			CalendarBlackoutDatesCollection blackoutDates = BlackoutDates;
			BlackoutDates = _calendar.BlackoutDates;
			if (blackoutDates != null)
			{
				foreach (CalendarDateRange item in blackoutDates)
				{
					BlackoutDates.Add(item);
				}
			}
		}
		if (_popUp != null)
		{
			_popUp.Child = null;
			_popUp.Closed -= PopUp_Closed;
		}
		_popUp = e.NameScope.Find<Popup>("PART_Popup");
		if (_popUp != null)
		{
			_popUp.Closed += PopUp_Closed;
			if (IsDropDownOpen)
			{
				OpenDropDown();
			}
		}
		if (_dropDownButton != null)
		{
			_dropDownButton.Click -= DropDownButton_Click;
			_buttonPointerPressedSubscription?.Dispose();
		}
		_dropDownButton = e.NameScope.Find<Button>("PART_Button");
		if (_dropDownButton != null)
		{
			_dropDownButton.Click += DropDownButton_Click;
			_buttonPointerPressedSubscription = new CompositeDisposable(_dropDownButton.AddDisposableHandler(InputElement.PointerPressedEvent, DropDownButton_PointerPressed, RoutingStrategies.Direct | RoutingStrategies.Bubble, handledEventsToo: true), _dropDownButton.AddDisposableHandler(InputElement.PointerReleasedEvent, DropDownButton_PointerReleased, RoutingStrategies.Direct | RoutingStrategies.Bubble, handledEventsToo: true));
		}
		if (_textBox != null)
		{
			_textBox.KeyDown -= TextBox_KeyDown;
			_textBox.GotFocus -= TextBox_GotFocus;
			_textBoxTextChangedSubscription?.Dispose();
		}
		_textBox = e.NameScope.Find<TextBox>("PART_TextBox");
		if (!SelectedDate.HasValue)
		{
			SetWaterMarkText();
		}
		if (_textBox != null)
		{
			_textBox.KeyDown += TextBox_KeyDown;
			_textBox.GotFocus += TextBox_GotFocus;
			_textBoxTextChangedSubscription = _textBox.GetObservable(TextBox.TextProperty).Subscribe(delegate
			{
				TextBox_TextChanged();
			});
			if (SelectedDate.HasValue)
			{
				_textBox.Text = DateTimeToString(SelectedDate.Value);
			}
			else if (!string.IsNullOrEmpty(_defaultText))
			{
				_textBox.Text = _defaultText;
				SetSelectedDate();
			}
		}
		UpdatePseudoClasses();
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		if (change.Property == CustomDateFormatStringProperty)
		{
			if (SelectedDateFormat == CalendarDatePickerFormat.Custom)
			{
				OnDateFormatChanged();
			}
		}
		else if (change.Property == IsDropDownOpenProperty)
		{
			var (flag, flag2) = change.GetOldAndNewValue<bool>();
			if (_popUp != null && _popUp.Child != null && flag2 != flag)
			{
				if (_calendar.DisplayMode != 0)
				{
					_calendar.DisplayMode = CalendarMode.Month;
				}
				if (flag2)
				{
					OpenDropDown();
				}
				else
				{
					_popUp.IsOpen = false;
					_isFlyoutOpen = _popUp.IsOpen;
					_isPressed = false;
					UpdatePseudoClasses();
					OnCalendarClosed(new RoutedEventArgs());
				}
			}
		}
		else if (change.Property == SelectedDateProperty)
		{
			var (removedDate, addedDate) = change.GetOldAndNewValue<DateTime?>();
			if (SelectedDate.HasValue)
			{
				DateTime day = SelectedDate.Value;
				Dispatcher.UIThread.InvokeAsync(delegate
				{
					_settingSelectedDate = true;
					SetCurrentValue(TextProperty, DateTimeToString(day));
					_settingSelectedDate = false;
					OnDateSelected(addedDate, removedDate);
				});
				if ((day.Month != DisplayDate.Month || day.Year != DisplayDate.Year) && (_calendar == null || !_calendar.CalendarDatePickerDisplayDateFlag))
				{
					SetCurrentValue(DisplayDateProperty, day);
				}
				if (_calendar != null)
				{
					_calendar.CalendarDatePickerDisplayDateFlag = false;
				}
			}
			else
			{
				_settingSelectedDate = true;
				SetWaterMarkText();
				_settingSelectedDate = false;
				OnDateSelected(addedDate, removedDate);
			}
		}
		else if (change.Property == SelectedDateFormatProperty)
		{
			OnDateFormatChanged();
		}
		else if (change.Property == TextProperty)
		{
			string item = change.GetOldAndNewValue<string>().newValue;
			if (!_suspendTextChangeHandler)
			{
				if (item != null)
				{
					if (_textBox != null)
					{
						_textBox.Text = item;
					}
					else
					{
						_defaultText = item;
					}
					if (!_settingSelectedDate)
					{
						SetSelectedDate();
					}
				}
				else if (!_settingSelectedDate)
				{
					_settingSelectedDate = true;
					SetCurrentValue(SelectedDateProperty, null);
					_settingSelectedDate = false;
				}
			}
			else
			{
				SetWaterMarkText();
			}
		}
		base.OnPropertyChanged(change);
	}

	protected override void UpdateDataValidation(AvaloniaProperty property, BindingValueType state, Exception? error)
	{
		if (property == SelectedDateProperty)
		{
			DataValidationErrors.SetError(this, error);
		}
		base.UpdateDataValidation(property, state, error);
	}

	protected override void OnPointerPressed(PointerPressedEventArgs e)
	{
		base.OnPointerPressed(e);
		if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
		{
			e.Handled = true;
			_ignoreButtonClick = _isPopupClosing;
			_isPressed = true;
			UpdatePseudoClasses();
		}
	}

	protected override void OnPointerReleased(PointerReleasedEventArgs e)
	{
		base.OnPointerReleased(e);
		if (_isPressed && e.InitialPressMouseButton == MouseButton.Left)
		{
			e.Handled = true;
			if (!_ignoreButtonClick)
			{
				TogglePopUp();
			}
			else
			{
				_ignoreButtonClick = false;
			}
			_isPressed = false;
			UpdatePseudoClasses();
		}
	}

	protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
	{
		base.OnPointerCaptureLost(e);
		_isPressed = false;
		UpdatePseudoClasses();
	}

	protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
	{
		base.OnPointerWheelChanged(e);
		if (!e.Handled && SelectedDate.HasValue && _calendar != null)
		{
			DateTime? value = DateTimeHelper.AddDays(SelectedDate.Value, (!(e.Delta.Y > 0.0)) ? 1 : (-1));
			if (value.HasValue && Calendar.IsValidDateSelection(_calendar, value.Value))
			{
				SetCurrentValue(SelectedDateProperty, value);
				e.Handled = true;
			}
		}
	}

	protected override void OnGotFocus(GotFocusEventArgs e)
	{
		base.OnGotFocus(e);
		if (base.IsEnabled && _textBox != null && e.NavigationMethod == NavigationMethod.Tab)
		{
			_textBox.Focus();
			string text = _textBox.Text;
			if (!string.IsNullOrEmpty(text))
			{
				_textBox.SelectionStart = 0;
				_textBox.SelectionEnd = text.Length;
			}
		}
	}

	protected override void OnLostFocus(RoutedEventArgs e)
	{
		base.OnLostFocus(e);
		_isPressed = false;
		UpdatePseudoClasses();
		SetSelectedDate();
	}

	protected override void OnKeyUp(KeyEventArgs e)
	{
		Key key = e.Key;
		if (((key != Key.Space && key != Key.Return) || !base.IsEffectivelyEnabled) && key == Key.Down && e.KeyModifiers.HasAllFlags(KeyModifiers.Alt) && base.IsEffectivelyEnabled && !IsDropDownOpen)
		{
			e.Handled = true;
			if (!_ignoreButtonClick)
			{
				TogglePopUp();
			}
			else
			{
				_ignoreButtonClick = false;
			}
			UpdatePseudoClasses();
		}
		base.OnKeyUp(e);
	}

	private void OnDateFormatChanged()
	{
		if (_textBox == null)
		{
			return;
		}
		if (SelectedDate.HasValue)
		{
			SetCurrentValue(TextProperty, DateTimeToString(SelectedDate.Value));
			return;
		}
		if (string.IsNullOrEmpty(_textBox.Text))
		{
			SetWaterMarkText();
			return;
		}
		DateTime? dateTime = ParseText(_textBox.Text);
		if (dateTime.HasValue)
		{
			string value = DateTimeToString(dateTime.Value);
			SetCurrentValue(TextProperty, value);
		}
	}

	protected virtual void OnDateValidationError(CalendarDatePickerDateValidationErrorEventArgs e)
	{
		this.DateValidationError?.Invoke(this, e);
	}

	private void OnDateSelected(DateTime? addedDate, DateTime? removedDate)
	{
		EventHandler<SelectionChangedEventArgs> eventHandler = this.SelectedDateChanged;
		if (eventHandler != null)
		{
			Collection<DateTime> collection = new Collection<DateTime>();
			Collection<DateTime> collection2 = new Collection<DateTime>();
			if (addedDate.HasValue)
			{
				collection.Add(addedDate.Value);
			}
			if (removedDate.HasValue)
			{
				collection2.Add(removedDate.Value);
			}
			eventHandler(this, new SelectionChangedEventArgs(SelectingItemsControl.SelectionChangedEvent, collection2, collection));
		}
	}

	private void OnCalendarClosed(EventArgs e)
	{
		this.CalendarClosed?.Invoke(this, e);
	}

	private void OnCalendarOpened(EventArgs e)
	{
		this.CalendarOpened?.Invoke(this, e);
	}

	private void Calendar_DayButtonMouseUp(object? sender, PointerReleasedEventArgs e)
	{
		Focus();
		SetCurrentValue(IsDropDownOpenProperty, value: false);
	}

	private void Calendar_DisplayDateChanged(object? sender, CalendarDateChangedEventArgs e)
	{
		if (e.AddedDate != DisplayDate)
		{
			SetValue(DisplayDateProperty, e.AddedDate.Value);
		}
	}

	private void Calendar_SelectedDatesChanged(object? sender, SelectionChangedEventArgs e)
	{
		if (e.AddedItems.Count > 0 && SelectedDate.HasValue && DateTime.Compare((DateTime)e.AddedItems[0], SelectedDate.Value) != 0)
		{
			SetCurrentValue(SelectedDateProperty, (DateTime?)e.AddedItems[0]);
		}
		else if (e.AddedItems.Count == 0)
		{
			SetCurrentValue(SelectedDateProperty, null);
		}
		else if (!SelectedDate.HasValue && e.AddedItems.Count > 0)
		{
			SetCurrentValue(SelectedDateProperty, (DateTime?)e.AddedItems[0]);
		}
	}

	private void Calendar_PointerReleased(object? sender, PointerReleasedEventArgs e)
	{
		if (e.InitialPressMouseButton == MouseButton.Left)
		{
			e.Handled = true;
		}
	}

	private void Calendar_KeyDown(object? sender, KeyEventArgs e)
	{
		if (!e.Handled && sender is Calendar { DisplayMode: CalendarMode.Month } && (e.Key == Key.Return || e.Key == Key.Space || e.Key == Key.Escape))
		{
			Focus();
			SetCurrentValue(IsDropDownOpenProperty, value: false);
			if (e.Key == Key.Escape)
			{
				SetCurrentValue(SelectedDateProperty, _onOpenSelectedDate);
			}
		}
	}

	private void TextBox_GotFocus(object? sender, RoutedEventArgs e)
	{
		SetCurrentValue(IsDropDownOpenProperty, value: false);
	}

	private void TextBox_KeyDown(object? sender, KeyEventArgs e)
	{
		if (!e.Handled)
		{
			e.Handled = ProcessDatePickerKey(e);
		}
	}

	private void TextBox_TextChanged()
	{
		if (_textBox != null)
		{
			_suspendTextChangeHandler = true;
			SetCurrentValue(TextProperty, _textBox.Text);
			_suspendTextChangeHandler = false;
		}
	}

	private void DropDownButton_PointerPressed(object? sender, PointerPressedEventArgs e)
	{
		_ignoreButtonClick = _isPopupClosing;
		_isPressed = true;
		UpdatePseudoClasses();
	}

	private void DropDownButton_PointerReleased(object? sender, PointerReleasedEventArgs e)
	{
		_isPressed = false;
		UpdatePseudoClasses();
	}

	private void DropDownButton_Click(object? sender, RoutedEventArgs e)
	{
		if (!_ignoreButtonClick)
		{
			TogglePopUp();
		}
		else
		{
			_ignoreButtonClick = false;
		}
	}

	private void PopUp_Closed(object? sender, EventArgs e)
	{
		SetCurrentValue(IsDropDownOpenProperty, value: false);
		if (!_isPopupClosing)
		{
			_isPopupClosing = true;
			Dispatcher.UIThread.InvokeAsync(() => _isPopupClosing = false);
		}
	}

	private void TogglePopUp()
	{
		if (IsDropDownOpen)
		{
			Focus();
			SetCurrentValue(IsDropDownOpenProperty, value: false);
		}
		else
		{
			SetSelectedDate();
			SetCurrentValue(IsDropDownOpenProperty, value: true);
			_calendar.Focus();
		}
	}

	private void OpenDropDown()
	{
		if (_calendar != null)
		{
			_calendar.Focus();
			_onOpenSelectedDate = SelectedDate;
			_popUp.IsOpen = true;
			_isFlyoutOpen = _popUp.IsOpen;
			UpdatePseudoClasses();
			_calendar.ResetStates();
			OnCalendarOpened(new RoutedEventArgs());
		}
	}

	private DateTime? ParseText(string text)
	{
		try
		{
			DateTime value = DateTime.Parse(text, DateTimeHelper.GetCurrentDateFormat());
			if (Calendar.IsValidDateSelection(_calendar, value))
			{
				return value;
			}
			CalendarDatePickerDateValidationErrorEventArgs calendarDatePickerDateValidationErrorEventArgs = new CalendarDatePickerDateValidationErrorEventArgs(new ArgumentOutOfRangeException("text", "SelectedDate value is not valid."), text);
			OnDateValidationError(calendarDatePickerDateValidationErrorEventArgs);
			if (calendarDatePickerDateValidationErrorEventArgs.ThrowException)
			{
				throw calendarDatePickerDateValidationErrorEventArgs.Exception;
			}
		}
		catch (FormatException exception)
		{
			CalendarDatePickerDateValidationErrorEventArgs calendarDatePickerDateValidationErrorEventArgs2 = new CalendarDatePickerDateValidationErrorEventArgs(exception, text);
			OnDateValidationError(calendarDatePickerDateValidationErrorEventArgs2);
			if (calendarDatePickerDateValidationErrorEventArgs2.ThrowException)
			{
				throw calendarDatePickerDateValidationErrorEventArgs2.Exception;
			}
		}
		return null;
	}

	private string? DateTimeToString(DateTime d)
	{
		DateTimeFormatInfo currentDateFormat = DateTimeHelper.GetCurrentDateFormat();
		return SelectedDateFormat switch
		{
			CalendarDatePickerFormat.Short => string.Format(CultureInfo.CurrentCulture, d.ToString(currentDateFormat.ShortDatePattern, currentDateFormat)), 
			CalendarDatePickerFormat.Long => string.Format(CultureInfo.CurrentCulture, d.ToString(currentDateFormat.LongDatePattern, currentDateFormat)), 
			CalendarDatePickerFormat.Custom => string.Format(CultureInfo.CurrentCulture, d.ToString(CustomDateFormatString, currentDateFormat)), 
			_ => null, 
		};
	}

	private bool ProcessDatePickerKey(KeyEventArgs e)
	{
		switch (e.Key)
		{
		case Key.Return:
			SetSelectedDate();
			return true;
		case Key.Down:
			if ((e.KeyModifiers & KeyModifiers.Control) == KeyModifiers.Control)
			{
				TogglePopUp();
				return true;
			}
			break;
		}
		return false;
	}

	private void SetSelectedDate()
	{
		if (_textBox != null)
		{
			if (!string.IsNullOrEmpty(_textBox.Text))
			{
				string text = _textBox.Text;
				if (!SelectedDate.HasValue || !(DateTimeToString(SelectedDate.Value) == text))
				{
					DateTime? dateTime = SetTextBoxValue(text);
					if (SelectedDate != dateTime)
					{
						SetCurrentValue(SelectedDateProperty, dateTime);
					}
				}
			}
			else if (SelectedDate.HasValue)
			{
				SetCurrentValue(SelectedDateProperty, null);
			}
		}
		else
		{
			DateTime? dateTime2 = SetTextBoxValue(_defaultText);
			if (SelectedDate != dateTime2)
			{
				SetCurrentValue(SelectedDateProperty, dateTime2);
			}
		}
	}

	private DateTime? SetTextBoxValue(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			SetValue(TextProperty, s);
			return SelectedDate;
		}
		DateTime? result = ParseText(s);
		if (result.HasValue)
		{
			SetValue(TextProperty, s);
			return result;
		}
		if (SelectedDate.HasValue)
		{
			string value = DateTimeToString(SelectedDate.Value);
			SetValue(TextProperty, value);
			return SelectedDate;
		}
		SetWaterMarkText();
		return null;
	}

	private void SetWaterMarkText()
	{
		if (_textBox != null)
		{
			SetCurrentValue(TextProperty, string.Empty);
			if (string.IsNullOrEmpty(Watermark) && !UseFloatingWatermark)
			{
				DateTimeFormatInfo currentDateFormat = DateTimeHelper.GetCurrentDateFormat();
				_defaultText = string.Empty;
				string format = "<{0}>";
				string watermark = SelectedDateFormat switch
				{
					CalendarDatePickerFormat.Long => string.Format(CultureInfo.CurrentCulture, format, currentDateFormat.LongDatePattern.ToString()), 
					_ => string.Format(CultureInfo.CurrentCulture, format, currentDateFormat.ShortDatePattern.ToString()), 
				};
				_textBox.Watermark = watermark;
			}
			else
			{
				_textBox.ClearValue(TextBox.WatermarkProperty);
			}
		}
	}

	private static bool IsValidSelectedDateFormat(CalendarDatePickerFormat value)
	{
		if (value != 0 && value != CalendarDatePickerFormat.Short)
		{
			return value == CalendarDatePickerFormat.Custom;
		}
		return true;
	}

	private static bool IsValidDateFormatString(string formatString)
	{
		return !string.IsNullOrWhiteSpace(formatString);
	}
}
