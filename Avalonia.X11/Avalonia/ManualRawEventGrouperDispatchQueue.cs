using System;
using System.Collections.Generic;
using Avalonia.Input.Raw;

namespace Avalonia;

internal class ManualRawEventGrouperDispatchQueue : IRawEventGrouperDispatchQueue
{
	private readonly Queue<(RawInputEventArgs args, Action<RawInputEventArgs> handler)> _inputQueue = new Queue<(RawInputEventArgs, Action<RawInputEventArgs>)>();

	public bool HasJobs => _inputQueue.Count > 0;

	public void Add(RawInputEventArgs args, Action<RawInputEventArgs> handler)
	{
		_inputQueue.Enqueue((args, handler));
	}

	public void DispatchNext()
	{
		if (_inputQueue.Count != 0)
		{
			(RawInputEventArgs, Action<RawInputEventArgs>) tuple = _inputQueue.Dequeue();
			tuple.Item2(tuple.Item1);
		}
	}
}
