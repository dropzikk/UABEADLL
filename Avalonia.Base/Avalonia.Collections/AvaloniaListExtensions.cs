using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Avalonia.Reactive;

namespace Avalonia.Collections;

public static class AvaloniaListExtensions
{
	public static IDisposable ForEachItem<T>(this IAvaloniaReadOnlyList<T> collection, Action<T> added, Action<T> removed, Action reset, bool weakSubscription = false)
	{
		return collection.ForEachItem(delegate(int _, T i)
		{
			added(i);
		}, delegate(int _, T i)
		{
			removed(i);
		}, reset, weakSubscription);
	}

	public static IDisposable ForEachItem<T>(this IAvaloniaReadOnlyList<T> collection, Action<int, T> added, Action<int, T> removed, Action reset, bool weakSubscription = false)
	{
		NotifyCollectionChangedEventHandler handler = delegate(object? _, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
			case NotifyCollectionChangedAction.Add:
				Add(e.NewStartingIndex, e.NewItems);
				break;
			case NotifyCollectionChangedAction.Replace:
			case NotifyCollectionChangedAction.Move:
			{
				Remove(e.OldStartingIndex, e.OldItems);
				int num = e.NewStartingIndex;
				if (num > e.OldStartingIndex)
				{
					num -= e.OldItems.Count;
				}
				Add(num, e.NewItems);
				break;
			}
			case NotifyCollectionChangedAction.Remove:
				Remove(e.OldStartingIndex, e.OldItems);
				break;
			case NotifyCollectionChangedAction.Reset:
				if (reset == null)
				{
					throw new InvalidOperationException("Reset called on collection without reset handler.");
				}
				reset();
				Add(0, (IList)collection);
				break;
			}
		};
		Add(0, (IList)collection);
		if (weakSubscription)
		{
			return collection.WeakSubscribe(handler);
		}
		collection.CollectionChanged += handler;
		return Disposable.Create(delegate
		{
			collection.CollectionChanged -= handler;
		});
		void Add(int index, IList items)
		{
			foreach (T item in items)
			{
				added(index++, item);
			}
		}
		void Remove(int index, IList items)
		{
			for (int num2 = items.Count - 1; num2 >= 0; num2--)
			{
				removed(index + num2, (T)items[num2]);
			}
		}
	}

	public static IDisposable TrackItemPropertyChanged<T>(this IAvaloniaReadOnlyList<T> collection, Action<Tuple<object?, PropertyChangedEventArgs>> callback)
	{
		List<INotifyPropertyChanged> tracked = new List<INotifyPropertyChanged>();
		PropertyChangedEventHandler handler = delegate(object? s, PropertyChangedEventArgs e)
		{
			callback(Tuple.Create(s, e));
		};
		collection.ForEachItem(delegate(T x)
		{
			if (x is INotifyPropertyChanged notifyPropertyChanged)
			{
				notifyPropertyChanged.PropertyChanged += handler;
				tracked.Add(notifyPropertyChanged);
			}
		}, delegate(T x)
		{
			if (x is INotifyPropertyChanged notifyPropertyChanged2)
			{
				notifyPropertyChanged2.PropertyChanged -= handler;
				tracked.Remove(notifyPropertyChanged2);
			}
		}, delegate
		{
			throw new NotSupportedException("Collection reset not supported.");
		});
		return Disposable.Create(delegate
		{
			foreach (INotifyPropertyChanged item in tracked)
			{
				item.PropertyChanged -= handler;
			}
		});
	}
}
