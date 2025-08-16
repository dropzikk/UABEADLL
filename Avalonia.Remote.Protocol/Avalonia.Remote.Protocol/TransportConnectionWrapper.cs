using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Avalonia.Remote.Protocol;

public class TransportConnectionWrapper : IAvaloniaRemoteTransportConnection, IDisposable
{
	private class SendOperation
	{
		public object Message { get; set; }

		public TaskCompletionSource<int> Tcs { get; set; }
	}

	private readonly IAvaloniaRemoteTransportConnection _conn;

	private EventStash<object> _onMessage;

	private EventStash<Exception> _onException;

	private Queue<SendOperation> _sendQueue = new Queue<SendOperation>();

	private object _lock = new object();

	private TaskCompletionSource<int> _signal;

	private bool _workerIsAlive;

	public event Action<IAvaloniaRemoteTransportConnection, object> OnMessage
	{
		add
		{
			_onMessage.Add(value);
		}
		remove
		{
			_onMessage.Remove(value);
		}
	}

	public event Action<IAvaloniaRemoteTransportConnection, Exception> OnException
	{
		add
		{
			_onException.Add(value);
		}
		remove
		{
			_onException.Remove(value);
		}
	}

	public TransportConnectionWrapper(IAvaloniaRemoteTransportConnection conn)
	{
		_conn = conn;
		_onException = new EventStash<Exception>(this);
		_onMessage = new EventStash<object>(this, delegate(Exception e)
		{
			_onException.Fire(this, e);
		});
		_conn.OnException += _onException.Fire;
		conn.OnMessage += _onMessage.Fire;
	}

	public void Dispose()
	{
		_conn.Dispose();
	}

	private async void Worker()
	{
		while (true)
		{
			SendOperation wi = null;
			lock (_lock)
			{
				if (_sendQueue.Count != 0)
				{
					wi = _sendQueue.Dequeue();
				}
			}
			if (wi == null)
			{
				TaskCompletionSource<int> taskCompletionSource = new TaskCompletionSource<int>();
				lock (_lock)
				{
					_signal = taskCompletionSource;
				}
				await taskCompletionSource.Task.ConfigureAwait(continueOnCapturedContext: false);
				continue;
			}
			try
			{
				await _conn.Send(wi.Message).ConfigureAwait(continueOnCapturedContext: false);
				wi.Tcs.TrySetResult(0);
			}
			catch (Exception exception)
			{
				wi.Tcs.TrySetException(exception);
			}
		}
	}

	public Task Send(object data)
	{
		TaskCompletionSource<int> taskCompletionSource = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
		lock (_lock)
		{
			if (!_workerIsAlive)
			{
				_workerIsAlive = true;
				Worker();
			}
			_sendQueue.Enqueue(new SendOperation
			{
				Message = data,
				Tcs = taskCompletionSource
			});
			if (_signal != null)
			{
				TaskCompletionSource<int> signal = _signal;
				_signal = null;
				signal.SetResult(0);
			}
		}
		return taskCompletionSource.Task;
	}

	public void Start()
	{
		_conn.Start();
	}
}
