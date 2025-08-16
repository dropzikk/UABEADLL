using System;
using System.Runtime.CompilerServices;
using Avalonia.Collections.Pooled;
using Avalonia.Threading;

namespace Avalonia.Utilities;

public class WeakEvent<TSender, TEventArgs> : WeakEvent where TSender : class where TEventArgs : EventArgs
{
	private class Subscription
	{
		private readonly WeakEvent<TSender, TEventArgs> _ev;

		private readonly TSender _target;

		private readonly Action _compact;

		private readonly Action _unsubscribe;

		private readonly WeakHashList<IWeakEventSubscriber<TEventArgs>> _list = new WeakHashList<IWeakEventSubscriber<TEventArgs>>();

		private bool _compactScheduled;

		private bool _destroyed;

		public Subscription(WeakEvent<TSender, TEventArgs> ev, TSender target)
		{
			_ev = ev;
			_target = target;
			_compact = Compact;
			_unsubscribe = ev._subscribe(target, OnEvent);
		}

		private void Destroy()
		{
			if (!_destroyed)
			{
				_destroyed = true;
				_unsubscribe();
				_ev._subscriptions.Remove(_target);
			}
		}

		public void Add(IWeakEventSubscriber<TEventArgs> s)
		{
			_list.Add(s);
		}

		public void Remove(IWeakEventSubscriber<TEventArgs> s)
		{
			_list.Remove(s);
			if (_list.IsEmpty)
			{
				Destroy();
			}
			else if (_list.NeedCompact && _compactScheduled)
			{
				ScheduleCompact();
			}
		}

		private void ScheduleCompact()
		{
			if (!_compactScheduled && !_destroyed)
			{
				_compactScheduled = true;
				Dispatcher.UIThread.Post(_compact, DispatcherPriority.Background);
			}
		}

		private void Compact()
		{
			if (_compactScheduled)
			{
				_compactScheduled = false;
				_list.Compact();
				if (_list.IsEmpty)
				{
					Destroy();
				}
			}
		}

		private void OnEvent(object? sender, TEventArgs eventArgs)
		{
			PooledList<IWeakEventSubscriber<TEventArgs>> alive = _list.GetAlive();
			if (alive == null)
			{
				Destroy();
				return;
			}
			Span<IWeakEventSubscriber<TEventArgs>> span = alive.Span;
			for (int i = 0; i < span.Length; i++)
			{
				span[i].OnEvent(_target, _ev, eventArgs);
			}
			WeakHashList<IWeakEventSubscriber<TEventArgs>>.ReturnToSharedPool(alive);
			if (_list.NeedCompact && !_compactScheduled)
			{
				ScheduleCompact();
			}
		}
	}

	private readonly Func<TSender, EventHandler<TEventArgs>, Action> _subscribe;

	private readonly ConditionalWeakTable<object, Subscription> _subscriptions = new ConditionalWeakTable<object, Subscription>();

	internal WeakEvent(Action<TSender, EventHandler<TEventArgs>> subscribe, Action<TSender, EventHandler<TEventArgs>> unsubscribe)
	{
		_subscribe = delegate(TSender t, EventHandler<TEventArgs> s)
		{
			subscribe(t, s);
			return delegate
			{
				unsubscribe(t, s);
			};
		};
	}

	internal WeakEvent(Func<TSender, EventHandler<TEventArgs>, Action> subscribe)
	{
		_subscribe = subscribe;
	}

	public void Subscribe(TSender target, IWeakEventSubscriber<TEventArgs> subscriber)
	{
		if (!_subscriptions.TryGetValue(target, out Subscription value))
		{
			_subscriptions.Add(target, value = new Subscription(this, target));
		}
		value.Add(subscriber);
	}

	public void Unsubscribe(TSender target, IWeakEventSubscriber<TEventArgs> subscriber)
	{
		if (_subscriptions.TryGetValue(target, out Subscription value))
		{
			value.Remove(subscriber);
		}
	}
}
public class WeakEvent
{
	public static WeakEvent<TSender, TEventArgs> Register<TSender, TEventArgs>(Action<TSender, EventHandler<TEventArgs>> subscribe, Action<TSender, EventHandler<TEventArgs>> unsubscribe) where TSender : class where TEventArgs : EventArgs
	{
		return new WeakEvent<TSender, TEventArgs>(subscribe, unsubscribe);
	}

	public static WeakEvent<TSender, TEventArgs> Register<TSender, TEventArgs>(Func<TSender, EventHandler<TEventArgs>, Action> subscribe) where TSender : class where TEventArgs : EventArgs
	{
		return new WeakEvent<TSender, TEventArgs>(subscribe);
	}

	public static WeakEvent<TSender, EventArgs> Register<TSender>(Action<TSender, EventHandler> subscribe, Action<TSender, EventHandler> unsubscribe) where TSender : class
	{
		return Register(delegate(TSender s, EventHandler<EventArgs> h)
		{
			EventHandler handler = delegate(object? _, EventArgs e)
			{
				h(s, e);
			};
			subscribe(s, handler);
			return delegate
			{
				unsubscribe(s, handler);
			};
		});
	}
}
