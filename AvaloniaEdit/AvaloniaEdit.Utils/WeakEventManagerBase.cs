using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace AvaloniaEdit.Utils;

public abstract class WeakEventManagerBase<TEventManager, TEventSource, TEventHandler, TEventArgs> where TEventManager : WeakEventManagerBase<TEventManager, TEventSource, TEventHandler, TEventArgs>, new()
{
	internal class WeakHandler
	{
		private readonly WeakReference _source;

		private readonly WeakReference _originalHandler;

		public bool IsActive
		{
			get
			{
				if (_source != null && _source.IsAlive && _originalHandler != null)
				{
					return _originalHandler.IsAlive;
				}
				return false;
			}
		}

		public TEventHandler Handler
		{
			get
			{
				if (_originalHandler == null)
				{
					return default(TEventHandler);
				}
				return (TEventHandler)_originalHandler.Target;
			}
		}

		public WeakHandler(object source, TEventHandler originalHandler)
		{
			_source = new WeakReference(source);
			_originalHandler = new WeakReference(originalHandler);
		}

		public bool Matches(object o, TEventHandler handler)
		{
			if (_source != null && _source.Target == o && _originalHandler != null)
			{
				if (_originalHandler.Target != (object)handler)
				{
					if (_originalHandler.Target is TEventHandler && handler != null && handler is Delegate @delegate && _originalHandler.Target is Delegate delegate2)
					{
						return object.Equals(@delegate.Target, delegate2.Target);
					}
					return false;
				}
				return true;
			}
			return false;
		}
	}

	internal class WeakHandlerList
	{
		private int _deliveries;

		private readonly List<WeakHandler> _handlers;

		public int Count => _handlers.Count;

		public bool IsDeliverActive => _deliveries > 0;

		public WeakHandlerList()
		{
			_handlers = new List<WeakHandler>();
		}

		public void AddWeakHandler(TEventSource source, TEventHandler handler)
		{
			WeakHandler item = new WeakHandler(source, handler);
			_handlers.Add(item);
		}

		public bool RemoveWeakHandler(TEventSource source, TEventHandler handler)
		{
			foreach (WeakHandler handler2 in _handlers)
			{
				if (handler2.Matches(source, handler))
				{
					return _handlers.Remove(handler2);
				}
			}
			return false;
		}

		public WeakHandlerList Clone()
		{
			WeakHandlerList weakHandlerList = new WeakHandlerList();
			weakHandlerList._handlers.AddRange(_handlers.Where((WeakHandler h) => h.IsActive));
			return weakHandlerList;
		}

		public IDisposable DeliverActive()
		{
			Interlocked.Increment(ref _deliveries);
			return new Disposable(delegate
			{
				Interlocked.Decrement(ref _deliveries);
			});
		}

		public virtual bool DeliverEvent(object sender, TEventArgs args)
		{
			bool result = false;
			foreach (WeakHandler handler in _handlers)
			{
				if (handler.IsActive)
				{
					(handler.Handler as Delegate)?.DynamicInvoke(sender, args);
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		public void Purge()
		{
			for (int num = _handlers.Count - 1; num >= 0; num--)
			{
				if (!_handlers[num].IsActive)
				{
					_handlers.RemoveAt(num);
				}
			}
		}
	}

	private static readonly object StaticSource = new object();

	private readonly ConditionalWeakTable<object, List<Delegate>> _targetToEventHandler = new ConditionalWeakTable<object, List<Delegate>>();

	private readonly ConditionalWeakTable<object, WeakHandlerList> _sourceToWeakHandlers = new ConditionalWeakTable<object, WeakHandlerList>();

	private static readonly Lazy<TEventManager> CurrentLazy = new Lazy<TEventManager>(() => new TEventManager());

	private static TEventManager Current => CurrentLazy.Value;

	public static void AddHandler(TEventSource source, TEventHandler handler)
	{
		Current.PrivateAddHandler(source, handler);
	}

	public static void RemoveHandler(TEventSource source, TEventHandler handler)
	{
		Current.PrivateRemoveHandler(source, handler);
	}

	protected static void DeliverEvent(object sender, TEventArgs args)
	{
		Current.PrivateDeliverEvent(sender, args);
	}

	protected abstract void StartListening(TEventSource source);

	protected abstract void StopListening(TEventSource source);

	protected void PrivateAddHandler(TEventSource source, TEventHandler handler)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (handler == null)
		{
			throw new ArgumentNullException("handler");
		}
		if (!typeof(TEventHandler).GetTypeInfo().IsSubclassOf(typeof(Delegate)))
		{
			throw new ArgumentException("Handler must be Delegate type");
		}
		AddWeakHandler(source, handler);
		AddTargetHandler(handler);
	}

	private void AddWeakHandler(TEventSource source, TEventHandler handler)
	{
		if (_sourceToWeakHandlers.TryGetValue(source, out var value))
		{
			if (value.IsDeliverActive)
			{
				value = value.Clone();
				_sourceToWeakHandlers.Remove(source);
				_sourceToWeakHandlers.Add(source, value);
			}
			value.AddWeakHandler(source, handler);
		}
		else
		{
			value = new WeakHandlerList();
			value.AddWeakHandler(source, handler);
			_sourceToWeakHandlers.Add(source, value);
			StartListening(source);
		}
		Purge(source);
	}

	private void AddTargetHandler(TEventHandler handler)
	{
		Delegate @delegate = handler as Delegate;
		object key = @delegate?.Target ?? StaticSource;
		if (_targetToEventHandler.TryGetValue(key, out var value))
		{
			value.Add(@delegate);
			return;
		}
		value = new List<Delegate> { @delegate };
		_targetToEventHandler.Add(key, value);
	}

	protected void PrivateRemoveHandler(TEventSource source, TEventHandler handler)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (handler == null)
		{
			throw new ArgumentNullException("handler");
		}
		if (!typeof(TEventHandler).GetTypeInfo().IsSubclassOf(typeof(Delegate)))
		{
			throw new ArgumentException("handler must be Delegate type");
		}
		RemoveWeakHandler(source, handler);
		RemoveTargetHandler(handler);
	}

	private void RemoveWeakHandler(TEventSource source, TEventHandler handler)
	{
		if (_sourceToWeakHandlers.TryGetValue(source, out var value))
		{
			if (value.IsDeliverActive)
			{
				value = value.Clone();
				_sourceToWeakHandlers.Remove(source);
				_sourceToWeakHandlers.Add(source, value);
			}
			if (value.RemoveWeakHandler(source, handler) && value.Count == 0)
			{
				_sourceToWeakHandlers.Remove(source);
				StopListening(source);
			}
		}
	}

	private void RemoveTargetHandler(TEventHandler handler)
	{
		Delegate @delegate = handler as Delegate;
		object key = @delegate?.Target ?? StaticSource;
		if (_targetToEventHandler.TryGetValue(key, out var value))
		{
			value.Remove(@delegate);
			if (value.Count == 0)
			{
				_targetToEventHandler.Remove(key);
			}
		}
	}

	private void PrivateDeliverEvent(object sender, TEventArgs args)
	{
		object obj = sender ?? StaticSource;
		bool flag = false;
		if (_sourceToWeakHandlers.TryGetValue(obj, out var value))
		{
			using (value.DeliverActive())
			{
				flag = value.DeliverEvent(obj, args);
			}
		}
		if (flag)
		{
			Purge(obj);
		}
	}

	private void Purge(object source)
	{
		if (_sourceToWeakHandlers.TryGetValue(source, out var value))
		{
			if (value.IsDeliverActive)
			{
				value = value.Clone();
				_sourceToWeakHandlers.Remove(source);
				_sourceToWeakHandlers.Add(source, value);
			}
			else
			{
				value.Purge();
			}
		}
	}
}
