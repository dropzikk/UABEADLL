using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace Avalonia.Controls.Primitives;

public class DateTimePickerPanel : Panel, ILogicalScrollable, IScrollable
{
	public static readonly StyledProperty<double> ItemHeightProperty;

	public static readonly StyledProperty<DateTimePickerPanelType> PanelTypeProperty;

	public static readonly StyledProperty<string> ItemFormatProperty;

	public static readonly StyledProperty<bool> ShouldLoopProperty;

	private int _minimumValue = 1;

	private int _maximumValue = 2;

	private int _selectedValue = 1;

	private int _increment = 1;

	private int _selectedIndex;

	private int _totalItems;

	private int _numItemsAboveBelowSelected;

	private int _range;

	private double _extentOne;

	private Size _extent;

	private Vector _offset;

	private bool _hasInit;

	private bool _suppressUpdateOffset;

	private ScrollContentPresenter? _parentScroller;

	public DateTimePickerPanelType PanelType
	{
		get
		{
			return GetValue(PanelTypeProperty);
		}
		set
		{
			SetValue(PanelTypeProperty, value);
		}
	}

	public double ItemHeight
	{
		get
		{
			return GetValue(ItemHeightProperty);
		}
		set
		{
			SetValue(ItemHeightProperty, value);
		}
	}

	public string ItemFormat
	{
		get
		{
			return GetValue(ItemFormatProperty);
		}
		set
		{
			SetValue(ItemFormatProperty, value);
		}
	}

	public bool ShouldLoop
	{
		get
		{
			return GetValue(ShouldLoopProperty);
		}
		set
		{
			SetValue(ShouldLoopProperty, value);
		}
	}

	public int MinimumValue
	{
		get
		{
			return _minimumValue;
		}
		set
		{
			if (value > MaximumValue)
			{
				throw new InvalidOperationException("Minimum cannot be greater than Maximum");
			}
			_minimumValue = value;
			UpdateHelperInfo();
			int num = CoerceSelected(SelectedValue);
			if (num != SelectedValue)
			{
				SelectedValue = num;
			}
			UpdateItems();
			InvalidateArrange();
			RaiseScrollInvalidated(EventArgs.Empty);
		}
	}

	public int MaximumValue
	{
		get
		{
			return _maximumValue;
		}
		set
		{
			if (value < MinimumValue)
			{
				throw new InvalidOperationException("Maximum cannot be less than Minimum");
			}
			_maximumValue = value;
			UpdateHelperInfo();
			int num = CoerceSelected(SelectedValue);
			if (num != SelectedValue)
			{
				SelectedValue = num;
			}
			UpdateItems();
			InvalidateArrange();
			RaiseScrollInvalidated(EventArgs.Empty);
		}
	}

	public int SelectedValue
	{
		get
		{
			return _selectedValue;
		}
		set
		{
			if (value > MaximumValue || value < MinimumValue)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			int selectedValue = CoerceSelected(value);
			_selectedValue = selectedValue;
			_selectedIndex = (value - MinimumValue) / Increment;
			if (!ShouldLoop)
			{
				CreateOrDestroyItems(base.Children);
			}
			if (!_suppressUpdateOffset)
			{
				_offset = new Vector(0.0, ShouldLoop ? ((double)_selectedIndex * ItemHeight + _extentOne * 50.0) : ((double)_selectedIndex * ItemHeight));
			}
			UpdateItems();
			InvalidateArrange();
			RaiseScrollInvalidated(EventArgs.Empty);
			this.SelectionChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	public int Increment
	{
		get
		{
			return _increment;
		}
		set
		{
			if (value <= 0 || value > _range)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			_increment = value;
			UpdateHelperInfo();
			int num = CoerceSelected(SelectedValue);
			if (num != SelectedValue)
			{
				SelectedValue = num;
			}
			UpdateItems();
			InvalidateArrange();
			RaiseScrollInvalidated(EventArgs.Empty);
		}
	}

	internal DateTime FormatDate { get; set; }

	public Vector Offset
	{
		get
		{
			return _offset;
		}
		set
		{
			Vector offset = _offset;
			_offset = value;
			double num = _offset.Y - offset.Y;
			Controls children = base.Children;
			if (num > 0.0)
			{
				int num2 = 0;
				for (int i = 0; i < children.Count && children[i].Bounds.Bottom - num < 0.0; i++)
				{
					num2++;
				}
				children.MoveRange(0, num2, children.Count);
				double num3 = _extent.Height - Viewport.Height;
				if (ShouldLoop && value.Y >= num3 - _extentOne)
				{
					_offset = new Vector(0.0, value.Y - _extentOne * 50.0);
				}
			}
			else if (num < 0.0)
			{
				int num4 = 0;
				int num5 = children.Count - 1;
				while (num5 >= 0 && children[num5].Bounds.Top - num > base.Bounds.Height)
				{
					num4++;
					num5--;
				}
				children.MoveRange(children.Count - num4, num4, 0);
				if (ShouldLoop && value.Y < _extentOne)
				{
					_offset = new Vector(0.0, value.Y + _extentOne * 50.0);
				}
			}
			double num6 = Offset.Y / ItemHeight % (double)_totalItems;
			_suppressUpdateOffset = true;
			SelectedValue = (int)num6 * Increment + MinimumValue;
			_suppressUpdateOffset = false;
		}
	}

	public bool CanHorizontallyScroll
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public bool CanVerticallyScroll
	{
		get
		{
			return true;
		}
		set
		{
		}
	}

	public bool IsLogicalScrollEnabled => true;

	public Size ScrollSize => new Size(0.0, ItemHeight);

	public Size PageScrollSize => new Size(0.0, ItemHeight * 4.0);

	public Size Extent => _extent;

	public Size Viewport => base.Bounds.Size;

	public event EventHandler? ScrollInvalidated;

	public event EventHandler? SelectionChanged;

	public DateTimePickerPanel()
	{
		FormatDate = DateTime.Now;
		AddHandler(InputElement.TappedEvent, OnItemTapped, RoutingStrategies.Bubble);
	}

	static DateTimePickerPanel()
	{
		ItemHeightProperty = AvaloniaProperty.Register<DateTimePickerPanel, double>("ItemHeight", 40.0);
		PanelTypeProperty = AvaloniaProperty.Register<DateTimePickerPanel, DateTimePickerPanelType>("PanelType", DateTimePickerPanelType.Year);
		ItemFormatProperty = AvaloniaProperty.Register<DateTimePickerPanel, string>("ItemFormat", "yyyy");
		ShouldLoopProperty = AvaloniaProperty.Register<DateTimePickerPanel, bool>("ShouldLoop", defaultValue: false);
		InputElement.FocusableProperty.OverrideDefaultValue<DateTimePickerPanel>(defaultValue: true);
		Panel.BackgroundProperty.OverrideDefaultValue<DateTimePickerPanel>(Brushes.Transparent);
		Layoutable.AffectsMeasure<DateTimePickerPanel>(new AvaloniaProperty[1] { ItemHeightProperty });
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		if (double.IsInfinity(availableSize.Width) || double.IsInfinity(availableSize.Height))
		{
			throw new InvalidOperationException("Panel must have finite height");
		}
		if (!_hasInit)
		{
			UpdateHelperInfo();
		}
		double num = availableSize.Height / 2.0 - ItemHeight / 2.0;
		_numItemsAboveBelowSelected = (int)Math.Ceiling(num / ItemHeight) + 1;
		Controls children = base.Children;
		CreateOrDestroyItems(children);
		for (int i = 0; i < children.Count; i++)
		{
			children[i].Measure(availableSize);
		}
		if (!_hasInit)
		{
			UpdateItems();
			RaiseScrollInvalidated(EventArgs.Empty);
			_hasInit = true;
		}
		return availableSize;
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		if (base.Children.Count == 0)
		{
			return base.ArrangeOverride(finalSize);
		}
		double itemHeight = ItemHeight;
		Controls children = base.Children;
		double num = finalSize.Height / 2.0 - itemHeight / 2.0;
		Rect rect;
		if (ShouldLoop)
		{
			double num2 = Math.Truncate(Offset.Y / _extentOne);
			num += _extentOne * num2 + (double)(_selectedIndex - _numItemsAboveBelowSelected) * ItemHeight;
			for (int i = 0; i < children.Count; i++)
			{
				rect = new Rect(0.0, num - Offset.Y, finalSize.Width, itemHeight);
				children[i].Arrange(rect);
				num += itemHeight;
			}
		}
		else
		{
			int num3 = Math.Max(0, _selectedIndex - _numItemsAboveBelowSelected);
			for (int j = 0; j < children.Count; j++)
			{
				rect = new Rect(0.0, num + (double)num3 * itemHeight - Offset.Y, finalSize.Width, itemHeight);
				children[j].Arrange(rect);
				num += itemHeight;
			}
		}
		return finalSize;
	}

	protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnAttachedToVisualTree(e);
		_parentScroller = this.GetVisualParent() as ScrollContentPresenter;
		_parentScroller?.AddHandler(Gestures.ScrollGestureEndedEvent, OnScrollGestureEnded);
	}

	protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnDetachedFromVisualTree(e);
		_parentScroller?.RemoveHandler(Gestures.ScrollGestureEndedEvent, OnScrollGestureEnded);
		_parentScroller = null;
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		switch (e.Key)
		{
		case Key.Up:
			ScrollUp();
			e.Handled = true;
			break;
		case Key.Down:
			ScrollDown();
			e.Handled = true;
			break;
		case Key.PageUp:
			ScrollUp(4);
			e.Handled = true;
			break;
		case Key.PageDown:
			ScrollDown(4);
			e.Handled = true;
			break;
		}
		base.OnKeyDown(e);
	}

	public void RefreshItems()
	{
		UpdateItems();
	}

	public void ScrollUp(int numItems = 1)
	{
		double y = Math.Max(Offset.Y - (double)numItems * ItemHeight, 0.0);
		Offset = new Vector(0.0, y);
	}

	public void ScrollDown(int numItems = 1)
	{
		double val = _extent.Height - Viewport.Height;
		double y = Math.Min(Offset.Y + (double)numItems * ItemHeight, val);
		Offset = new Vector(0.0, y);
	}

	private void UpdateHelperInfo()
	{
		_range = _maximumValue - _minimumValue + 1;
		_totalItems = (int)Math.Ceiling((double)_range / (double)_increment);
		double itemHeight = ItemHeight;
		_extent = new Size(0.0, ShouldLoop ? ((double)_totalItems * itemHeight * 100.0) : ((double)_totalItems * itemHeight));
		_extentOne = (double)_totalItems * itemHeight;
		_offset = new Vector(0.0, ShouldLoop ? (_extentOne * 50.0 + (double)_selectedIndex * itemHeight) : ((double)_selectedIndex * itemHeight));
	}

	private void CreateOrDestroyItems(Controls children)
	{
		int num = _numItemsAboveBelowSelected * 2 + 1;
		if (!ShouldLoop)
		{
			int num2 = _numItemsAboveBelowSelected;
			if (_selectedIndex - _numItemsAboveBelowSelected < 0)
			{
				num2 = _selectedIndex;
			}
			int num3 = _numItemsAboveBelowSelected;
			if (_selectedIndex + _numItemsAboveBelowSelected >= _totalItems)
			{
				num3 = _totalItems - _selectedIndex - 1;
			}
			num = num3 + num2 + 1;
		}
		while (children.Count < num)
		{
			children.Add(new ListBoxItem
			{
				Height = ItemHeight,
				Classes = { $"{PanelType}Item" },
				VerticalContentAlignment = VerticalAlignment.Center,
				Focusable = false
			});
		}
		if (children.Count > num)
		{
			int num4 = children.Count - num;
			children.RemoveRange(children.Count - num4, num4);
		}
	}

	private void UpdateItems()
	{
		Controls children = base.Children;
		int minimumValue = MinimumValue;
		DateTimePickerPanelType panelType = PanelType;
		int selectedValue = SelectedValue;
		int maximumValue = MaximumValue;
		int num;
		if (ShouldLoop)
		{
			num = (_selectedIndex - _numItemsAboveBelowSelected) % _totalItems;
			num = ((num < 0) ? (minimumValue + (num + _totalItems) * Increment) : (minimumValue + num * Increment));
		}
		else
		{
			num = minimumValue + Math.Max(0, _selectedIndex - _numItemsAboveBelowSelected) * Increment;
		}
		for (int i = 0; i < children.Count; i++)
		{
			ListBoxItem obj = (ListBoxItem)children[i];
			obj.Content = FormatContent(num, panelType);
			obj.Tag = num;
			obj.IsSelected = num == selectedValue;
			num += Increment;
			if (num > maximumValue)
			{
				num = minimumValue;
			}
		}
	}

	private string FormatContent(int value, DateTimePickerPanelType panelType)
	{
		switch (panelType)
		{
		case DateTimePickerPanelType.Year:
			return new DateTime(value, 1, 1).ToString(ItemFormat);
		case DateTimePickerPanelType.Month:
			return new DateTime(FormatDate.Year, value, 1).ToString(ItemFormat);
		case DateTimePickerPanelType.Day:
			return new DateTime(FormatDate.Year, FormatDate.Month, value).ToString(ItemFormat);
		case DateTimePickerPanelType.Hour:
			return new TimeSpan(value, 0, 0).ToString(ItemFormat);
		case DateTimePickerPanelType.Minute:
			return new TimeSpan(0, value, 0).ToString(ItemFormat);
		case DateTimePickerPanelType.TimePeriod:
			if (value != MinimumValue)
			{
				return CultureInfo.CurrentCulture.DateTimeFormat.PMDesignator;
			}
			return CultureInfo.CurrentCulture.DateTimeFormat.AMDesignator;
		default:
			return "";
		}
	}

	private int CoerceSelected(int newValue)
	{
		if (newValue < MinimumValue)
		{
			return MinimumValue;
		}
		if (newValue > MaximumValue)
		{
			return MaximumValue;
		}
		if (newValue % Increment != 0)
		{
			List<int> list = (from i in Enumerable.Range(MinimumValue, MaximumValue + 1)
				where i % Increment == 0
				select i).ToList();
			int item = list.Aggregate((int x, int y) => (Math.Abs(x - newValue) <= Math.Abs(y - newValue)) ? x : y);
			return list.IndexOf(item) * Increment;
		}
		return newValue;
	}

	private void OnItemTapped(object? sender, TappedEventArgs e)
	{
		if (e.Source is Visual src)
		{
			ListBoxItem itemFromSource = GetItemFromSource(src);
			if (itemFromSource != null && itemFromSource.Tag is int selectedValue)
			{
				SelectedValue = selectedValue;
				e.Handled = true;
			}
		}
	}

	private ListBoxItem? GetItemFromSource(Visual src)
	{
		Visual visual = src;
		while (visual != null && !(visual is ListBoxItem))
		{
			visual = visual.VisualParent;
		}
		return (ListBoxItem)visual;
	}

	public bool BringIntoView(Control target, Rect targetRect)
	{
		return false;
	}

	public Control? GetControlInDirection(NavigationDirection direction, Control? from)
	{
		return null;
	}

	public void RaiseScrollInvalidated(EventArgs e)
	{
		this.ScrollInvalidated?.Invoke(this, e);
	}

	private void OnScrollGestureEnded(object? sender, ScrollGestureEndedEventArgs e)
	{
		double num = Math.Round(Offset.Y / ItemHeight) * ItemHeight;
		if (num != Offset.Y)
		{
			Offset = Offset.WithY(num);
		}
	}
}
