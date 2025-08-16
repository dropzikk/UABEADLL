using System;
using System.Threading;

namespace Avalonia.Controls;

public class RefreshCompletionDeferral
{
	private Action _deferredAction;

	private int _deferCount;

	public RefreshCompletionDeferral(Action deferredAction)
	{
		_deferredAction = deferredAction;
	}

	public void Complete()
	{
		Interlocked.Decrement(ref _deferCount);
		if (_deferCount == 0)
		{
			_deferredAction?.Invoke();
		}
	}

	public RefreshCompletionDeferral Get()
	{
		Interlocked.Increment(ref _deferCount);
		return this;
	}
}
