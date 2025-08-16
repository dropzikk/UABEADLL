using System;
using System.Linq;
using Avalonia.Automation.Peers;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Reactive;
using Avalonia.VisualTree;

namespace Avalonia.Controls;

[TemplatePart("PART_Popup", typeof(Popup))]
[PseudoClasses(new string[] { ":dropdownopen", ":pressed" })]
public class ComboBox : SelectingItemsControl
{
	internal const string pcDropdownOpen = ":dropdownopen";

	internal const string pcPressed = ":pressed";

	private static readonly FuncTemplate<Panel?> DefaultPanel;

	public static readonly StyledProperty<bool> IsDropDownOpenProperty;

	public static readonly StyledProperty<double> MaxDropDownHeightProperty;

	public static readonly DirectProperty<ComboBox, object?> SelectionBoxItemProperty;

	public static readonly StyledProperty<string?> PlaceholderTextProperty;

	public static readonly StyledProperty<IBrush?> PlaceholderForegroundProperty;

	public static readonly StyledProperty<HorizontalAlignment> HorizontalContentAlignmentProperty;

	public static readonly StyledProperty<VerticalAlignment> VerticalContentAlignmentProperty;

	private Popup? _popup;

	private object? _selectionBoxItem;

	private readonly CompositeDisposable _subscriptionsOnOpen = new CompositeDisposable();

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

	public double MaxDropDownHeight
	{
		get
		{
			return GetValue(MaxDropDownHeightProperty);
		}
		set
		{
			SetValue(MaxDropDownHeightProperty, value);
		}
	}

	public object? SelectionBoxItem
	{
		get
		{
			return _selectionBoxItem;
		}
		protected set
		{
			SetAndRaise(SelectionBoxItemProperty, ref _selectionBoxItem, value);
		}
	}

	public string? PlaceholderText
	{
		get
		{
			return GetValue(PlaceholderTextProperty);
		}
		set
		{
			SetValue(PlaceholderTextProperty, value);
		}
	}

	public IBrush? PlaceholderForeground
	{
		get
		{
			return GetValue(PlaceholderForegroundProperty);
		}
		set
		{
			SetValue(PlaceholderForegroundProperty, value);
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

	public event EventHandler? DropDownClosed;

	public event EventHandler? DropDownOpened;

	static ComboBox()
	{
		DefaultPanel = new FuncTemplate<Panel>(() => new VirtualizingStackPanel());
		IsDropDownOpenProperty = AvaloniaProperty.Register<ComboBox, bool>("IsDropDownOpen", defaultValue: false);
		MaxDropDownHeightProperty = AvaloniaProperty.Register<ComboBox, double>("MaxDropDownHeight", 200.0);
		SelectionBoxItemProperty = AvaloniaProperty.RegisterDirect("SelectionBoxItem", (ComboBox o) => o.SelectionBoxItem);
		PlaceholderTextProperty = AvaloniaProperty.Register<ComboBox, string>("PlaceholderText");
		PlaceholderForegroundProperty = AvaloniaProperty.Register<ComboBox, IBrush>("PlaceholderForeground");
		HorizontalContentAlignmentProperty = ContentControl.HorizontalContentAlignmentProperty.AddOwner<ComboBox>();
		VerticalContentAlignmentProperty = ContentControl.VerticalContentAlignmentProperty.AddOwner<ComboBox>();
		ItemsControl.ItemsPanelProperty.OverrideDefaultValue<ComboBox>(DefaultPanel);
		InputElement.FocusableProperty.OverrideDefaultValue<ComboBox>(defaultValue: true);
		SelectingItemsControl.IsTextSearchEnabledProperty.OverrideDefaultValue<ComboBox>(defaultValue: true);
	}

	protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnAttachedToVisualTree(e);
		UpdateSelectionBoxItem(base.SelectedItem);
	}

	protected internal override void InvalidateMirrorTransform()
	{
		base.InvalidateMirrorTransform();
		UpdateFlowDirection();
	}

	protected internal override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
	{
		return new ComboBoxItem();
	}

	protected internal override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
	{
		return NeedsContainer<ComboBoxItem>(item, out recycleKey);
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		base.OnKeyDown(e);
		if (e.Handled)
		{
			return;
		}
		if ((e.Key == Key.F4 && !e.KeyModifiers.HasAllFlags(KeyModifiers.Alt)) || ((e.Key == Key.Down || e.Key == Key.Up) && e.KeyModifiers.HasAllFlags(KeyModifiers.Alt)))
		{
			SetCurrentValue(IsDropDownOpenProperty, !IsDropDownOpen);
			e.Handled = true;
		}
		else if (IsDropDownOpen && e.Key == Key.Escape)
		{
			SetCurrentValue(IsDropDownOpenProperty, value: false);
			e.Handled = true;
		}
		else if (!IsDropDownOpen && (e.Key == Key.Return || e.Key == Key.Space))
		{
			SetCurrentValue(IsDropDownOpenProperty, value: true);
			e.Handled = true;
		}
		else if (IsDropDownOpen && (e.Key == Key.Return || e.Key == Key.Space))
		{
			SelectFocusedItem();
			SetCurrentValue(IsDropDownOpenProperty, value: false);
			e.Handled = true;
		}
		else if (!IsDropDownOpen)
		{
			if (e.Key == Key.Down)
			{
				SelectNext();
				e.Handled = true;
			}
			else if (e.Key == Key.Up)
			{
				SelectPrevious();
				e.Handled = true;
			}
		}
		else if (IsDropDownOpen && base.SelectedIndex < 0 && base.ItemCount > 0 && (e.Key == Key.Up || e.Key == Key.Down) && base.IsFocused)
		{
			Control control = base.Presenter?.Panel?.Children.FirstOrDefault((Control c) => CanFocus(c));
			if (control != null)
			{
				e.Handled = control.Focus(NavigationMethod.Directional);
			}
		}
	}

	protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
	{
		base.OnPointerWheelChanged(e);
		if (e.Handled)
		{
			return;
		}
		if (!IsDropDownOpen)
		{
			if (base.IsFocused)
			{
				if (e.Delta.Y < 0.0)
				{
					SelectNext();
				}
				else
				{
					SelectPrevious();
				}
				e.Handled = true;
			}
		}
		else
		{
			e.Handled = true;
		}
	}

	protected override void OnPointerPressed(PointerPressedEventArgs e)
	{
		base.OnPointerPressed(e);
		if (!e.Handled && e.Source is Visual visual)
		{
			Popup? popup = _popup;
			if (popup != null && popup.IsInsidePopup(visual))
			{
				e.Handled = true;
				return;
			}
		}
		base.PseudoClasses.Set(":pressed", value: true);
	}

	protected override void OnPointerReleased(PointerReleasedEventArgs e)
	{
		if (!e.Handled && e.Source is Visual visual)
		{
			Popup? popup = _popup;
			if (popup != null && popup.IsInsidePopup(visual))
			{
				if (UpdateSelectionFromEventSource(e.Source))
				{
					_popup?.Close();
					e.Handled = true;
				}
			}
			else
			{
				SetCurrentValue(IsDropDownOpenProperty, !IsDropDownOpen);
				e.Handled = true;
			}
		}
		base.PseudoClasses.Set(":pressed", value: false);
		base.OnPointerReleased(e);
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		if (_popup != null)
		{
			_popup.Opened -= PopupOpened;
			_popup.Closed -= PopupClosed;
		}
		_popup = e.NameScope.Get<Popup>("PART_Popup");
		_popup.Opened += PopupOpened;
		_popup.Closed += PopupClosed;
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		if (change.Property == SelectingItemsControl.SelectedItemProperty)
		{
			UpdateSelectionBoxItem(change.NewValue);
			TryFocusSelectedItem();
		}
		else if (change.Property == IsDropDownOpenProperty)
		{
			base.PseudoClasses.Set(":dropdownopen", change.GetNewValue<bool>());
		}
		base.OnPropertyChanged(change);
	}

	protected override AutomationPeer OnCreateAutomationPeer()
	{
		return new ComboBoxAutomationPeer(this);
	}

	internal void ItemFocused(ComboBoxItem dropDownItem)
	{
		if (IsDropDownOpen && dropDownItem.IsFocused && dropDownItem.IsArrangeValid)
		{
			dropDownItem.BringIntoView();
		}
	}

	private void PopupClosed(object? sender, EventArgs e)
	{
		_subscriptionsOnOpen.Clear();
		if (CanFocus(this))
		{
			Focus();
		}
		this.DropDownClosed?.Invoke(this, EventArgs.Empty);
	}

	private void PopupOpened(object? sender, EventArgs e)
	{
		TryFocusSelectedItem();
		_subscriptionsOnOpen.Clear();
		this.GetObservable(Visual.IsVisibleProperty).Subscribe(IsVisibleChanged).DisposeWith(_subscriptionsOnOpen);
		foreach (Control item in this.GetVisualAncestors().OfType<Control>())
		{
			item.GetObservable(Visual.IsVisibleProperty).Subscribe(IsVisibleChanged).DisposeWith(_subscriptionsOnOpen);
		}
		UpdateFlowDirection();
		this.DropDownOpened?.Invoke(this, EventArgs.Empty);
	}

	private void IsVisibleChanged(bool isVisible)
	{
		if (!isVisible && IsDropDownOpen)
		{
			SetCurrentValue(IsDropDownOpenProperty, value: false);
		}
	}

	private void TryFocusSelectedItem()
	{
		int selectedIndex = base.SelectedIndex;
		if (IsDropDownOpen && selectedIndex != -1)
		{
			Control control = ContainerFromIndex(selectedIndex);
			if (control == null && base.SelectedIndex != -1)
			{
				ScrollIntoView(base.Selection.SelectedIndex);
				control = ContainerFromIndex(selectedIndex);
			}
			if (control != null && CanFocus(control))
			{
				control.Focus();
			}
		}
	}

	private bool CanFocus(Control control)
	{
		if (control.Focusable && control.IsEffectivelyEnabled)
		{
			return control.IsVisible;
		}
		return false;
	}

	private void UpdateSelectionBoxItem(object? item)
	{
		if (item is IContentControl contentControl)
		{
			item = contentControl.Content;
		}
		if (item is Control control)
		{
			if (base.VisualRoot != null)
			{
				control.Measure(Size.Infinity);
				SelectionBoxItem = new Rectangle
				{
					Width = control.DesiredSize.Width,
					Height = control.DesiredSize.Height,
					Fill = new VisualBrush
					{
						Visual = control,
						Stretch = Stretch.None,
						AlignmentX = AlignmentX.Left
					}
				};
			}
			UpdateFlowDirection();
			return;
		}
		if (base.ItemTemplate == null)
		{
			IBinding binding = base.DisplayMemberBinding;
			if (binding != null)
			{
				Control selectionBoxItem = new FuncDataTemplate<object>((object? _, INameScope _) => new TextBlock
				{
					[StyledElement.DataContextProperty] = item,
					[!TextBlock.TextProperty] = binding
				}).Build(item);
				SelectionBoxItem = selectionBoxItem;
				return;
			}
		}
		SelectionBoxItem = item;
	}

	private void UpdateFlowDirection()
	{
		if (SelectionBoxItem is Rectangle rectangle)
		{
			Visual visual = (rectangle.Fill as VisualBrush)?.Visual;
			if (visual != null)
			{
				FlowDirection flowDirection = visual.VisualParent?.FlowDirection ?? FlowDirection.LeftToRight;
				rectangle.FlowDirection = flowDirection;
			}
		}
	}

	private void SelectFocusedItem()
	{
		foreach (Control realizedContainer in GetRealizedContainers())
		{
			if (realizedContainer.IsFocused)
			{
				base.SelectedIndex = IndexFromContainer(realizedContainer);
				break;
			}
		}
	}

	private void SelectNext()
	{
		MoveSelection(base.SelectedIndex, 1, base.WrapSelection);
	}

	private void SelectPrevious()
	{
		MoveSelection(base.SelectedIndex, -1, base.WrapSelection);
	}

	private void MoveSelection(int startIndex, int step, bool wrap)
	{
		int itemCount = base.ItemCount;
		for (int i = startIndex + step; i != startIndex; i += step)
		{
			if (i < 0 || i >= itemCount)
			{
				if (!wrap)
				{
					break;
				}
				if (i < 0)
				{
					i += itemCount;
				}
				else if (i >= itemCount)
				{
					i %= itemCount;
				}
			}
			object? o2 = base.ItemsView[i];
			Control o3 = ContainerFromIndex(i);
			if (IsSelectable(o2) && IsSelectable(o3))
			{
				base.SelectedIndex = i;
				break;
			}
		}
		static bool IsSelectable(object? o)
		{
			return (o as AvaloniaObject)?.GetValue(InputElement.IsEnabledProperty) ?? true;
		}
	}

	public void Clear()
	{
		base.SelectedItem = null;
		base.SelectedIndex = -1;
	}
}
