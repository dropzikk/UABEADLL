using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Avalonia.Collections;
using Avalonia.Controls.Generators;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace Avalonia.Controls;

public class TreeView : ItemsControl, ICustomKeyboardNavigation
{
	public static readonly StyledProperty<bool> AutoScrollToSelectedItemProperty;

	public static readonly DirectProperty<TreeView, object?> SelectedItemProperty;

	public static readonly DirectProperty<TreeView, IList> SelectedItemsProperty;

	public static readonly StyledProperty<SelectionMode> SelectionModeProperty;

	private static readonly IList Empty;

	private object? _selectedItem;

	private IList? _selectedItems;

	private bool _syncingSelectedItems;

	public new TreeItemContainerGenerator ItemContainerGenerator => (TreeItemContainerGenerator)base.ItemContainerGenerator;

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

	public SelectionMode SelectionMode
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

	public object? SelectedItem
	{
		get
		{
			return _selectedItem;
		}
		set
		{
			IList selectedItems = SelectedItems;
			SetAndRaise(SelectedItemProperty, ref _selectedItem, value);
			if (value != null)
			{
				if (selectedItems.Count != 1 || selectedItems[0] != value)
				{
					SelectSingleItem(value);
				}
			}
			else if (SelectedItems.Count > 0)
			{
				SelectedItems.Clear();
			}
		}
	}

	public IList SelectedItems
	{
		get
		{
			if (_selectedItems == null)
			{
				_selectedItems = new AvaloniaList<object>();
				SubscribeToSelectedItems();
			}
			return _selectedItems;
		}
		[param: AllowNull]
		set
		{
			if ((value != null && value.IsFixedSize) || (value != null && value.IsReadOnly))
			{
				throw new NotSupportedException("Cannot use a fixed size or read-only collection as SelectedItems.");
			}
			UnsubscribeFromSelectedItems();
			_selectedItems = value ?? new AvaloniaList<object>();
			SubscribeToSelectedItems();
		}
	}

	public event EventHandler<SelectionChangedEventArgs>? SelectionChanged
	{
		add
		{
			AddHandler(SelectingItemsControl.SelectionChangedEvent, value);
		}
		remove
		{
			RemoveHandler(SelectingItemsControl.SelectionChangedEvent, value);
		}
	}

	static TreeView()
	{
		AutoScrollToSelectedItemProperty = SelectingItemsControl.AutoScrollToSelectedItemProperty.AddOwner<TreeView>();
		SelectedItemProperty = SelectingItemsControl.SelectedItemProperty.AddOwner((TreeView o) => o.SelectedItem, delegate(TreeView o, object? v)
		{
			o.SelectedItem = v;
		});
		SelectedItemsProperty = AvaloniaProperty.RegisterDirect("SelectedItems", (TreeView o) => o.SelectedItems, delegate(TreeView o, IList v)
		{
			o.SelectedItems = v;
		});
		SelectionModeProperty = ListBox.SelectionModeProperty.AddOwner<TreeView>();
		Empty = Array.Empty<object>();
		SelectingItemsControl.IsSelectedChangedEvent.AddClassHandler(delegate(TreeView x, RoutedEventArgs e)
		{
			x.ContainerSelectionChanged(e);
		});
	}

	public void ExpandSubTree(TreeViewItem item)
	{
		item.IsExpanded = true;
		if (item.Presenter?.Panel == null)
		{
			(this.GetVisualRoot() as ILayoutRoot)?.LayoutManager.ExecuteLayoutPass();
		}
		Panel panel = item.Presenter?.Panel;
		if (panel == null)
		{
			return;
		}
		foreach (Control child in panel.Children)
		{
			if (child is TreeViewItem item2)
			{
				ExpandSubTree(item2);
			}
		}
	}

	public void CollapseSubTree(TreeViewItem item)
	{
		item.IsExpanded = false;
		if (item.Presenter?.Panel == null)
		{
			return;
		}
		foreach (Control child in item.Presenter.Panel.Children)
		{
			if (child is TreeViewItem item2)
			{
				CollapseSubTree(item2);
			}
		}
	}

	public void SelectAll()
	{
		List<object> allItems = new List<object>();
		AddItems(this);
		SynchronizeItems(SelectedItems, allItems);
		void AddItems(ItemsControl itemsControl)
		{
			foreach (object item in itemsControl.ItemsView)
			{
				allItems.Add(item);
			}
			foreach (Control realizedContainer in itemsControl.GetRealizedContainers())
			{
				if (realizedContainer is ItemsControl itemsControl2)
				{
					AddItems(itemsControl2);
				}
			}
		}
	}

	public void UnselectAll()
	{
		SelectedItems.Clear();
	}

	public IEnumerable<Control> GetRealizedTreeContainers()
	{
		return GetRealizedContainers(this);
		static IEnumerable<Control> GetRealizedContainers(ItemsControl itemsControl)
		{
			foreach (Control container in itemsControl.GetRealizedContainers())
			{
				yield return container;
				if (container is ItemsControl itemsControl2)
				{
					foreach (Control item in GetRealizedContainers(itemsControl2))
					{
						yield return item;
					}
				}
			}
		}
	}

	public Control? TreeContainerFromItem(object item)
	{
		return TreeContainerFromItem(this, item);
		static Control? TreeContainerFromItem(ItemsControl itemsControl, object item)
		{
			Control control = itemsControl.ContainerFromItem(item);
			if (control != null)
			{
				return control;
			}
			foreach (Control realizedContainer in itemsControl.GetRealizedContainers())
			{
				if (realizedContainer is ItemsControl itemsControl2)
				{
					Control control2 = TreeContainerFromItem(itemsControl2, item);
					if (control2 != null)
					{
						return control2;
					}
				}
			}
			return null;
		}
	}

	public object? TreeItemFromContainer(Control container)
	{
		return TreeItemFromContainer(this, container);
		static object? TreeItemFromContainer(ItemsControl itemsControl, Control container)
		{
			object obj = itemsControl.ItemFromContainer(container);
			if (obj != null)
			{
				return obj;
			}
			foreach (Control realizedContainer in itemsControl.GetRealizedContainers())
			{
				if (realizedContainer is ItemsControl itemsControl2)
				{
					object obj2 = TreeItemFromContainer(itemsControl2, container);
					if (obj2 != null)
					{
						return obj2;
					}
				}
			}
			return null;
		}
	}

	private protected override void OnItemsViewCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		base.OnItemsViewCollectionChanged(sender, e);
		switch (e.Action)
		{
		case NotifyCollectionChangedAction.Remove:
		case NotifyCollectionChangedAction.Replace:
		{
			foreach (object oldItem in e.OldItems)
			{
				SelectedItems.Remove(oldItem);
			}
			break;
		}
		case NotifyCollectionChangedAction.Reset:
			SelectedItems.Clear();
			break;
		}
	}

	private void SubscribeToSelectedItems()
	{
		if (_selectedItems is INotifyCollectionChanged notifyCollectionChanged)
		{
			notifyCollectionChanged.CollectionChanged += SelectedItemsCollectionChanged;
		}
		SelectedItemsCollectionChanged(_selectedItems, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
	}

	private void SelectSingleItem(object item)
	{
		object selectedItem = _selectedItem;
		_syncingSelectedItems = true;
		SelectedItems.Clear();
		_selectedItem = item;
		SelectedItems.Add(item);
		_syncingSelectedItems = false;
		RaisePropertyChanged(SelectedItemProperty, selectedItem, _selectedItem);
	}

	private void SelectedItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		IList list = null;
		IList list2 = null;
		switch (e.Action)
		{
		case NotifyCollectionChangedAction.Add:
		{
			SelectedItemsAdded(e.NewItems.Cast<object>().ToArray());
			object selectedItem2 = SelectedItem;
			if (AutoScrollToSelectedItem && selectedItem2 != null && e.NewItems[0] == selectedItem2)
			{
				TreeContainerFromItem(selectedItem2)?.BringIntoView();
			}
			list = e.NewItems;
			break;
		}
		case NotifyCollectionChangedAction.Remove:
			if (!_syncingSelectedItems)
			{
				if (SelectedItems.Count == 0)
				{
					SelectedItem = null;
				}
				else if (SelectedItems.IndexOf(_selectedItem) == -1)
				{
					object selectedItem3 = _selectedItem;
					_selectedItem = SelectedItems[0];
					RaisePropertyChanged(SelectedItemProperty, selectedItem3, _selectedItem);
				}
			}
			foreach (object oldItem in e.OldItems)
			{
				MarkItemSelected(oldItem, selected: false);
			}
			list2 = e.OldItems;
			break;
		case NotifyCollectionChangedAction.Reset:
			foreach (Control realizedTreeContainer in GetRealizedTreeContainers())
			{
				MarkContainerSelected(realizedTreeContainer, selected: false);
			}
			if (SelectedItems.Count > 0)
			{
				SelectedItemsAdded(SelectedItems);
				list = SelectedItems;
			}
			else if (!_syncingSelectedItems)
			{
				SelectedItem = null;
			}
			break;
		case NotifyCollectionChangedAction.Replace:
			foreach (object oldItem2 in e.OldItems)
			{
				MarkItemSelected(oldItem2, selected: false);
			}
			foreach (object newItem in e.NewItems)
			{
				MarkItemSelected(newItem, selected: true);
			}
			if (SelectedItem != SelectedItems[0] && !_syncingSelectedItems)
			{
				object selectedItem = SelectedItem;
				RaisePropertyChanged(newValue: _selectedItem = SelectedItems[0], property: SelectedItemProperty, oldValue: selectedItem);
			}
			list = e.NewItems;
			list2 = e.OldItems;
			break;
		}
		if ((list != null && list.Count > 0) || (list2 != null && list2.Count > 0))
		{
			SelectionChangedEventArgs e2 = new SelectionChangedEventArgs(SelectingItemsControl.SelectionChangedEvent, list2 ?? Empty, list ?? Empty);
			RaiseEvent(e2);
		}
	}

	private void MarkItemSelected(object item, bool selected)
	{
		Control control = TreeContainerFromItem(item);
		if (control != null)
		{
			MarkContainerSelected(control, selected);
		}
	}

	private void SelectedItemsAdded(IList items)
	{
		if (items.Count == 0)
		{
			return;
		}
		foreach (object item in items)
		{
			MarkItemSelected(item, selected: true);
		}
		if (SelectedItem == null && !_syncingSelectedItems)
		{
			SetAndRaise(SelectedItemProperty, ref _selectedItem, items[0]);
		}
	}

	private void UnsubscribeFromSelectedItems()
	{
		if (_selectedItems is INotifyCollectionChanged notifyCollectionChanged)
		{
			notifyCollectionChanged.CollectionChanged -= SelectedItemsCollectionChanged;
		}
	}

	(bool handled, IInputElement? next) ICustomKeyboardNavigation.GetNext(IInputElement element, NavigationDirection direction)
	{
		if (direction == NavigationDirection.Next || direction == NavigationDirection.Previous)
		{
			if (!this.IsVisualAncestorOf((Visual)element))
			{
				Control control = ((_selectedItem != null) ? TreeContainerFromItem(_selectedItem) : ContainerFromIndex(0));
				return (handled: control != null, next: control);
			}
			return (handled: true, next: null);
		}
		return (handled: false, next: null);
	}

	protected internal override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
	{
		return new TreeViewItem();
	}

	protected internal override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
	{
		return NeedsContainer<TreeViewItem>(item, out recycleKey);
	}

	protected internal override void ContainerForItemPreparedOverride(Control container, object? item, int index)
	{
		base.ContainerForItemPreparedOverride(container, item, index);
		if (container.IsSet(SelectingItemsControl.IsSelectedProperty))
		{
			bool isSelected = SelectingItemsControl.GetIsSelected(container);
			UpdateSelectionFromContainer(container, isSelected, rangeModifier: false, toggleModifier: true);
		}
		MarkContainerSelected(container, SelectedItems.Contains(item));
		if (AutoScrollToSelectedItem && SelectedItem == item)
		{
			Dispatcher.UIThread.Post(container.BringIntoView, DispatcherPriority.Loaded);
		}
	}

	protected override void OnGotFocus(GotFocusEventArgs e)
	{
		if (e.NavigationMethod == NavigationMethod.Directional)
		{
			e.Handled = UpdateSelectionFromEventSource(e.Source, select: true, e.KeyModifiers.HasAllFlags(KeyModifiers.Shift));
		}
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		NavigationDirection? navigationDirection = e.Key.ToNavigationDirection();
		if (navigationDirection.HasValue && navigationDirection.GetValueOrDefault().IsDirectional() && !e.Handled)
		{
			if (SelectedItem != null)
			{
				TreeViewItem containerInDirection = GetContainerInDirection(GetContainerFromEventSource(e.Source), navigationDirection.Value, intoChildren: true);
				if (containerInDirection != null)
				{
					e.Handled = containerInDirection.Focus(NavigationMethod.Directional);
				}
			}
			else
			{
				SelectedItem = base.ItemsView[0];
			}
		}
		if (!e.Handled)
		{
			PlatformHotkeyConfiguration hotkeyConfiguration = Application.Current.PlatformSettings.HotkeyConfiguration;
			if (SelectionMode == SelectionMode.Multiple && Match(hotkeyConfiguration?.SelectAll))
			{
				SelectAll();
				e.Handled = true;
			}
		}
		bool Match(List<KeyGesture>? gestures)
		{
			return gestures?.Any((KeyGesture g) => g.Matches(e)) ?? false;
		}
	}

	private TreeViewItem? GetContainerInDirection(TreeViewItem? from, NavigationDirection direction, bool intoChildren)
	{
		StyledElement styledElement = from?.Parent;
		ItemsControl itemsControl = ((styledElement is TreeView treeView) ? ((ItemsControl)treeView) : ((ItemsControl)((!(styledElement is TreeViewItem treeViewItem)) ? null : treeViewItem)));
		ItemsControl itemsControl2 = itemsControl;
		if (itemsControl2 == null)
		{
			return null;
		}
		int num = ((from != null) ? itemsControl2.IndexFromContainer(from) : (-1));
		ItemsControl itemsControl3 = from?.Parent as ItemsControl;
		TreeViewItem result = null;
		switch (direction)
		{
		case NavigationDirection.Up:
			if (num > 0)
			{
				TreeViewItem treeViewItem2 = (TreeViewItem)itemsControl2.ContainerFromIndex(num - 1);
				result = ((treeViewItem2.IsExpanded && treeViewItem2.ItemCount > 0) ? ((TreeViewItem)treeViewItem2.ContainerFromIndex(treeViewItem2.ItemCount - 1)) : treeViewItem2);
			}
			else
			{
				result = from?.Parent as TreeViewItem;
			}
			break;
		case NavigationDirection.Right:
		case NavigationDirection.Down:
			if ((from?.IsExpanded ?? false) && intoChildren && from.ItemCount > 0)
			{
				result = (TreeViewItem)from.ContainerFromIndex(0);
			}
			else if (num < ((itemsControl3 != null) ? new int?(itemsControl3.ItemCount - 1) : ((int?)null)))
			{
				result = (TreeViewItem)itemsControl2.ContainerFromIndex(num + 1);
			}
			else if (itemsControl3 is TreeViewItem from2)
			{
				return GetContainerInDirection(from2, direction, intoChildren: false);
			}
			break;
		}
		return result;
	}

	protected override void OnPointerPressed(PointerPressedEventArgs e)
	{
		base.OnPointerPressed(e);
		if (e.Source is Visual relativeTo)
		{
			PointerPoint currentPoint = e.GetCurrentPoint(relativeTo);
			if (currentPoint.Properties.IsLeftButtonPressed || currentPoint.Properties.IsRightButtonPressed)
			{
				PlatformHotkeyConfiguration hotkeyConfiguration = Application.Current.PlatformSettings.HotkeyConfiguration;
				e.Handled = UpdateSelectionFromEventSource(e.Source, select: true, e.KeyModifiers.HasAllFlags(KeyModifiers.Shift), e.KeyModifiers.HasAllFlags(hotkeyConfiguration.CommandModifiers), currentPoint.Properties.IsRightButtonPressed);
			}
		}
	}

	protected void UpdateSelectionFromContainer(Control container, bool select = true, bool rangeModifier = false, bool toggleModifier = false, bool rightButton = false)
	{
		object obj = TreeItemFromContainer(container);
		if (obj == null)
		{
			return;
		}
		Control control = null;
		if (SelectedItem != null)
		{
			control = TreeContainerFromItem(SelectedItem);
		}
		SelectionMode selectionMode = SelectionMode;
		bool flag = toggleModifier || selectionMode.HasAllFlags(SelectionMode.Toggle);
		bool flag2 = selectionMode.HasAllFlags(SelectionMode.Multiple);
		bool flag3 = flag2 && rangeModifier && control != null;
		if (!select)
		{
			SelectedItems.Remove(obj);
		}
		else if (rightButton)
		{
			if (!SelectedItems.Contains(obj))
			{
				SelectSingleItem(obj);
			}
		}
		else if (!flag && !flag3)
		{
			SelectSingleItem(obj);
		}
		else if (flag2 && flag3)
		{
			SynchronizeItems(SelectedItems, GetItemsInRange(control as TreeViewItem, container as TreeViewItem));
		}
		else if (SelectedItems.IndexOf(obj) != -1)
		{
			SelectedItems.Remove(obj);
		}
		else if (flag2)
		{
			SelectedItems.Add(obj);
		}
		else
		{
			SelectedItem = obj;
		}
	}

	[Obsolete]
	[EditorBrowsable(EditorBrowsableState.Never)]
	private protected override ItemContainerGenerator CreateItemContainerGenerator()
	{
		return new TreeItemContainerGenerator(this);
	}

	private static TreeViewItem? FindFirstNode(TreeView treeView, TreeViewItem nodeA, TreeViewItem nodeB)
	{
		return FindInContainers(treeView, nodeA, nodeB);
	}

	private static TreeViewItem? FindInContainers(ItemsControl itemsControl, TreeViewItem nodeA, TreeViewItem nodeB)
	{
		foreach (Control realizedContainer in itemsControl.GetRealizedContainers())
		{
			TreeViewItem treeViewItem = FindFirstNode(realizedContainer as TreeViewItem, nodeA, nodeB);
			if (treeViewItem != null)
			{
				return treeViewItem;
			}
		}
		return null;
	}

	private static TreeViewItem? FindFirstNode(TreeViewItem? node, TreeViewItem nodeA, TreeViewItem nodeB)
	{
		if (node == null)
		{
			return null;
		}
		TreeViewItem treeViewItem = ((node == nodeA) ? nodeA : ((node == nodeB) ? nodeB : null));
		if (treeViewItem != null)
		{
			return treeViewItem;
		}
		return FindInContainers(node, nodeA, nodeB);
	}

	private List<object> GetItemsInRange(TreeViewItem? from, TreeViewItem? to)
	{
		List<object> list = new List<object>();
		if (from == null || to == null)
		{
			return list;
		}
		TreeViewItem treeViewItem = FindFirstNode(this, from, to);
		if (treeViewItem == null)
		{
			return list;
		}
		bool flag = false;
		if (treeViewItem == to)
		{
			TreeViewItem? treeViewItem2 = from;
			from = to;
			to = treeViewItem2;
			flag = true;
		}
		TreeViewItem treeViewItem3 = from;
		while (treeViewItem3 != null && treeViewItem3 != to)
		{
			object obj = TreeItemFromContainer(treeViewItem3);
			if (obj != null)
			{
				list.Add(obj);
			}
			treeViewItem3 = GetContainerInDirection(treeViewItem3, NavigationDirection.Down, intoChildren: true);
		}
		object obj2 = TreeItemFromContainer(to);
		if (obj2 != null)
		{
			list.Add(obj2);
		}
		if (flag)
		{
			list.Reverse();
		}
		return list;
	}

	protected bool UpdateSelectionFromEventSource(object eventSource, bool select = true, bool rangeModifier = false, bool toggleModifier = false, bool rightButton = false)
	{
		TreeViewItem containerFromEventSource = GetContainerFromEventSource(eventSource);
		if (containerFromEventSource != null)
		{
			UpdateSelectionFromContainer(containerFromEventSource, select, rangeModifier, toggleModifier, rightButton);
			return true;
		}
		return false;
	}

	protected TreeViewItem? GetContainerFromEventSource(object eventSource)
	{
		TreeViewItem treeViewItem = ((Visual)eventSource).GetSelfAndVisualAncestors().OfType<TreeViewItem>().FirstOrDefault();
		if (treeViewItem?.TreeViewOwner != this)
		{
			return null;
		}
		return treeViewItem;
	}

	private void ContainerSelectionChanged(RoutedEventArgs e)
	{
		if (e.Source is TreeViewItem treeViewItem && treeViewItem.TreeViewOwner == this)
		{
			object obj = TreeItemFromContainer(treeViewItem);
			if (obj != null)
			{
				bool isSelected = SelectingItemsControl.GetIsSelected(treeViewItem);
				bool flag = SelectedItems.Contains(obj);
				if (isSelected != flag)
				{
					if (isSelected)
					{
						SelectedItems.Add(obj);
					}
					else
					{
						SelectedItems.Remove(obj);
					}
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
		container.SetCurrentValue(SelectingItemsControl.IsSelectedProperty, selected);
	}

	private static void SynchronizeItems(IList items, IEnumerable<object> desired)
	{
		IEnumerable<object> enumerable = items.Cast<object>();
		List<object> list = enumerable.Except(desired).ToList();
		List<object> list2 = desired.Except(enumerable).ToList();
		foreach (object item in list)
		{
			items.Remove(item);
		}
		foreach (object item2 in list2)
		{
			items.Add(item2);
		}
	}
}
