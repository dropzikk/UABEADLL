using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using Avalonia.Utilities;

namespace Avalonia.Data.Core;

internal abstract class IndexerNodeBase : SettableNode, IWeakEventSubscriber<NotifyCollectionChangedEventArgs>, IWeakEventSubscriber<PropertyChangedEventArgs>
{
	protected override void StartListeningCore(WeakReference<object?> reference)
	{
		reference.TryGetTarget(out object target);
		if (target is INotifyCollectionChanged target2)
		{
			WeakEvents.CollectionChanged.Subscribe(target2, this);
		}
		if (target is INotifyPropertyChanged target3)
		{
			WeakEvents.ThreadSafePropertyChanged.Subscribe(target3, this);
		}
		ValueChanged(GetValue(target));
	}

	protected override void StopListeningCore()
	{
		if (base.Target.TryGetTarget(out object target))
		{
			if (target is INotifyCollectionChanged target2)
			{
				WeakEvents.CollectionChanged.Unsubscribe(target2, this);
			}
			if (target is INotifyPropertyChanged target3)
			{
				WeakEvents.ThreadSafePropertyChanged.Unsubscribe(target3, this);
			}
		}
	}

	protected abstract object? GetValue(object? target);

	protected abstract int? TryGetFirstArgumentAsInt();

	private bool ShouldUpdate(object? sender, NotifyCollectionChangedEventArgs e)
	{
		if (sender is IList)
		{
			int? num = TryGetFirstArgumentAsInt();
			if (!num.HasValue)
			{
				return false;
			}
			switch (e.Action)
			{
			case NotifyCollectionChangedAction.Add:
				return num >= e.NewStartingIndex;
			case NotifyCollectionChangedAction.Remove:
				return num >= e.OldStartingIndex;
			case NotifyCollectionChangedAction.Replace:
				if (num >= e.NewStartingIndex)
				{
					return num < e.NewStartingIndex + e.NewItems.Count;
				}
				return false;
			case NotifyCollectionChangedAction.Move:
				if (!(num >= e.NewStartingIndex) || !(num < e.NewStartingIndex + e.NewItems.Count))
				{
					if (num >= e.OldStartingIndex)
					{
						return num < e.OldStartingIndex + e.OldItems.Count;
					}
					return false;
				}
				return true;
			case NotifyCollectionChangedAction.Reset:
				return true;
			}
		}
		return true;
	}

	protected abstract bool ShouldUpdate(object? sender, PropertyChangedEventArgs e);

	void IWeakEventSubscriber<NotifyCollectionChangedEventArgs>.OnEvent(object? sender, WeakEvent ev, NotifyCollectionChangedEventArgs e)
	{
		if (ShouldUpdate(sender, e))
		{
			ValueChanged(GetValue(sender));
		}
	}

	void IWeakEventSubscriber<PropertyChangedEventArgs>.OnEvent(object? sender, WeakEvent ev, PropertyChangedEventArgs e)
	{
		if (ShouldUpdate(sender, e))
		{
			ValueChanged(GetValue(sender));
		}
	}
}
