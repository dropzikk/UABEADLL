using System;
using System.Collections;
using System.Collections.Specialized;
using Avalonia.Controls.Generators;
using Avalonia.Controls.Utils;

namespace Avalonia.Controls.Presenters;

internal class PanelContainerGenerator : IDisposable
{
	private static readonly AttachedProperty<bool> ItemIsOwnContainerProperty = AvaloniaProperty.RegisterAttached<PanelContainerGenerator, Control, bool>("ItemIsOwnContainer", defaultValue: false);

	private readonly ItemsPresenter _presenter;

	public PanelContainerGenerator(ItemsPresenter presenter)
	{
		_presenter = presenter;
		_presenter.ItemsControl.ItemsView.PostCollectionChanged += OnItemsChanged;
		OnItemsChanged(null, CollectionUtils.ResetEventArgs);
	}

	public void Dispose()
	{
		ItemsControl itemsControl = _presenter.ItemsControl;
		if (itemsControl != null)
		{
			itemsControl.ItemsView.PostCollectionChanged -= OnItemsChanged;
			ClearItemsControlLogicalChildren();
		}
		_presenter.Panel?.Children.Clear();
	}

	internal void Refresh()
	{
		OnItemsChanged(null, CollectionUtils.ResetEventArgs);
	}

	private void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		ItemsControl itemsControl;
		ItemContainerGenerator generator;
		Controls children;
		if (_presenter.Panel != null && _presenter.ItemsControl != null)
		{
			itemsControl = _presenter.ItemsControl;
			generator = itemsControl.ItemContainerGenerator;
			children = _presenter.Panel.Children;
			switch (e.Action)
			{
			case NotifyCollectionChangedAction.Add:
				Add(e.NewStartingIndex, e.NewItems);
				break;
			case NotifyCollectionChangedAction.Remove:
				Remove(e.OldStartingIndex, e.OldItems.Count);
				break;
			case NotifyCollectionChangedAction.Replace:
			case NotifyCollectionChangedAction.Move:
				Remove(e.OldStartingIndex, e.OldItems.Count);
				Add(e.NewStartingIndex, e.NewItems);
				break;
			case NotifyCollectionChangedAction.Reset:
				ClearItemsControlLogicalChildren();
				children.Clear();
				Add(0, _presenter.ItemsControl.ItemsView);
				break;
			}
		}
		void Add(int index, IEnumerable items)
		{
			int i = index;
			foreach (object item in items)
			{
				InsertContainer(itemsControl, children, item, i++);
			}
			int count2 = children.Count;
			int num = i - index;
			for (; i < count2; i++)
			{
				generator.ItemContainerIndexChanged(children[i], i - num, i);
			}
		}
		void Remove(int index, int count)
		{
			for (int j = 0; j < count; j++)
			{
				Control control = children[index + j];
				if (!control.IsSet(ItemIsOwnContainerProperty))
				{
					itemsControl.RemoveLogicalChild(children[j + index]);
					generator.ClearItemContainer(control);
				}
			}
			children.RemoveRange(index, count);
			int count3 = children.Count;
			for (int k = index; k < count3; k++)
			{
				generator.ItemContainerIndexChanged(children[k], k + count, k);
			}
		}
	}

	private static void InsertContainer(ItemsControl itemsControl, Controls children, object? item, int index)
	{
		ItemContainerGenerator itemContainerGenerator = itemsControl.ItemContainerGenerator;
		Control control;
		if (itemContainerGenerator.NeedsContainer(item, index, out object recycleKey))
		{
			control = itemContainerGenerator.CreateContainer(item, index, recycleKey);
		}
		else
		{
			control = (Control)item;
			control.SetValue(ItemIsOwnContainerProperty, value: true);
		}
		itemContainerGenerator.PrepareItemContainer(control, item, index);
		itemsControl.AddLogicalChild(control);
		children.Insert(index, control);
		itemContainerGenerator.ItemContainerPrepared(control, item, index);
	}

	private void ClearItemsControlLogicalChildren()
	{
		if (_presenter.Panel == null || _presenter.ItemsControl == null)
		{
			return;
		}
		ItemsControl itemsControl = _presenter.ItemsControl;
		foreach (Control child in _presenter.Panel.Children)
		{
			if (!child.IsSet(ItemIsOwnContainerProperty))
			{
				itemsControl.RemoveLogicalChild(child);
			}
		}
	}
}
