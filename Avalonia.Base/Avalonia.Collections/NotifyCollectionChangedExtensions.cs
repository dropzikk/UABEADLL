using System;
using System.Collections.Specialized;
using Avalonia.Reactive;
using Avalonia.Utilities;

namespace Avalonia.Collections;

public static class NotifyCollectionChangedExtensions
{
	private class WeakCollectionChangedObservable : LightweightObservableBase<NotifyCollectionChangedEventArgs>, IWeakEventSubscriber<NotifyCollectionChangedEventArgs>
	{
		private WeakReference<INotifyCollectionChanged> _sourceReference;

		public WeakCollectionChangedObservable(WeakReference<INotifyCollectionChanged> source)
		{
			_sourceReference = source;
		}

		public void OnEvent(object? sender, WeakEvent ev, NotifyCollectionChangedEventArgs e)
		{
			PublishNext(e);
		}

		protected override void Initialize()
		{
			if (_sourceReference.TryGetTarget(out INotifyCollectionChanged target))
			{
				WeakEvents.CollectionChanged.Subscribe(target, this);
			}
		}

		protected override void Deinitialize()
		{
			if (_sourceReference.TryGetTarget(out INotifyCollectionChanged target))
			{
				WeakEvents.CollectionChanged.Unsubscribe(target, this);
			}
		}
	}

	public static IObservable<NotifyCollectionChangedEventArgs> GetWeakCollectionChangedObservable(this INotifyCollectionChanged collection)
	{
		if (collection == null)
		{
			throw new ArgumentNullException("collection");
		}
		return new WeakCollectionChangedObservable(new WeakReference<INotifyCollectionChanged>(collection));
	}

	public static IDisposable WeakSubscribe(this INotifyCollectionChanged collection, NotifyCollectionChangedEventHandler handler)
	{
		if (collection == null)
		{
			throw new ArgumentNullException("collection");
		}
		if (handler == null)
		{
			throw new ArgumentNullException("handler");
		}
		return collection.GetWeakCollectionChangedObservable().Subscribe(delegate(NotifyCollectionChangedEventArgs e)
		{
			handler(collection, e);
		});
	}

	public static IDisposable WeakSubscribe(this INotifyCollectionChanged collection, Action<NotifyCollectionChangedEventArgs> handler)
	{
		if (collection == null)
		{
			throw new ArgumentNullException("collection");
		}
		if (handler == null)
		{
			throw new ArgumentNullException("handler");
		}
		return collection.GetWeakCollectionChangedObservable().Subscribe(handler);
	}
}
