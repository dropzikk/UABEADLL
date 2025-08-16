using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Platform;
using Avalonia.Utilities;

namespace Avalonia.Threading;

public class Dispatcher : IDispatcher
{
	public record struct DispatcherProcessingDisabled : IDisposable
	{
		private readonly SynchronizationContext? _oldContext;

		private readonly bool _restoreContext;

		private Dispatcher? _dispatcher;

		internal DispatcherProcessingDisabled(Dispatcher dispatcher)
		{
			_oldContext = null;
			_restoreContext = false;
			_dispatcher = dispatcher;
		}

		internal DispatcherProcessingDisabled(Dispatcher dispatcher, SynchronizationContext? oldContext)
			: this(dispatcher)
		{
			_oldContext = oldContext;
			_restoreContext = true;
		}

		public void Dispose()
		{
			if (_dispatcher != null)
			{
				_dispatcher.DisabledProcessingCount--;
				_dispatcher = null;
				if (_restoreContext)
				{
					SynchronizationContext.SetSynchronizationContext(_oldContext);
				}
			}
		}

		[CompilerGenerated]
		private readonly bool PrintMembers(StringBuilder builder)
		{
			return false;
		}
	}

	private class DummyShuttingDownUnitTestDispatcherImpl : IDispatcherImpl
	{
		public bool CurrentThreadIsLoopThread => true;

		public long Now => 0L;

		public event Action? Signaled;

		public event Action? Timer;

		public void Signal()
		{
		}

		public void UpdateTimer(long? dueTimeInMs)
		{
		}
	}

	private IDispatcherImpl _impl;

	private IControlledDispatcherImpl? _controlledImpl;

	private static Dispatcher? s_uiThread;

	private IDispatcherImplWithPendingInput? _pendingInputImpl;

	private readonly IDispatcherImplWithExplicitBackgroundProcessing? _backgroundProcessingImpl;

	private readonly AvaloniaSynchronizationContext?[] _priorityContexts = new AvaloniaSynchronizationContext[(int)DispatcherPriority.MaxValue - (int)DispatcherPriority.MinValue + 1];

	private bool _hasShutdownFinished;

	private bool _startingShutdown;

	private Stack<DispatcherFrame> _frames = new Stack<DispatcherFrame>();

	private readonly DispatcherPriorityQueue _queue = new DispatcherPriorityQueue();

	private bool _signaled;

	private bool _explicitBackgroundProcessingRequested;

	private const int MaximumInputStarvationTimeInFallbackMode = 50;

	private const int MaximumInputStarvationTimeInExplicitProcessingExplicitMode = 50;

	private int _maximumInputStarvationTime;

	private List<DispatcherTimer> _timers = new List<DispatcherTimer>();

	private long _timersVersion;

	private bool _dueTimeFound;

	private long _dueTimeInMs;

	private long? _dueTimeForTimers;

	private long? _dueTimeForBackgroundProcessing;

	private long? _osTimerSetTo;

	internal object InstanceLock { get; } = new object();

	public static Dispatcher UIThread => s_uiThread ?? (s_uiThread = CreateUIThreadDispatcher());

	public bool SupportsRunLoops => _controlledImpl != null;

	internal bool ExitAllFramesRequested { get; private set; }

	internal bool HasShutdownStarted { get; private set; }

	internal int DisabledProcessingCount { get; set; }

	internal long Now => _impl.Now;

	public event EventHandler? ShutdownStarted;

	public event EventHandler? ShutdownFinished;

	internal Dispatcher(IDispatcherImpl impl)
	{
		_impl = impl;
		impl.Timer += OnOSTimer;
		impl.Signaled += Signaled;
		_controlledImpl = _impl as IControlledDispatcherImpl;
		_pendingInputImpl = _impl as IDispatcherImplWithPendingInput;
		_backgroundProcessingImpl = _impl as IDispatcherImplWithExplicitBackgroundProcessing;
		_maximumInputStarvationTime = ((_backgroundProcessingImpl == null) ? 50 : 50);
		if (_backgroundProcessingImpl != null)
		{
			_backgroundProcessingImpl.ReadyForBackgroundProcessing += OnReadyForExplicitBackgroundProcessing;
		}
	}

	private static Dispatcher CreateUIThreadDispatcher()
	{
		IDispatcherImpl dispatcherImpl = AvaloniaLocator.Current.GetService<IDispatcherImpl>();
		if (dispatcherImpl == null)
		{
			IPlatformThreadingInterface service = AvaloniaLocator.Current.GetService<IPlatformThreadingInterface>();
			dispatcherImpl = ((service == null) ? ((IDispatcherImpl)new NullDispatcherImpl()) : ((IDispatcherImpl)new LegacyDispatcherImpl(service)));
		}
		return new Dispatcher(dispatcherImpl);
	}

	public bool CheckAccess()
	{
		return _impl?.CurrentThreadIsLoopThread ?? true;
	}

	public void VerifyAccess()
	{
		if (!CheckAccess())
		{
			ThrowVerifyAccess();
		}
		[MethodImpl(MethodImplOptions.NoInlining)]
		[DoesNotReturn]
		static void ThrowVerifyAccess()
		{
			throw new InvalidOperationException("Call from invalid thread");
		}
	}

	internal AvaloniaSynchronizationContext GetContextWithPriority(DispatcherPriority priority)
	{
		DispatcherPriority.Validate(priority, "priority");
		int num = (int)priority - (int)DispatcherPriority.MinValue;
		AvaloniaSynchronizationContext[] priorityContexts = _priorityContexts;
		int num2 = num;
		return priorityContexts[num2] ?? (priorityContexts[num2] = new AvaloniaSynchronizationContext(priority));
	}

	public void Invoke(Action callback)
	{
		Invoke(callback, DispatcherPriority.Send, CancellationToken.None, TimeSpan.FromMilliseconds(-1.0));
	}

	public void Invoke(Action callback, DispatcherPriority priority)
	{
		Invoke(callback, priority, CancellationToken.None, TimeSpan.FromMilliseconds(-1.0));
	}

	public void Invoke(Action callback, DispatcherPriority priority, CancellationToken cancellationToken)
	{
		Invoke(callback, priority, cancellationToken, TimeSpan.FromMilliseconds(-1.0));
	}

	public void Invoke(Action callback, DispatcherPriority priority, CancellationToken cancellationToken, TimeSpan timeout)
	{
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		DispatcherPriority.Validate(priority, "priority");
		if (timeout.TotalMilliseconds < 0.0 && timeout != TimeSpan.FromMilliseconds(-1.0))
		{
			throw new ArgumentOutOfRangeException("timeout");
		}
		if (!cancellationToken.IsCancellationRequested && priority == DispatcherPriority.Send && CheckAccess())
		{
			using (AvaloniaSynchronizationContext.Ensure(this, priority))
			{
				callback();
				return;
			}
		}
		DispatcherOperation operation = new DispatcherOperation(this, priority, callback, throwOnUiThread: false);
		InvokeImpl(operation, cancellationToken, timeout);
	}

	public TResult Invoke<TResult>(Func<TResult> callback)
	{
		return Invoke(callback, DispatcherPriority.Send, CancellationToken.None, TimeSpan.FromMilliseconds(-1.0));
	}

	public TResult Invoke<TResult>(Func<TResult> callback, DispatcherPriority priority)
	{
		return Invoke(callback, priority, CancellationToken.None, TimeSpan.FromMilliseconds(-1.0));
	}

	public TResult Invoke<TResult>(Func<TResult> callback, DispatcherPriority priority, CancellationToken cancellationToken)
	{
		return Invoke(callback, priority, cancellationToken, TimeSpan.FromMilliseconds(-1.0));
	}

	public TResult Invoke<TResult>(Func<TResult> callback, DispatcherPriority priority, CancellationToken cancellationToken, TimeSpan timeout)
	{
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		DispatcherPriority.Validate(priority, "priority");
		if (timeout.TotalMilliseconds < 0.0 && timeout != TimeSpan.FromMilliseconds(-1.0))
		{
			throw new ArgumentOutOfRangeException("timeout");
		}
		if (!cancellationToken.IsCancellationRequested && priority == DispatcherPriority.Send && CheckAccess())
		{
			using (AvaloniaSynchronizationContext.Ensure(this, priority))
			{
				return callback();
			}
		}
		DispatcherOperation<TResult> operation = new DispatcherOperation<TResult>(this, priority, callback);
		return (TResult)InvokeImpl(operation, cancellationToken, timeout);
	}

	public DispatcherOperation InvokeAsync(Action callback)
	{
		return InvokeAsync(callback, default(DispatcherPriority), CancellationToken.None);
	}

	public DispatcherOperation InvokeAsync(Action callback, DispatcherPriority priority)
	{
		return InvokeAsync(callback, priority, CancellationToken.None);
	}

	public DispatcherOperation InvokeAsync(Action callback, DispatcherPriority priority, CancellationToken cancellationToken)
	{
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		DispatcherPriority.Validate(priority, "priority");
		DispatcherOperation dispatcherOperation = new DispatcherOperation(this, priority, callback, throwOnUiThread: false);
		InvokeAsyncImpl(dispatcherOperation, cancellationToken);
		return dispatcherOperation;
	}

	public DispatcherOperation<TResult> InvokeAsync<TResult>(Func<TResult> callback)
	{
		return InvokeAsync(callback, DispatcherPriority.Default, CancellationToken.None);
	}

	public DispatcherOperation<TResult> InvokeAsync<TResult>(Func<TResult> callback, DispatcherPriority priority)
	{
		return InvokeAsync(callback, priority, CancellationToken.None);
	}

	public DispatcherOperation<TResult> InvokeAsync<TResult>(Func<TResult> callback, DispatcherPriority priority, CancellationToken cancellationToken)
	{
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		DispatcherPriority.Validate(priority, "priority");
		DispatcherOperation<TResult> dispatcherOperation = new DispatcherOperation<TResult>(this, priority, callback);
		InvokeAsyncImpl(dispatcherOperation, cancellationToken);
		return dispatcherOperation;
	}

	internal void InvokeAsyncImpl(DispatcherOperation operation, CancellationToken cancellationToken)
	{
		bool flag = false;
		lock (InstanceLock)
		{
			if (!cancellationToken.IsCancellationRequested && !_hasShutdownFinished && !Environment.HasShutdownStarted)
			{
				_queue.Enqueue(operation.Priority, operation);
				flag = RequestProcessing();
				if (!flag)
				{
					_queue.RemoveItem(operation);
				}
			}
		}
		if (flag)
		{
			if (cancellationToken.CanBeCanceled)
			{
				CancellationTokenRegistration cancellationRegistration = cancellationToken.Register(delegate(object? s)
				{
					((DispatcherOperation)s).Abort();
				}, operation);
				operation.Aborted += delegate
				{
					cancellationRegistration.Dispose();
				};
				operation.Completed += delegate
				{
					cancellationRegistration.Dispose();
				};
			}
		}
		else
		{
			operation.DoAbort();
		}
	}

	private object? InvokeImpl(DispatcherOperation operation, CancellationToken cancellationToken, TimeSpan timeout)
	{
		object result = null;
		if (!cancellationToken.IsCancellationRequested)
		{
			InvokeAsyncImpl(operation, cancellationToken);
			CancellationToken cancellationToken2 = CancellationToken.None;
			CancellationTokenRegistration cancellationTokenRegistration = default(CancellationTokenRegistration);
			CancellationTokenSource cancellationTokenSource = null;
			if (timeout.TotalMilliseconds >= 0.0)
			{
				cancellationTokenSource = new CancellationTokenSource(timeout);
				cancellationToken2 = cancellationTokenSource.Token;
				cancellationTokenRegistration = cancellationToken2.Register(delegate(object? s)
				{
					((DispatcherOperation)s).Abort();
				}, operation);
			}
			try
			{
				operation.Wait();
				result = operation.GetResult();
			}
			catch (OperationCanceledException)
			{
				if (cancellationToken2.IsCancellationRequested)
				{
					throw new TimeoutException();
				}
				throw;
			}
			finally
			{
				cancellationTokenRegistration.Dispose();
				cancellationTokenSource?.Dispose();
			}
		}
		return result;
	}

	public void Post(Action action, DispatcherPriority priority = default(DispatcherPriority))
	{
		if (action == null)
		{
			throw new ArgumentNullException("action");
		}
		InvokeAsyncImpl(new DispatcherOperation(this, priority, action, throwOnUiThread: true), CancellationToken.None);
	}

	public Task InvokeAsync(Func<Task> callback)
	{
		return InvokeAsync(callback, DispatcherPriority.Default);
	}

	public Task InvokeAsync(Func<Task> callback, DispatcherPriority priority)
	{
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		return this.InvokeAsync<Task>(callback, priority).GetTask().Unwrap();
	}

	public Task<TResult> InvokeAsync<TResult>(Func<Task<TResult>> action)
	{
		return InvokeAsync(action, DispatcherPriority.Default);
	}

	public Task<TResult> InvokeAsync<TResult>(Func<Task<TResult>> action, DispatcherPriority priority)
	{
		if (action == null)
		{
			throw new ArgumentNullException("action");
		}
		return this.InvokeAsync<Task<TResult>>(action, priority).GetTask().Unwrap();
	}

	public void Post(SendOrPostCallback action, object? arg, DispatcherPriority priority = default(DispatcherPriority))
	{
		if (action == null)
		{
			throw new ArgumentNullException("action");
		}
		InvokeAsyncImpl(new SendOrPostCallbackDispatcherOperation(this, priority, action, arg, throwOnUiThread: true), CancellationToken.None);
	}

	public void PushFrame(DispatcherFrame frame)
	{
		VerifyAccess();
		if (_controlledImpl == null)
		{
			throw new PlatformNotSupportedException();
		}
		if (frame == null)
		{
			throw new ArgumentNullException("frame");
		}
		if (_hasShutdownFinished)
		{
			throw new InvalidOperationException("Cannot perform requested operation because the Dispatcher shut down");
		}
		if (DisabledProcessingCount > 0)
		{
			throw new InvalidOperationException("Cannot perform this operation while dispatcher processing is suspended.");
		}
		try
		{
			_frames.Push(frame);
			using (AvaloniaSynchronizationContext.Ensure(this, DispatcherPriority.Normal))
			{
				frame.Run(_controlledImpl);
			}
		}
		finally
		{
			_frames.Pop();
			if (_frames.Count == 0)
			{
				if (HasShutdownStarted)
				{
					ShutdownImpl();
				}
				else
				{
					ExitAllFramesRequested = false;
				}
			}
		}
	}

	public void MainLoop(CancellationToken cancellationToken)
	{
		if (_controlledImpl == null)
		{
			throw new PlatformNotSupportedException();
		}
		DispatcherFrame frame = new DispatcherFrame();
		cancellationToken.Register(delegate
		{
			frame.Continue = false;
		});
		PushFrame(frame);
	}

	public void ExitAllFrames()
	{
		if (_frames.Count == 0)
		{
			return;
		}
		ExitAllFramesRequested = true;
		foreach (DispatcherFrame frame in _frames)
		{
			frame.MaybeExitOnDispatcherRequest();
		}
	}

	public void BeginInvokeShutdown(DispatcherPriority priority)
	{
		Post(StartShutdownImpl, priority);
	}

	public void InvokeShutdown()
	{
		Invoke(StartShutdownImpl, DispatcherPriority.Send);
	}

	private void StartShutdownImpl()
	{
		if (!_startingShutdown)
		{
			_startingShutdown = true;
			this.ShutdownStarted?.Invoke(this, EventArgs.Empty);
			HasShutdownStarted = true;
			if (_frames.Count > 0)
			{
				ExitAllFrames();
			}
			else
			{
				ShutdownImpl();
			}
		}
	}

	private void ShutdownImpl()
	{
		DispatcherOperation dispatcherOperation = null;
		_impl.Timer -= PromoteTimers;
		_impl.Signaled -= Signaled;
		do
		{
			lock (InstanceLock)
			{
				dispatcherOperation = ((!(_queue.MaxPriority != DispatcherPriority.Invalid)) ? null : _queue.Peek());
			}
			dispatcherOperation?.Abort();
		}
		while (dispatcherOperation != null);
		_impl.UpdateTimer(null);
		_hasShutdownFinished = true;
		this.ShutdownFinished?.Invoke(this, EventArgs.Empty);
	}

	public DispatcherProcessingDisabled DisableProcessing()
	{
		VerifyAccess();
		DisabledProcessingCount++;
		SynchronizationContext current = SynchronizationContext.Current;
		if (current is AvaloniaSynchronizationContext || current is NonPumpingSyncContext)
		{
			return new DispatcherProcessingDisabled(this);
		}
		NonPumpingLockHelper.IHelperImpl service = AvaloniaLocator.Current.GetService<NonPumpingLockHelper.IHelperImpl>();
		if (service == null)
		{
			return new DispatcherProcessingDisabled(this);
		}
		SynchronizationContext.SetSynchronizationContext(new NonPumpingSyncContext(service, current));
		return new DispatcherProcessingDisabled(this, current);
	}

	private void RequestBackgroundProcessing()
	{
		lock (InstanceLock)
		{
			if (_backgroundProcessingImpl != null)
			{
				if (!_explicitBackgroundProcessingRequested)
				{
					_explicitBackgroundProcessingRequested = true;
					_backgroundProcessingImpl.RequestBackgroundProcessing();
				}
			}
			else if (!_dueTimeForBackgroundProcessing.HasValue)
			{
				_dueTimeForBackgroundProcessing = Now + 1;
				UpdateOSTimer();
			}
		}
	}

	private void OnReadyForExplicitBackgroundProcessing()
	{
		lock (InstanceLock)
		{
			_explicitBackgroundProcessingRequested = false;
		}
		ExecuteJobsCore(fromExplicitBackgroundProcessingCallback: true);
	}

	public void RunJobs(DispatcherPriority? priority = null)
	{
		RunJobs(priority, CancellationToken.None);
	}

	internal void RunJobs(DispatcherPriority? priority, CancellationToken cancellationToken)
	{
		if (DisabledProcessingCount > 0)
		{
			throw new InvalidOperationException("Cannot perform this operation while dispatcher processing is suspended.");
		}
		DispatcherPriority valueOrDefault = priority.GetValueOrDefault();
		if (!priority.HasValue)
		{
			valueOrDefault = DispatcherPriority.MinimumActiveValue;
			priority = valueOrDefault;
		}
		if (priority < DispatcherPriority.MinimumActiveValue)
		{
			priority = DispatcherPriority.MinimumActiveValue;
		}
		while (!cancellationToken.IsCancellationRequested)
		{
			DispatcherOperation dispatcherOperation;
			lock (InstanceLock)
			{
				dispatcherOperation = _queue.Peek();
			}
			if (dispatcherOperation == null || (priority.HasValue && dispatcherOperation.Priority < priority.Value))
			{
				break;
			}
			ExecuteJob(dispatcherOperation);
		}
	}

	internal static void ResetBeforeUnitTests()
	{
		s_uiThread = null;
	}

	internal static void ResetForUnitTests()
	{
		if (s_uiThread == null)
		{
			return;
		}
		Stopwatch stopwatch = Stopwatch.StartNew();
		while (true)
		{
			s_uiThread._pendingInputImpl = (s_uiThread._controlledImpl = null);
			s_uiThread._impl = new DummyShuttingDownUnitTestDispatcherImpl();
			if (stopwatch.Elapsed.TotalSeconds > 5.0)
			{
				throw new InvalidProgramException("You've caused dispatcher loop");
			}
			DispatcherOperation dispatcherOperation;
			lock (s_uiThread.InstanceLock)
			{
				dispatcherOperation = s_uiThread._queue.Peek();
			}
			if (dispatcherOperation == null || dispatcherOperation.Priority <= DispatcherPriority.Inactive)
			{
				break;
			}
			s_uiThread.ExecuteJob(dispatcherOperation);
		}
		s_uiThread = null;
	}

	private void ExecuteJob(DispatcherOperation job)
	{
		lock (InstanceLock)
		{
			_queue.RemoveItem(job);
		}
		job.Execute();
		PromoteTimers();
	}

	private void Signaled()
	{
		lock (InstanceLock)
		{
			_signaled = false;
		}
		ExecuteJobsCore(fromExplicitBackgroundProcessingCallback: false);
	}

	private void ExecuteJobsCore(bool fromExplicitBackgroundProcessingCallback)
	{
		long? num = null;
		while (true)
		{
			DispatcherOperation dispatcherOperation;
			lock (InstanceLock)
			{
				dispatcherOperation = _queue.Peek();
			}
			if (dispatcherOperation == null || dispatcherOperation.Priority < DispatcherPriority.MinimumActiveValue)
			{
				return;
			}
			if (dispatcherOperation.Priority > DispatcherPriority.Input)
			{
				ExecuteJob(dispatcherOperation);
				continue;
			}
			IDispatcherImplWithPendingInput? pendingInputImpl = _pendingInputImpl;
			if (pendingInputImpl != null && pendingInputImpl.CanQueryPendingInput)
			{
				if (!_pendingInputImpl.HasPendingInput)
				{
					ExecuteJob(dispatcherOperation);
					continue;
				}
				RequestBackgroundProcessing();
				return;
			}
			if (_backgroundProcessingImpl != null && !fromExplicitBackgroundProcessingCallback)
			{
				RequestBackgroundProcessing();
				return;
			}
			if (!num.HasValue)
			{
				num = Now;
			}
			if (Now - num.Value > _maximumInputStarvationTime)
			{
				break;
			}
			ExecuteJob(dispatcherOperation);
		}
		_signaled = true;
		RequestBackgroundProcessing();
	}

	internal bool RequestProcessing()
	{
		lock (InstanceLock)
		{
			if (!CheckAccess())
			{
				RequestForegroundProcessing();
				return true;
			}
			if (_queue.MaxPriority <= DispatcherPriority.Input)
			{
				IDispatcherImplWithPendingInput pendingInputImpl = _pendingInputImpl;
				if (pendingInputImpl != null && pendingInputImpl.CanQueryPendingInput && !pendingInputImpl.HasPendingInput)
				{
					RequestForegroundProcessing();
				}
				else
				{
					RequestBackgroundProcessing();
				}
			}
			else
			{
				RequestForegroundProcessing();
			}
		}
		return true;
	}

	private void RequestForegroundProcessing()
	{
		if (!_signaled)
		{
			_signaled = true;
			_impl.Signal();
		}
	}

	internal void Abort(DispatcherOperation operation)
	{
		lock (InstanceLock)
		{
			_queue.RemoveItem(operation);
		}
		operation.DoAbort();
	}

	internal bool SetPriority(DispatcherOperation operation, DispatcherPriority priority)
	{
		bool flag = false;
		lock (InstanceLock)
		{
			if (operation.IsQueued)
			{
				_queue.ChangeItemPriority(operation, priority);
				flag = true;
				if (flag)
				{
					RequestProcessing();
				}
			}
		}
		return flag;
	}

	public bool HasJobsWithPriority(DispatcherPriority priority)
	{
		lock (InstanceLock)
		{
			return _queue.MaxPriority >= priority;
		}
	}

	private void UpdateOSTimer()
	{
		VerifyAccess();
		long? num = ((_dueTimeForTimers.HasValue && _dueTimeForBackgroundProcessing.HasValue) ? new long?(Math.Min(_dueTimeForTimers.Value, _dueTimeForBackgroundProcessing.Value)) : (_dueTimeForTimers ?? _dueTimeForBackgroundProcessing));
		if (_osTimerSetTo != num)
		{
			_impl.UpdateTimer(_osTimerSetTo = num);
		}
	}

	internal void RescheduleTimers()
	{
		if (!CheckAccess())
		{
			Post(RescheduleTimers, DispatcherPriority.Send);
			return;
		}
		lock (InstanceLock)
		{
			if (_hasShutdownFinished)
			{
				return;
			}
			bool dueTimeFound = _dueTimeFound;
			long dueTimeInMs = _dueTimeInMs;
			_dueTimeFound = false;
			_dueTimeInMs = 0L;
			if (_timers.Count > 0)
			{
				for (int i = 0; i < _timers.Count; i++)
				{
					DispatcherTimer dispatcherTimer = _timers[i];
					if (!_dueTimeFound || dispatcherTimer.DueTimeInMs - _dueTimeInMs < 0)
					{
						_dueTimeFound = true;
						_dueTimeInMs = dispatcherTimer.DueTimeInMs;
					}
				}
			}
			if (_dueTimeFound)
			{
				if (!_dueTimeForTimers.HasValue || !dueTimeFound || dueTimeInMs != _dueTimeInMs)
				{
					_dueTimeForTimers = _dueTimeInMs;
					UpdateOSTimer();
				}
			}
			else if (dueTimeFound)
			{
				_dueTimeForTimers = null;
				UpdateOSTimer();
			}
		}
	}

	internal void AddTimer(DispatcherTimer timer)
	{
		lock (InstanceLock)
		{
			if (!_hasShutdownFinished)
			{
				_timers.Add(timer);
				_timersVersion++;
			}
		}
		RescheduleTimers();
	}

	internal void RemoveTimer(DispatcherTimer timer)
	{
		lock (InstanceLock)
		{
			if (!_hasShutdownFinished)
			{
				_timers.Remove(timer);
				_timersVersion++;
			}
		}
		RescheduleTimers();
	}

	private void OnOSTimer()
	{
		_impl.UpdateTimer(null);
		_osTimerSetTo = null;
		bool flag = false;
		bool flag2 = false;
		lock (InstanceLock)
		{
			_impl.UpdateTimer(_osTimerSetTo = null);
			flag = _dueTimeForTimers.HasValue && _dueTimeForTimers.Value <= Now;
			if (flag)
			{
				_dueTimeForTimers = null;
			}
			flag2 = _dueTimeForBackgroundProcessing.HasValue && _dueTimeForBackgroundProcessing.Value <= Now;
			if (flag2)
			{
				_dueTimeForBackgroundProcessing = null;
			}
		}
		if (flag)
		{
			PromoteTimers();
		}
		if (flag2)
		{
			ExecuteJobsCore(fromExplicitBackgroundProcessingCallback: false);
		}
		UpdateOSTimer();
	}

	internal void PromoteTimers()
	{
		long now = Now;
		try
		{
			List<DispatcherTimer> list = null;
			long num = 0L;
			lock (InstanceLock)
			{
				if (!_hasShutdownFinished && _dueTimeFound && _dueTimeInMs - now <= 0)
				{
					list = _timers;
					num = _timersVersion;
				}
			}
			if (list == null)
			{
				return;
			}
			DispatcherTimer dispatcherTimer = null;
			int i = 0;
			do
			{
				lock (InstanceLock)
				{
					dispatcherTimer = null;
					if (num != _timersVersion)
					{
						num = _timersVersion;
						i = 0;
					}
					for (; i < _timers.Count; i++)
					{
						if (list[i].DueTimeInMs - now <= 0)
						{
							dispatcherTimer = list[i];
							list.RemoveAt(i);
							break;
						}
					}
				}
				dispatcherTimer?.Promote();
			}
			while (dispatcherTimer != null);
		}
		finally
		{
			RescheduleTimers();
		}
	}

	internal static List<DispatcherTimer> SnapshotTimersForUnitTests()
	{
		return s_uiThread._timers.ToList();
	}
}
