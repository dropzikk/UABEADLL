using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Avalonia.FreeDesktop;

internal class DBusCallQueue
{
	private record Item(Func<Task> Callback)
	{
		public Action<Exception?>? OnFinish;
	}

	private readonly Func<Exception, Task> _errorHandler;

	private readonly Queue<Item> _q = new Queue<Item>();

	private bool _processing;

	public DBusCallQueue(Func<Exception, Task> errorHandler)
	{
		_errorHandler = errorHandler;
	}

	public void Enqueue(Func<Task> cb)
	{
		_q.Enqueue(new Item(cb));
		Process();
	}

	public Task EnqueueAsync(Func<Task> cb)
	{
		TaskCompletionSource<int> tcs = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
		_q.Enqueue(new Item(cb)
		{
			OnFinish = delegate(Exception? e)
			{
				if (e == null)
				{
					tcs.TrySetResult(0);
				}
				else
				{
					tcs.TrySetException(e);
				}
			}
		});
		Process();
		return tcs.Task;
	}

	public Task<T> EnqueueAsync<T>(Func<Task<T>> cb)
	{
		TaskCompletionSource<T> tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
		_q.Enqueue(new Item(async delegate
		{
			T result = await cb();
			tcs.TrySetResult(result);
		})
		{
			OnFinish = delegate(Exception? e)
			{
				if (e != null)
				{
					tcs.TrySetException(e);
				}
			}
		});
		Process();
		return tcs.Task;
	}

	private async void Process()
	{
		if (_processing)
		{
			return;
		}
		_processing = true;
		try
		{
			while (_q.Count > 0)
			{
				Item item = _q.Dequeue();
				try
				{
					await item.Callback();
					item.OnFinish?.Invoke(null);
				}
				catch (Exception ex)
				{
					if (item.OnFinish == null)
					{
						await _errorHandler(ex);
					}
					else
					{
						item.OnFinish(ex);
					}
				}
			}
		}
		finally
		{
			_processing = false;
		}
	}

	public void FailAll()
	{
		while (_q.Count > 0)
		{
			_q.Dequeue().OnFinish?.Invoke(new OperationCanceledException());
		}
	}
}
