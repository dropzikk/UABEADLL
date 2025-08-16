using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Platform;
using Avalonia.Threading;

namespace Avalonia.Rendering.Composition.Transport;

internal abstract class BatchStreamPoolBase<T> : IDisposable
{
	private readonly Stack<T> _pool = new Stack<T>();

	private bool _disposed;

	private int _usage;

	private readonly int[] _usageStatistics = new int[10];

	private int _usageStatisticsSlot;

	private bool _reclaimImmediately;

	public int CurrentUsage => _usage;

	public int CurrentPool => _pool.Count;

	public BatchStreamPoolBase(bool needsFinalize, bool reclaimImmediately, Action<Func<bool>>? startTimer = null)
	{
		if (!needsFinalize)
		{
			GC.SuppressFinalize(needsFinalize);
		}
		WeakReference<BatchStreamPoolBase<T>> updateRef = new WeakReference<BatchStreamPoolBase<T>>(this);
		if (reclaimImmediately || (AvaloniaLocator.Current.GetService<IPlatformThreadingInterface>() == null && AvaloniaLocator.Current.GetService<IDispatcherImpl>() == null))
		{
			_reclaimImmediately = true;
		}
		else
		{
			StartUpdateTimer(startTimer, updateRef);
		}
	}

	private static void StartUpdateTimer(Action<Func<bool>>? startTimer, WeakReference<BatchStreamPoolBase<T>> updateRef)
	{
		Func<bool> func = delegate
		{
			if (updateRef.TryGetTarget(out BatchStreamPoolBase<T> target))
			{
				target.UpdateStatistics();
				return true;
			}
			return false;
		};
		if (startTimer != null)
		{
			startTimer(func);
		}
		else
		{
			DispatcherTimer.Run(func, TimeSpan.FromSeconds(1.0));
		}
	}

	private void UpdateStatistics()
	{
		lock (_pool)
		{
			int num = Math.Max(_usageStatistics.Max() - _usage, 10);
			while (num < _pool.Count)
			{
				DestroyItem(_pool.Pop());
			}
			_usageStatisticsSlot = (_usageStatisticsSlot + 1) % _usageStatistics.Length;
			_usageStatistics[_usageStatisticsSlot] = 0;
		}
	}

	protected abstract T CreateItem();

	protected virtual void ClearItem(T item)
	{
	}

	protected virtual void DestroyItem(T item)
	{
	}

	public T Get()
	{
		lock (_pool)
		{
			_usage++;
			if (_usageStatistics[_usageStatisticsSlot] < _usage)
			{
				_usageStatistics[_usageStatisticsSlot] = _usage;
			}
			if (_pool.Count != 0)
			{
				return _pool.Pop();
			}
		}
		return CreateItem();
	}

	public void Return(T item)
	{
		ClearItem(item);
		lock (_pool)
		{
			_usage--;
			if (!_disposed && !_reclaimImmediately)
			{
				_pool.Push(item);
				return;
			}
		}
		DestroyItem(item);
	}

	public void Dispose()
	{
		lock (_pool)
		{
			_disposed = true;
			foreach (T item in _pool)
			{
				DestroyItem(item);
			}
			_pool.Clear();
		}
	}

	~BatchStreamPoolBase()
	{
		Dispose();
	}
}
