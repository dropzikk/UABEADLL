using System;
using System.Runtime.ConstrainedExecution;
using System.Threading;

namespace Avalonia.Utilities;

internal static class RefCountable
{
	private class RefCounter
	{
		private IDisposable? _item;

		private volatile int _refs;

		internal int RefCount => _refs;

		public RefCounter(IDisposable item)
		{
			_item = item;
			_refs = 1;
		}

		public void AddRef()
		{
			int num = _refs;
			while (true)
			{
				if (num == 0)
				{
					throw new ObjectDisposedException("Cannot add a reference to a nonreferenced item");
				}
				int num2 = Interlocked.CompareExchange(ref _refs, num + 1, num);
				if (num2 != num)
				{
					num = num2;
					continue;
				}
				break;
			}
		}

		public void Release()
		{
			int num = _refs;
			while (true)
			{
				int num2 = Interlocked.CompareExchange(ref _refs, num - 1, num);
				if (num2 == num)
				{
					break;
				}
				num = num2;
			}
			if (num == 1)
			{
				_item?.Dispose();
				_item = null;
			}
		}
	}

	private class Ref<T> : CriticalFinalizerObject, IRef<T>, IDisposable where T : class
	{
		private T? _item;

		private RefCounter _counter;

		private object _lock = new object();

		public T Item
		{
			get
			{
				lock (_lock)
				{
					return _item;
				}
			}
		}

		public int RefCount => _counter.RefCount;

		public Ref(T item, RefCounter counter)
		{
			_item = item;
			_counter = counter;
		}

		public void Dispose()
		{
			lock (_lock)
			{
				if (_item != null)
				{
					_counter.Release();
					_item = null;
				}
				GC.SuppressFinalize(this);
			}
		}

		~Ref()
		{
			Dispose();
		}

		public IRef<T> Clone()
		{
			lock (_lock)
			{
				if (_item != null)
				{
					Ref<T> result = new Ref<T>(_item, _counter);
					_counter.AddRef();
					return result;
				}
				throw new ObjectDisposedException("Ref<" + typeof(T)?.ToString() + ">");
			}
		}

		public IRef<TResult> CloneAs<TResult>() where TResult : class
		{
			lock (_lock)
			{
				if (_item != null)
				{
					Ref<TResult> result = new Ref<TResult>((TResult)(object)_item, _counter);
					Interlocked.MemoryBarrier();
					_counter.AddRef();
					return result;
				}
				throw new ObjectDisposedException("Ref<" + typeof(T)?.ToString() + ">");
			}
		}
	}

	public static IRef<T> Create<T>(T item) where T : class, IDisposable
	{
		return new Ref<T>(item, new RefCounter(item));
	}
}
