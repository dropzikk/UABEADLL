using System;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Reactive;

namespace Avalonia.Controls;

[TemplatePart("PART_ExpanderButton", typeof(ToggleButton))]
[TemplatePart("PART_IndentSpacer", typeof(Control))]
[TemplatePart("PART_ItemCountElement", typeof(TextBlock))]
[TemplatePart("PART_PropertyNameElement", typeof(TextBlock))]
[TemplatePart("PART_Root", typeof(Panel))]
[TemplatePart("PART_RowHeader", typeof(DataGridRowHeader))]
[PseudoClasses(new string[] { ":pressed", ":current", ":expanded" })]
public class DataGridRowGroupHeader : TemplatedControl
{
	private const string DATAGRIDROWGROUPHEADER_expanderButton = "PART_ExpanderButton";

	private const string DATAGRIDROWGROUPHEADER_indentSpacer = "PART_IndentSpacer";

	private const string DATAGRIDROWGROUPHEADER_itemCountElement = "PART_ItemCountElement";

	private const string DATAGRIDROWGROUPHEADER_propertyNameElement = "PART_PropertyNameElement";

	private bool _areIsCheckedHandlersSuspended;

	private ToggleButton _expanderButton;

	private DataGridRowHeader _headerElement;

	private Control _indentSpacer;

	private TextBlock _itemCountElement;

	private TextBlock _propertyNameElement;

	private Panel _rootElement;

	private double _totalIndent;

	public static readonly StyledProperty<bool> IsItemCountVisibleProperty;

	public static readonly StyledProperty<string> PropertyNameProperty;

	public static readonly StyledProperty<bool> IsPropertyNameVisibleProperty;

	public static readonly StyledProperty<double> SublevelIndentProperty;

	private IDisposable _expanderButtonSubscription;

	public bool IsItemCountVisible
	{
		get
		{
			return GetValue(IsItemCountVisibleProperty);
		}
		set
		{
			SetValue(IsItemCountVisibleProperty, value);
		}
	}

	public string PropertyName
	{
		get
		{
			return GetValue(PropertyNameProperty);
		}
		set
		{
			SetValue(PropertyNameProperty, value);
		}
	}

	public bool IsPropertyNameVisible
	{
		get
		{
			return GetValue(IsPropertyNameVisibleProperty);
		}
		set
		{
			SetValue(IsPropertyNameVisibleProperty, value);
		}
	}

	public double SublevelIndent
	{
		get
		{
			return GetValue(SublevelIndentProperty);
		}
		set
		{
			SetValue(SublevelIndentProperty, value);
		}
	}

	internal DataGridRowHeader HeaderCell => _headerElement;

	private bool IsCurrent => RowGroupInfo.Slot == OwningGrid.CurrentSlot;

	private bool IsMouseOver { get; set; }

	internal bool IsRecycled { get; set; }

	internal int Level { get; set; }

	internal DataGrid OwningGrid { get; set; }

	internal DataGridRowGroupInfo RowGroupInfo { get; set; }

	internal double TotalIndent
	{
		set
		{
			_totalIndent = value;
			if (_indentSpacer != null)
			{
				_indentSpacer.Width = _totalIndent;
			}
		}
	}

	private static bool IsValidSublevelIndent(double value)
	{
		if (!double.IsNaN(value) && !double.IsInfinity(value))
		{
			return value >= 0.0;
		}
		return false;
	}

	private void OnSublevelIndentChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (OwningGrid != null)
		{
			OwningGrid.OnSublevelIndentUpdated(this, (double)e.NewValue);
		}
	}

	static DataGridRowGroupHeader()
	{
		IsItemCountVisibleProperty = AvaloniaProperty.Register<DataGridRowGroupHeader, bool>("IsItemCountVisible", defaultValue: false);
		PropertyNameProperty = AvaloniaProperty.Register<DataGridRowGroupHeader, string>("PropertyName");
		IsPropertyNameVisibleProperty = AvaloniaProperty.Register<DataGridRowGroupHeader, bool>("IsPropertyNameVisible", defaultValue: false);
		SublevelIndentProperty = AvaloniaProperty.Register<DataGridRowGroupHeader, double>("SublevelIndent", 20.0, inherits: false, BindingMode.OneWay, IsValidSublevelIndent);
		SublevelIndentProperty.Changed.AddClassHandler(delegate(DataGridRowGroupHeader x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnSublevelIndentChanged(e);
		});
		PressedMixin.Attach<DataGridRowGroupHeader>();
		InputElement.IsTabStopProperty.OverrideDefaultValue<DataGridRowGroupHeader>(defaultValue: false);
	}

	public DataGridRowGroupHeader()
	{
		AddHandler(InputElement.PointerPressedEvent, delegate(object? s, PointerPressedEventArgs e)
		{
			DataGridRowGroupHeader_PointerPressed(e);
		}, RoutingStrategies.Direct | RoutingStrategies.Bubble, handledEventsToo: true);
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		_rootElement = e.NameScope.Find<Panel>("PART_Root");
		_expanderButtonSubscription?.Dispose();
		_expanderButton = e.NameScope.Find<ToggleButton>("PART_ExpanderButton");
		if (_expanderButton != null)
		{
			EnsureExpanderButtonIsChecked();
			_expanderButtonSubscription = _expanderButton.GetObservable(ToggleButton.IsCheckedProperty).Skip(1).Subscribe(delegate(bool? v)
			{
				OnExpanderButtonIsCheckedChanged(v);
			});
		}
		_headerElement = e.NameScope.Find<DataGridRowHeader>("PART_RowHeader");
		if (_headerElement != null)
		{
			_headerElement.Owner = this;
			EnsureHeaderVisibility();
		}
		_indentSpacer = e.NameScope.Find<Control>("PART_IndentSpacer");
		if (_indentSpacer != null)
		{
			_indentSpacer.Width = _totalIndent;
		}
		_itemCountElement = e.NameScope.Find<TextBlock>("PART_ItemCountElement");
		_propertyNameElement = e.NameScope.Find<TextBlock>("PART_PropertyNameElement");
		UpdateTitleElements();
	}

	internal void ApplyHeaderStatus()
	{
		if (_headerElement != null && OwningGrid.AreRowHeadersVisible)
		{
			_headerElement.UpdatePseudoClasses();
		}
	}

	internal void UpdatePseudoClasses()
	{
		base.PseudoClasses.Set(":current", IsCurrent);
		if (RowGroupInfo?.CollectionViewGroup != null)
		{
			base.PseudoClasses.Set(":expanded", RowGroupInfo.IsVisible && RowGroupInfo.CollectionViewGroup.ItemCount > 0);
		}
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		if (OwningGrid == null)
		{
			return base.ArrangeOverride(finalSize);
		}
		Size result = base.ArrangeOverride(finalSize);
		if (_rootElement != null)
		{
			if (OwningGrid.AreRowGroupHeadersFrozen)
			{
				foreach (Control child in _rootElement.Children)
				{
					child.Clip = null;
				}
			}
			else
			{
				double num = 0.0;
				foreach (Control child2 in _rootElement.Children)
				{
					if (DataGridFrozenGrid.GetIsFrozen(child2) && child2.IsVisible)
					{
						TranslateTransform translateTransform = new TranslateTransform();
						translateTransform.X = Math.Round(OwningGrid.HorizontalOffset);
						child2.RenderTransform = translateTransform;
						double num2 = child2.Translate(this, new Point(child2.Bounds.Width, 0.0)).X - translateTransform.X;
						num = Math.Max(num, num2 + OwningGrid.HorizontalOffset);
					}
				}
				foreach (Control child3 in _rootElement.Children)
				{
					if (!DataGridFrozenGrid.GetIsFrozen(child3))
					{
						EnsureChildClip(child3, num);
					}
				}
			}
		}
		return result;
	}

	internal void ClearFrozenStates()
	{
		if (_rootElement == null)
		{
			return;
		}
		foreach (Control child in _rootElement.Children)
		{
			child.RenderTransform = null;
		}
	}

	private void DataGridRowGroupHeader_PointerPressed(PointerPressedEventArgs e)
	{
		if (OwningGrid == null)
		{
			return;
		}
		if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
		{
			if (OwningGrid.IsDoubleClickRecordsClickOnCall(this) && !e.Handled)
			{
				ToggleExpandCollapse(!RowGroupInfo.IsVisible, setCurrent: true);
				e.Handled = true;
				return;
			}
			if (!e.Handled && OwningGrid.IsTabStop)
			{
				OwningGrid.Focus();
			}
			e.Handled = OwningGrid.UpdateStateOnMouseLeftButtonDown(e, OwningGrid.CurrentColumnIndex, RowGroupInfo.Slot, allowEdit: false);
		}
		else if (e.GetCurrentPoint(this).Properties.IsRightButtonPressed)
		{
			if (!e.Handled)
			{
				OwningGrid.Focus();
			}
			e.Handled = OwningGrid.UpdateStateOnMouseRightButtonDown(e, OwningGrid.CurrentColumnIndex, RowGroupInfo.Slot, allowEdit: false);
		}
	}

	private void EnsureChildClip(Visual child, double frozenLeftEdge)
	{
		double x = child.Translate(this, new Point(0.0, 0.0)).X;
		if (frozenLeftEdge > x)
		{
			double num = Math.Round(frozenLeftEdge - x);
			RectangleGeometry rectangleGeometry = new RectangleGeometry();
			rectangleGeometry.Rect = new Rect(num, 0.0, Math.Max(0.0, child.Bounds.Width - num), child.Bounds.Height);
			child.Clip = rectangleGeometry;
		}
		else
		{
			child.Clip = null;
		}
	}

	internal void EnsureExpanderButtonIsChecked()
	{
		if (_expanderButton != null && RowGroupInfo != null && RowGroupInfo.CollectionViewGroup != null && RowGroupInfo.CollectionViewGroup.ItemCount != 0)
		{
			SetIsCheckedNoCallBack(RowGroupInfo.IsVisible);
		}
	}

	internal void EnsureHeaderVisibility()
	{
		if (_headerElement != null && OwningGrid != null)
		{
			_headerElement.IsVisible = OwningGrid.AreRowHeadersVisible;
		}
	}

	private void OnExpanderButtonIsCheckedChanged(bool? value)
	{
		if (!_areIsCheckedHandlersSuspended)
		{
			ToggleExpandCollapse(value == true, setCurrent: true);
		}
	}

	internal void LoadVisualsForDisplay()
	{
		EnsureExpanderButtonIsChecked();
		EnsureHeaderVisibility();
		UpdatePseudoClasses();
		ApplyHeaderStatus();
	}

	protected override void OnPointerEntered(PointerEventArgs e)
	{
		if (base.IsEnabled)
		{
			IsMouseOver = true;
			UpdatePseudoClasses();
		}
		base.OnPointerEntered(e);
	}

	protected override void OnPointerExited(PointerEventArgs e)
	{
		if (base.IsEnabled)
		{
			IsMouseOver = false;
			UpdatePseudoClasses();
		}
		base.OnPointerExited(e);
	}

	private void SetIsCheckedNoCallBack(bool value)
	{
		if (_expanderButton != null && _expanderButton.IsChecked != value)
		{
			_areIsCheckedHandlersSuspended = true;
			try
			{
				_expanderButton.IsChecked = value;
			}
			finally
			{
				_areIsCheckedHandlersSuspended = false;
			}
		}
	}

	internal void ToggleExpandCollapse(bool isVisible, bool setCurrent)
	{
		if (RowGroupInfo.CollectionViewGroup.ItemCount != 0)
		{
			if (OwningGrid == null)
			{
				RowGroupInfo.IsVisible = isVisible;
			}
			else if (RowGroupInfo.IsVisible != isVisible)
			{
				OwningGrid.OnRowGroupHeaderToggled(this, isVisible, setCurrent);
			}
			EnsureExpanderButtonIsChecked();
			UpdatePseudoClasses();
		}
	}

	internal void UpdateTitleElements()
	{
		if (_propertyNameElement != null)
		{
			string text = ((!string.IsNullOrWhiteSpace(PropertyName)) ? $"{PropertyName}:" : string.Empty);
			_propertyNameElement.Text = text;
		}
		if (_itemCountElement != null && RowGroupInfo != null && RowGroupInfo.CollectionViewGroup != null)
		{
			string format = ((RowGroupInfo.CollectionViewGroup.ItemCount != 1) ? "({0} Items)" : "({0} Item)");
			_itemCountElement.Text = string.Format(format, RowGroupInfo.CollectionViewGroup.ItemCount);
		}
	}
}
