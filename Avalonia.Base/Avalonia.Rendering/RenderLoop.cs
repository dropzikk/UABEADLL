using System;
using System.Collections.Generic;
using System.Threading;
using Avalonia.Logging;
using Avalonia.Threading;

namespace Avalonia.Rendering;

internal class RenderLoop : IRenderLoop
{
	private List<IRenderLoopTask> _items = new List<IRenderLoopTask>();

	private List<IRenderLoopTask> _itemsCopy = new List<IRenderLoopTask>();

	private IRenderTimer? _timer;

	private int _inTick;

	private int _inUpdate;

	public static IRenderLoop LocatorAutoInstance
	{
		get
		{
			IRenderLoop renderLoop = AvaloniaLocator.Current.GetService<IRenderLoop>();
			if (renderLoop == null)
			{
				IRenderTimer requiredService = AvaloniaLocator.Current.GetRequiredService<IRenderTimer>();
				AvaloniaLocator.CurrentMutable.Bind<IRenderLoop>().ToConstant(renderLoop = new RenderLoop(requiredService));
			}
			return renderLoop;
		}
	}

	protected IRenderTimer Timer => _timer ?? (_timer = AvaloniaLocator.Current.GetRequiredService<IRenderTimer>());

	public bool RunsInBackground => Timer.RunsInBackground;

	public RenderLoop(IRenderTimer timer)
	{
		_timer = timer;
	}

	public void Add(IRenderLoopTask i)
	{
		if (i == null)
		{
			throw new ArgumentNullException("i");
		}
		Dispatcher.UIThread.VerifyAccess();
		lock (_items)
		{
			_items.Add(i);
			if (_items.Count == 1)
			{
				Timer.Tick += TimerTick;
			}
		}
	}

	public void Remove(IRenderLoopTask i)
	{
		if (i == null)
		{
			throw new ArgumentNullException("i");
		}
		Dispatcher.UIThread.VerifyAccess();
		lock (_items)
		{
			_items.Remove(i);
			if (_items.Count == 0)
			{
				Timer.Tick -= TimerTick;
			}
		}
	}

	private void TimerTick(TimeSpan time)
	{
		if (Interlocked.CompareExchange(ref _inTick, 1, 0) != 0)
		{
			return;
		}
		try
		{
			lock (_items)
			{
				_itemsCopy.Clear();
				_itemsCopy.AddRange(_items);
			}
			for (int i = 0; i < _itemsCopy.Count; i++)
			{
				_itemsCopy[i].Render();
			}
			_itemsCopy.Clear();
		}
		catch (Exception propertyValue)
		{
			Logger.TryGet(LogEventLevel.Error, "Visual")?.Log(this, "Exception in render loop: {Error}", propertyValue);
		}
		finally
		{
			Interlocked.Exchange(ref _inTick, 0);
		}
	}
}
