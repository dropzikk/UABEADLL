using System;
using System.Collections.Generic;
using System.Threading;

namespace Avalonia.Reactive;

internal abstract class LightweightObservableBase<T> : IObservable<T>
{
	private sealed class RemoveObserver : IDisposable
	{
		private LightweightObservableBase<T>? _parent;

		private IObserver<T>? _observer;

		public RemoveObserver(LightweightObservableBase<T> parent, IObserver<T> observer)
		{
			_parent = parent;
			Volatile.Write(ref _observer, observer);
		}

		public void Dispose()
		{
			IObserver<T> observer = _observer;
			Interlocked.Exchange(ref _parent, null)?.Remove(observer);
			_observer = null;
		}
	}

	private Exception? _error;

	private List<IObserver<T>>? _observers = new List<IObserver<T>>();

	public bool HasObservers
	{
		get
		{
			List<IObserver<T>>? observers = _observers;
			if (observers == null)
			{
				return false;
			}
			return observers.Count > 0;
		}
	}

	public IDisposable Subscribe(IObserver<T> observer)
	{
		if (observer == null)
		{
			throw new ArgumentNullException("observer");
		}
		bool flag = false;
		while (true)
		{
			if (Volatile.Read(ref _observers) == null)
			{
				if (_error != null)
				{
					observer.OnError(_error);
				}
				else
				{
					observer.OnCompleted();
				}
				return Disposable.Empty;
			}
			lock (this)
			{
				if (_observers == null)
				{
					continue;
				}
				flag = _observers.Count == 0;
				_observers.Add(observer);
				break;
			}
		}
		if (flag)
		{
			Initialize();
		}
		Subscribed(observer, flag);
		return new RemoveObserver(this, observer);
	}

	private void Remove(IObserver<T> observer)
	{
		if (Volatile.Read(ref _observers) == null)
		{
			return;
		}
		lock (this)
		{
			List<IObserver<T>> observers = _observers;
			if (observers != null)
			{
				observers.Remove(observer);
				if (observers.Count == 0)
				{
					observers.TrimExcess();
					Deinitialize();
				}
			}
		}
	}

	protected abstract void Initialize();

	protected abstract void Deinitialize();

	protected void PublishNext(T value)
	{
		if (Volatile.Read(ref _observers) == null)
		{
			return;
		}
		IObserver<T>[] array = null;
		IObserver<T> observer = null;
		lock (this)
		{
			if (_observers == null)
			{
				return;
			}
			if (_observers.Count == 1)
			{
				observer = _observers[0];
			}
			else
			{
				array = _observers.ToArray();
			}
		}
		if (observer != null)
		{
			observer.OnNext(value);
			return;
		}
		IObserver<T>[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].OnNext(value);
		}
	}

	protected void PublishCompleted()
	{
		if (Volatile.Read(ref _observers) == null)
		{
			return;
		}
		IObserver<T>[] array;
		lock (this)
		{
			if (_observers == null)
			{
				return;
			}
			array = _observers.ToArray();
			Volatile.Write(ref _observers, null);
		}
		IObserver<T>[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].OnCompleted();
		}
		Deinitialize();
	}

	protected void PublishError(Exception error)
	{
		if (Volatile.Read(ref _observers) == null)
		{
			return;
		}
		IObserver<T>[] array;
		lock (this)
		{
			if (_observers == null)
			{
				return;
			}
			_error = error;
			array = _observers.ToArray();
			Volatile.Write(ref _observers, null);
		}
		IObserver<T>[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].OnError(error);
		}
		Deinitialize();
	}

	protected virtual void Subscribed(IObserver<T> observer, bool first)
	{
	}
}
