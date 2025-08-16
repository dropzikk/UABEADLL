using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Avalonia.Automation.Peers;
using Avalonia.Controls.Generators;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Metadata;
using Avalonia.Styling;

namespace Avalonia.Controls;

[PseudoClasses(new string[] { ":empty", ":singleitem" })]
public class ItemsControl : TemplatedControl, IChildIndexProvider
{
	private static readonly FuncTemplate<Panel?> DefaultPanel = new FuncTemplate<Panel>(() => new StackPanel());

	public static readonly StyledProperty<ControlTheme?> ItemContainerThemeProperty = AvaloniaProperty.Register<ItemsControl, ControlTheme>("ItemContainerTheme");

	public static readonly DirectProperty<ItemsControl, int> ItemCountProperty = AvaloniaProperty.RegisterDirect("ItemCount", (ItemsControl o) => o.ItemCount, null, 0);

	public static readonly StyledProperty<ITemplate<Panel?>> ItemsPanelProperty = AvaloniaProperty.Register<ItemsControl, ITemplate<Panel>>("ItemsPanel", DefaultPanel);

	public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty = AvaloniaProperty.Register<ItemsControl, IEnumerable>("ItemsSource");

	public static readonly StyledProperty<IDataTemplate?> ItemTemplateProperty = AvaloniaProperty.Register<ItemsControl, IDataTemplate>("ItemTemplate");

	public static readonly StyledProperty<IBinding?> DisplayMemberBindingProperty = AvaloniaProperty.Register<ItemsControl, IBinding>("DisplayMemberBinding");

	private readonly ItemCollection _items = new ItemCollection();

	private int _itemCount;

	private ItemContainerGenerator? _itemContainerGenerator;

	private EventHandler<ChildIndexChangedEventArgs>? _childIndexChanged;

	private IDataTemplate? _displayMemberItemTemplate;

	private ItemsPresenter? _itemsPresenter;

	[AssignBinding]
	[InheritDataTypeFromItems("ItemsSource")]
	public IBinding? DisplayMemberBinding
	{
		get
		{
			return GetValue(DisplayMemberBindingProperty);
		}
		set
		{
			SetValue(DisplayMemberBindingProperty, value);
		}
	}

	public ItemContainerGenerator ItemContainerGenerator => _itemContainerGenerator ?? (_itemContainerGenerator = CreateItemContainerGenerator());

	[Content]
	public ItemCollection Items => _items;

	public ControlTheme? ItemContainerTheme
	{
		get
		{
			return GetValue(ItemContainerThemeProperty);
		}
		set
		{
			SetValue(ItemContainerThemeProperty, value);
		}
	}

	public int ItemCount
	{
		get
		{
			return _itemCount;
		}
		private set
		{
			if (SetAndRaise(ItemCountProperty, ref _itemCount, value))
			{
				UpdatePseudoClasses();
				_childIndexChanged?.Invoke(this, ChildIndexChangedEventArgs.TotalCountChanged);
			}
		}
	}

	public ITemplate<Panel?> ItemsPanel
	{
		get
		{
			return GetValue(ItemsPanelProperty);
		}
		set
		{
			SetValue(ItemsPanelProperty, value);
		}
	}

	public IEnumerable? ItemsSource
	{
		get
		{
			return GetValue(ItemsSourceProperty);
		}
		set
		{
			SetValue(ItemsSourceProperty, value);
		}
	}

	[InheritDataTypeFromItems("ItemsSource")]
	public IDataTemplate? ItemTemplate
	{
		get
		{
			return GetValue(ItemTemplateProperty);
		}
		set
		{
			SetValue(ItemTemplateProperty, value);
		}
	}

	public ItemsPresenter? Presenter { get; private set; }

	public Panel? ItemsPanelRoot => Presenter?.Panel;

	public ItemsSourceView ItemsView => _items;

	private protected bool WrapFocus { get; set; }

	protected static object DefaultRecycleKey { get; } = new object();

	event EventHandler<ChildIndexChangedEventArgs>? IChildIndexProvider.ChildIndexChanged
	{
		add
		{
			_childIndexChanged = (EventHandler<ChildIndexChangedEventArgs>)Delegate.Combine(_childIndexChanged, value);
		}
		remove
		{
			_childIndexChanged = (EventHandler<ChildIndexChangedEventArgs>)Delegate.Remove(_childIndexChanged, value);
		}
	}

	public event EventHandler<ContainerPreparedEventArgs>? ContainerPrepared;

	public event EventHandler<ContainerIndexChangedEventArgs>? ContainerIndexChanged;

	public event EventHandler<ContainerClearingEventArgs>? ContainerClearing;

	public ItemsControl()
	{
		UpdatePseudoClasses();
		_items.CollectionChanged += OnItemsViewCollectionChanged;
	}

	public Control? ContainerFromIndex(int index)
	{
		return Presenter?.ContainerFromIndex(index);
	}

	public Control? ContainerFromItem(object item)
	{
		int num = _items.IndexOf(item);
		if (num < 0)
		{
			return null;
		}
		return ContainerFromIndex(num);
	}

	public int IndexFromContainer(Control container)
	{
		return Presenter?.IndexFromContainer(container) ?? (-1);
	}

	public object? ItemFromContainer(Control container)
	{
		int num = IndexFromContainer(container);
		if (num < 0 || num >= _items.Count)
		{
			return null;
		}
		return _items[num];
	}

	public IEnumerable<Control> GetRealizedContainers()
	{
		return Presenter?.GetRealizedContainers() ?? Array.Empty<Control>();
	}

	public static ItemsControl? ItemsControlFromItemContaner(Control container)
	{
		for (Control control = container.Parent as Control; control != null; control = control.Parent as Control)
		{
			if (control is ItemsControl itemsControl)
			{
				if (itemsControl.IndexFromContainer(container) < 0)
				{
					return null;
				}
				return itemsControl;
			}
		}
		return null;
	}

	protected internal virtual Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
	{
		return new ContentPresenter();
	}

	protected internal virtual void PrepareContainerForItemOverride(Control container, object? item, int index)
	{
		if (container == item)
		{
			return;
		}
		IDataTemplate effectiveItemTemplate = GetEffectiveItemTemplate();
		if (container is HeaderedContentControl target)
		{
			SetIfUnset(target, ContentControl.ContentProperty, item);
			if (item is IHeadered headered)
			{
				SetIfUnset(target, HeaderedContentControl.HeaderProperty, headered.Header);
			}
			else if (!(item is Visual))
			{
				SetIfUnset(target, HeaderedContentControl.HeaderProperty, item);
			}
			if (effectiveItemTemplate != null)
			{
				SetIfUnset(target, HeaderedContentControl.HeaderTemplateProperty, effectiveItemTemplate);
			}
		}
		else if (container is ContentControl target2)
		{
			SetIfUnset(target2, ContentControl.ContentProperty, item);
			if (effectiveItemTemplate != null)
			{
				SetIfUnset(target2, ContentControl.ContentTemplateProperty, effectiveItemTemplate);
			}
		}
		else if (container is ContentPresenter target3)
		{
			SetIfUnset(target3, ContentPresenter.ContentProperty, item);
			if (effectiveItemTemplate != null)
			{
				SetIfUnset(target3, ContentPresenter.ContentTemplateProperty, effectiveItemTemplate);
			}
		}
		else if (container is ItemsControl target4)
		{
			if (effectiveItemTemplate != null)
			{
				SetIfUnset(target4, ItemTemplateProperty, effectiveItemTemplate);
			}
			ControlTheme itemContainerTheme = ItemContainerTheme;
			if (itemContainerTheme != null)
			{
				SetIfUnset(target4, ItemContainerThemeProperty, itemContainerTheme);
			}
		}
		if (container is HeaderedItemsControl headeredItemsControl)
		{
			SetIfUnset(headeredItemsControl, HeaderedItemsControl.HeaderProperty, item);
			SetIfUnset(headeredItemsControl, HeaderedItemsControl.HeaderTemplateProperty, effectiveItemTemplate);
			headeredItemsControl.PrepareItemContainer(this);
		}
		else if (container is HeaderedSelectingItemsControl headeredSelectingItemsControl)
		{
			SetIfUnset(headeredSelectingItemsControl, HeaderedSelectingItemsControl.HeaderProperty, item);
			SetIfUnset(headeredSelectingItemsControl, HeaderedSelectingItemsControl.HeaderTemplateProperty, effectiveItemTemplate);
			headeredSelectingItemsControl.PrepareItemContainer(this);
		}
	}

	protected internal virtual void ContainerForItemPreparedOverride(Control container, object? item, int index)
	{
	}

	protected virtual void ContainerIndexChangedOverride(Control container, int oldIndex, int newIndex)
	{
	}

	protected internal virtual void ClearContainerForItemOverride(Control container)
	{
		if (container is HeaderedContentControl headeredContentControl)
		{
			headeredContentControl.ClearValue(ContentControl.ContentProperty);
			headeredContentControl.ClearValue(HeaderedContentControl.HeaderProperty);
			headeredContentControl.ClearValue(HeaderedContentControl.HeaderTemplateProperty);
		}
		else if (container is ContentControl contentControl)
		{
			contentControl.ClearValue(ContentControl.ContentProperty);
			contentControl.ClearValue(ContentControl.ContentTemplateProperty);
		}
		else if (container is ContentPresenter contentPresenter)
		{
			contentPresenter.ClearValue(ContentPresenter.ContentProperty);
			contentPresenter.ClearValue(ContentPresenter.ContentTemplateProperty);
		}
		else if (container is ItemsControl itemsControl)
		{
			itemsControl.ClearValue(ItemTemplateProperty);
			itemsControl.ClearValue(ItemContainerThemeProperty);
		}
		if (container is HeaderedItemsControl headeredItemsControl)
		{
			headeredItemsControl.ClearValue(HeaderedItemsControl.HeaderProperty);
			headeredItemsControl.ClearValue(HeaderedItemsControl.HeaderTemplateProperty);
		}
		else if (container is HeaderedSelectingItemsControl headeredSelectingItemsControl)
		{
			headeredSelectingItemsControl.ClearValue(HeaderedSelectingItemsControl.HeaderProperty);
			headeredSelectingItemsControl.ClearValue(HeaderedSelectingItemsControl.HeaderTemplateProperty);
		}
	}

	protected internal virtual bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
	{
		return NeedsContainer<Control>(item, out recycleKey);
	}

	protected bool NeedsContainer<T>(object? item, out object? recycleKey) where T : Control
	{
		if (item is T)
		{
			recycleKey = null;
			return false;
		}
		recycleKey = DefaultRecycleKey;
		return true;
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);
		_itemsPresenter = e.NameScope.Find<ItemsPresenter>("PART_ItemsPresenter");
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		if (!e.Handled)
		{
			FocusManager focusManager = FocusManager.GetFocusManager(this);
			NavigationDirection? navigationDirection = e.Key.ToNavigationDirection();
			INavigableContainer navigableContainer = Presenter?.Panel as INavigableContainer;
			if (focusManager == null || navigableContainer == null || focusManager.GetFocusedElement() == null || !navigationDirection.HasValue || navigationDirection.Value.IsTab())
			{
				return;
			}
			for (Visual visual = focusManager.GetFocusedElement() as Visual; visual != null; visual = visual.VisualParent)
			{
				if (visual.VisualParent == navigableContainer && visual is IInputElement from)
				{
					IInputElement nextControl = GetNextControl(navigableContainer, navigationDirection.Value, from, WrapFocus);
					if (nextControl != null)
					{
						nextControl.Focus(NavigationMethod.Directional, e.KeyModifiers);
						e.Handled = true;
					}
					break;
				}
			}
		}
		base.OnKeyDown(e);
	}

	protected override AutomationPeer OnCreateAutomationPeer()
	{
		return new ItemsControlAutomationPeer(this);
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == ItemContainerThemeProperty && _itemContainerGenerator != null)
		{
			RefreshContainers();
		}
		else if (change.Property == ItemsSourceProperty)
		{
			_items.SetItemsSource(change.GetNewValue<IEnumerable>());
		}
		else if (change.Property == ItemTemplateProperty)
		{
			if (change.NewValue != null && DisplayMemberBinding != null)
			{
				throw new InvalidOperationException("Cannot set both DisplayMemberBinding and ItemTemplate.");
			}
			RefreshContainers();
		}
		else if (change.Property == DisplayMemberBindingProperty)
		{
			if (change.NewValue != null && ItemTemplate != null)
			{
				throw new InvalidOperationException("Cannot set both DisplayMemberBinding and ItemTemplate.");
			}
			_displayMemberItemTemplate = null;
			RefreshContainers();
		}
	}

	protected void RefreshContainers()
	{
		Presenter?.Refresh();
	}

	private protected virtual void OnItemsViewCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		if (!_items.IsReadOnly)
		{
			switch (e.Action)
			{
			case NotifyCollectionChangedAction.Add:
				AddControlItemsToLogicalChildren(e.NewItems);
				break;
			case NotifyCollectionChangedAction.Remove:
				RemoveControlItemsFromLogicalChildren(e.OldItems);
				break;
			}
		}
		ItemCount = ItemsView.Count;
	}

	[Obsolete]
	[EditorBrowsable(EditorBrowsableState.Never)]
	private protected virtual ItemContainerGenerator CreateItemContainerGenerator()
	{
		return new ItemContainerGenerator(this);
	}

	internal void AddLogicalChild(Control c)
	{
		if (!base.LogicalChildren.Contains(c))
		{
			base.LogicalChildren.Add(c);
		}
	}

	internal void RemoveLogicalChild(Control c)
	{
		base.LogicalChildren.Remove(c);
	}

	internal void RegisterItemsPresenter(ItemsPresenter presenter)
	{
		Presenter = presenter;
		_childIndexChanged?.Invoke(this, ChildIndexChangedEventArgs.ChildIndexesReset);
	}

	internal void PrepareItemContainer(Control container, object? item, int index)
	{
		ControlTheme itemContainerTheme = ItemContainerTheme;
		if (itemContainerTheme != null && !container.IsSet(StyledElement.ThemeProperty) && StyledElement.GetStyleKey(container) == itemContainerTheme.TargetType)
		{
			container.Theme = itemContainerTheme;
		}
		if (!(item is Control))
		{
			container.DataContext = item;
		}
		PrepareContainerForItemOverride(container, item, index);
	}

	internal void ItemContainerPrepared(Control container, object? item, int index)
	{
		ContainerForItemPreparedOverride(container, item, index);
		_childIndexChanged?.Invoke(this, new ChildIndexChangedEventArgs(container, index));
		this.ContainerPrepared?.Invoke(this, new ContainerPreparedEventArgs(container, index));
	}

	internal void ItemContainerIndexChanged(Control container, int oldIndex, int newIndex)
	{
		ContainerIndexChangedOverride(container, oldIndex, newIndex);
		_childIndexChanged?.Invoke(this, new ChildIndexChangedEventArgs(container, newIndex));
		this.ContainerIndexChanged?.Invoke(this, new ContainerIndexChangedEventArgs(container, oldIndex, newIndex));
	}

	internal void ClearItemContainer(Control container)
	{
		ClearContainerForItemOverride(container);
		this.ContainerClearing?.Invoke(this, new ContainerClearingEventArgs(container));
	}

	private void AddControlItemsToLogicalChildren(IEnumerable? items)
	{
		if (items == null)
		{
			return;
		}
		List<ILogical> list = null;
		foreach (object item2 in items)
		{
			if (item2 is Control item && !base.LogicalChildren.Contains(item))
			{
				if (list == null)
				{
					list = new List<ILogical>();
				}
				list.Add(item);
			}
		}
		if (list != null)
		{
			base.LogicalChildren.AddRange(list);
		}
	}

	private void SetIfUnset<T>(AvaloniaObject target, StyledProperty<T> property, T value)
	{
		if (!target.IsSet(property))
		{
			target.SetCurrentValue(property, value);
		}
	}

	private void RemoveControlItemsFromLogicalChildren(IEnumerable? items)
	{
		if (items == null)
		{
			return;
		}
		List<ILogical> list = null;
		foreach (object item2 in items)
		{
			if (item2 is Control item)
			{
				if (list == null)
				{
					list = new List<ILogical>();
				}
				list.Add(item);
			}
		}
		if (list != null)
		{
			base.LogicalChildren.RemoveAll(list);
		}
	}

	private IDataTemplate? GetEffectiveItemTemplate()
	{
		IDataTemplate itemTemplate = ItemTemplate;
		if (itemTemplate != null)
		{
			return itemTemplate;
		}
		if (_displayMemberItemTemplate == null)
		{
			IBinding binding = DisplayMemberBinding;
			if (binding != null)
			{
				_displayMemberItemTemplate = new FuncDataTemplate<object>((object? _, INameScope _) => new TextBlock { [!TextBlock.TextProperty] = binding });
			}
		}
		return _displayMemberItemTemplate;
	}

	private void UpdatePseudoClasses()
	{
		base.PseudoClasses.Set(":empty", ItemCount == 0);
		base.PseudoClasses.Set(":singleitem", ItemCount == 1);
	}

	protected static IInputElement? GetNextControl(INavigableContainer container, NavigationDirection direction, IInputElement? from, bool wrap)
	{
		IInputElement inputElement = from;
		while (true)
		{
			IInputElement control = container.GetControl(direction, inputElement, wrap);
			if (control == null)
			{
				return null;
			}
			if (control.Focusable && control.IsEffectivelyEnabled && control.IsEffectivelyVisible)
			{
				return control;
			}
			inputElement = control;
			if (inputElement == from)
			{
				break;
			}
			switch (direction)
			{
			case NavigationDirection.First:
				direction = NavigationDirection.Down;
				from = control;
				break;
			case NavigationDirection.Last:
				direction = NavigationDirection.Up;
				from = control;
				break;
			}
		}
		return null;
	}

	int IChildIndexProvider.GetChildIndex(ILogical child)
	{
		if (!(child is Control container))
		{
			return -1;
		}
		return IndexFromContainer(container);
	}

	bool IChildIndexProvider.TryGetTotalCount(out int count)
	{
		count = ItemsView.Count;
		return true;
	}
}
