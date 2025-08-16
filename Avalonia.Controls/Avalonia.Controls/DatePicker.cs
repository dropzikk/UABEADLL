using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Layout;

namespace Avalonia.Controls;

[TemplatePart("PART_ButtonContentGrid", typeof(Grid))]
[TemplatePart("PART_DayTextBlock", typeof(TextBlock))]
[TemplatePart("PART_FirstSpacer", typeof(Rectangle))]
[TemplatePart("PART_FlyoutButton", typeof(Button))]
[TemplatePart("PART_MonthTextBlock", typeof(TextBlock))]
[TemplatePart("PART_PickerPresenter", typeof(DatePickerPresenter))]
[TemplatePart("PART_Popup", typeof(Popup))]
[TemplatePart("PART_SecondSpacer", typeof(Rectangle))]
[TemplatePart("PART_YearTextBlock", typeof(TextBlock))]
[PseudoClasses(new string[] { ":hasnodate" })]
public class DatePicker : TemplatedControl
{
	public static readonly StyledProperty<string> DayFormatProperty = AvaloniaProperty.Register<DatePicker, string>("DayFormat", "%d");

	public static readonly StyledProperty<bool> DayVisibleProperty = AvaloniaProperty.Register<DatePicker, bool>("DayVisible", defaultValue: true);

	public static readonly StyledProperty<DateTimeOffset> MaxYearProperty = AvaloniaProperty.Register<DatePicker, DateTimeOffset>("MaxYear", DateTimeOffset.MaxValue, inherits: false, BindingMode.OneWay, null, CoerceMaxYear);

	public static readonly StyledProperty<DateTimeOffset> MinYearProperty = AvaloniaProperty.Register<DatePicker, DateTimeOffset>("MinYear", DateTimeOffset.MinValue, inherits: false, BindingMode.OneWay, null, CoerceMinYear);

	public static readonly StyledProperty<string> MonthFormatProperty = AvaloniaProperty.Register<DatePicker, string>("MonthFormat", "MMMM");

	public static readonly StyledProperty<bool> MonthVisibleProperty = AvaloniaProperty.Register<DatePicker, bool>("MonthVisible", defaultValue: true);

	public static readonly StyledProperty<string> YearFormatProperty = AvaloniaProperty.Register<DatePicker, string>("YearFormat", "yyyy");

	public static readonly StyledProperty<bool> YearVisibleProperty = AvaloniaProperty.Register<DatePicker, bool>("YearVisible", defaultValue: true);

	public static readonly StyledProperty<DateTimeOffset?> SelectedDateProperty = AvaloniaProperty.Register<DatePicker, DateTimeOffset?>("SelectedDate", null, inherits: false, BindingMode.TwoWay);

	private Button? _flyoutButton;

	private TextBlock? _dayText;

	private TextBlock? _monthText;

	private TextBlock? _yearText;

	private Grid? _container;

	private Rectangle? _spacer1;

	private Rectangle? _spacer2;

	private Popup? _popup;

	private DatePickerPresenter? _presenter;

	private bool _areControlsAvailable;

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

	public DateTimeOffset? SelectedDate
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

	public event EventHandler<DatePickerSelectedValueChangedEventArgs>? SelectedDateChanged;

	public DatePicker()
	{
		base.PseudoClasses.Set(":hasnodate", value: true);
		DateTimeOffset now = DateTimeOffset.Now;
		SetCurrentValue(MinYearProperty, new DateTimeOffset(now.Date.Year - 100, 1, 1, 0, 0, 0, now.Offset));
		SetCurrentValue(MaxYearProperty, new DateTimeOffset(now.Date.Year + 100, 12, 31, 0, 0, 0, now.Offset));
	}

	private static void OnGridVisibilityChanged(DatePicker sender, AvaloniaPropertyChangedEventArgs e)
	{
		sender.SetGrid();
	}

	private static DateTimeOffset CoerceMaxYear(AvaloniaObject sender, DateTimeOffset value)
	{
		if (value < sender.GetValue(MinYearProperty))
		{
			throw new InvalidOperationException(MaxYearProperty.Name + " cannot be less than " + MinYearProperty.Name);
		}
		return value;
	}

	private void OnMaxYearChanged(DateTimeOffset? value)
	{
		if (SelectedDate.HasValue && SelectedDate.Value > value)
		{
			SetCurrentValue(SelectedDateProperty, value);
		}
	}

	private static DateTimeOffset CoerceMinYear(AvaloniaObject sender, DateTimeOffset value)
	{
		if (value > sender.GetValue(MaxYearProperty))
		{
			throw new InvalidOperationException(MinYearProperty.Name + " cannot be greater than " + MaxYearProperty.Name);
		}
		return value;
	}

	private void OnMinYearChanged(DateTimeOffset? value)
	{
		if (SelectedDate.HasValue && SelectedDate.Value < value)
		{
			SetCurrentValue(SelectedDateProperty, value);
		}
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		_areControlsAvailable = false;
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
		_dayText = e.NameScope.Find<TextBlock>("PART_DayTextBlock");
		_monthText = e.NameScope.Find<TextBlock>("PART_MonthTextBlock");
		_yearText = e.NameScope.Find<TextBlock>("PART_YearTextBlock");
		_container = e.NameScope.Find<Grid>("PART_ButtonContentGrid");
		_spacer1 = e.NameScope.Find<Rectangle>("PART_FirstSpacer");
		_spacer2 = e.NameScope.Find<Rectangle>("PART_SecondSpacer");
		_popup = e.NameScope.Find<Popup>("PART_Popup");
		_presenter = e.NameScope.Find<DatePickerPresenter>("PART_PickerPresenter");
		_areControlsAvailable = true;
		SetGrid();
		SetSelectedDateText();
		if (_flyoutButton != null)
		{
			_flyoutButton.Click += OnFlyoutButtonClicked;
		}
		if (_presenter != null)
		{
			_presenter.Confirmed += OnConfirmed;
			_presenter.Dismissed += OnDismissPicker;
			_presenter[!DatePickerPresenter.MaxYearProperty] = base[!MaxYearProperty];
			_presenter[!DatePickerPresenter.MinYearProperty] = base[!MinYearProperty];
			_presenter[!DatePickerPresenter.MonthVisibleProperty] = base[!MonthVisibleProperty];
			_presenter[!DatePickerPresenter.MonthFormatProperty] = base[!MonthFormatProperty];
			_presenter[!DatePickerPresenter.DayVisibleProperty] = base[!DayVisibleProperty];
			_presenter[!DatePickerPresenter.DayFormatProperty] = base[!DayFormatProperty];
			_presenter[!DatePickerPresenter.YearVisibleProperty] = base[!YearVisibleProperty];
			_presenter[!DatePickerPresenter.YearFormatProperty] = base[!YearFormatProperty];
		}
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == DayVisibleProperty || change.Property == MonthVisibleProperty || change.Property == YearVisibleProperty)
		{
			SetGrid();
		}
		else if (change.Property == MaxYearProperty)
		{
			OnMaxYearChanged(change.GetNewValue<DateTimeOffset>());
		}
		else if (change.Property == MinYearProperty)
		{
			OnMinYearChanged(change.GetNewValue<DateTimeOffset>());
		}
		else if (change.Property == SelectedDateProperty)
		{
			SetSelectedDateText();
			var (oldDate, newDate) = change.GetOldAndNewValue<DateTimeOffset?>();
			OnSelectedDateChanged(this, new DatePickerSelectedValueChangedEventArgs(oldDate, newDate));
		}
	}

	private void OnDismissPicker(object? sender, EventArgs e)
	{
		_popup.Close();
		Focus();
	}

	private void OnConfirmed(object? sender, EventArgs e)
	{
		_popup.Close();
		SetCurrentValue(SelectedDateProperty, _presenter.Date);
	}

	private void SetGrid()
	{
		if (_container == null)
		{
			return;
		}
		string shortDatePattern = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
		List<(TextBlock, int)> list = new List<(TextBlock, int)>();
		list.Add((_monthText, MonthVisible ? shortDatePattern.IndexOf("m", StringComparison.OrdinalIgnoreCase) : (-1)));
		list.Add((_yearText, YearVisible ? shortDatePattern.IndexOf("y", StringComparison.OrdinalIgnoreCase) : (-1)));
		list.Add((_dayText, DayVisible ? shortDatePattern.IndexOf("d", StringComparison.OrdinalIgnoreCase) : (-1)));
		list.Sort(((TextBlock?, int) x, (TextBlock?, int) y) => x.Item2 - y.Item2);
		_container.ColumnDefinitions.Clear();
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
					_container.ColumnDefinitions.Add(new ColumnDefinition(0.0, GridUnitType.Auto));
				}
				_container.ColumnDefinitions.Add(new ColumnDefinition((item.Item1 == _monthText) ? 138 : 78, GridUnitType.Star));
				if (item.Item1.Parent == null)
				{
					_container.Children.Add(item.Item1);
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

	private void SetSelectedDateText()
	{
		if (_areControlsAvailable)
		{
			if (SelectedDate.HasValue)
			{
				base.PseudoClasses.Set(":hasnodate", value: false);
				DateTimeOffset value = SelectedDate.Value;
				_monthText.Text = value.ToString(MonthFormat);
				_yearText.Text = value.ToString(YearFormat);
				_dayText.Text = value.ToString(DayFormat);
			}
			else
			{
				base.PseudoClasses.Set(":hasnodate", value: true);
				_monthText.Text = "month";
				_yearText.Text = "year";
				_dayText.Text = "day";
			}
		}
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
		_presenter.Date = SelectedDate ?? DateTimeOffset.Now;
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

	protected virtual void OnSelectedDateChanged(object? sender, DatePickerSelectedValueChangedEventArgs e)
	{
		this.SelectedDateChanged?.Invoke(sender, e);
	}
}
