using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using Avalonia.Threading;
using Avalonia.Utilities;

namespace Avalonia.Controls.Utils;

internal class CollectionChangedEventManager
{
	private class Entry : IWeakEventSubscriber<NotifyCollectionChangedEventArgs>, IDisposable
	{
		private INotifyCollectionChanged _collection;

		public List<WeakReference<ICollectionChangedListener>> Listeners { get; }

		public Entry(INotifyCollectionChanged collection)
		{
			_collection = collection;
			Listeners = new List<WeakReference<ICollectionChangedListener>>();
			WeakEvents.CollectionChanged.Subscribe(_collection, this);
		}

		public void Dispose()
		{
			WeakEvents.CollectionChanged.Unsubscribe(_collection, this);
		}

		void IWeakEventSubscriber<NotifyCollectionChangedEventArgs>.OnEvent(object? notifyCollectionChanged, WeakEvent ev, NotifyCollectionChangedEventArgs e)
		{
			WeakReference<ICollectionChangedListener>[] l = Listeners.ToArray();
			if (Dispatcher.UIThread.CheckAccess())
			{
				Notify(_collection, e, l);
				return;
			}
			NotifyCollectionChangedEventArgs eCapture = e;
			Dispatcher.UIThread.Post(delegate
			{
				Notify(_collection, eCapture, l);
			}, DispatcherPriority.Send);
			static void Notify(INotifyCollectionChanged incc, NotifyCollectionChangedEventArgs args, WeakReference<ICollectionChangedListener>[] listeners)
			{
				WeakReference<ICollectionChangedListener>[] array = listeners;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].TryGetTarget(out var target))
					{
						target.PreChanged(incc, args);
					}
				}
				array = listeners;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].TryGetTarget(out var target2))
					{
						target2.Changed(incc, args);
					}
				}
				array = listeners;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].TryGetTarget(out var target3))
					{
						target3.PostChanged(incc, args);
					}
				}
			}
		}
	}

	private ConditionalWeakTable<INotifyCollectionChanged, Entry> _entries = new ConditionalWeakTable<INotifyCollectionChanged, Entry>();

	public static CollectionChangedEventManager Instance { get; } = new CollectionChangedEventManager();

	private CollectionChangedEventManager()
	{
	}

	public void AddListener(INotifyCollectionChanged collection, ICollectionChangedListener listener)
	{
		collection = collection ?? throw new ArgumentNullException("collection");
		listener = listener ?? throw new ArgumentNullException("listener");
		Dispatcher.UIThread.VerifyAccess();
		if (!_entries.TryGetValue(collection, out Entry value))
		{
			value = new Entry(collection);
			_entries.Add(collection, value);
		}
		foreach (WeakReference<ICollectionChangedListener> listener2 in value.Listeners)
		{
			if (listener2.TryGetTarget(out var target) && target == listener)
			{
				throw new InvalidOperationException("Collection listener already added for this collection/listener combination.");
			}
		}
		value.Listeners.Add(new WeakReference<ICollectionChangedListener>(listener));
	}

	public void RemoveListener(INotifyCollectionChanged collection, ICollectionChangedListener listener)
	{
		collection = collection ?? throw new ArgumentNullException("collection");
		listener = listener ?? throw new ArgumentNullException("listener");
		Dispatcher.UIThread.VerifyAccess();
		if (_entries.TryGetValue(collection, out Entry value))
		{
			List<WeakReference<ICollectionChangedListener>> listeners = value.Listeners;
			for (int i = 0; i < listeners.Count; i++)
			{
				if (listeners[i].TryGetTarget(out var target) && target == listener)
				{
					listeners.RemoveAt(i);
					if (listeners.Count == 0)
					{
						value.Dispose();
						_entries.Remove(collection);
					}
					return;
				}
			}
		}
		throw new InvalidOperationException("Collection listener not registered for this collection/listener combination.");
	}
}
