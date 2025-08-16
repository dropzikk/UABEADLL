using System;
using System.ComponentModel;

namespace Avalonia.Controls.Generators;

public class ItemContainerGenerator
{
	private readonly ItemsControl _owner;

	internal ItemContainerGenerator(ItemsControl owner)
	{
		_owner = owner;
	}

	public bool NeedsContainer(object? item, int index, out object? recycleKey)
	{
		return _owner.NeedsContainerOverride(item, index, out recycleKey);
	}

	public Control CreateContainer(object? item, int index, object? recycleKey)
	{
		return _owner.CreateContainerForItemOverride(item, index, recycleKey);
	}

	public void PrepareItemContainer(Control container, object? item, int index)
	{
		_owner.PrepareItemContainer(container, item, index);
	}

	public void ItemContainerPrepared(Control container, object? item, int index)
	{
		_owner.ItemContainerPrepared(container, item, index);
	}

	public void ItemContainerIndexChanged(Control container, int oldIndex, int newIndex)
	{
		_owner.ItemContainerIndexChanged(container, oldIndex, newIndex);
	}

	public void ClearItemContainer(Control container)
	{
		_owner.ClearItemContainer(container);
	}

	[Obsolete("Use ItemsControl.ContainerFromIndex")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public Control? ContainerFromIndex(int index)
	{
		return _owner.ContainerFromIndex(index);
	}

	[Obsolete("Use ItemsControl.IndexFromContainer")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public int IndexFromContainer(Control container)
	{
		return _owner.IndexFromContainer(container);
	}
}
