using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Collections;
using Avalonia.Interactivity;

namespace Avalonia.Diagnostics.ViewModels;

internal class EventOwnerTreeNode : EventTreeNodeBase
{
	public override bool? IsEnabled
	{
		get
		{
			return base.IsEnabled;
		}
		set
		{
			if (base.IsEnabled == value)
			{
				return;
			}
			base.IsEnabled = value;
			if (!_updateChildren || !value.HasValue)
			{
				return;
			}
			foreach (EventTreeNodeBase child in base.Children)
			{
				try
				{
					child._updateParent = false;
					child.IsEnabled = value;
				}
				finally
				{
					child._updateParent = true;
				}
			}
		}
	}

	public EventOwnerTreeNode(Type type, IEnumerable<RoutedEvent> events, EventsPageViewModel vm)
		: base(null, type.Name)
	{
		EventOwnerTreeNode parent = this;
		base.Children = new AvaloniaList<EventTreeNodeBase>(from e in events
			orderby e.Name
			select new EventTreeNode(parent, e, vm));
		base.IsExpanded = true;
	}
}
