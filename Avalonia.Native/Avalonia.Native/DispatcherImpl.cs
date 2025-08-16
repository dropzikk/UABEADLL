using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading;
using Avalonia.Native.Interop;
using Avalonia.Threading;
using MicroCom.Runtime;

namespace Avalonia.Native;

internal class DispatcherImpl : IControlledDispatcherImpl, IDispatcherImplWithPendingInput, IDispatcherImpl, IDispatcherImplWithExplicitBackgroundProcessing
{
	private class Events : NativeCallbackBase, IAvnPlatformThreadingInterfaceEvents, IUnknown, IDisposable
	{
		private readonly DispatcherImpl _parent;

		public Events(DispatcherImpl parent)
		{
			_parent = parent;
		}

		public void Signaled()
		{
			_parent.Signaled?.Invoke();
		}

		public void Timer()
		{
			_parent.Timer?.Invoke();
		}

		public void ReadyForBackgroundProcessing()
		{
			_parent.ReadyForBackgroundProcessing?.Invoke();
		}
	}

	private class RunLoopFrame : IDisposable
	{
		public ExceptionDispatchInfo? Exception;

		public CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

		public RunLoopFrame(CancellationToken token)
		{
			CancellationTokenSource = System.Threading.CancellationTokenSource.CreateLinkedTokenSource(token);
		}

		public void Dispose()
		{
			CancellationTokenSource.Dispose();
		}
	}

	private readonly IAvnPlatformThreadingInterface _native;

	private Thread? _loopThread;

	private Stopwatch _clock = Stopwatch.StartNew();

	private Stack<RunLoopFrame> _managedFrames = new Stack<RunLoopFrame>();

	public bool CurrentThreadIsLoopThread
	{
		get
		{
			if (_loopThread != null)
			{
				return Thread.CurrentThread == _loopThread;
			}
			if (_native.CurrentThreadIsLoopThread == 0)
			{
				return false;
			}
			_loopThread = Thread.CurrentThread;
			return true;
		}
	}

	public bool CanQueryPendingInput => false;

	public bool HasPendingInput => false;

	public long Now => _clock.ElapsedMilliseconds;

	public event Action Signaled;

	public event Action Timer;

	public event Action ReadyForBackgroundProcessing;

	public DispatcherImpl(IAvnPlatformThreadingInterface native)
	{
		_native = native;
		using Events events = new Events(this);
		_native.SetEvents(events);
	}

	public void Signal()
	{
		_native.Signal();
	}

	public void UpdateTimer(long? dueTimeInMs)
	{
		int ms = (int)((!dueTimeInMs.HasValue) ? (-1) : Math.Min(2147483637L, Math.Max(1L, dueTimeInMs.Value - Now)));
		_native.UpdateTimer(ms);
	}

	public void RunLoop(CancellationToken token)
	{
		if (token.IsCancellationRequested)
		{
			return;
		}
		object l = new object();
		bool exited = false;
		using RunLoopFrame runLoopFrame = new RunLoopFrame(token);
		IAvnLoopCancellation cancel = _native.CreateLoopCancellation();
		try
		{
			runLoopFrame.CancellationTokenSource.Token.Register(delegate
			{
				lock (l)
				{
					if (!exited)
					{
						cancel.Cancel();
					}
				}
			});
			try
			{
				_managedFrames.Push(runLoopFrame);
				_native.RunLoop(cancel);
			}
			finally
			{
				lock (l)
				{
					exited = true;
				}
				_managedFrames.Pop();
				if (runLoopFrame.Exception != null)
				{
					runLoopFrame.Exception.Throw();
				}
			}
		}
		finally
		{
			if (cancel != null)
			{
				cancel.Dispose();
			}
		}
	}

	public void PropagateCallbackException(ExceptionDispatchInfo capture)
	{
		if (_managedFrames.Count != 0)
		{
			RunLoopFrame runLoopFrame = _managedFrames.Peek();
			runLoopFrame.Exception = capture;
			runLoopFrame.CancellationTokenSource.Cancel();
		}
	}

	public void RequestBackgroundProcessing()
	{
		_native.RequestBackgroundProcessing();
	}
}
