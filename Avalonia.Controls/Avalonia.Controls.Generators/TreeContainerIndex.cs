using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Avalonia.Controls.Generators;

public class TreeContainerIndex
{
	private readonly TreeView _owner;

	[Obsolete("Use TreeView.GetRealizedTreeContainers")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public IEnumerable<Control> Containers => _owner.GetRealizedTreeContainers();

	internal TreeContainerIndex(TreeView owner)
	{
		_owner = owner;
	}

	[Obsolete("Use TreeView.TreeContainerFromItem")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public Control? ContainerFromItem(object item)
	{
		return _owner.TreeContainerFromItem(item);
	}

	[Obsolete("Use TreeView.TreeItemFromContainer")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public object? ItemFromContainer(Control container)
	{
		return _owner.TreeItemFromContainer(container);
	}
}
