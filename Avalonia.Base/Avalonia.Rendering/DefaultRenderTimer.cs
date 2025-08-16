using System;
using System.Threading;
using Avalonia.Metadata;

namespace Avalonia.Rendering;

[PrivateApi]
public class DefaultRenderTimer : IRenderTimer
{
	private int _subscriberCount;

	private Action<TimeSpan>? _tick;

	private IDisposable? _subscription;

	public int FramesPerSecond { get; }

	public virtual bool RunsInBackground => true;

	public event Action<TimeSpan> Tick
	{
		add
		{
			_tick = (Action<TimeSpan>)Delegate.Combine(_tick, value);
			if (_subscriberCount++ == 0)
			{
				Start();
			}
		}
		remove
		{
			if (--_subscriberCount == 0)
			{
				Stop();
			}
			_tick = (Action<TimeSpan>)Delegate.Remove(_tick, value);
		}
	}

	public DefaultRenderTimer(int framesPerSecond)
	{
		FramesPerSecond = framesPerSecond;
	}

	protected void Start()
	{
		_subscription = StartCore(InternalTick);
	}

	protected virtual IDisposable StartCore(Action<TimeSpan> tick)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(1.0 / (double)FramesPerSecond);
		return new Timer(delegate
		{
			tick(TimeSpan.FromMilliseconds(Environment.TickCount));
		}, null, timeSpan, timeSpan);
	}

	protected void Stop()
	{
		_subscription?.Dispose();
		_subscription = null;
	}

	private void InternalTick(TimeSpan tickCount)
	{
		_tick?.Invoke(tickCount);
	}
}
