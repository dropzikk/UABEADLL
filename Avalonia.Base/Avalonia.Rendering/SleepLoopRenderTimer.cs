using System;
using System.Diagnostics;
using System.Threading;
using Avalonia.Metadata;

namespace Avalonia.Rendering;

[PrivateApi]
public class SleepLoopRenderTimer : IRenderTimer
{
	private Action<TimeSpan>? _tick;

	private int _count;

	private readonly object _lock = new object();

	private bool _running;

	private readonly Stopwatch _st = Stopwatch.StartNew();

	private readonly TimeSpan _timeBetweenTicks;

	public bool RunsInBackground => true;

	public event Action<TimeSpan> Tick
	{
		add
		{
			lock (_lock)
			{
				_tick = (Action<TimeSpan>)Delegate.Combine(_tick, value);
				_count++;
				if (!_running)
				{
					_running = true;
					Thread thread = new Thread(LoopProc);
					thread.IsBackground = true;
					thread.Start();
				}
			}
		}
		remove
		{
			lock (_lock)
			{
				_tick = (Action<TimeSpan>)Delegate.Remove(_tick, value);
				_count--;
			}
		}
	}

	public SleepLoopRenderTimer(int fps)
	{
		_timeBetweenTicks = TimeSpan.FromSeconds(1.0 / (double)fps);
	}

	private void LoopProc()
	{
		TimeSpan timeSpan = _st.Elapsed;
		while (true)
		{
			TimeSpan elapsed = _st.Elapsed;
			TimeSpan timeout = timeSpan + _timeBetweenTicks - elapsed;
			if (timeout.TotalMilliseconds > 1.0)
			{
				Thread.Sleep(timeout);
			}
			timeSpan = (elapsed = _st.Elapsed);
			lock (_lock)
			{
				if (_count == 0)
				{
					_running = false;
					break;
				}
			}
			_tick?.Invoke(elapsed);
		}
	}
}
