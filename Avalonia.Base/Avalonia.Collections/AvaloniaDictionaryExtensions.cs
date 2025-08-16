using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Avalonia.Reactive;

namespace Avalonia.Collections;

public static class AvaloniaDictionaryExtensions
{
	public static IDisposable ForEachItem<TKey, TValue>(this IAvaloniaReadOnlyDictionary<TKey, TValue> collection, Action<TKey, TValue> added, Action<TKey, TValue> removed, Action reset, bool weakSubscription = false) where TKey : notnull
	{
		NotifyCollectionChangedEventHandler handler = delegate(object? _, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
			case NotifyCollectionChangedAction.Add:
				Add(e.NewItems);
				break;
			case NotifyCollectionChangedAction.Replace:
			case NotifyCollectionChangedAction.Move:
			{
				Remove(e.OldItems);
				int newStartingIndex = e.NewStartingIndex;
				if (newStartingIndex > e.OldStartingIndex)
				{
					newStartingIndex -= e.OldItems.Count;
				}
				Add(e.NewItems);
				break;
			}
			case NotifyCollectionChangedAction.Remove:
				Remove(e.OldItems);
				break;
			case NotifyCollectionChangedAction.Reset:
				if (reset == null)
				{
					throw new InvalidOperationException("Reset called on collection without reset handler.");
				}
				reset();
				Add(collection);
				break;
			}
		};
		Add(collection);
		if (weakSubscription)
		{
			return collection.WeakSubscribe(handler);
		}
		collection.CollectionChanged += handler;
		return Disposable.Create(delegate
		{
			collection.CollectionChanged -= handler;
		});
		void Add(IEnumerable items)
		{
			foreach (KeyValuePair<TKey, TValue> item in items)
			{
				added(item.Key, item.Value);
			}
		}
		void Remove(IEnumerable items)
		{
			foreach (KeyValuePair<TKey, TValue> item2 in items)
			{
				removed(item2.Key, item2.Value);
			}
		}
	}
}
