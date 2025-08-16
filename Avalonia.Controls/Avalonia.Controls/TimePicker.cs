using System;
using System.Globalization;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Layout;

namespace Avalonia.Controls;

[TemplatePart("PART_FirstColumnDivider", typeof(Rectangle))]
[TemplatePart("PART_FirstPickerHost", typeof(Border))]
[TemplatePart("PART_FlyoutButton", typeof(Button))]
[TemplatePart("PART_FlyoutButtonContentGrid", typeof(Grid))]
[TemplatePart("PART_HourTextBlock", typeof(TextBlock))]
[TemplatePart("PART_MinuteTextBlock", typeof(TextBlock))]
[TemplatePart("PART_PeriodTextBlock", typeof(TextBlock))]
[TemplatePart("PART_PickerPresenter", typeof(TimePickerPresenter))]
[TemplatePart("PART_Popup", typeof(Popup))]
[TemplatePart("PART_SecondColumnDivider", typeof(Rectangle))]
[TemplatePart("PART_SecondPickerHost", typeof(Border))]
[TemplatePart("PART_ThirdPickerHost", typeof(Border))]
[PseudoClasses(new string[] { ":hasnotime" })]
public class TimePicker : TemplatedControl
{
	public static readonly StyledProperty<int> MinuteIncrementProperty = AvaloniaProperty.Register<TimePicker, int>("MinuteIncrement", 1, inherits: false, BindingMode.OneWay, null, CoerceMinuteIncrement);

	public static readonly StyledProperty<string> ClockIdentifierProperty = AvaloniaProperty.Register<TimePicker, string>("ClockIdentifier", "12HourClock", inherits: false, BindingMode.OneWay, null, CoerceClockIdentifier);

	public static readonly StyledProperty<TimeSpan?> SelectedTimeProperty = AvaloniaProperty.Register<TimePicker, TimeSpan?>("SelectedTime", null, inherits: false, BindingMode.TwoWay);

	private TimePickerPresenter? _presenter;

	private Button? _flyoutButton;

	private Border? _firstPickerHost;

	private Border? _secondPickerHost;

	private Border? _thirdPickerHost;

	private TextBlock? _hourText;

	private TextBlock? _minuteText;

	private TextBlock? _periodText;

	private Rectangle? _firstSplitter;

	private Rectangle? _secondSplitter;

	private Grid? _contentGrid;

	private Popup? _popup;

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

	public TimeSpan? SelectedTime
	{
		get
		{
			return GetValue(SelectedTimeProperty);
		}
		set
		{
			SetValue(SelectedTimeProperty, value);
		}
	}

	public event EventHandler<TimePickerSelectedValueChangedEventArgs>? SelectedTimeChanged;

	public TimePicker()
	{
		base.PseudoClasses.Set(":hasnotime", value: true);
		if (CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern.IndexOf("H") != -1)
		{
			SetCurrentValue(ClockIdentifierProperty, "24HourClock");
		}
	}

	private static int CoerceMinuteIncrement(AvaloniaObject sender, int value)
	{
		if (value < 1 || value > 59)
		{
			throw new ArgumentOutOfRangeException(null, "1 >= MinuteIncrement <= 59");
		}
		return value;
	}

	private static string CoerceClockIdentifier(AvaloniaObject sender, string value)
	{
		if (!string.IsNullOrEmpty(value) && !(value == "12HourClock") && !(value == "24HourClock"))
		{
			throw new ArgumentException("Invalid ClockIdentifier", (string?)null);
		}
		return value;
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		if (_flyoutButton != null)
		{
			_flyoutButton.Click -= OnFlyoutButtonClicked;
		}
		if (_presenter != null)
		{
			_presenter.Confirmed -= OnConfirmed;
			_presenter.Dismissed -= OnDismissPicker;
		}
		base.OnApplyTemplate(e);
		_flyoutButton = e.NameScope.Find<Button>("PART_FlyoutButton");
		_firstPickerHost = e.NameScope.Find<Border>("PART_FirstPickerHost");
		_secondPickerHost = e.NameScope.Find<Border>("PART_SecondPickerHost");
		_thirdPickerHost = e.NameScope.Find<Border>("PART_ThirdPickerHost");
		_hourText = e.NameScope.Find<TextBlock>("PART_HourTextBlock");
		_minuteText = e.NameScope.Find<TextBlock>("PART_MinuteTextBlock");
		_periodText = e.NameScope.Find<TextBlock>("PART_PeriodTextBlock");
		_firstSplitter = e.NameScope.Find<Rectangle>("PART_FirstColumnDivider");
		_secondSplitter = e.NameScope.Find<Rectangle>("PART_SecondColumnDivider");
		_contentGrid = e.NameScope.Find<Grid>("PART_FlyoutButtonContentGrid");
		_popup = e.NameScope.Find<Popup>("PART_Popup");
		_presenter = e.NameScope.Find<TimePickerPresenter>("PART_PickerPresenter");
		if (_flyoutButton != null)
		{
			_flyoutButton.Click += OnFlyoutButtonClicked;
		}
		SetGrid();
		SetSelectedTimeText();
		if (_presenter != null)
		{
			_presenter.Confirmed += OnConfirmed;
			_presenter.Dismissed += OnDismissPicker;
			_presenter[!TimePickerPresenter.MinuteIncrementProperty] = base[!MinuteIncrementProperty];
			_presenter[!TimePickerPresenter.ClockIdentifierProperty] = base[!ClockIdentifierProperty];
		}
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == MinuteIncrementProperty)
		{
			SetSelectedTimeText();
		}
		else if (change.Property == ClockIdentifierProperty)
		{
			SetGrid();
			SetSelectedTimeText();
		}
		else if (change.Property == SelectedTimeProperty)
		{
			var (oldTime, newTime) = change.GetOldAndNewValue<TimeSpan?>();
			OnSelectedTimeChanged(oldTime, newTime);
			SetSelectedTimeText();
		}
	}

	private void SetGrid()
	{
		if (_contentGrid != null)
		{
			bool flag = ClockIdentifier == "24HourClock";
			string s = (flag ? "*, Auto, *" : "*, Auto, *, Auto, *");
			_contentGrid.ColumnDefinitions = new ColumnDefinitions(s);
			_thirdPickerHost.IsVisible = !flag;
			_secondSplitter.IsVisible = !flag;
			Grid.SetColumn(_firstPickerHost, 0);
			Grid.SetColumn(_secondPickerHost, 2);
			Grid.SetColumn(_thirdPickerHost, (!flag) ? 4 : 0);
			Grid.SetColumn(_firstSplitter, 1);
			Grid.SetColumn(_secondSplitter, (!flag) ? 3 : 0);
		}
	}

	private void SetSelectedTimeText()
	{
		if (_hourText == null || _minuteText == null || _periodText == null)
		{
			return;
		}
		TimeSpan? selectedTime = SelectedTime;
		if (selectedTime.HasValue)
		{
			TimeSpan timeSpan = SelectedTime.Value;
			if (ClockIdentifier == "12HourClock")
			{
				int hours = timeSpan.Hours;
				hours = ((hours > 12) ? (hours - 12) : ((hours == 0) ? 12 : hours));
				timeSpan = new TimeSpan(hours, timeSpan.Minutes, 0);
			}
			_hourText.Text = timeSpan.ToString("%h");
			_minuteText.Text = timeSpan.ToString("mm");
			base.PseudoClasses.Set(":hasnotime", value: false);
			_periodText.Text = ((selectedTime.Value.Hours >= 12) ? CultureInfo.CurrentCulture.DateTimeFormat.PMDesignator : CultureInfo.CurrentCulture.DateTimeFormat.AMDesignator);
		}
		else
		{
			_hourText.Text = "hour";
			_minuteText.Text = "minute";
			base.PseudoClasses.Set(":hasnotime", value: true);
			_periodText.Text = ((DateTime.Now.Hour >= 12) ? CultureInfo.CurrentCulture.DateTimeFormat.PMDesignator : CultureInfo.CurrentCulture.DateTimeFormat.AMDesignator);
		}
	}

	protected virtual void OnSelectedTimeChanged(TimeSpan? oldTime, TimeSpan? newTime)
	{
		this.SelectedTimeChanged?.Invoke(this, new TimePickerSelectedValueChangedEventArgs(oldTime, newTime));
	}

	private void OnFlyoutButtonClicked(object? sender, RoutedEventArgs e)
	{
		if (_presenter == null)
		{
			throw new InvalidOperationException("No DatePickerPresenter found.");
		}
		if (_popup == null)
		{
			throw new InvalidOperationException("No Popup found.");
		}
		_presenter.Time = SelectedTime ?? DateTime.Now.TimeOfDay;
		_popup.Placement = PlacementMode.AnchorAndGravity;
		_popup.PlacementAnchor = PopupAnchor.Bottom;
		_popup.PlacementGravity = PopupGravity.Bottom;
		_popup.PlacementConstraintAdjustment = PopupPositionerConstraintAdjustment.SlideY;
		_popup.IsOpen = true;
		if (!_presenter.IsMeasureValid)
		{
			(base.VisualRoot as ILayoutRoot)?.LayoutManager?.ExecuteInitialLayoutPass();
		}
		double offsetForPopup = _presenter.GetOffsetForPopup();
		_popup.VerticalOffset = offsetForPopup + 5.0;
	}

	private void OnDismissPicker(object? sender, EventArgs e)
	{
		_popup.Close();
		Focus();
	}

	private void OnConfirmed(object? sender, EventArgs e)
	{
		_popup.Close();
		SetCurrentValue(SelectedTimeProperty, _presenter.Time);
	}
}
