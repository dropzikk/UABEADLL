using System;
using Avalonia.Reactive;

namespace Avalonia.Threading;

public class DispatcherTimer
{
	private object _instanceLock = new object();

	private Dispatcher _dispatcher;

	private DispatcherPriority _priority;

	private TimeSpan _interval;

	private DispatcherOperation? _operation;

	private bool _isEnabled;

	public Dispatcher Dispatcher => _dispatcher;

	public bool IsEnabled
	{
		get
		{
			return _isEnabled;
		}
		set
		{
			lock (_instanceLock)
			{
				if (!value && _isEnabled)
				{
					Stop();
				}
				else if (value && !_isEnabled)
				{
					Start();
				}
			}
		}
	}

	public TimeSpan Interval
	{
		get
		{
			return _interval;
		}
		set
		{
			bool flag = false;
			if (value.TotalMilliseconds < 0.0)
			{
				throw new ArgumentOutOfRangeException("value", "TimeSpan period must be greater than or equal to zero.");
			}
			if (value.TotalMilliseconds > 2147483647.0)
			{
				throw new ArgumentOutOfRangeException("value", "TimeSpan period must be less than or equal to Int32.MaxValue.");
			}
			lock (_instanceLock)
			{
				_interval = value;
				if (_isEnabled)
				{
					DueTimeInMs = _dispatcher.Now + (long)_interval.TotalMilliseconds;
					flag = true;
				}
			}
			if (flag)
			{
				_dispatcher.RescheduleTimers();
			}
		}
	}

	public object? Tag { get; set; }

	internal long DueTimeInMs { get; private set; }

	public event EventHandler? Tick;

	public DispatcherTimer()
		: this(DispatcherPriority.Background)
	{
	}

	public DispatcherTimer(DispatcherPriority priority)
		: this(Avalonia.Threading.Dispatcher.UIThread, priority, TimeSpan.FromMilliseconds(0.0))
	{
	}

	internal DispatcherTimer(DispatcherPriority priority, Dispatcher dispatcher)
		: this(dispatcher, priority, TimeSpan.FromMilliseconds(0.0))
	{
	}

	public DispatcherTimer(TimeSpan interval, DispatcherPriority priority, EventHandler callback)
		: this(Avalonia.Threading.Dispatcher.UIThread, priority, interval)
	{
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		Tick += callback;
		Start();
	}

	public void Start()
	{
		lock (_instanceLock)
		{
			if (!_isEnabled)
			{
				_isEnabled = true;
				Restart();
			}
		}
	}

	public void Stop()
	{
		bool flag = false;
		lock (_instanceLock)
		{
			if (_isEnabled)
			{
				_isEnabled = false;
				flag = true;
				if (_operation != null)
				{
					_operation.Abort();
					_operation = null;
				}
			}
		}
		if (flag)
		{
			_dispatcher.RemoveTimer(this);
		}
	}

	public static IDisposable Run(Func<bool> action, TimeSpan interval, DispatcherPriority priority = default(DispatcherPriority))
	{
		DispatcherTimer timer = new DispatcherTimer(priority)
		{
			Interval = interval
		};
		timer.Tick += delegate
		{
			if (!action())
			{
				timer.Stop();
			}
		};
		timer.Start();
		return Disposable.Create(delegate
		{
			timer.Stop();
		});
	}

	public static IDisposable RunOnce(Action action, TimeSpan interval, DispatcherPriority priority = default(DispatcherPriority))
	{
		interval = ((interval != TimeSpan.Zero) ? interval : TimeSpan.FromTicks(1L));
		DispatcherTimer timer = new DispatcherTimer(priority)
		{
			Interval = interval
		};
		timer.Tick += delegate
		{
			action();
			timer.Stop();
		};
		timer.Start();
		return Disposable.Create(delegate
		{
			timer.Stop();
		});
	}

	internal DispatcherTimer(Dispatcher dispatcher, DispatcherPriority priority, TimeSpan interval)
	{
		if (dispatcher == null)
		{
			throw new ArgumentNullException("dispatcher");
		}
		DispatcherPriority.Validate(priority, "priority");
		if (priority == DispatcherPriority.Inactive)
		{
			throw new ArgumentException("Specified priority is not valid.", "priority");
		}
		if (interval.TotalMilliseconds < 0.0)
		{
			throw new ArgumentOutOfRangeException("interval", "TimeSpan period must be greater than or equal to zero.");
		}
		if (interval.TotalMilliseconds > 2147483647.0)
		{
			throw new ArgumentOutOfRangeException("interval", "TimeSpan period must be less than or equal to Int32.MaxValue.");
		}
		_dispatcher = dispatcher;
		_priority = priority;
		_interval = interval;
	}

	private void Restart()
	{
		lock (_instanceLock)
		{
			if (_operation == null)
			{
				_operation = _dispatcher.InvokeAsync((Action)FireTick, DispatcherPriority.Inactive);
				DueTimeInMs = _dispatcher.Now + (long)_interval.TotalMilliseconds;
				if (_interval.TotalMilliseconds == 0.0 && _dispatcher.CheckAccess())
				{
					Promote();
				}
				else
				{
					_dispatcher.AddTimer(this);
				}
			}
		}
	}

	internal void Promote()
	{
		lock (_instanceLock)
		{
			if (_operation != null)
			{
				_operation.Priority = _priority;
			}
		}
	}

	private void FireTick()
	{
		_operation = null;
		if (this.Tick != null)
		{
			this.Tick(this, EventArgs.Empty);
		}
		if (_isEnabled)
		{
			Restart();
		}
	}
}
