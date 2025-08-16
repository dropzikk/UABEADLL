using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Avalonia.Controls;

namespace Avalonia.Diagnostics.Controls;

internal class TopLevelGroup : AvaloniaObject
{
	public IDevToolsTopLevelGroup Group { get; }

	public IReadOnlyList<TopLevel> Items => Group.Items;

	public event EventHandler<TopLevel>? Added;

	public event EventHandler<TopLevel>? Removed;

	public TopLevelGroup(IDevToolsTopLevelGroup group)
	{
		Group = group;
		if (!(Group.Items is INotifyCollectionChanged notifyCollectionChanged))
		{
			return;
		}
		notifyCollectionChanged.CollectionChanged += delegate(object? _, NotifyCollectionChangedEventArgs args)
		{
			if (args.OldItems != null)
			{
				foreach (TopLevel oldItem in args.OldItems)
				{
					this.Removed?.Invoke(this, oldItem);
				}
			}
			if (args.NewItems != null)
			{
				foreach (TopLevel newItem in args.NewItems)
				{
					this.Added?.Invoke(this, newItem);
				}
			}
		};
	}
}
