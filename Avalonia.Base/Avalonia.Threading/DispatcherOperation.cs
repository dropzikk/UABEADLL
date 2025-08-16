using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Avalonia.Threading;

public class DispatcherOperation
{
	private class DispatcherOperationFrame : DispatcherFrame
	{
		private DispatcherOperation _operation;

		private Timer? _waitTimer;

		public DispatcherOperationFrame(DispatcherOperation op, TimeSpan timeout)
			: base(exitWhenRequested: false)
		{
			_operation = op;
			_operation.Aborted += OnCompletedOrAborted;
			_operation.Completed += OnCompletedOrAborted;
			if (timeout.TotalMilliseconds > 0.0)
			{
				_waitTimer = new Timer(delegate
				{
					Exit();
				}, null, timeout, TimeSpan.FromMilliseconds(-1.0));
			}
			if (_operation.Status != 0)
			{
				Exit();
			}
		}

		private void Exit()
		{
			base.Continue = false;
			if (_waitTimer != null)
			{
				_waitTimer.Dispose();
			}
			_operation.Aborted -= OnCompletedOrAborted;
			_operation.Completed -= OnCompletedOrAborted;
		}

		private void OnCompletedOrAborted(object? sender, EventArgs e)
		{
			Exit();
		}
	}

	protected readonly bool ThrowOnUiThread;

	protected object? Callback;

	protected object? TaskSource;

	private EventHandler? _aborted;

	private EventHandler? _completed;

	private DispatcherPriority _priority;

	private static readonly Task s_abortedTask = Task.FromCanceled(CreateCancelledToken());

	public DispatcherOperationStatus Status { get; protected set; }

	public Dispatcher Dispatcher { get; }

	public DispatcherPriority Priority
	{
		get
		{
			return _priority;
		}
		set
		{
			_priority = value;
			Dispatcher?.SetPriority(this, value);
		}
	}

	internal DispatcherOperation? SequentialPrev { get; set; }

	internal DispatcherOperation? SequentialNext { get; set; }

	internal DispatcherOperation? PriorityPrev { get; set; }

	internal DispatcherOperation? PriorityNext { get; set; }

	internal PriorityChain? Chain { get; set; }

	internal bool IsQueued => Chain != null;

	public event EventHandler Aborted
	{
		add
		{
			lock (Dispatcher.InstanceLock)
			{
				_aborted = (EventHandler)Delegate.Combine(_aborted, value);
			}
		}
		remove
		{
			lock (Dispatcher.InstanceLock)
			{
				_aborted = (EventHandler)Delegate.Remove(_aborted, value);
			}
		}
	}

	public event EventHandler Completed
	{
		add
		{
			lock (Dispatcher.InstanceLock)
			{
				_completed = (EventHandler)Delegate.Combine(_completed, value);
			}
		}
		remove
		{
			lock (Dispatcher.InstanceLock)
			{
				_completed = (EventHandler)Delegate.Remove(_completed, value);
			}
		}
	}

	internal DispatcherOperation(Dispatcher dispatcher, DispatcherPriority priority, Action callback, bool throwOnUiThread)
		: this(dispatcher, priority, throwOnUiThread)
	{
		Callback = callback;
	}

	private protected DispatcherOperation(Dispatcher dispatcher, DispatcherPriority priority, bool throwOnUiThread)
	{
		ThrowOnUiThread = throwOnUiThread;
		Priority = priority;
		Dispatcher = dispatcher;
	}

	public bool Abort()
	{
		lock (Dispatcher.InstanceLock)
		{
			if (Status != 0)
			{
				return false;
			}
			Dispatcher.Abort(this);
			return true;
		}
	}

	public void Wait()
	{
		Wait(TimeSpan.FromMilliseconds(-1.0));
	}

	public void Wait(TimeSpan timeout)
	{
		if ((Status == DispatcherOperationStatus.Pending || Status == DispatcherOperationStatus.Executing) && timeout.TotalMilliseconds != 0.0 && Dispatcher.CheckAccess())
		{
			if (Status == DispatcherOperationStatus.Executing)
			{
				throw new InvalidOperationException("A thread cannot wait on operations already running on the same thread.");
			}
			CancellationTokenSource cts = new CancellationTokenSource();
			EventHandler value = delegate
			{
				cts.Cancel();
			};
			Completed += value;
			Aborted += value;
			try
			{
				while (Status == DispatcherOperationStatus.Pending)
				{
					if (Dispatcher.SupportsRunLoops)
					{
						if (Priority >= DispatcherPriority.MinimumForegroundPriority)
						{
							Dispatcher.RunJobs(Priority, cts.Token);
						}
						else
						{
							Dispatcher.PushFrame(new DispatcherOperationFrame(this, timeout));
						}
					}
					else
					{
						Dispatcher.RunJobs(DispatcherPriority.MinimumActiveValue, cts.Token);
					}
				}
			}
			finally
			{
				Completed -= value;
				Aborted -= value;
			}
		}
		GetTask().GetAwaiter().GetResult();
	}

	public Task GetTask()
	{
		return GetTaskCore();
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public TaskAwaiter GetAwaiter()
	{
		return GetTask().GetAwaiter();
	}

	internal void DoAbort()
	{
		Status = DispatcherOperationStatus.Aborted;
		AbortTask();
		_aborted?.Invoke(this, EventArgs.Empty);
	}

	internal void Execute()
	{
		lock (Dispatcher.InstanceLock)
		{
			Status = DispatcherOperationStatus.Executing;
		}
		try
		{
			using (AvaloniaSynchronizationContext.Ensure(Dispatcher, Priority))
			{
				InvokeCore();
			}
		}
		finally
		{
			_completed?.Invoke(this, EventArgs.Empty);
		}
	}

	protected virtual void InvokeCore()
	{
		try
		{
			((Action)Callback)();
			lock (Dispatcher.InstanceLock)
			{
				Status = DispatcherOperationStatus.Completed;
				if (TaskSource is TaskCompletionSource<object> taskCompletionSource)
				{
					taskCompletionSource.SetResult(null);
				}
			}
		}
		catch (Exception exception)
		{
			lock (Dispatcher.InstanceLock)
			{
				Status = DispatcherOperationStatus.Completed;
				if (TaskSource is TaskCompletionSource<object> taskCompletionSource2)
				{
					taskCompletionSource2.SetException(exception);
				}
			}
			if (ThrowOnUiThread)
			{
				throw;
			}
		}
	}

	internal virtual object? GetResult()
	{
		return null;
	}

	protected virtual void AbortTask()
	{
		(TaskSource as TaskCompletionSource<object>)?.SetCanceled();
	}

	private static CancellationToken CreateCancelledToken()
	{
		CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
		cancellationTokenSource.Cancel();
		return cancellationTokenSource.Token;
	}

	protected virtual Task GetTaskCore()
	{
		lock (Dispatcher.InstanceLock)
		{
			if (Status == DispatcherOperationStatus.Aborted)
			{
				return s_abortedTask;
			}
			if (Status == DispatcherOperationStatus.Completed)
			{
				return Task.CompletedTask;
			}
			TaskCompletionSource<object> taskCompletionSource = TaskSource as TaskCompletionSource<object>;
			if (taskCompletionSource == null)
			{
				taskCompletionSource = (TaskCompletionSource<object>)(TaskSource = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously));
			}
			return taskCompletionSource.Task;
		}
	}
}
public class DispatcherOperation<T> : DispatcherOperation
{
	private TaskCompletionSource<T> TaskCompletionSource => (TaskCompletionSource<T>)TaskSource;

	public T Result
	{
		get
		{
			if (TaskCompletionSource.Task.IsCompleted || !base.Dispatcher.CheckAccess())
			{
				return TaskCompletionSource.Task.GetAwaiter().GetResult();
			}
			throw new InvalidOperationException("Synchronous wait is only supported on non-UI threads");
		}
	}

	public DispatcherOperation(Dispatcher dispatcher, DispatcherPriority priority, Func<T> callback)
		: base(dispatcher, priority, throwOnUiThread: false)
	{
		TaskSource = new TaskCompletionSource<T>();
		Callback = callback;
	}

	public new TaskAwaiter<T> GetAwaiter()
	{
		return GetTask().GetAwaiter();
	}

	public new Task<T> GetTask()
	{
		return TaskCompletionSource.Task;
	}

	protected override Task GetTaskCore()
	{
		return GetTask();
	}

	protected override void AbortTask()
	{
		TaskCompletionSource.SetCanceled();
	}

	internal override object? GetResult()
	{
		return GetTask().Result;
	}

	protected override void InvokeCore()
	{
		try
		{
			T result = ((Func<T>)Callback)();
			lock (base.Dispatcher.InstanceLock)
			{
				base.Status = DispatcherOperationStatus.Completed;
				TaskCompletionSource.SetResult(result);
			}
		}
		catch (Exception exception)
		{
			lock (base.Dispatcher.InstanceLock)
			{
				base.Status = DispatcherOperationStatus.Completed;
				TaskCompletionSource.SetException(exception);
			}
		}
	}
}
