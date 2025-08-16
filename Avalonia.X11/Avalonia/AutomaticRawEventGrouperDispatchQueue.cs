using System;
using System.Collections.Generic;
using Avalonia.Input.Raw;
using Avalonia.Threading;

namespace Avalonia;

internal class AutomaticRawEventGrouperDispatchQueue : IRawEventGrouperDispatchQueue
{
	private readonly Queue<(RawInputEventArgs args, Action<RawInputEventArgs> handler)> _inputQueue = new Queue<(RawInputEventArgs, Action<RawInputEventArgs>)>();

	private readonly Action _dispatchFromQueue;

	public AutomaticRawEventGrouperDispatchQueue()
	{
		_dispatchFromQueue = DispatchFromQueue;
	}

	public void Add(RawInputEventArgs args, Action<RawInputEventArgs> handler)
	{
		_inputQueue.Enqueue((args, handler));
		if (_inputQueue.Count == 1)
		{
			Dispatcher.UIThread.Post(_dispatchFromQueue, DispatcherPriority.Input);
		}
	}

	private void DispatchFromQueue()
	{
		do
		{
			if (_inputQueue.Count == 0)
			{
				return;
			}
			(RawInputEventArgs, Action<RawInputEventArgs>) tuple = _inputQueue.Dequeue();
			tuple.Item2(tuple.Item1);
		}
		while (!Dispatcher.UIThread.HasJobsWithPriority((int)DispatcherPriority.Input + 1));
		Dispatcher.UIThread.Post(_dispatchFromQueue, DispatcherPriority.Input);
	}
}
