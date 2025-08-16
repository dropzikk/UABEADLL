using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls.Generators;
using Avalonia.Controls.Utils;
using Avalonia.Input;

namespace Avalonia.Controls;

public abstract class VirtualizingPanel : Panel, INavigableContainer
{
	private ItemsControl? _itemsControl;

	public ItemContainerGenerator? ItemContainerGenerator => _itemsControl?.ItemContainerGenerator;

	protected IReadOnlyList<object?> Items
	{
		get
		{
			IReadOnlyList<object> readOnlyList = ItemsControl?.ItemsView;
			return readOnlyList ?? Array.Empty<object>();
		}
	}

	protected ItemsControl? ItemsControl
	{
		get
		{
			return _itemsControl;
		}
		private set
		{
			if (_itemsControl != value)
			{
				ItemsControl itemsControl = _itemsControl;
				_itemsControl = value;
				OnItemsControlChanged(itemsControl);
			}
		}
	}

	IInputElement? INavigableContainer.GetControl(NavigationDirection direction, IInputElement? from, bool wrap)
	{
		return GetControl(direction, from, wrap);
	}

	protected internal abstract Control? ScrollIntoView(int index);

	protected internal abstract Control? ContainerFromIndex(int index);

	protected internal abstract int IndexFromContainer(Control container);

	protected internal abstract IEnumerable<Control>? GetRealizedContainers();

	protected abstract IInputElement? GetControl(NavigationDirection direction, IInputElement? from, bool wrap);

	protected virtual void OnItemsControlChanged(ItemsControl? oldValue)
	{
	}

	protected virtual void OnItemsChanged(IReadOnlyList<object?> items, NotifyCollectionChangedEventArgs e)
	{
	}

	protected void AddInternalChild(Control control)
	{
		EnsureItemsControl().AddLogicalChild(control);
		base.Children.Add(control);
	}

	protected void InsertInternalChild(int index, Control control)
	{
		EnsureItemsControl().AddLogicalChild(control);
		base.Children.Insert(index, control);
	}

	protected void RemoveInternalChild(Control child)
	{
		EnsureItemsControl().RemoveLogicalChild(child);
		base.Children.Remove(child);
	}

	protected void RemoveInternalChildRange(int index, int count)
	{
		ItemsControl itemsControl = EnsureItemsControl();
		for (int i = 0; i < count; i++)
		{
			Control c = base.Children[i];
			itemsControl.RemoveLogicalChild(c);
		}
		base.Children.RemoveRange(index, count);
	}

	internal void Attach(ItemsControl itemsControl)
	{
		if (ItemsControl != null)
		{
			throw new InvalidOperationException("The VirtualizingPanel is already attached to an ItemsControl");
		}
		ItemsControl = itemsControl;
		ItemsControl.ItemsView.PostCollectionChanged += OnItemsControlItemsChanged;
	}

	internal void Detach()
	{
		EnsureItemsControl().ItemsView.PostCollectionChanged -= OnItemsControlItemsChanged;
		ItemsControl = null;
		base.Children.Clear();
	}

	internal void Refresh()
	{
		OnItemsControlItemsChanged(null, CollectionUtils.ResetEventArgs);
	}

	private ItemsControl EnsureItemsControl()
	{
		if (ItemsControl == null)
		{
			ThrowNotAttached();
		}
		return ItemsControl;
	}

	private void OnItemsControlItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		OnItemsChanged(Items, e);
	}

	[DoesNotReturn]
	private static void ThrowNotAttached()
	{
		throw new InvalidOperationException("The VirtualizingPanel does not belong to an ItemsControl.");
	}
}
