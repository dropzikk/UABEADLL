using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Avalonia.Controls.Selection;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Metadata;
using Avalonia.Threading;

namespace Avalonia.Controls.Primitives;

public class SelectingItemsControl : ItemsControl
{
	private class UpdateState
	{
		private Optional<int> _selectedIndex;

		private Optional<object?> _selectedItem;

		private Optional<object?> _selectedValue;

		public int UpdateCount { get; set; }

		public Optional<ISelectionModel> Selection { get; set; }

		public Optional<IList?> SelectedItems { get; set; }

		public Optional<int> SelectedIndex
		{
			get
			{
				return _selectedIndex;
			}
			set
			{
				_selectedIndex = value;
				_selectedItem = default(Optional<object>);
			}
		}

		public Optional<object?> SelectedItem
		{
			get
			{
				return _selectedItem;
			}
			set
			{
				_selectedItem = value;
				_selectedIndex = default(Optional<int>);
			}
		}

		public Optional<object?> SelectedValue
		{
			get
			{
				return _selectedValue;
			}
			set
			{
				_selectedValue = value;
			}
		}
	}

	private class BindingHelper : StyledElement
	{
		public static readonly StyledProperty<object> ValueProperty = AvaloniaProperty.Register<BindingHelper, object>("Value");

		private IBinding? _lastBinding;

		public BindingHelper(IBinding binding)
		{
			UpdateBinding(binding);
		}

		public object Evaluate(object? dataContext)
		{
			if (!object.Equals(dataContext, base.DataContext))
			{
				base.DataContext = dataContext;
			}
			return GetValue(ValueProperty);
		}

		public void UpdateBinding(IBinding binding)
		{
			_lastBinding = binding;
			InstancedBinding instancedBinding = binding.Initiate(this, ValueProperty);
			if (instancedBinding == null)
			{
				throw new InvalidOperationException("Unable to create binding");
			}
			BindingOperations.Apply(this, ValueProperty, instancedBinding, null);
		}
	}

	public static readonly StyledProperty<bool> AutoScrollToSelectedItemProperty;

	public static readonly DirectProperty<SelectingItemsControl, int> SelectedIndexProperty;

	public static readonly DirectProperty<SelectingItemsControl, object?> SelectedItemProperty;

	public static readonly StyledProperty<object?> SelectedValueProperty;

	public static readonly StyledProperty<IBinding?> SelectedValueBindingProperty;

	protected static readonly DirectProperty<SelectingItemsControl, IList?> SelectedItemsProperty;

	protected static readonly DirectProperty<SelectingItemsControl, ISelectionModel> SelectionProperty;

	protected static readonly StyledProperty<SelectionMode> SelectionModeProperty;

	public static readonly StyledProperty<bool> IsSelectedProperty;

	public static readonly StyledProperty<bool> IsTextSearchEnabledProperty;

	public static readonly RoutedEvent<RoutedEventArgs> IsSelectedChangedEvent;

	public static readonly RoutedEvent<SelectionChangedEventArgs> SelectionChangedEvent;

	public static readonly StyledProperty<bool> WrapSelectionProperty;

	private string _textSearchTerm = string.Empty;

	private DispatcherTimer? _textSearchTimer;

	private ISelectionModel? _selection;

	private int _oldSelectedIndex;

	private object? _oldSelectedItem;

	private IList? _oldSelectedItems;

	private bool _ignoreContainerSelectionChanged;

	private UpdateState? _updateState;

	private bool _hasScrolledToSelectedItem;

	private BindingHelper? _bindingHelper;

	private bool _isSelectionChangeActive;

	public bool AutoScrollToSelectedItem
	{
		get
		{
			return GetValue(AutoScrollToSelectedItemProperty);
		}
		set
		{
			SetValue(AutoScrollToSelectedItemProperty, value);
		}
	}

	public int SelectedIndex
	{
		get
		{
			UpdateState? updateState = _updateState;
			if (updateState == null || !updateState.SelectedIndex.HasValue)
			{
				return Selection.SelectedIndex;
			}
			return _updateState.SelectedIndex.Value;
		}
		set
		{
			if (_updateState != null)
			{
				_updateState.SelectedIndex = value;
			}
			else
			{
				Selection.SelectedIndex = value;
			}
		}
	}

	public object? SelectedItem
	{
		get
		{
			UpdateState? updateState = _updateState;
			if (updateState == null || !updateState.SelectedItem.HasValue)
			{
				return Selection.SelectedItem;
			}
			return _updateState.SelectedItem.Value;
		}
		set
		{
			if (_updateState != null)
			{
				_updateState.SelectedItem = value;
			}
			else
			{
				Selection.SelectedItem = value;
			}
		}
	}

	[AssignBinding]
	[InheritDataTypeFromItems("ItemsSource")]
	public IBinding? SelectedValueBinding
	{
		get
		{
			return GetValue(SelectedValueBindingProperty);
		}
		set
		{
			SetValue(SelectedValueBindingProperty, value);
		}
	}

	public object? SelectedValue
	{
		get
		{
			return GetValue(SelectedValueProperty);
		}
		set
		{
			SetValue(SelectedValueProperty, value);
		}
	}

	protected IList? SelectedItems
	{
		get
		{
			UpdateState? updateState = _updateState;
			if (updateState != null && updateState.SelectedItems.HasValue)
			{
				return _updateState.SelectedItems.Value;
			}
			if (Selection is InternalSelectionModel internalSelectionModel)
			{
				return _oldSelectedItems = internalSelectionModel.WritableSelectedItems;
			}
			return null;
		}
		set
		{
			if (_updateState != null)
			{
				_updateState.SelectedItems = new Optional<IList>(value);
				return;
			}
			if (Selection is InternalSelectionModel internalSelectionModel)
			{
				internalSelectionModel.WritableSelectedItems = value;
				return;
			}
			throw new InvalidOperationException("Cannot set both Selection and SelectedItems.");
		}
	}

	protected ISelectionModel Selection
	{
		get
		{
			UpdateState? updateState = _updateState;
			if (updateState == null || !updateState.Selection.HasValue)
			{
				return GetOrCreateSelectionModel();
			}
			return _updateState.Selection.Value;
		}
		[param: AllowNull]
		set
		{
			if (value == null)
			{
				value = CreateDefaultSelectionModel();
			}
			if (_updateState != null)
			{
				_updateState.Selection = new Optional<ISelectionModel>(value);
			}
			else if (_selection != value)
			{
				if (value.Source != null && value.Source != base.ItemsView.Source)
				{
					throw new ArgumentException("The supplied ISelectionModel already has an assigned Source but this collection is different to the Items on the control.");
				}
				object[] array = _selection?.SelectedItems.ToArray();
				DeinitializeSelectionModel(_selection);
				_selection = value;
				if (array != null && array.Length != 0)
				{
					RaiseEvent(new SelectionChangedEventArgs(SelectionChangedEvent, array, Array.Empty<object>()));
				}
				InitializeSelectionModel(_selection);
				if (_oldSelectedItems != SelectedItems)
				{
					RaisePropertyChanged(SelectedItemsProperty, _oldSelectedItems, SelectedItems);
					_oldSelectedItems = SelectedItems;
				}
			}
		}
	}

	public bool IsTextSearchEnabled
	{
		get
		{
			return GetValue(IsTextSearchEnabledProperty);
		}
		set
		{
			SetValue(IsTextSearchEnabledProperty, value);
		}
	}

	public bool WrapSelection
	{
		get
		{
			return GetValue(WrapSelectionProperty);
		}
		set
		{
			SetValue(WrapSelectionProperty, value);
		}
	}

	protected SelectionMode SelectionMode
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

	protected bool AlwaysSelected => SelectionMode.HasAllFlags(SelectionMode.AlwaysSelected);

	public event EventHandler<SelectionChangedEventArgs>? SelectionChanged
	{
		add
		{
			AddHandler(SelectionChangedEvent, value);
		}
		remove
		{
			RemoveHandler(SelectionChangedEvent, value);
		}
	}

	public SelectingItemsControl()
	{
		((ItemCollection)base.ItemsView).SourceChanged += OnItemsViewSourceChanged;
	}

	static SelectingItemsControl()
	{
		AutoScrollToSelectedItemProperty = AvaloniaProperty.Register<SelectingItemsControl, bool>("AutoScrollToSelectedItem", defaultValue: true);
		SelectedIndexProperty = AvaloniaProperty.RegisterDirect("SelectedIndex", (SelectingItemsControl o) => o.SelectedIndex, delegate(SelectingItemsControl o, int v)
		{
			o.SelectedIndex = v;
		}, -1, BindingMode.TwoWay);
		SelectedItemProperty = AvaloniaProperty.RegisterDirect("SelectedItem", (SelectingItemsControl o) => o.SelectedItem, delegate(SelectingItemsControl o, object? v)
		{
			o.SelectedItem = v;
		}, null, BindingMode.TwoWay, enableDataValidation: true);
		SelectedValueProperty = AvaloniaProperty.Register<SelectingItemsControl, object>("SelectedValue", null, inherits: false, BindingMode.TwoWay);
		SelectedValueBindingProperty = AvaloniaProperty.Register<SelectingItemsControl, IBinding>("SelectedValueBinding");
		SelectedItemsProperty = AvaloniaProperty.RegisterDirect("SelectedItems", (SelectingItemsControl o) => o.SelectedItems, delegate(SelectingItemsControl o, IList? v)
		{
			o.SelectedItems = v;
		});
		SelectionProperty = AvaloniaProperty.RegisterDirect("Selection", (SelectingItemsControl o) => o.Selection, delegate(SelectingItemsControl o, ISelectionModel v)
		{
			o.Selection = v;
		});
		SelectionModeProperty = AvaloniaProperty.Register<SelectingItemsControl, SelectionMode>("SelectionMode", SelectionMode.Single);
		IsSelectedProperty = AvaloniaProperty.RegisterAttached<SelectingItemsControl, Control, bool>("IsSelected", defaultValue: false, inherits: false, BindingMode.TwoWay);
		IsTextSearchEnabledProperty = AvaloniaProperty.Register<SelectingItemsControl, bool>("IsTextSearchEnabled", defaultValue: false);
		IsSelectedChangedEvent = RoutedEvent.Register<SelectingItemsControl, RoutedEventArgs>("IsSelectedChanged", RoutingStrategies.Bubble);
		SelectionChangedEvent = RoutedEvent.Register<SelectingItemsControl, SelectionChangedEventArgs>("SelectionChanged", RoutingStrategies.Bubble);
		WrapSelectionProperty = AvaloniaProperty.Register<SelectingItemsControl, bool>("WrapSelection", defaultValue: false);
		IsSelectedChangedEvent.AddClassHandler(delegate(SelectingItemsControl x, RoutedEventArgs e)
		{
			x.ContainerSelectionChanged(e);
		});
	}

	public override void BeginInit()
	{
		base.BeginInit();
		BeginUpdating();
	}

	public override void EndInit()
	{
		base.EndInit();
		EndUpdating();
	}

	public void ScrollIntoView(int index)
	{
		base.Presenter?.ScrollIntoView(index);
	}

	public void ScrollIntoView(object item)
	{
		ScrollIntoView(base.ItemsView.IndexOf(item));
	}

	public static bool GetIsSelected(Control control)
	{
		return control.GetValue(IsSelectedProperty);
	}

	public static void SetIsSelected(Control control, bool value)
	{
		control.SetValue(IsSelectedProperty, value);
	}

	protected Control? GetContainerFromEventSource(object? eventSource)
	{
		for (Visual visual = eventSource as Visual; visual != null; visual = visual.VisualParent)
		{
			if (visual is Control control && control.Parent == this && IndexFromContainer(control) != -1)
			{
				return control;
			}
		}
		return null;
	}

	private protected override void OnItemsViewCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		base.OnItemsViewCollectionChanged(sender, e);
		if (AlwaysSelected && SelectedIndex == -1 && base.ItemCount > 0)
		{
			SelectedIndex = 0;
		}
	}

	protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnAttachedToVisualTree(e);
		int? num = Selection?.AnchorIndex;
		if (num.HasValue)
		{
			int valueOrDefault = num.GetValueOrDefault();
			AutoScrollToSelectedItemIfNecessary(valueOrDefault);
		}
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);
		if (AutoScrollToSelectedItem)
		{
			base.LayoutUpdated += ExecuteScrollWhenLayoutUpdated;
		}
		void ExecuteScrollWhenLayoutUpdated(object? sender, EventArgs e)
		{
			base.LayoutUpdated -= ExecuteScrollWhenLayoutUpdated;
			int? num = Selection?.AnchorIndex;
			if (num.HasValue)
			{
				int valueOrDefault = num.GetValueOrDefault();
				AutoScrollToSelectedItemIfNecessary(valueOrDefault);
			}
		}
	}

	protected internal override void PrepareContainerForItemOverride(Control container, object? item, int index)
	{
		GetOrCreateSelectionModel();
		base.PrepareContainerForItemOverride(container, item, index);
	}

	protected internal override void ContainerForItemPreparedOverride(Control container, object? item, int index)
	{
		base.ContainerForItemPreparedOverride(container, item, index);
		if (!container.IsSet(IsSelectedProperty))
		{
			MarkContainerSelected(container, Selection.IsSelected(index));
			return;
		}
		bool isSelected = GetIsSelected(container);
		UpdateSelection(index, isSelected, rangeModifier: false, toggleModifier: true);
	}

	protected override void ContainerIndexChangedOverride(Control container, int oldIndex, int newIndex)
	{
		base.ContainerIndexChangedOverride(container, oldIndex, newIndex);
		MarkContainerSelected(container, Selection.IsSelected(newIndex));
	}

	protected internal override void ClearContainerForItemOverride(Control element)
	{
		base.ClearContainerForItemOverride(element);
		try
		{
			_ignoreContainerSelectionChanged = true;
			element.ClearValue(IsSelectedProperty);
		}
		finally
		{
			_ignoreContainerSelectionChanged = false;
		}
	}

	protected override void OnDataContextBeginUpdate()
	{
		base.OnDataContextBeginUpdate();
		BeginUpdating();
	}

	protected override void OnDataContextEndUpdate()
	{
		base.OnDataContextEndUpdate();
		EndUpdating();
	}

	protected override void UpdateDataValidation(AvaloniaProperty property, BindingValueType state, Exception? error)
	{
		if (property == SelectedItemProperty)
		{
			DataValidationErrors.SetError(this, error);
		}
	}

	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (_selection != null)
		{
			_selection.Source = base.ItemsView.Source;
		}
	}

	protected override void OnTextInput(TextInputEventArgs e)
	{
		if (!e.Handled)
		{
			if (!IsTextSearchEnabled)
			{
				return;
			}
			StopTextSearchTimer();
			_textSearchTerm += e.Text;
			Control control = GetRealizedContainers().FirstOrDefault(Match);
			if (control != null)
			{
				SelectedIndex = IndexFromContainer(control);
			}
			StartTextSearchTimer();
			e.Handled = true;
		}
		base.OnTextInput(e);
		bool Match(Control container)
		{
			if (container != null && container.IsSet(TextSearch.TextProperty))
			{
				string? value = container.GetValue(TextSearch.TextProperty);
				if (value != null && value.StartsWith(_textSearchTerm, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			if (container is IContentControl contentControl)
			{
				object? content = contentControl.Content;
				if (content == null)
				{
					return false;
				}
				return content.ToString()?.StartsWith(_textSearchTerm, StringComparison.OrdinalIgnoreCase) == true;
			}
			return false;
		}
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == AutoScrollToSelectedItemProperty)
		{
			int? num = Selection?.AnchorIndex;
			if (num.HasValue)
			{
				int valueOrDefault = num.GetValueOrDefault();
				AutoScrollToSelectedItemIfNecessary(valueOrDefault);
			}
		}
		else if (change.Property == SelectionModeProperty && _selection != null)
		{
			SelectionMode newValue = change.GetNewValue<SelectionMode>();
			_selection.SingleSelect = !newValue.HasAllFlags(SelectionMode.Multiple);
		}
		else if (change.Property == WrapSelectionProperty)
		{
			base.WrapFocus = WrapSelection;
		}
		else if (change.Property == SelectedValueProperty)
		{
			if (!_isSelectionChangeActive)
			{
				if (_updateState != null)
				{
					_updateState.SelectedValue = change.NewValue;
				}
				else
				{
					SelectItemWithValue(change.NewValue);
				}
			}
		}
		else
		{
			if (!(change.Property == SelectedValueBindingProperty) || SelectedIndex == -1)
			{
				return;
			}
			IBinding newValue2 = change.GetNewValue<IBinding>();
			if (newValue2 != null)
			{
				object selectedItem = SelectedItem;
				try
				{
					_isSelectionChangeActive = true;
					if (_bindingHelper == null)
					{
						_bindingHelper = new BindingHelper(newValue2);
					}
					else
					{
						_bindingHelper.UpdateBinding(newValue2);
					}
					SelectedValue = _bindingHelper.Evaluate(selectedItem);
					return;
				}
				finally
				{
					_isSelectionChangeActive = false;
				}
			}
			SelectedValue = SelectedItem;
		}
	}

	protected bool MoveSelection(NavigationDirection direction, bool wrap = false, bool rangeModifier = false)
	{
		IInputElement eventSource = FocusManager.GetFocusManager(this)?.GetFocusedElement();
		Control from = GetContainerFromEventSource(eventSource) ?? ContainerFromIndex(Selection.AnchorIndex);
		return MoveSelection(from, direction, wrap, rangeModifier);
	}

	protected bool MoveSelection(Control? from, NavigationDirection direction, bool wrap = false, bool rangeModifier = false)
	{
		if (!(base.Presenter?.Panel is INavigableContainer container))
		{
			return false;
		}
		if (from == null)
		{
			direction = direction switch
			{
				NavigationDirection.Down => NavigationDirection.First, 
				NavigationDirection.Up => NavigationDirection.Last, 
				NavigationDirection.Right => NavigationDirection.First, 
				NavigationDirection.Left => NavigationDirection.Last, 
				_ => direction, 
			};
		}
		if (ItemsControl.GetNextControl(container, direction, from, wrap) is Control control)
		{
			int num = IndexFromContainer(control);
			if (num != -1)
			{
				UpdateSelection(num, select: true, rangeModifier);
				control.Focus();
				return true;
			}
		}
		return false;
	}

	protected void UpdateSelection(int index, bool select = true, bool rangeModifier = false, bool toggleModifier = false, bool rightButton = false, bool fromFocus = false)
	{
		if (index < 0 || index >= base.ItemCount)
		{
			return;
		}
		SelectionMode selectionMode = SelectionMode;
		bool flag = selectionMode.HasAllFlags(SelectionMode.Multiple);
		bool flag2 = toggleModifier || selectionMode.HasAllFlags(SelectionMode.Toggle);
		bool flag3 = flag && rangeModifier;
		if (!select)
		{
			Selection.Deselect(index);
			return;
		}
		if (rightButton)
		{
			if (!Selection.IsSelected(index))
			{
				SelectedIndex = index;
			}
			return;
		}
		if (flag3)
		{
			using (Selection.BatchUpdate())
			{
				Selection.Clear();
				Selection.SelectRange(Selection.AnchorIndex, index);
				return;
			}
		}
		if (!fromFocus && flag2)
		{
			if (flag)
			{
				if (Selection.IsSelected(index))
				{
					Selection.Deselect(index);
				}
				else
				{
					Selection.Select(index);
				}
			}
			else
			{
				SelectedIndex = ((SelectedIndex == index) ? (-1) : index);
			}
		}
		else if (!flag2)
		{
			using (Selection.BatchUpdate())
			{
				Selection.Clear();
				Selection.Select(index);
			}
		}
	}

	protected void UpdateSelection(Control container, bool select = true, bool rangeModifier = false, bool toggleModifier = false, bool rightButton = false, bool fromFocus = false)
	{
		int num = IndexFromContainer(container);
		if (num != -1)
		{
			UpdateSelection(num, select, rangeModifier, toggleModifier, rightButton, fromFocus);
		}
	}

	protected bool UpdateSelectionFromEventSource(object? eventSource, bool select = true, bool rangeModifier = false, bool toggleModifier = false, bool rightButton = false, bool fromFocus = false)
	{
		Control containerFromEventSource = GetContainerFromEventSource(eventSource);
		if (containerFromEventSource != null)
		{
			UpdateSelection(containerFromEventSource, select, rangeModifier, toggleModifier, rightButton, fromFocus);
			return true;
		}
		return false;
	}

	private ISelectionModel GetOrCreateSelectionModel()
	{
		if (_selection == null)
		{
			_selection = CreateDefaultSelectionModel();
			InitializeSelectionModel(_selection);
		}
		return _selection;
	}

	private void OnItemsViewSourceChanged(object? sender, EventArgs e)
	{
		if (_selection != null && _updateState == null)
		{
			_selection.Source = base.ItemsView.Source;
		}
	}

	private void OnSelectionModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == "AnchorIndex")
		{
			_hasScrolledToSelectedItem = false;
			int? num = Selection?.AnchorIndex;
			if (num.HasValue)
			{
				int valueOrDefault = num.GetValueOrDefault();
				KeyboardNavigation.SetTabOnceActiveElement(this, ContainerFromIndex(valueOrDefault));
				AutoScrollToSelectedItemIfNecessary(valueOrDefault);
			}
		}
		else if (e.PropertyName == "SelectedIndex" && _oldSelectedIndex != SelectedIndex)
		{
			RaisePropertyChanged(SelectedIndexProperty, _oldSelectedIndex, SelectedIndex);
			_oldSelectedIndex = SelectedIndex;
		}
		else if (e.PropertyName == "SelectedItem" && _oldSelectedItem != SelectedItem)
		{
			RaisePropertyChanged(SelectedItemProperty, _oldSelectedItem, SelectedItem);
			_oldSelectedItem = SelectedItem;
		}
		else if (e.PropertyName == "WritableSelectedItems" && _oldSelectedItems != (Selection as InternalSelectionModel)?.SelectedItems)
		{
			RaisePropertyChanged(SelectedItemsProperty, _oldSelectedItems, SelectedItems);
			_oldSelectedItems = SelectedItems;
		}
		else if (e.PropertyName == "Source")
		{
			ClearValue(SelectedValueProperty);
		}
	}

	private void OnSelectionModelSelectionChanged(object? sender, SelectionModelSelectionChangedEventArgs e)
	{
		foreach (int selectedIndex in e.SelectedIndexes)
		{
			Mark(selectedIndex, selected: true);
		}
		foreach (int deselectedIndex in e.DeselectedIndexes)
		{
			Mark(deselectedIndex, selected: false);
		}
		if (!_isSelectionChangeActive)
		{
			UpdateSelectedValueFromItem();
		}
		if (BuildEventRoute(SelectionChangedEvent).HasHandlers)
		{
			SelectionChangedEventArgs e2 = new SelectionChangedEventArgs(SelectionChangedEvent, e.DeselectedItems.ToArray(), e.SelectedItems.ToArray());
			RaiseEvent(e2);
		}
		void Mark(int index, bool selected)
		{
			Control control = ContainerFromIndex(index);
			if (control != null)
			{
				MarkContainerSelected(control, selected);
			}
		}
	}

	private void OnSelectionModelLostSelection(object? sender, EventArgs e)
	{
		if (AlwaysSelected && base.ItemsView.Count > 0)
		{
			SelectedIndex = 0;
		}
	}

	private void SelectItemWithValue(object? value)
	{
		if (base.ItemCount == 0 || _isSelectionChangeActive)
		{
			return;
		}
		try
		{
			_isSelectionChangeActive = true;
			object obj = FindItemWithValue(value);
			if (obj != AvaloniaProperty.UnsetValue)
			{
				SelectedItem = obj;
			}
			else
			{
				SelectedItem = null;
			}
		}
		finally
		{
			_isSelectionChangeActive = false;
		}
	}

	private object? FindItemWithValue(object? value)
	{
		if (base.ItemCount == 0 || value == null)
		{
			return AvaloniaProperty.UnsetValue;
		}
		ItemsSourceView itemsView = base.ItemsView;
		IBinding selectedValueBinding = SelectedValueBinding;
		if (selectedValueBinding == null)
		{
			if (itemsView.IndexOf(value) >= 0)
			{
				return value;
			}
			return AvaloniaProperty.UnsetValue;
		}
		if (_bindingHelper == null)
		{
			_bindingHelper = new BindingHelper(selectedValueBinding);
		}
		foreach (object item in itemsView)
		{
			if (_bindingHelper.Evaluate(item).Equals(value))
			{
				return item;
			}
		}
		return AvaloniaProperty.UnsetValue;
	}

	private void UpdateSelectedValueFromItem()
	{
		if (_isSelectionChangeActive)
		{
			return;
		}
		IBinding selectedValueBinding = SelectedValueBinding;
		object selectedItem = SelectedItem;
		if (selectedValueBinding == null || selectedItem == null)
		{
			try
			{
				_isSelectionChangeActive = true;
				SelectedValue = selectedItem;
				return;
			}
			finally
			{
				_isSelectionChangeActive = false;
			}
		}
		if (_bindingHelper == null)
		{
			_bindingHelper = new BindingHelper(selectedValueBinding);
		}
		try
		{
			_isSelectionChangeActive = true;
			SelectedValue = _bindingHelper.Evaluate(selectedItem);
		}
		finally
		{
			_isSelectionChangeActive = false;
		}
	}

	private void AutoScrollToSelectedItemIfNecessary(int anchorIndex)
	{
		if (AutoScrollToSelectedItem && !_hasScrolledToSelectedItem && base.Presenter != null && anchorIndex >= 0 && base.IsAttachedToVisualTree)
		{
			Dispatcher.UIThread.Post(delegate(object? state)
			{
				ScrollIntoView((int)state);
				_hasScrolledToSelectedItem = true;
			}, anchorIndex);
		}
	}

	private void ContainerSelectionChanged(RoutedEventArgs e)
	{
		if (!_ignoreContainerSelectionChanged && e.Source is Control control && control.Parent == this)
		{
			int num = IndexFromContainer(control);
			if (num >= 0)
			{
				if (GetIsSelected(control))
				{
					Selection.Select(num);
				}
				else
				{
					Selection.Deselect(num);
				}
			}
		}
		if (e.Source != this)
		{
			e.Handled = true;
		}
	}

	private void MarkContainerSelected(Control container, bool selected)
	{
		_ignoreContainerSelectionChanged = true;
		try
		{
			container.SetCurrentValue(IsSelectedProperty, selected);
		}
		finally
		{
			_ignoreContainerSelectionChanged = false;
		}
	}

	private void UpdateContainerSelection()
	{
		Panel panel = base.Presenter?.Panel;
		if (panel == null)
		{
			return;
		}
		foreach (Control child in panel.Children)
		{
			MarkContainerSelected(child, Selection.IsSelected(IndexFromContainer(child)));
		}
	}

	private ISelectionModel CreateDefaultSelectionModel()
	{
		return new InternalSelectionModel
		{
			SingleSelect = !SelectionMode.HasAllFlags(SelectionMode.Multiple)
		};
	}

	private void InitializeSelectionModel(ISelectionModel model)
	{
		if (_updateState == null)
		{
			model.Source = base.ItemsView.Source;
		}
		model.PropertyChanged += OnSelectionModelPropertyChanged;
		model.SelectionChanged += OnSelectionModelSelectionChanged;
		model.LostSelection += OnSelectionModelLostSelection;
		if (model.SingleSelect)
		{
			SelectionMode &= ~SelectionMode.Multiple;
		}
		else
		{
			SelectionMode |= SelectionMode.Multiple;
		}
		_oldSelectedIndex = model.SelectedIndex;
		_oldSelectedItem = model.SelectedItem;
		if (AlwaysSelected && model.Count == 0)
		{
			model.SelectedIndex = 0;
		}
		UpdateContainerSelection();
		if (SelectedIndex != -1)
		{
			RaiseEvent(new SelectionChangedEventArgs(SelectionChangedEvent, Array.Empty<object>(), Selection.SelectedItems.ToArray()));
		}
	}

	private void DeinitializeSelectionModel(ISelectionModel? model)
	{
		if (model != null)
		{
			model.PropertyChanged -= OnSelectionModelPropertyChanged;
			model.SelectionChanged -= OnSelectionModelSelectionChanged;
		}
	}

	private void BeginUpdating()
	{
		if (_updateState == null)
		{
			_updateState = new UpdateState();
		}
		_updateState.UpdateCount++;
	}

	private void EndUpdating()
	{
		if (_updateState == null || --_updateState.UpdateCount != 0)
		{
			return;
		}
		UpdateState updateState = _updateState;
		_updateState = null;
		if (updateState.Selection.HasValue)
		{
			Selection = updateState.Selection.Value;
		}
		if (_selection is InternalSelectionModel internalSelectionModel)
		{
			internalSelectionModel.Update(base.ItemsView.Source, updateState.SelectedItems);
		}
		else
		{
			if (updateState.SelectedItems.HasValue)
			{
				SelectedItems = updateState.SelectedItems.Value;
			}
			Selection.Source = base.ItemsView.Source;
		}
		if (updateState.SelectedValue.HasValue)
		{
			object obj = FindItemWithValue(updateState.SelectedValue.Value);
			if (obj != AvaloniaProperty.UnsetValue)
			{
				updateState.SelectedItem = obj;
			}
		}
		if (updateState.SelectedIndex.HasValue)
		{
			SelectedIndex = updateState.SelectedIndex.Value;
		}
		else if (updateState.SelectedItem.HasValue)
		{
			SelectedItem = updateState.SelectedItem.Value;
		}
	}

	private void StartTextSearchTimer()
	{
		_textSearchTimer = new DispatcherTimer
		{
			Interval = TimeSpan.FromSeconds(1.0)
		};
		_textSearchTimer.Tick += TextSearchTimer_Tick;
		_textSearchTimer.Start();
	}

	private void StopTextSearchTimer()
	{
		if (_textSearchTimer != null)
		{
			_textSearchTimer.Tick -= TextSearchTimer_Tick;
			_textSearchTimer.Stop();
			_textSearchTimer = null;
		}
	}

	private void TextSearchTimer_Tick(object? sender, EventArgs e)
	{
		_textSearchTerm = string.Empty;
		StopTextSearchTimer();
	}
}
