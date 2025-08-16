using System;
using System.Threading;

namespace Avalonia.Threading;

public class DispatcherFrame
{
	private bool _exitWhenRequested;

	private bool _continue;

	private bool _isRunning;

	private CancellationTokenSource? _cancellationTokenSource;

	public Dispatcher Dispatcher { get; }

	public bool Continue
	{
		get
		{
			bool flag = _continue;
			if (flag && _exitWhenRequested)
			{
				Dispatcher dispatcher = Dispatcher;
				if (dispatcher.ExitAllFramesRequested || dispatcher.HasShutdownStarted)
				{
					flag = false;
				}
			}
			return flag;
		}
		set
		{
			lock (Dispatcher.InstanceLock)
			{
				_continue = value;
				if (!_continue)
				{
					_cancellationTokenSource?.Cancel();
				}
			}
		}
	}

	public DispatcherFrame()
		: this(exitWhenRequested: true)
	{
	}

	public DispatcherFrame(bool exitWhenRequested)
		: this(Avalonia.Threading.Dispatcher.UIThread, exitWhenRequested)
	{
		Dispatcher.VerifyAccess();
	}

	internal DispatcherFrame(Dispatcher dispatcher, bool exitWhenRequested)
	{
		Dispatcher = dispatcher;
		_exitWhenRequested = exitWhenRequested;
		_continue = true;
	}

	internal void Run(IControlledDispatcherImpl impl)
	{
		Dispatcher.VerifyAccess();
		while (true)
		{
			lock (Dispatcher.InstanceLock)
			{
				if (!Continue)
				{
					break;
				}
				if (_isRunning)
				{
					throw new InvalidOperationException("This frame is already running");
				}
				_cancellationTokenSource = new CancellationTokenSource();
				_isRunning = true;
			}
			try
			{
				Dispatcher.RequestProcessing();
				impl.RunLoop(_cancellationTokenSource.Token);
			}
			finally
			{
				lock (Dispatcher.InstanceLock)
				{
					_isRunning = false;
					_cancellationTokenSource?.Cancel();
					_cancellationTokenSource?.Dispose();
					_cancellationTokenSource = null;
				}
			}
		}
	}

	internal void MaybeExitOnDispatcherRequest()
	{
		if (_exitWhenRequested)
		{
			_cancellationTokenSource?.Cancel();
		}
	}
}
