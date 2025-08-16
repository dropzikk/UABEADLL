using System;
using System.Diagnostics;
using System.Threading;
using Avalonia.Metadata;
using Avalonia.Threading;

namespace Avalonia.Controls.Platform;

[Unstable]
public class ManagedDispatcherImpl : IControlledDispatcherImpl, IDispatcherImplWithPendingInput, IDispatcherImpl
{
	public interface IManagedDispatcherInputProvider
	{
		bool HasInput { get; }

		void DispatchNextInputEvent();
	}

	private readonly IManagedDispatcherInputProvider? _inputProvider;

	private readonly AutoResetEvent _wakeup = new AutoResetEvent(initialState: false);

	private bool _signaled;

	private readonly object _lock = new object();

	private readonly Stopwatch _clock = Stopwatch.StartNew();

	private TimeSpan? _nextTimer;

	private readonly Thread _loopThread = Thread.CurrentThread;

	public bool CurrentThreadIsLoopThread => _loopThread == Thread.CurrentThread;

	public long Now => _clock.ElapsedMilliseconds;

	public bool CanQueryPendingInput => _inputProvider != null;

	public bool HasPendingInput => _inputProvider?.HasInput ?? false;

	public event Action? Signaled;

	public event Action? Timer;

	public ManagedDispatcherImpl(IManagedDispatcherInputProvider? inputProvider)
	{
		_inputProvider = inputProvider;
	}

	public void Signal()
	{
		lock (_lock)
		{
			_signaled = true;
			_wakeup.Set();
		}
	}

	public void UpdateTimer(long? dueTimeInMs)
	{
		lock (_lock)
		{
			_nextTimer = ((!dueTimeInMs.HasValue) ? ((TimeSpan?)null) : new TimeSpan?(TimeSpan.FromMilliseconds(dueTimeInMs.Value)));
			if (!CurrentThreadIsLoopThread)
			{
				_wakeup.Set();
			}
		}
	}

	public void RunLoop(CancellationToken token)
	{
		CancellationTokenRegistration cancellationTokenRegistration = default(CancellationTokenRegistration);
		if (token.CanBeCanceled)
		{
			cancellationTokenRegistration = token.Register(delegate
			{
				_wakeup.Set();
			});
		}
		while (!token.IsCancellationRequested)
		{
			bool signaled;
			lock (_lock)
			{
				signaled = _signaled;
				_signaled = false;
			}
			if (signaled)
			{
				this.Signaled?.Invoke();
				continue;
			}
			bool flag = false;
			lock (_lock)
			{
				if (_nextTimer < _clock.Elapsed)
				{
					flag = true;
					_nextTimer = null;
				}
			}
			if (flag)
			{
				this.Timer?.Invoke();
				continue;
			}
			IManagedDispatcherInputProvider? inputProvider = _inputProvider;
			if (inputProvider != null && inputProvider.HasInput)
			{
				_inputProvider.DispatchNextInputEvent();
				continue;
			}
			TimeSpan? nextTimer;
			lock (_lock)
			{
				nextTimer = _nextTimer;
			}
			if (nextTimer.HasValue)
			{
				TimeSpan timeout = nextTimer.Value - _clock.Elapsed;
				if (!(timeout.TotalMilliseconds < 1.0))
				{
					_wakeup.WaitOne(timeout);
				}
			}
			else
			{
				_wakeup.WaitOne();
			}
		}
		cancellationTokenRegistration.Dispose();
	}
}
