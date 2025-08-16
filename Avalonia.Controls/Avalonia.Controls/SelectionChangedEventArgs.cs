using System.Collections;
using Avalonia.Interactivity;

namespace Avalonia.Controls;

public class SelectionChangedEventArgs : RoutedEventArgs
{
	public IList AddedItems { get; }

	public IList RemovedItems { get; }

	public SelectionChangedEventArgs(RoutedEvent routedEvent, IList removedItems, IList addedItems)
		: base(routedEvent)
	{
		RemovedItems = removedItems;
		AddedItems = addedItems;
	}
}
